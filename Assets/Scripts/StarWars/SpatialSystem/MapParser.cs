using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using UnityEngine;
using StarWars;

namespace StarWarsSpatial
{
    public sealed class MapParser
    {
        public List<TiledData> WalkAreaList
        {
            get { return walk_area_list_; }
        }
        public List<TiledData> ObstacleAreaList
        {
            get { return obstacle_area_list_; }
        }
        public List<TiledData> ObstacleLineList
        {
            get { return obstacle_line_list_; }
        }
        public List<TiledData> ShotproofAreaList
        {
            get { return shotproof_area_list_; }
        }
        public List<TiledData> ShotproofLineList
        {
            get { return shotproof_line_list_; }
        }
        public List<TiledData> RoadblockAreaList
        {
            get { return roadblock_area_list_; }
        }
        public List<TiledData> RoadblockLineList
        {
            get { return roadblock_line_list_; }
        }
        public List<TiledData> BlindingAreaList
        {
            get { return blinding_area_list_; }
        }
        public List<TiledData> BlindingLineList
        {
            get { return blinding_line_list_; }
        }
        public List<TiledData> EnergywallAreaList
        {
            get { return energywall_area_list_; }
        }
        public List<TiledData> EnergywallLineList
        {
            get { return energywall_line_list_; }
        }
        public bool ParseTiledData(string xml_file, float width, float height)
        {
            map_width_ = width;
            map_height_ = height;

            XmlDocument xmldoc = new XmlDocument();
            System.IO.Stream ms = null;
            try
            {
                ms = StarWars.FileReaderProxy.ReadFileAsMemoryStream(xml_file);
                if (ms == null) { return false; }
                xmldoc.Load(ms);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                LogSystem.Error("config xml file {0} not find!\n{1}", xml_file, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                LogSystem.Error("parse xml file {0} error!\n{1}", xml_file, ex.Message);
                return false;
            }
            finally
            {
                if (ms != null)
                {
                    ms.Close();
                }
            }

            XmlNode root = xmldoc.SelectSingleNode("map");
            XmlNodeList objgroups = root.SelectNodes("objectgroup");

            foreach (XmlNode objgroup in objgroups)
            {
                string groupName = objgroup.Attributes["name"].Value;
                foreach (XmlNode obj in objgroup.ChildNodes)
                {
                    if (groupName == "obstacle")
                    {
                        ParseObject(obj, obstacle_area_list_, obstacle_line_list_);
                    }
                    else if (groupName == "shotproof")
                    {
                        ParseObject(obj, shotproof_area_list_, shotproof_line_list_);
                    }
                    else if (groupName == "roadblock")
                    {
                        ParseObject(obj, roadblock_area_list_, roadblock_line_list_);
                    }
                    else if (groupName == "blinding")
                    {
                        ParseObject(obj, blinding_area_list_, blinding_line_list_);
                    }
                    else if (groupName == "energywall")
                    {
                        ParseObject(obj, energywall_area_list_, energywall_line_list_);
                    }
                    else if (groupName == "underfloor2")
                    {
                        ParseObject(obj, underfloor2_area_list_, underfloor2_line_list_);
                    }
                    else if (groupName == "underfloor1")
                    {
                        ParseObject(obj, underfloor1_area_list_, underfloor1_line_list_);
                    }
                    else if (groupName == "floor1")
                    {
                        ParseObject(obj, floor1_area_list_, floor1_line_list_);
                    }
                    else if (groupName == "floor2")
                    {
                        ParseObject(obj, floor2_area_list_, floor2_line_list_);
                    }
                    else if (groupName == "floor3")
                    {
                        ParseObject(obj, floor3_area_list_, floor3_line_list_);
                    }
                    else if (groupName == "floor4")
                    {
                        ParseObject(obj, floor4_area_list_, floor4_line_list_);
                    }
                    else if (groupName == "blindage")
                    {
                        ParseObject(obj, blindage_area_list_, blindage_line_list_);
                    }
                    else
                    {
                        ParseObject(obj, walk_area_list_, walk_area_list_);
                    }
                }
            }
            return true;
        }

        public void GenerateObstacleInfo(CellManager cellMgr)
        {
            //标记所有格子为阻挡
            for (int row = 0; row < cellMgr.GetMaxRow(); row++)
            {
                for (int col = 0; col < cellMgr.GetMaxCol(); col++)
                {
                    cellMgr.SetCellStatus(row, col, (byte)(BlockType.STATIC_BLOCK | BlockType.SUBTYPE_OBSTACLE | BlockType.LEVEL_GROUND));
                }
            }
            //打开可行走区
            foreach (TiledData data in walk_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkObstacleArea(pts, cellMgr, (byte)(BlockType.NOT_BLOCK | BlockType.LEVEL_GROUND));
            }
            //标记阻挡区
            foreach (TiledData data in obstacle_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkObstacleArea(pts, cellMgr, (byte)(BlockType.STATIC_BLOCK | BlockType.SUBTYPE_OBSTACLE | BlockType.LEVEL_GROUND));
            }
            //标记阻挡线
            foreach (TiledData data in obstacle_line_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkObstacleLine(pts, cellMgr, (byte)(BlockType.STATIC_BLOCK | BlockType.SUBTYPE_OBSTACLE | BlockType.LEVEL_GROUND));
            }
            //标记防弹区
            foreach (TiledData data in shotproof_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkObstacleArea(pts, cellMgr, (byte)(BlockType.STATIC_BLOCK | BlockType.SUBTYPE_SHOTPROOF | BlockType.LEVEL_GROUND));
            }
            //标记防弹线
            foreach (TiledData data in shotproof_line_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkObstacleLine(pts, cellMgr, (byte)(BlockType.STATIC_BLOCK | BlockType.SUBTYPE_SHOTPROOF | BlockType.LEVEL_GROUND));
            }
            //标记路障区
            foreach (TiledData data in roadblock_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkObstacleArea(pts, cellMgr, (byte)(BlockType.STATIC_BLOCK | BlockType.SUBTYPE_ROADBLOCK | BlockType.LEVEL_GROUND));
            }
            //标记路障线
            foreach (TiledData data in roadblock_line_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkObstacleLine(pts, cellMgr, (byte)(BlockType.STATIC_BLOCK | BlockType.SUBTYPE_ROADBLOCK | BlockType.LEVEL_GROUND));
            }
            //标记能量墙阻挡区
            foreach (TiledData data in energywall_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkObstacleArea(pts, cellMgr, (byte)(BlockType.STATIC_BLOCK | BlockType.SUBTYPE_ENERGYWALL | BlockType.LEVEL_GROUND));
            }
            //标记能量墙阻挡线
            foreach (TiledData data in energywall_line_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkObstacleLine(pts, cellMgr, (byte)(BlockType.STATIC_BLOCK | BlockType.SUBTYPE_ENERGYWALL | BlockType.LEVEL_GROUND));
            }

            //标记视野阻挡区
            foreach (TiledData data in blinding_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkBlindingArea(pts, cellMgr, BlockType.BLINDING_BLINDING);
            }
            //标记视野阻挡线
            foreach (TiledData data in blinding_line_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkBlindingLine(pts, cellMgr, BlockType.BLINDING_BLINDING);
            }

            //标记地下二层
            foreach (TiledData data in underfloor2_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkLevelArea(pts, cellMgr, BlockType.LEVEL_UNDERFLOOR_2);
            }
            foreach (TiledData data in underfloor2_line_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkLevelLine(pts, cellMgr, BlockType.LEVEL_UNDERFLOOR_2);
            }
            //标记地下一层
            foreach (TiledData data in underfloor1_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkLevelArea(pts, cellMgr, BlockType.LEVEL_UNDERFLOOR_1);
            }
            foreach (TiledData data in underfloor1_line_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkLevelLine(pts, cellMgr, BlockType.LEVEL_UNDERFLOOR_1);
            }
            //标记二楼
            foreach (TiledData data in floor1_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkLevelArea(pts, cellMgr, BlockType.LEVEL_FLOOR_1);
            }
            foreach (TiledData data in floor1_line_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkLevelLine(pts, cellMgr, BlockType.LEVEL_FLOOR_1);
            }
            //标记三楼
            foreach (TiledData data in floor2_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkLevelArea(pts, cellMgr, BlockType.LEVEL_FLOOR_2);
            }
            foreach (TiledData data in floor2_line_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkLevelLine(pts, cellMgr, BlockType.LEVEL_FLOOR_2);
            }
            //标记四楼
            foreach (TiledData data in floor3_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkLevelArea(pts, cellMgr, BlockType.LEVEL_FLOOR_3);
            }
            foreach (TiledData data in floor3_line_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkLevelLine(pts, cellMgr, BlockType.LEVEL_FLOOR_3);
            }
            //标记五楼
            foreach (TiledData data in floor4_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkLevelArea(pts, cellMgr, BlockType.LEVEL_FLOOR_4);
            }
            foreach (TiledData data in floor4_line_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkLevelLine(pts, cellMgr, BlockType.LEVEL_FLOOR_4);
            }
            //标记六楼
            foreach (TiledData data in blindage_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkLevelArea(pts, cellMgr, BlockType.LEVEL_FLOOR_BLINDAGE);
            }
            foreach (TiledData data in blindage_line_list_)
            {
                List<Vector3> pts = data.GetPoints();
                MarkLevelLine(pts, cellMgr, BlockType.LEVEL_FLOOR_BLINDAGE);
            }
        }

        public void GenerateObstacleInfo(KdObstacleTree tree, float scale)
        {
            //可行走区
            foreach (TiledData data in walk_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                AddObstacle(pts, true, tree, scale);
            }
            //阻挡区
            foreach (TiledData data in obstacle_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                AddObstacle(pts, true, tree, scale);
            }
            //阻挡线
            foreach (TiledData data in obstacle_line_list_)
            {
                List<Vector3> pts = data.GetPoints();
                AddObstacle(pts, false, tree, scale);
            }
            //防弹区
            foreach (TiledData data in shotproof_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                AddObstacle(pts, true, tree, scale);
            }
            //防弹线
            foreach (TiledData data in shotproof_line_list_)
            {
                List<Vector3> pts = data.GetPoints();
                AddObstacle(pts, false, tree, scale);
            }
            //路障区
            foreach (TiledData data in roadblock_area_list_)
            {
                List<Vector3> pts = data.GetPoints();
                AddObstacle(pts, true, tree, scale);
            }
            //路障线
            foreach (TiledData data in roadblock_line_list_)
            {
                List<Vector3> pts = data.GetPoints();
                AddObstacle(pts, false, tree, scale);
            }
            //能量墙不影响行走，故无需加入避让阻挡信息
        }

        private void MarkObstacleArea(List<Vector3> pts, CellManager cellMgr, byte obstacle)
        {
            List<CellPos> cells = cellMgr.GetCellsInPolygon(pts);
            foreach (CellPos cell in cells)
            {
                cellMgr.SetCellStatus(cell.row, cell.col, obstacle);
            }
        }

        private void MarkObstacleLine(List<Vector3> pts, CellManager cellMgr, byte obstacle)
        {
            List<CellPos> pos_list = cellMgr.GetCellsCrossByPolyline(pts);
            foreach (CellPos pos in pos_list)
            {
                cellMgr.SetCellStatus(pos.row, pos.col, obstacle);
            }
        }

        private void MarkBlindingArea(List<Vector3> pts, CellManager cellMgr, byte blinding)
        {
            List<CellPos> cells = cellMgr.GetCellsInPolygon(pts);
            foreach (CellPos cell in cells)
            {
                byte status = cellMgr.GetCellStatus(cell.row, cell.col);
                byte typeAndSubType = BlockType.GetBlockTypeWithoutBlinding(status);
                cellMgr.SetCellStatus(cell.row, cell.col, (byte)(typeAndSubType | blinding));
            }
        }

        private void MarkBlindingLine(List<Vector3> pts, CellManager cellMgr, byte blinding)
        {
            List<CellPos> pos_list = cellMgr.GetCellsCrossByPolyline(pts);
            foreach (CellPos pos in pos_list)
            {
                byte status = cellMgr.GetCellStatus(pos.row, pos.col);
                byte typeAndSubType = BlockType.GetBlockTypeWithoutBlinding(status);
                cellMgr.SetCellStatus(pos.row, pos.col, (byte)(typeAndSubType | blinding));
            }
        }

        private void MarkLevelArea(List<Vector3> pts, CellManager cellMgr, byte level)
        {
            List<CellPos> cells = cellMgr.GetCellsInPolygon(pts);
            foreach (CellPos cell in cells)
            {
                byte status = cellMgr.GetCellStatus(cell.row, cell.col);
                byte typeAndSubType = BlockType.GetBlockTypeWithoutLevel(status);
                cellMgr.SetCellStatus(cell.row, cell.col, (byte)(typeAndSubType | level));
            }
        }

        private void MarkLevelLine(List<Vector3> pts, CellManager cellMgr, byte level)
        {
            List<CellPos> pos_list = cellMgr.GetCellsCrossByPolyline(pts);
            foreach (CellPos pos in pos_list)
            {
                byte status = cellMgr.GetCellStatus(pos.row, pos.col);
                byte typeAndSubType = BlockType.GetBlockTypeWithoutLevel(status);
                cellMgr.SetCellStatus(pos.row, pos.col, (byte)(typeAndSubType | level));
            }
        }

        private void AddObstacle(List<Vector3> pts, bool isPolygon, KdObstacleTree tree, float scale)
        {
            if (isPolygon)
            {
                List<Vector3> pts2 = new List<Vector3>();
                foreach (Vector3 pt in pts)
                {
                    pts2.Add(pt * scale);
                }
                pts2.Add(pts[0] * scale);
                tree.AddObstacle(pts2);
            }
            else
            {
                List<Vector3> pts2 = new List<Vector3>();
                foreach (Vector3 pt in pts)
                {
                    pts2.Add(pt * scale);
                }
                tree.AddObstacle(pts2);
            }
        }

        private void ParseObject(XmlNode obj, List<TiledData> polygons, List<TiledData> polylines)
        {
            TiledData td = new TiledData(map_width_, map_height_);
            if (td.CollectDataFromXml(obj))
            {
                if (td.IsPolygon)
                    polygons.Add(td);
                else
                    polylines.Add(td);
            }
        }

        private List<TiledData> walk_area_list_ = new List<TiledData>();
        private List<TiledData> obstacle_area_list_ = new List<TiledData>();
        private List<TiledData> obstacle_line_list_ = new List<TiledData>();
        private List<TiledData> shotproof_area_list_ = new List<TiledData>();
        private List<TiledData> shotproof_line_list_ = new List<TiledData>();
        private List<TiledData> roadblock_area_list_ = new List<TiledData>();
        private List<TiledData> roadblock_line_list_ = new List<TiledData>();
        private List<TiledData> blinding_area_list_ = new List<TiledData>();
        private List<TiledData> blinding_line_list_ = new List<TiledData>();
        private List<TiledData> energywall_area_list_ = new List<TiledData>();
        private List<TiledData> energywall_line_list_ = new List<TiledData>();

        private List<TiledData> underfloor2_area_list_ = new List<TiledData>();
        private List<TiledData> underfloor2_line_list_ = new List<TiledData>();
        private List<TiledData> underfloor1_area_list_ = new List<TiledData>();
        private List<TiledData> underfloor1_line_list_ = new List<TiledData>();
        private List<TiledData> floor1_area_list_ = new List<TiledData>();
        private List<TiledData> floor1_line_list_ = new List<TiledData>();
        private List<TiledData> floor2_area_list_ = new List<TiledData>();
        private List<TiledData> floor2_line_list_ = new List<TiledData>();
        private List<TiledData> floor3_area_list_ = new List<TiledData>();
        private List<TiledData> floor3_line_list_ = new List<TiledData>();
        private List<TiledData> floor4_area_list_ = new List<TiledData>();
        private List<TiledData> floor4_line_list_ = new List<TiledData>();
        private List<TiledData> blindage_area_list_ = new List<TiledData>();
        private List<TiledData> blindage_line_list_ = new List<TiledData>();

        private float map_width_ = 0;
        private float map_height_ = 0;
    }
    public class MapPatchParser
    {
        public void Load(string filename)
        {
            if (!FileReaderProxy.Exists(filename))
            {
                return;
            }
            try
            {
                using (MemoryStream ms = FileReaderProxy.ReadFileAsMemoryStream(filename))
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        while (ms.Position <= ms.Length - c_RecordSize)
                        {
                            short row = br.ReadInt16();
                            short col = br.ReadInt16();
                            byte obstacle = br.ReadByte();
                            byte oldObstacle = br.ReadByte();
                            Update(row, col, obstacle, oldObstacle);
                        }
                        br.Close();
                    }
                    ms.Close();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }
        public void Save(string filename)
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        foreach (KeyValuePair<int, ObstacleInfo> pair in m_PatchDatas)
                        {
                            byte obstacle = pair.Value.m_UpdatedValue;
                            byte oldObstacle = pair.Value.m_OldValue;
                            if (obstacle != oldObstacle)
                            {
                                int key = pair.Key;
                                int row, col;
                                DecodeKey(key, out row, out col);
                                bw.Write((short)row);
                                bw.Write((short)col);
                                bw.Write(obstacle);
                                bw.Write(oldObstacle);
                            }
                        }
                        bw.Flush();
                        bw.Close();
                    }
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                LogSystem.Error("{0}\n{1}", e.Message, e.StackTrace);
            }
        }
        public bool Exist(int row, int col)
        {
            if (row >= 10000 || col >= 10000)
            {
                throw new Exception("Can't support huge map (row>=10000 || col>=10000)");
            }
            int key = EncodeKey(row, col);
            return m_PatchDatas.ContainsKey(key);
        }
        public void Update(int row, int col, byte obstacle)
        {
            if (row >= 10000 || col >= 10000)
            {
                throw new Exception("Can't support huge map (row>=10000 || col>=10000)");
            }
            int key = EncodeKey(row, col);
            if (m_PatchDatas.ContainsKey(key))
            {
                m_PatchDatas[key].m_UpdatedValue = obstacle;
            }
        }
        public void Update(int row, int col, byte obstacle, byte oldObstacle)
        {
            if (row >= 10000 || col >= 10000)
            {
                throw new Exception("Can't support huge map (row>=10000 || col>=10000)");
            }
            int key = EncodeKey(row, col);
            if (m_PatchDatas.ContainsKey(key))
            {
                m_PatchDatas[key].m_OldValue = oldObstacle;
                m_PatchDatas[key].m_UpdatedValue = obstacle;
            }
            else
            {
                m_PatchDatas.Add(key, new ObstacleInfo(oldObstacle, obstacle));
            }
        }
        public void VisitPatches(MyAction<int, int, byte> visitor)
        {
            foreach (KeyValuePair<int, ObstacleInfo> pair in m_PatchDatas)
            {
                byte obstacle = pair.Value.m_UpdatedValue;
                byte oldObstacle = pair.Value.m_OldValue;
                if (obstacle != oldObstacle)
                {
                    int key = pair.Key;
                    int row, col;
                    DecodeKey(key, out row, out col);
                    visitor(row, col, obstacle);
                }
            }
        }

        private int EncodeKey(int row, int col)
        {
            return row * 10000 + col;
        }
        private void DecodeKey(int key, out int row, out int col)
        {
            row = key / 10000;
            col = key % 10000;
        }
        private class ObstacleInfo
        {
            public byte m_OldValue;
            public byte m_UpdatedValue;

            public ObstacleInfo(byte oldValue, byte updatedValue)
            {
                m_OldValue = oldValue;
                m_UpdatedValue = updatedValue;
            }
        }
        private SortedDictionary<int, ObstacleInfo> m_PatchDatas = new SortedDictionary<int, ObstacleInfo>();

        private const int c_RecordSize = sizeof(short) + sizeof(short) + sizeof(byte) * 2;
    }
}
