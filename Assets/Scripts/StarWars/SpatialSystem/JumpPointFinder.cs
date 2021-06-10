using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarWars;
using UnityEngine;

namespace StarWarsSpatial
{
    class JpsPathNodeCompare : IComparer<PathNode>
    {
        public int Compare(PathNode x, PathNode y)
        {
            if (x.cast > y.cast)
                return -1;
            else if (x.cast == y.cast)
                return 0;
            else
                return 1;
        }
    }
    public sealed class JumpPointFinder
    {
        public JumpPointFinder(ICellMapView cellMapView)
        {
            m_CellMapView = cellMapView;
            m_MinCell.row = 0;
            m_MinCell.col = 0;
            m_MaxCell.row = cellMapView.MaxRowCount - 1;
            m_MaxCell.col = cellMapView.MaxColCount - 1;
        }
        public JumpPointFinder(ICellMapView cellMapView, CellPos minCell, CellPos maxCell)
        {
            m_CellMapView = cellMapView;
            m_MinCell = minCell;
            m_MaxCell = maxCell;
        }

        public List<CellPos> FindPath(CellPos start, CellPos target)
        {
            m_OpenQueue.Clear();
            m_OpenNodes.Clear();
            m_CloseSet.Clear();

            m_StartNode = new PathNode(start);
            m_EndNode = new PathNode(target);

            m_StartNode.cast = 0;
            m_StartNode.from_start_cast = 0;

            m_OpenQueue.Push(m_StartNode);
            m_OpenNodes.Add(m_StartNode.cell, m_StartNode);

            PathNode curNode = null;
            while (m_OpenQueue.Count > 0)
            {
                curNode = m_OpenQueue.Pop();
                m_OpenNodes.Remove(curNode.cell);
                m_CloseSet.Add(curNode.cell, true);

                if (isEndNode(curNode))
                {
                    break;
                }

                // test set close color
                //m_CellManager.SetCellStatus(curNode.cell.row, curNode.cell.col, '\x10');

                IdentifySuccessors(curNode);
            }

            List<CellPos> path = new List<CellPos>();
            if (isEndNode(curNode))
            {
                while (curNode != null)
                {
                    path.Add(curNode.cell);
                    curNode = curNode.parent;
                }
            }
            return path;
        }

        private void IdentifySuccessors(PathNode node)
        {
            int endX = m_EndNode.cell.row;
            int endY = m_EndNode.cell.col;
            int x = node.cell.row;
            int y = node.cell.col;

            List<CellPos> neighbors = FindNeighbors(node);
            foreach (CellPos pos in neighbors)
            {
                CellPos jumpPoint = Jump(pos.row, pos.col, x, y);
                if (jumpPoint.row >= 0 && jumpPoint.col >= 0)
                {
                    int jx = jumpPoint.row;
                    int jy = jumpPoint.col;
                    if (m_CloseSet.ContainsKey(jumpPoint))
                        continue;
                    Vector3 pos1 = m_CellMapView.GetCellCenter(jx, jy);
                    Vector3 pos2 = m_CellMapView.GetCellCenter(x, y);
                    float d = (pos1 - pos2).magnitude;
                    float ng = node.from_start_cast + d;

                    bool isOpened = true;
                    PathNode jumpNode = null;
                    if (m_OpenNodes.ContainsKey(jumpPoint))
                    {
                        jumpNode = m_OpenNodes[jumpPoint];
                    }
                    else
                    {
                        jumpNode = new PathNode(jumpPoint);
                        isOpened = false;
                    }
                    if (!isOpened || ng < jumpNode.from_start_cast)
                    {
                        jumpNode.from_start_cast = ng;
                        jumpNode.cast = jumpNode.from_start_cast + CalcHeuristic(jumpPoint);
                        jumpNode.parent = node;

                        if (!isOpened)
                        {
                            m_OpenQueue.Push(jumpNode);
                            m_OpenNodes.Add(jumpPoint, jumpNode);
                        }
                        else
                        {
                            int index = m_OpenQueue.IndexOf(jumpNode);
                            m_OpenQueue.Update(index, jumpNode);
                        }
                    }
                }
            }
        }

