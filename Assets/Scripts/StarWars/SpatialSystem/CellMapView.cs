using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using StarWars;

namespace StarWarsSpatial
{
    public sealed class CellMapView : ICellMapView
    {
        public CellMapView(CellManager cellMgr, int radius)
        {
            m_Radius = radius;
            m_CellNumPerCellView = 2 * radius - 1;
            m_CellMgr = cellMgr;
            m_MaxRowCount = m_CellMgr.GetMaxRow() / m_CellNumPerCellView;
            m_MaxColCount = m_CellMgr.GetMaxCol() / m_CellNumPerCellView;
            m_RadiusLength = (2 * m_Radius - 1) * m_CellMgr.GetCellWidth() / 2;
            if (radius > 1)
            {
                m_Delegation = new CellMapViewWithMapData(cellMgr, radius);
            }
        }
        public int Radius
        {
            get { return m_Radius; }
        }
        public float RadiusLength
        {
            get { return m_RadiusLength; }
        }
        public int MaxRowCount
        {
            get { return m_MaxRowCount; }
        }
        public int MaxColCount
        {
            get { return m_MaxColCount; }
        }
        public bool IsCellValid(int row, int col)
        {
            return row >= 0 && row < m_MaxRowCount && col >= 0 && col < m_MaxColCount;
        }
        public bool GetCell(Vector3 pos, out int row, out int col)
        {
            if (1 == m_Radius)
            {
                return m_CellMgr.GetCell(pos, out row, out col);
            }
            else
            {
                if (null != m_Delegation)
                {
                    return m_Delegation.GetCell(pos, out row, out col);
                }
                else
                {
                    row = 0;
                    col = 0;
                    return false;
                }
            }
        }
        public Vector3 GetCellCenter(int row, int col)
        {
            if (1 == m_Radius)
            {
                return m_CellMgr.GetCellCenter(row, col);
            }
            else
            {
                if (null != m_Delegation)
                {
                    return m_Delegation.GetCellCenter(row, col);
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }
        public List<CellPos> GetCellAdjacent(CellPos center)
        {
            return GetCellAdjacent(center, m_MaxRowCount, m_MaxColCount);
        }
        public bool CanPass(int row, int col)
        {
            if (1 == m_Radius)
            {
                byte status = m_CellMgr.GetCellStatus(row, col);
                byte block = BlockType.GetBlockType(status);
                byte subtype = BlockType.GetBlockSubType(status);
                if (BlockType.NOT_BLOCK != block && subtype != BlockType.SUBTYPE_ENERGYWALL)
                {
                    return false;
                }
            }
            else
            {
                if (null != m_Delegation)
                {
                    return m_Delegation.CanPass(row, col);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        public bool CanPass(Vector3 targetPos)
        {
            int row = 0;
            int col = 0;
            GetCell(targetPos, out row, out col);
            return CanPass(row, col);
        }
        public bool CanShoot(int row, int col)
        {
            if (1 == m_Radius)
            {
                byte status = m_CellMgr.GetCellStatus(row, col);
                byte block = BlockType.GetBlockType(status);
                byte subtype = BlockType.GetBlockSubType(status);
                if (BlockType.NOT_BLOCK != block && subtype != BlockType.SUBTYPE_ROADBLOCK)
                {
                    return false;
                }
            }
            else
            {
                if (null != m_Delegation)
                {
                    return m_Delegation.CanShoot(row, col);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        public bool CanShoot(Vector3 targetPos)
        {
            int row = 0;
            int col = 0;
            GetCell(targetPos, out row, out col);
            return CanShoot(row, col);
        }
        public bool CanLeap(int row, int col)
        {
            if (1 == m_Radius)
            {
                byte status = m_CellMgr.GetCellStatus(row, col);
                byte block = BlockType.GetBlockType(status);
                byte subtype = BlockType.GetBlockSubType(status);
                if (BlockType.NOT_BLOCK != block && BlockType.SUBTYPE_OBSTACLE == subtype)
                {
                    return false;
                }
            }
            else
            {
                if (null != m_Delegation)
                {
                    return m_Delegation.CanLeap(row, col);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        public bool CanLeap(Vector3 targetPos)
        {
            int row = 0;
            int col = 0;
            GetCell(targetPos, out row, out col);
            return CanLeap(row, col);
        }
        public bool CanSee(int row, int col)
        {
            if (1 == m_Radius)
            {
                byte status = m_CellMgr.GetCellStatus(row, col);
                byte block = BlockType.GetBlockType(status);
                byte blinding = BlockType.GetBlockBlinding(status);
                if (BlockType.BLINDING_BLINDING == blinding)
                {
                    return false;
                }
            }
            else
            {
                if (null != m_Delegation)
                {
                    return m_Delegation.CanSee(row, col);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        public bool CanSee(Vector3 targetPos)
        {
            int row = 0;
            int col = 0;
            GetCell(targetPos, out row, out col);
            return CanSee(row, col);
        }
        public byte GetCellLevel(int row, int col)
        {
            byte level = BlockType.LEVEL_UNDERFLOOR_2;
            if (1 == m_Radius)
            {
                byte status = m_CellMgr.GetCellStatus(row, col);
                level = BlockType.GetBlockLevel(status);
            }
            else
            {
                if (null != m_Delegation)
                {
                    level = m_Delegation.GetCellLevel(row, col);
                }
            }
            return level;
        }
        public byte GetCellLevel(Vector3 targetPos)
        {
            int row = 0;
            int col = 0;
            GetCell(targetPos, out row, out col);
            return GetCellLevel(row, col);
        }

        public CellPos GetFirstWalkableCell(Vector3 from, Vector3 to)
        {
            return GetFirstWalkableCell(this, from, to);
        }

        private int m_Radius = 0;
        private int m_CellNumPerCellView = 0;
        private int m_MaxRowCount = 0;
        private int m_MaxColCount = 0;
        private float m_RadiusLength = 0;
        private CellManager m_CellMgr = null;
        private CellMapViewWithMapData m_Delegation = null;

        public static List<CellPos> GetCellAdjacent(CellPos center, int maxRow, int maxCol)
        {
            List<CellPos> cells_list = new List<CellPos>();
            //上面3个
            if (center.row > 0)
            {
                if (center.col > 0)
                {
                    cells_list.Add(new CellPos(center.row - 1, center.col - 1));
                }
                cells_list.Add(new CellPos(center.row - 1, center.col));
                if (center.col < maxCol - 1)
                {
                    cells_list.Add(new CellPos(center.row - 1, center.col + 1));
                }
            }
            //同行2个
            if (center.col > 0)
            {
                cells_list.Add(new CellPos(center.row, center.col - 1));
            }
            if (center.col < maxCol - 1)
            {
                cells_list.Add(new CellPos(center.row, center.col + 1));
            }
            //下面3个
            if (center.row < maxRow - 1)
            {
                if (center.col > 0)
                {
                    cells_list.Add(new CellPos(center.row + 1, center.col - 1));
                }
                cells_list.Add(new CellPos(center.row + 1, center.col));
                if (center.col < maxCol - 1)
                {
                    cells_list.Add(new CellPos(center.row + 1, center.col + 1));
                }
            }
            return cells_list;
        }
        public static CellPos GetFirstWalkableCell(ICellMapView view, Vector3 from, Vector3 to)
        {
            CellPos start, end, ret = new CellPos(-1, -1);
            view.GetCell(from, out start.row, out start.col);
            if (view.CanPass(start.row, start.col))
                ret = start;
            else
            {
                view.GetCell(to, out end.row, out end.col);
                from = view.GetCellCenter(start.row, start.col);
                to = view.GetCellCenter(end.row, end.col);
                from.y = 0; to.y = 0;
                float step = view.RadiusLength * 2;
                float angle = Geometry.GetYAngle(new Vector2(from.x, from.z), new Vector2(to.x, to.z));
                float xstep = step * (float)Math.Sin(angle);
                float zstep = step * (float)Math.Cos(angle);
                int count = (int)((to - from).magnitude / step);
                for (int ix = 0; ix < count; ++ix)
                {
                    from.x += xstep;
                    from.z += zstep;
                    int row, col;
                    view.GetCell(from, out row, out col);
                    if (view.CanPass(row, col))
                    {
                        ret.row = row;
                        ret.col = col;
                        break;
                    }
                }
            }
            return ret;
        }
    }
}
