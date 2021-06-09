using System.Collections.Generic;
using System;

using UnityEngine;
using StarWarsSpatial;

namespace StarWars
{
    public class KdTreeObject
    {
        public ISpaceObject SpaceObject;
        public Vector3 Position;
        public float Radius;
        public Vector3 Velocity;
        public bool IsAvoidable;

        public float MaxX;
        public float MinX;
        public float MaxZ;
        public float MinZ;

        public KdTreeObject(ISpaceObject obj)
        {
            CopyFrom(obj);
        }
        public void CopyFrom(ISpaceObject obj)
        {
            if (null != obj)
            {
                SpaceObject = obj;
                Position = obj.GetPosition();
                Radius = (float)obj.GetRadius();
                Velocity = obj.GetVelocity();
                IsAvoidable = obj.IsAvoidable();
                MaxX = Position.x + Radius;
                MinX = Position.x - Radius;
                MaxZ = Position.z + Radius;
                MinZ = Position.z - Radius;
            }
            else
            {
                SpaceObject = null;
                Position = new Vector3();
                Radius = 0;
                Velocity = new Vector3();
                IsAvoidable = false;
                MaxX = MinX = 0;
                MaxZ = MinZ = 0;
            }
        }
    }
    public sealed class KdTree
    {
        public const int c_MaxLeafSize = 4;

        private struct KdTreeNode
        {
            public int m_Begin;
            public int m_End;
            public int m_Left;
            public float m_MaxX;
            public float m_MaxZ;
            public float m_MinX;
            public float m_MinZ;
            public int m_Right;
        }

        public void Build(IList<ISpaceObject> objs)
        {
            if (null == m_Objects || m_Objects.Length < objs.Count)
            {
                m_ObjectNum = 0;
                m_Objects = new KdTreeObject[objs.Count * 2];
                foreach (ISpaceObject obj in objs)
                {
                    m_Objects[m_ObjectNum++] = new KdTreeObject(obj);
                }
            }
            else
            {
                m_ObjectNum = 0;
                foreach (ISpaceObject obj in objs)
                {
                    if (null == m_Objects[m_ObjectNum])
                        m_Objects[m_ObjectNum] = new KdTreeObject(obj);
                    else
                        m_Objects[m_ObjectNum].CopyFrom(obj);
                    ++m_ObjectNum;
                }
            }
            if (m_ObjectNum > 0)
            {
                if (null == m_KdTree || m_KdTree.Length < 3 * m_ObjectNum)
                {
                    m_KdTree = new KdTreeNode[3 * m_ObjectNum];
                    for (int i = 0; i < m_KdTree.Length; ++i)
                    {
                        m_KdTree[i] = new KdTreeNode();
                    }
                }
                m_MaxNodeNum = 2 * m_ObjectNum;
                BuildImpl();
            }
        }
        public void Clear()
        {
            m_ObjectNum = 0;
        }

        public void Query(ISpaceObject obj, float range, MyAction<float, KdTreeObject> visitor)
        {
            Query(obj.GetPosition(), range, visitor);
        }

        public void Query(Vector3 pos, float range, MyAction<float, KdTreeObject> visitor)
        {
            if (null != m_KdTree && m_ObjectNum > 0 && m_KdTree.Length > 0)
            {
                float rangeSq = Sqr(range);
                QueryImpl(pos, rangeSq, range, (float distSqr, KdTreeObject obj) => { visitor(distSqr, obj); return true; });
            }
        }

        public void Query(ISpaceObject obj, float range, MyFunc<float, KdTreeObject, bool> visitor)
        {
            Query(obj.GetPosition(), range, visitor);
        }

        public void Query(Vector3 pos, float range, MyFunc<float, KdTreeObject, bool> visitor)
        {
            if (null != m_KdTree && m_ObjectNum > 0 && m_KdTree.Length > 0)
            {
                float rangeSq = Sqr(range);
                QueryImpl(pos, rangeSq, range, visitor);
            }
        }