        private CellPos Jump(int x, int y, int px, int py)
        {
            int dx = x - px;
            int dy = y - py;

            for (int ct = 0; ct < 1024; ++ct)
            {
                if (!IsWalkable(x, y))
                    return new CellPos(-1, -1);
                if (x == m_EndNode.cell.row && y == m_EndNode.cell.col)
                    return new CellPos(x, y);

                if (dx != 0 && dy != 0)
                {
                    if ((IsWalkable(x - dx, y + dy) && !IsWalkable(x - dx, y)) ||
                      (IsWalkable(x + dx, y - dy) && !IsWalkable(x, y - dy)))
                    {
                        return new CellPos(x, y);
                    }
                }
                else
                {
                    if (dx != 0)
                    {
                        if ((IsWalkable(x + dx, y + 1) && !IsWalkable(x, y + 1)) ||
                        (IsWalkable(x + dx, y - 1) && !IsWalkable(x, y - 1)))
                        {
                            return new CellPos(x, y);
                        }
                    }
                    else
                    {
                        if ((IsWalkable(x + 1, y + dy) && !IsWalkable(x + 1, y)) ||
                        (IsWalkable(x - 1, y + dy) && !IsWalkable(x - 1, y)))
                        {
                            return new CellPos(x, y);
                        }
                    }
                }

                if (dx != 0 && dy != 0)
                {
                    CellPos jx = Jump(x + dx, y, x, y);
                    CellPos jy = Jump(x, y + dy, x, y);
                    if (jx.row >= 0 && jx.col >= 0 || jy.row >= 0 && jy.col >= 0)
                        return new CellPos(x, y);
                }

                if (IsWalkable(x + dx, y) || IsWalkable(x, y + dy))
                {
                    x += dx;
                    y += dy;
                }
                else
                {
                    return new CellPos(-1, -1);
                }
            }
            return new CellPos(-1, -1);
        }

        private List<CellPos> FindNeighbors(PathNode node)
        {
            int x = node.cell.row;
            int y = node.cell.col;

            List<CellPos> neighbors = new List<CellPos>();

            PathNode parent = node.parent;
            if (null != parent)
            {
                int px = parent.cell.row;
                int py = parent.cell.col;

                int dx = (x == px ? 0 : (x - px) / Math.Abs(x - px));
                int dy = (y == py ? 0 : (y - py) / Math.Abs(y - py));

                if (dx != 0 && dy != 0)
                {
                    if (IsWalkable(x, y + dy))
                        neighbors.Add(new CellPos(x, y + dy));
                    if (IsWalkable(x + dx, y))
                        neighbors.Add(new CellPos(x + dx, y));
                    if (IsWalkable(x, y + dy) || IsWalkable(x + dx, y))
                        neighbors.Add(new CellPos(x + dx, y + dy));
                    if (!IsWalkable(x - dx, y) && IsWalkable(x, y + dy))
                        neighbors.Add(new CellPos(x - dx, y + dy));
                    if (!IsWalkable(x, y - dy) && IsWalkable(x + dx, y))
                        neighbors.Add(new CellPos(x + dx, y - dy));
                }
                else
                {
                    if (dx == 0)
                    {
                        if (IsWalkable(x, y + dy))
                        {
                            if (IsWalkable(x, y + dy))
                                neighbors.Add(new CellPos(x, y + dy));
                            if (!IsWalkable(x + 1, y))
                                neighbors.Add(new CellPos(x + 1, y + dy));
                            if (!IsWalkable(x - 1, y))
                                neighbors.Add(new CellPos(x - 1, y + dy));
                        }
                    }
                    else
                    {
                        if (IsWalkable(x + dx, y))
                        {
                            if (IsWalkable(x + dx, y))
                                neighbors.Add(new CellPos(x + dx, y));
                            if (!IsWalkable(x, y + 1))
                                neighbors.Add(new CellPos(x + dx, y + 1));
                            if (!IsWalkable(x, y - 1))
                                neighbors.Add(new CellPos(x + dx, y - 1));
                        }
                    }
                }
            }
            else
            {
                neighbors = m_CellMapView.GetCellAdjacent(node.cell);
            }
            return neighbors;
        }

        private bool IsWalkable(int x, int y)
        {
            return m_CellMapView.CanPass(x, y);
        }

        private float CalcHeuristic(CellPos pos)
        {
            return Math.Abs(pos.col - m_EndNode.cell.col) + Math.Abs(pos.row - m_EndNode.cell.row);
        }

        private bool isEndNode(PathNode node)
        {
            return isEndNode(node.cell);
        }

        private bool isEndNode(CellPos pos)
        {
            return isEndNode(pos.row, pos.col);
        }

        private bool isEndNode(int row, int col)
        {
            return row == m_EndNode.cell.row && col == m_EndNode.cell.col;
        }

        private ICellMapView m_CellMapView = null;
        private PathNode m_StartNode = null;
        private PathNode m_EndNode = null;
        private Heap<PathNode> m_OpenQueue = new Heap<PathNode>(new JpsPathNodeCompare());
        private Dictionary<CellPos, PathNode> m_OpenNodes = new Dictionary<CellPos, PathNode>();
        private Dictionary<CellPos, bool> m_CloseSet = new Dictionary<CellPos, bool>();

        private CellPos m_MinCell;
        private CellPos m_MaxCell;
    }

}
