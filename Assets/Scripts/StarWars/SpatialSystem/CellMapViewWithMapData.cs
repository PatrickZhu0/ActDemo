using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using StarWars;

namespace StarWarsSpatial
{
    public sealed class CellMapViewWithMapData : ICellMapView
    {
        public CellMapViewWithMapData(CellManager cellMgr, int radius)
        {
            m_Radius = radius;
            m_CellMgr = cellMgr;
            m_CellNumPerCellView = 2 * radius - 1;
            m_MaxRowCount = m_CellMgr.GetMaxRow() / m_CellNumPerCellView;
            m_MaxColCount = m_CellMgr.GetMaxCol() / m_CellNumPerCellView;
            m_RadiusLength = (2 * m_Radius - 1) * m_CellMgr.GetCellWidth() / 2;
            m_CellAttrs = new byte[m_MaxRowCount, m_MaxColCount];
            InitCellAttr();
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
            float cell_width_ = m_RadiusLength * 2;
            row = (int)(pos.z / cell_width_);
            float y = pos.z - row * cell_width_;
            if (y >= cell_width_ + 0.001f)
                ++row;

            col = (int)(pos.x / cell_width_);
            float x = pos.x - col * cell_width_;
            if (x >= cell_width_ + 0.001f)
                ++col;

            return row >= 0 && row < m_MaxRowCount && col >= 0 && col < m_MaxColCount;
        }
        public Vector3 GetCellCenter(int row, int col)
        {
            float cell_width_ = m_RadiusLength * 2;
            Vector3 center = new Vector3();
            center.x = col * cell_width_ + cell_width_ / 2;
            center.z = row * cell_width_ + cell_width_ / 2;
            return center;
        }
        public List<CellPos> GetCellAdjacent(CellPos center)
        {
            return CellMapView.GetCellAdjacent(center, m_MaxRowCount, m_MaxColCount);
        }
        public bool CanPass(int row, int col)
        {
            byte status = GetCellStatus(row, col);
            byte block = BlockType.GetBlockType(status);
            byte subtype = BlockType.GetBlockSubType(status);
            if (BlockType.NOT_BLOCK != block && subtype != BlockType.SUBTYPE_ENERGYWALL)
            {
                return false;
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
            byte status = GetCellStatus(row, col);
            byte block = BlockType.GetBlockType(status);
            byte subtype = BlockType.GetBlockSubType(status);
            if (BlockType.NOT_BLOCK != block && subtype != BlockType.SUBTYPE_ROADBLOCK)
            {
                return false;
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
            byte status = GetCellStatus(row, col);
            byte block = BlockType.GetBlockType(status);
            byte subtype = BlockType.GetBlockSubType(status);
            if (BlockType.NOT_BLOCK != block && BlockType.SUBTYPE_OBSTACLE == subtype)
            {
                return false;
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
            byte status = GetCellStatus(row, col);
            byte block = BlockType.GetBlockType(status);
            byte blinding = BlockType.GetBlockBlinding(status);
            if (BlockType.BLINDING_BLINDING == blinding)
            {
                return false;
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
            byte status = GetCellStatus(row, col);
            level = BlockType.GetBlockLevel(status);
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
            return CellMapView.GetFirstWalkableCell(this, from, to);
        }

        private byte GetCellStatus(int row, int col)
        {
            if (row >= m_MaxRowCount || col >= m_MaxColCount || row < 0 || col < 0)
            {
                return BlockType.OUT_OF_BLOCK;
            }
            return m_CellAttrs[row, col];
        }
        private CellPos GetCoveredOriginalCenterCell(int row, int col)
        {
            CellPos cell = new CellPos();
            if (IsCellValid(row, col))
            {
                int oRow = row * m_CellNumPerCellView + m_Radius - 1;
                int oCol = col * m_CellNumPerCellView + m_Radius - 1;
                cell.row = oRow;
                cell.col = oCol;
            }
            return cell;
        }
        private List<CellPos> GetCoveredOriginalCells(int row, int col)
        {
            List<CellPos> cells = null;
            int key = row * m_MaxColCount + col;
            cells = new List<CellPos>();
            if (IsCellValid(row, col))
            {
                int oRow = row * m_CellNumPerCellView + m_Radius - 1;
                int oCol = col * m_CellNumPerCellView + m_Radius - 1;
                for (int r = oRow - m_Radius + 1; r <= oRow + m_Radius - 1; ++r)
                {
                    for (int c = oCol - m_Radius + 1; c <= oCol + m_Radius - 1; ++c)
                    {
                        cells.Add(new CellPos(r, c));
                    }
                }
            }
            return cells;
        }
        private void InitCellAttr()
        {
            for (int row = 0; row < m_MaxRowCount; ++row)
            {
                for (int col = 0; col < m_MaxColCount; ++col)
                {
                    CellPos cellCenter = GetCoveredOriginalCenterCell(row, col);
                    byte status = m_CellMgr.GetCellStatus(cellCenter.row, cellCenter.col);
                    byte type = BlockType.GetBlockType(status);
                    byte subtype = BlockType.GetBlockSubType(status);
                    byte blinding = BlockType.BLINDING_NOTHING;
                    byte level = BlockType.GetBlockLevel(status);
                    List<CellPos> cells = GetCoveredOriginalCells(row, col);
                    foreach (CellPos cell in cells)
                    {
                        byte status_ = m_CellMgr.GetCellStatus(cell.row, cell.col);
                        byte type_ = BlockType.GetBlockType(status_);
                        byte subtype_ = BlockType.GetBlockSubType(status_);
                        byte blinding_ = BlockType.GetBlockBlinding(status_);
                        byte level_ = BlockType.GetBlockLevel(status_);
                        if (type == BlockType.NOT_BLOCK)
                            type = type_;
                        if (subtype > subtype_)
                            subtype = subtype_;
                        if (blinding != BlockType.BLINDING_BLINDING)
                            blinding = blinding_;
                        if (level == BlockType.LEVEL_GROUND)
                            level = level_;
                    }
                    m_CellAttrs[row, col] = (byte)(type | subtype | blinding | level);
                }
            }
        }

        private int m_Radius = 0;
        private int m_CellNumPerCellView = 0;
        private int m_MaxRowCount = 0;
        private int m_MaxColCount = 0;
        private float m_RadiusLength = 0;
        private CellManager m_CellMgr = null;
        private byte[,] m_CellAttrs = null;
    }
}