        private void BuildImpl()
        {
            m_BuildStack.Push(0);
            m_BuildStack.Push(m_ObjectNum);
            m_BuildStack.Push(0);
            while (m_BuildStack.Count >= 3)
            {
                int begin = m_BuildStack.Pop();
                int end = m_BuildStack.Pop();
                int node = m_BuildStack.Pop();

                m_KdTree[node].m_Begin = begin;
                m_KdTree[node].m_End = end;

                float minX = m_Objects[begin].MinX;
                float maxX = m_Objects[begin].MaxX;
                float minZ = m_Objects[begin].MinZ;
                float maxZ = m_Objects[begin].MaxZ;
                for (int i = begin + 1; i < end; ++i)
                {
                    float newMaxX = m_Objects[i].MaxX;
                    float newMinX = m_Objects[i].MinX;
                    float newMaxZ = m_Objects[i].MaxZ;
                    float newMinZ = m_Objects[i].MinZ;
                    if (minX > newMinX) minX = newMinX;
                    if (maxX < newMaxX) maxX = newMaxX;
                    if (minZ > newMinZ) minZ = newMinZ;
                    if (maxZ < newMaxZ) maxZ = newMaxZ;
                }
                m_KdTree[node].m_MinX = minX;
                m_KdTree[node].m_MaxX = maxX;
                m_KdTree[node].m_MinZ = minZ;
                m_KdTree[node].m_MaxZ = maxZ;

                if (end - begin > c_MaxLeafSize)
                {
                    bool isVertical = (maxX - minX > maxZ - minZ);
                    float splitValue = (isVertical ? 0.5f * (maxX + minX) : 0.5f * (maxZ + minZ));

                    int left = begin;
                    int right = end;

                    while (left < right)
                    {
                        while (left < right && (isVertical ? m_Objects[left].Position.x : m_Objects[left].Position.z) < splitValue)
                        {
                            ++left;
                        }

                        while (right > left && (isVertical ? m_Objects[right - 1].Position.x : m_Objects[right - 1].Position.z) >= splitValue)
                        {
                            --right;
                        }

                        if (left < right)
                        {
                            KdTreeObject tmp = m_Objects[left];
                            m_Objects[left] = m_Objects[right - 1];
                            m_Objects[right - 1] = tmp;
                            ++left;
                            --right;
                        }
                    }

                    if (left == end)
                    {
                        --left;
                    }

                    int leftSize = left - begin;

                    if (leftSize == 0)
                    {
                        ++leftSize;
                        ++left;
                    }

                    m_KdTree[node].m_Left = node + 1;
                    m_KdTree[node].m_Right = node + 1 + (2 * leftSize - 1);

                    if (m_KdTree[node].m_Left >= m_MaxNodeNum || m_KdTree[node].m_Right >= m_MaxNodeNum)
                    {
                        //LogSystem.Error("KdTree Error, node:{0} left:{1} right:{2} leftSize:{3} begin:{4} end:{5} maxNodeNum:{6}", node, left, right, leftSize, begin, end, m_MaxNodeNum);
                    }

                    m_BuildStack.Push(m_KdTree[node].m_Left);
                    m_BuildStack.Push(left);
                    m_BuildStack.Push(begin);

                    m_BuildStack.Push(m_KdTree[node].m_Right);
                    m_BuildStack.Push(end);
                    m_BuildStack.Push(left);
                }
            }
        }

        private void QueryImpl(Vector3 pos, float rangeSq, float range, MyFunc<float, KdTreeObject, bool> visitor)
        {
            m_QueryStack.Push(0);
            while (m_QueryStack.Count > 0)
            {
                int node = m_QueryStack.Pop();
                int begin = m_KdTree[node].m_Begin;
                int end = m_KdTree[node].m_End;
                int left = m_KdTree[node].m_Left;
                int right = m_KdTree[node].m_Right;

                if (end - begin <= c_MaxLeafSize)
                {
                    for (int i = begin; i < end; ++i)
                    {
                        float distSq = Geometry.DistanceSquare(pos, m_Objects[i].Position);
                        if (distSq <= Sqr(range + m_Objects[i].Radius))
                        {
                            if (!visitor(distSq, m_Objects[i]))
                            {
                                m_QueryStack.Clear();
                                return;
                            }
                        }
                    }
                }
                else
                {
                    float distSqLeft = CalcSquareDistToRectangle(m_KdTree[left].m_MinX - pos.x, pos.x - m_KdTree[left].m_MaxX, m_KdTree[left].m_MinZ - pos.z, pos.z - m_KdTree[left].m_MaxZ);
                    float distSqRight = CalcSquareDistToRectangle(m_KdTree[right].m_MinX - pos.x, pos.x - m_KdTree[right].m_MaxX, m_KdTree[right].m_MinZ - pos.z, pos.z - m_KdTree[right].m_MaxZ);

                    if (distSqLeft < distSqRight)
                    {
                        if (distSqLeft < rangeSq)
                        {
                            m_QueryStack.Push(left);

                            if (distSqRight < rangeSq)
                            {
                                m_QueryStack.Push(right);
                            }
                        }
                    }
                    else
                    {
                        if (distSqRight < rangeSq)
                        {
                            m_QueryStack.Push(right);

                            if (distSqLeft < rangeSq)
                            {
                                m_QueryStack.Push(left);
                            }
                        }
                    }
                }
            }
        }

        private static float Sqr(float v)
        {
            return v * v;
        }

        private static float CalcSquareDistToRectangle(float distMinX, float distMaxX, float distMinZ, float distMaxZ)
        {
            float ret = 0;
            if (distMinX > 0) ret += distMinX * distMinX;
            if (distMaxX > 0) ret += distMaxX * distMaxX;
            if (distMinZ > 0) ret += distMinZ * distMinZ;
            if (distMaxZ > 0) ret += distMaxZ * distMaxZ;
            return ret;
        }

        private KdTreeObject[] m_Objects = null;
        private int m_ObjectNum = 0;
        private KdTreeNode[] m_KdTree = null;
        private int m_MaxNodeNum = 0;
        private Stack<int> m_BuildStack = new Stack<int>(4096);
        private Stack<int> m_QueryStack = new Stack<int>(4096);
    }
}
