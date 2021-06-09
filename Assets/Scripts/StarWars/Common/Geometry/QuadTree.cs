using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace StarWars
{
    public interface IElement
    {
        int Id
        {
            get;
        }
        float MinX
        {
            get;
        }
        float MaxX
        {
            get;
        }
        float MinY
        {
            get;
        }
        float MaxY
        {
            get;
        }
    }
    public interface IQuadTreeInfo
    {
        int MaxLevel
        {
            get;
        }
        float Width
        {
            get;
        }
        float Height
        {
            get;
        }
    }
    /// <summary>
    /// 松散四叉树结点。
    /// </summary>
    /// <typeparam name="Element"></typeparam>
    /// <remarks>
    /// 坐标轴x正方向为屏幕右方，y正方向为屏幕上方
    /// </remarks>
    public class QuadTreeNode<Element> where Element : IElement
    {
        public int ElementCount
        {
            get
            {
                return m_Elements.Count;
            }
        }
        public int TotalElementCount
        {
            get
            {
                int count = ElementCount;
                foreach (QuadTreeNode<Element> node in m_Nodes)
                {
                    count += node.TotalElementCount;
                }
                return count;
            }
        }
        public bool CanContains(Element element)
        {
            return ContainsElement(element, m_Bounds[0].x, m_Bounds[0].y, m_Bounds[2].x, m_Bounds[2].y);
        }
        public bool Overlap(float x1, float y1, float x2, float y2)
        {
            return Overlap(x1, y1, x2, y2, m_Bounds[0].x, m_Bounds[0].y, m_Bounds[2].x, m_Bounds[2].y);
        }
        public void AddElement(Element element, int lvl, int ix, int iy)
        {
            if (m_Level < lvl)
            {
                int xi = (int)(ix / Math.Pow(2, lvl - m_Level - 1)) % 2;
                int yi = (int)(iy / Math.Pow(2, lvl - m_Level - 1)) % 2;
                if (m_Nodes.Count <= 0)
                {
                    MakeChildren();
                }
                int index = xi * 2 + yi;
                QuadTreeNode<Element> node = m_Nodes[index];
                node.AddElement(element, lvl, ix, iy);
            }
            else if (!m_Elements.Contains(element))
            {
                m_Elements.Add(element);
            }
        }
        public void RemoveElement(Element element)
        {
            foreach (QuadTreeNode<Element> node in m_Nodes)
            {
                if (node.CanContains(element))
                {
                    node.RemoveElement(element);
                }
            }
            if (CanContains(element))
            {
                m_Elements.Remove(element);
            }
        }
        public int FindElements(float x1, float y1, float x2, float y2, Dictionary<Element, bool> dict)
        {
            int count = m_Elements.Count;
            foreach (Element element in m_Elements)
            {
                dict[element] = true;
            }
            foreach (QuadTreeNode<Element> node in m_Nodes)
            {
                if (node.Overlap(x1, y1, x2, y2))
                {
                    count += node.FindElements(x1, y1, x2, y2, dict);
                }
            }
            return count;
        }
        public QuadTreeNode(int _level, float cx, float cy, IQuadTreeInfo info)
        {
            m_Level = _level;
            m_TreeInfo = info;
            m_CenterX = cx;
            m_CenterY = cy;
            float rx = (float)(m_TreeInfo.Width / Math.Pow(2, m_Level));
            float ry = (float)(m_TreeInfo.Height / Math.Pow(2, m_Level));
            //逆时针走向的矩形
            m_Bounds.Add(new Vector2(m_CenterX - rx, m_CenterY - ry));
            m_Bounds.Add(new Vector2(m_CenterX + rx, m_CenterY - ry));
            m_Bounds.Add(new Vector2(m_CenterX + rx, m_CenterY + ry));
            m_Bounds.Add(new Vector2(m_CenterX - rx, m_CenterY + ry));
        }
        public string ToString(string title)
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine(GetIndent(m_Level) + title + ":" + m_Level);
            foreach (Element element in m_Elements)
            {
                sw.WriteLine(GetIndent(m_Level) + "Element:" + element.Id);
            }
            int ix = 0;
            foreach (QuadTreeNode<Element> node in m_Nodes)
            {
                sw.Write(node.ToString(title + "-" + ix));
                ++ix;
            }
            sw.Close();
            return sw.ToString();
        }
        private void MakeChildren()
        {
            int lvl = m_Level + 1;
            float rx = (float)(m_TreeInfo.Width / 2 / Math.Pow(2, lvl));
            float ry = (float)(m_TreeInfo.Height / 2 / Math.Pow(2, lvl));

            //ix==0,iy==0
            m_Nodes.Add(new QuadTreeNode<Element>(lvl, m_CenterX - rx, m_CenterY - ry, m_TreeInfo));
            //ix==0,iy==1
            m_Nodes.Add(new QuadTreeNode<Element>(lvl, m_CenterX - rx, m_CenterY + ry, m_TreeInfo));
            //ix==1,iy==0
            m_Nodes.Add(new QuadTreeNode<Element>(lvl, m_CenterX + rx, m_CenterY - ry, m_TreeInfo));
            //ix==1,iy==1
            m_Nodes.Add(new QuadTreeNode<Element>(lvl, m_CenterX + rx, m_CenterY + ry, m_TreeInfo));
        }
        private static bool ContainsElement(Element element, float x1, float y1, float x2, float y2)
        {
            if (element.MinX > x1 &&
                element.MaxX < x2 &&
                element.MinY > y1 &&
                element.MaxY < y2)
                return true;
            else
                return false;
        }
        private static bool Overlap(float _x1, float _y1, float _x2, float _y2, float x1, float y1, float x2, float y2)
        {
            if (_x1 > x2)
                return false;
            if (_x2 < x1)
                return false;
            if (_y1 > y2)
                return false;
            if (_y2 < y1)
                return false;
            return true;
        }
        private static string GetIndent(int indent)
        {
            return "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t".Substring(0, indent);
        }

        private IQuadTreeInfo m_TreeInfo = null;
        private int m_Level = 0;
        private float m_CenterX = 0, m_CenterY = 0;
        private List<Vector2> m_Bounds = new List<Vector2>();
        private List<QuadTreeNode<Element>> m_Nodes = new List<QuadTreeNode<Element>>();
        private List<Element> m_Elements = new List<Element>();
    }
    /// <summary>
    /// 松散四叉树的简单实现(k=2)。
    /// </summary>
    /// <typeparam name="Element"></typeparam>
    /// <remarks>
    /// 1、元素的边界盒更新前需要从四叉树上删除，更新完成后再加入四叉树。
    /// 2、四叉树根结点的边界盒变化时需要重新构造整棵四叉树。
    /// 3、坐标轴x正方向为屏幕右方，y正方向为屏幕上方。
    /// </remarks>
    public class QuadTree<Element> : IQuadTreeInfo where Element : IElement
    {
        public class QuadIndexInfo
        {
            public int Level
            {
                get;
                set;
            }
            public int IndexX
            {
                get;
                set;
            }
            public int IndexY
            {
                get;
                set;
            }
        }
        public QuadTree(int _maxLevel)
        {
            m_MaxLevel = _maxLevel;
        }
        public void Initialize(float minX, float minY, float maxX, float maxY)
        {
            if (minX > maxX || minY > maxY)
                return;
            m_BaseX = minX;
            m_BaseY = minY;
            m_Width = maxX - minX;
            m_Height = maxY - minY;
            float cx = (maxX + minX) / 2;
            float cy = (maxY + minY) / 2;
            m_RootNode = new QuadTreeNode<Element>(0, cx, cy, this);
        }
        public bool IsNull
        {
            get
            {
                return m_RootNode == null;
            }
        }
        public int TotalElementCount
        {
            get
            {
                if (IsNull)
                    return 0;
                return m_RootNode.TotalElementCount;
            }
        }
        public bool CanContains(Element element)
        {
            if (IsNull)
                return false;
            return m_RootNode.CanContains(element);
        }
        public bool CalcIndexInfo(Element element, out int level, out int ix, out int iy)
        {
            bool ret = false;
            if (CanContains(element))
            {
                float rx = element.MaxX - element.MinX;
                float ry = element.MaxY - element.MinY;
                int lx = m_MaxLevel, ly = m_MaxLevel;
                if (rx > 0)
                {
                    lx = (int)Math.Log(m_Width / rx, 2);
                }
                if (ry > 0)
                {
                    ly = (int)Math.Log(m_Height / ry, 2);
                }
                level = Math.Min(lx, ly);
                if (level > m_MaxLevel)
                    level = m_MaxLevel;
                float lenx = (float)(m_Width / Math.Pow(2, level));
                float leny = (float)(m_Height / Math.Pow(2, level));
                float cx = (element.MinX + element.MaxX) / 2;
                float cy = (element.MinY + element.MaxY) / 2;
                ix = (int)((cx - m_BaseX) / lenx);
                iy = (int)((cy - m_BaseY) / leny);
                ret = true;
            }
            else
            {
                level = -1;
                ix = -1;
                iy = -1;
            }
            return ret;
        }
        public void AddElement(Element element)
        {
            int lvl, ix, iy;
            if (CalcIndexInfo(element, out lvl, out ix, out iy))
            {
                m_RootNode.AddElement(element, lvl, ix, iy);
            }
        }
        public void RemoveElement(Element element)
        {
            if (!IsNull)
            {
                m_RootNode.RemoveElement(element);
            }
        }
        public int FindElements(float x, float y, Dictionary<Element, bool> dict)
        {
            return FindElements(x, y, x, y, dict);
        }
        public int FindElements(float x1, float y1, float x2, float y2, Dictionary<Element, bool> dict)
        {
            if (IsNull)
                return 0;
            return m_RootNode.FindElements(x1, y1, x2, y2, dict);
        }
        public int FindElements(float x, float y, float radius, Dictionary<Element, bool> dict)
        {
            return FindElements(x - radius, y - radius, x + radius, y + radius, dict);
        }
        public int MaxLevel
        {
            get
            {
                return m_MaxLevel;
            }
        }
        public float Width
        {
            get
            {
                return m_Width;
            }
        }
        public float Height
        {
            get
            {
                return m_Height;
            }
        }
        public override string ToString()
        {
            return m_RootNode.ToString("0");
        }

        private QuadTreeNode<Element> m_RootNode = null;
        private int m_MaxLevel = 0;
        private float m_BaseX = 0;
        private float m_BaseY = 0;
        private float m_Width = 0;
        private float m_Height = 0;
    }
}
