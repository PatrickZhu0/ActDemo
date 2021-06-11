/**
 * @file   SpaceManager.cs
 * @author 
 * @date   
 * 
 * @brief 空间管理系统
 * 
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using StarWars;

namespace StarWarsSpatial
{
    public sealed class SpatialSystem : ISpatialSystem
    {
        public SpatialSystem()
        {
            current_spatial_id_ = spatial_system_id_++;
        }

        public void Init(string map_file, Vector3[] reachableSet)
        {
            map_file_ = map_file;
            if (!cell_manager_.Init(StarWars.HomePath.GetAbsolutePath(map_file_)))
            {
                cell_manager_.Init(1024, 1024, 0.5f);
                LogSystem.Error("Init SpatialSystem from map file failed: {0}", map_file_);
            }
            if (null != reachableSet)
            {
                m_ReachableSet.Clear();
                m_ReachableSet.Build(reachableSet);
            }
        }

        public void LoadPatch(string patch_file)
        {
            MapPatchParser patchParser = new MapPatchParser();
            patchParser.Load(StarWars.HomePath.GetAbsolutePath(patch_file));
            patchParser.VisitPatches((int row, int col, byte obstacle) =>
            {
                SetCellStatus(row, col, obstacle);
            });
        }

        public void LoadObstacle(string file, float scale)
        {
            if (!GlobalVariables.Instance.IsClient)
            {
                //客户端暂时不读避让信息（xml库在ios上有问题。。）
                float mapwidth = cell_manager_.GetMapWidth();
                float mapheight = cell_manager_.GetMapHeight();
                MapParser mapParser = new MapParser();
                mapParser.ParseTiledData(StarWars.HomePath.GetAbsolutePath(file), mapwidth * scale, mapheight * scale);
                mapParser.GenerateObstacleInfo(m_KdObstacleTree, scale);
            }
            m_KdObstacleTree.Build();
        }

        public void Reset()
        {
            space_obj_collection_dict_.Clear();
            delete_obj_buffer_.Clear();
            add_obj_buffer_.Clear();
            cell_manager_.Reset();
            m_KdTree.Clear();
            m_KdObstacleTree.Clear();
        }

        /// <summary>
        /// collide module 心跳tick, 处理相关计算
        /// </summary>
        public void Tick()
        {
            // remove objects in delete buffer
            foreach (KeyValuePair<uint, ISpaceObject> pair in delete_obj_buffer_)
            {
                // 删除与当前物体碰撞的物体信息
                ISpaceObject obj2 = pair.Value;
                foreach (ISpaceObject spaceobj in obj2.GetCollideObjects())
                {
                    spaceobj.OnDepartObject(obj2);
                }
                obj2.GetCollideObjects().Clear();
                space_obj_collection_dict_.Remove(pair.Key);
            }
            delete_obj_buffer_.Clear();

            // add new obj
            foreach (KeyValuePair<uint, ISpaceObject> pair in add_obj_buffer_)
            {
                if (!space_obj_collection_dict_.ContainsKey(pair.Key))
                {
                    space_obj_collection_dict_.Add(pair.Key, pair.Value);
                }
            }
            add_obj_buffer_.Clear();

            IList<ISpaceObject> obj_list = null;
            if (space_obj_collection_dict_.Count > 0)
            {
                ISpaceObject[] temp = new ISpaceObject[space_obj_collection_dict_.Count];
                space_obj_collection_dict_.Values.CopyTo(temp, 0);
                obj_list = temp;
            }
            else
            {
                obj_list = new List<ISpaceObject>();
            }

            //构造空间索引
            if (obj_list.Count > 0)
            {
                m_KdTree.Build(obj_list);
            }
            else
            {
                m_KdTree.Clear();
            }
            bool isCountTick = false;
            long curTime = TimeUtility.GetServerMilliseconds();
            if (m_LastCountTime + c_CountInterval < curTime)
            {
                m_LastCountTime = curTime;
                isCountTick = true;
            }
            int userCt = 0;
            int npcCt = 0;
            int bulletCt = 0;
            foreach (ISpaceObject hiter in obj_list)
            {
                if (isCountTick)
                {
                    switch (hiter.GetObjType())
                    {
                        case SpatialObjType.kUser:
                            ++userCt;
                            break;
                        case SpatialObjType.kNPC:
                            ++npcCt;
                            break;
                        case SpatialObjType.kBullet:
                            ++bulletCt;
                            break;
                    }
                }
            }
            if (isCountTick && !GlobalVariables.Instance.IsClient)
            {
                LogSystem.Debug("SpatialSystem object count:{0} user:{1} npc:{2} bullet:{3}", obj_list.Count, userCt, npcCt, bulletCt);
            }
        }

        public float Width
        {
            get
            {
                return cell_manager_.GetMapWidth();
            }
        }
        public float Height
        {
            get
            {
                return cell_manager_.GetMapHeight();
            }
        }

        /// <summary>
        /// 增加物体
        /// </summary>
        /// <param name="obj">增加的物体</param>
        /// <returns>成功返回SUCCESS，失败返回其它错误码</returns>
        public RetCode AddObj(ISpaceObject obj)
        {
            if (obj == null)
            {
                return RetCode.NULL_POINTER;
            }
            if (add_obj_buffer_.ContainsKey(CalcSpaceObjectKey(obj)))
            {
                return RetCode.OBJECT_EXIST;
            }
            add_obj_buffer_.Add(CalcSpaceObjectKey(obj), obj);
            return RetCode.SUCCESS;
        }

        /** 
         * 删除物体obj
         * @param objid 删除的物体
         * @return 成功返回true, 失败返回false
         */
        public bool RemoveObj(ISpaceObject obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (delete_obj_buffer_.ContainsKey(CalcSpaceObjectKey(obj)))
            {
                return false;
            }
            delete_obj_buffer_.Add(CalcSpaceObjectKey(obj), obj);
            return true;
        }

        public List<ISpaceObject> GetObjectInShape(Vector3 pos,
                                                  float direction,
                                                  Shape area)
        {
            area.Transform(pos, (float)Math.Cos(direction), (float)Math.Sin(direction));
            return GetObjectInArea(area);
        }
        public List<ISpaceObject> GetObjectInShape(Vector3 pos, float direction, Shape area, MyFunc<float, ISpaceObject, bool> pred)
        {
            area.Transform(pos, (float)Math.Cos(direction), (float)Math.Sin(direction));
            return GetObjectInArea(area, pred);
        }
        public void VisitObjectInShape(Vector3 pos, float direction, Shape area, MyAction<float, ISpaceObject> visitor)
        {
            area.Transform(pos, (float)Math.Cos(direction), (float)Math.Sin(direction));
            VisitObjectInArea(area, visitor);
        }
        public void VisitObjectInShape(Vector3 pos, float direction, Shape area, MyFunc<float, ISpaceObject, bool> visitor)
        {
            area.Transform(pos, (float)Math.Cos(direction), (float)Math.Sin(direction));
            VisitObjectInArea(area, visitor);
        }

        public List<ISpaceObject> GetObjectInArea(Shape area)
        {
            return GetObjectInArea(area, (distSqr, obj) => true);
        }
        public List<ISpaceObject> GetObjectInArea(Shape area, MyFunc<float, ISpaceObject, bool> pred)
        {
            List<ISpaceObject> list = new List<ISpaceObject>();
            VisitObjectInArea(area, (float distSqr, ISpaceObject obj) =>
            {
                if (pred(distSqr, obj))
                {
                    list.Add(obj);
                }
            });
            return list;
        }
        public void VisitObjectInArea(Shape area, MyAction<float, ISpaceObject> visitor)
        {
            m_KdTree.Query(area.GetCenter(), (float)area.GetRadius(), (float distSqr, KdTreeObject kdObj) =>
            {
                ISpaceObject obj = kdObj.SpaceObject;
                if (null != obj)
                {
                    if (IsObjectInArea(obj, area))
                    {
                        visitor(distSqr, obj);
                    }
                }
            });
        }
        public void VisitObjectInArea(Shape area, MyFunc<float, ISpaceObject, bool> visitor)
        {
            m_KdTree.Query(area.GetCenter(), (float)area.GetRadius(), (float distSqr, KdTreeObject kdObj) =>
            {
                ISpaceObject obj = kdObj.SpaceObject;
                if (null != obj)
                {
                    if (IsObjectInArea(obj, area))
                    {
                        return visitor(distSqr, obj);
                    }
                }
                return true;
            });
        }

        public bool IsObjectInArea(ISpaceObject obj, Shape area)
        {
            if (null == obj || null == area) return false;
            Shape shape = obj.GetCollideShape();
            if (null == shape)
            {
                LogSystem.Error("IsObjectInArea:obj({0}[{1}])'s shape is null", obj.GetID(), obj.GetObjType());
                return false;
            }
            return collide_detector_.Intersect(shape, area);
        }

        public List<ISpaceObject> GetObjectInCircle(Vector3 center, float radius)
        {
            return GetObjectInCircle(center, radius, (distSqr, obj) => true);
        }
        public List<ISpaceObject> GetObjectInCircle(Vector3 center, float radius, MyFunc<float, ISpaceObject, bool> pred)
        {
            List<ISpaceObject> objects_in_circle = new List<ISpaceObject>();
            VisitObjectInCircle(center, radius, (float distSqr, ISpaceObject obj) =>
            {
                if (pred(distSqr, obj))
                {
                    objects_in_circle.Add(obj);
                }
            });
            return objects_in_circle;
        }
        public void VisitObjectInCircle(Vector3 center, float radius, MyAction<float, ISpaceObject> visitor)
        {
            m_KdTree.Query(center, (float)radius, (float distSqr, KdTreeObject kdObj) =>
            {
                ISpaceObject obj = kdObj.SpaceObject;
                if (null != obj)
                {
                    visitor(distSqr, obj);
                }
            });
        }
        public void VisitObjectInCircle(Vector3 center, float radius, MyFunc<float, ISpaceObject, bool> visitor)
        {
            m_KdTree.Query(center, (float)radius, (float distSqr, KdTreeObject kdObj) =>
            {
                ISpaceObject obj = kdObj.SpaceObject;
                if (null != obj)
                {
                    return visitor(distSqr, obj);
                }
                return true;
            });
        }

        /** 
         * 取得与以startpos为起点，endpos为终点的线段碰撞的物体
         * @param startpos 线段起点
         * @param endpos 线段终点
         * @return 返回碰撞的物体ID列表
         */
        public List<ISpaceObject> GetObjectCrossByLine(Vector3 startpos, Vector3 endpos)
        {
            return GetObjectCrossByLine(startpos, endpos, false, false, false, false);
        }
        public List<ISpaceObject> GetObjectCrossByLineForPass(Vector3 startpos, Vector3 endpos)
        {
            return GetObjectCrossByLine(startpos, endpos, true, true, true, false);
        }
        public List<ISpaceObject> GetObjectCrossByLineForShoot(Vector3 startpos, Vector3 endpos)
        {
            return GetObjectCrossByLine(startpos, endpos, false, true, true, false);
        }
        public List<ISpaceObject> GetObjectCrossByLineForLeap(Vector3 startpos, Vector3 endpos)
        {
            return GetObjectCrossByLine(startpos, endpos, false, false, true, false);
        }
        public List<ISpaceObject> GetObjectCrossByLine(Vector3 startpos, Vector3 endpos, MyFunc<float, ISpaceObject, bool> pred)
        {
            return GetObjectCrossByLine(startpos, endpos, false, false, false, false, pred);
        }
        public List<ISpaceObject> GetObjectCrossByLineForPass(Vector3 startpos, Vector3 endpos, MyFunc<float, ISpaceObject, bool> pred)
        {
            return GetObjectCrossByLine(startpos, endpos, true, true, true, false, pred);
        }
        public List<ISpaceObject> GetObjectCrossByLineForShoot(Vector3 startpos, Vector3 endpos, MyFunc<float, ISpaceObject, bool> pred)
        {
            return GetObjectCrossByLine(startpos, endpos, false, true, true, false, pred);
        }
        public List<ISpaceObject> GetObjectCrossByLineForLeap(Vector3 startpos, Vector3 endpos, MyFunc<float, ISpaceObject, bool> pred)
        {
            return GetObjectCrossByLine(startpos, endpos, false, false, true, false, pred);
        }

        private List<ISpaceObject> GetObjectCrossByLine(Vector3 startpos, Vector3 endpos, bool includeCanShoot, bool includeCanLeap, bool includeCantLeap, bool includeLevel)
        {
            return GetObjectCrossByLine(startpos, endpos, includeCanShoot, includeCanLeap, includeCantLeap, includeLevel, (distSqr, obj) => true);
        }
        private List<ISpaceObject> GetObjectCrossByLine(Vector3 startpos, Vector3 endpos, bool includeCanShoot, bool includeCanLeap, bool includeCantLeap, bool includeLevel, MyFunc<float, ISpaceObject, bool> pred)
        {
            List<ISpaceObject> crossed_objects = new List<ISpaceObject>();
            Line line = new Line(startpos, endpos);
            line.IdenticalTransform();

            // 考虑阻挡对结果的影响
            float distance_with_block = -1;
            bool considerblock = (includeCanShoot || includeCanLeap || includeCantLeap);
            if (considerblock)
            {
                distance_with_block = (float)GetFirstBlockCellDistance(startpos, endpos, includeCanShoot, includeCanLeap, includeCantLeap, includeLevel);
                if (distance_with_block < 0)
                {
                    considerblock = false;
                }
            }

            m_KdTree.Query(line.GetCenter(), (float)line.GetRadius(), (float distSqr, KdTreeObject kdObj) =>
            {
                ISpaceObject obj = kdObj.SpaceObject;
                if (null != obj)
                {
                    Shape shape = obj.GetCollideShape();
                    if (null != shape)
                    {
                        if (collide_detector_.IsLineCrossShape(line, obj.GetCollideShape()) && pred(distSqr, obj))
                        {
                            if (considerblock)
                            {
                                if (GetDistanceWithPos(obj, startpos) < distance_with_block)
                                {
                                    crossed_objects.Add(obj);
                                }
                            }
                            else
                            {
                                crossed_objects.Add(obj);
                            }
                        }
                    }
                    else
                    {
                        LogSystem.Error("GetObjectCrossByLine:obj({0}[{1}])'s shape is null", obj.GetID(), obj.GetObjType());
                    }
                }
            });

            crossed_objects.Sort((obj1, obj2) =>
            {
                float d1 = GetDistanceWithPos(obj1, startpos);
                float d2 = GetDistanceWithPos(obj2, startpos);
                if (d1 > d2)
                {
                    return 1;
                }
                else if (Math.Abs(d1 - d2) < 0.0001)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            });
            return crossed_objects;
        }

        public List<ISpaceObject> GetObjectInPolygon(IList<Vector3> polygon)
        {
            return GetObjectInPolygon(polygon, (distSqr, obj) => true);
        }
        public List<ISpaceObject> GetObjectInPolygon(IList<Vector3> polygon, MyFunc<float, ISpaceObject, bool> pred)
        {
            List<ISpaceObject> objects_in_polygon = new List<ISpaceObject>();
            VisitObjectInPolygon(polygon, (float distSqr, ISpaceObject obj) =>
            {
                if (pred(distSqr, obj))
                {
                    objects_in_polygon.Add(obj);
                }
            });
            return objects_in_polygon;
        }
        public void VisitObjectInPolygon(IList<Vector3> polygon, MyAction<float, ISpaceObject> visitor)
        {
            Vector3 centroid;
            float radius = Geometry.CalcPolygonCentroidAndRadius(polygon, 0, polygon.Count, out centroid);

            if (radius > Geometry.c_FloatPrecision)
            {
                m_KdTree.Query(centroid, radius, (float distSqr, KdTreeObject kdObj) =>
                {
                    ISpaceObject obj = kdObj.SpaceObject;
                    if (null != obj)
                    {
                        if (Geometry.PointInPolygon(kdObj.Position, polygon, 0, polygon.Count) >= 0)
                        {
                            visitor(distSqr, obj);
                        }
                    }
                });
            }
        }
        public void VisitObjectInPolygon(IList<Vector3> polygon, MyFunc<float, ISpaceObject, bool> visitor)
        {
            Vector3 centroid;
            float radius = Geometry.CalcPolygonCentroidAndRadius(polygon, 0, polygon.Count, out centroid);

            if (radius > Geometry.c_FloatPrecision)
            {
                m_KdTree.Query(centroid, radius, (float distSqr, KdTreeObject kdObj) =>
                {
                    ISpaceObject obj = kdObj.SpaceObject;
                    if (null != obj)
                    {
                        if (Geometry.PointInPolygon(kdObj.Position, polygon, 0, polygon.Count) >= 0)
                        {
                            return visitor(distSqr, obj);
                        }
                    }
                    return true;
                });
            }
        }

        public bool CheckRoadBlock(Vector3 start, Vector3 end, Vector3[] polygon, out Vector3 roadBlock)
        {
            bool ret = false;
            List<CellPos> cells = cell_manager_.GetCellsCrossByLine(start, end);
            Vector3 blockPos = new Vector3();
            VisitCellsCrossByLine(start, end, (int row, int col) =>
            {
                byte status = cell_manager_.cells_arr_[row, col];
                byte block = BlockType.GetBlockType(status);
                byte subtype = BlockType.GetBlockSubType(status);
                if (BlockType.NOT_BLOCK != block && BlockType.SUBTYPE_ROADBLOCK == subtype)
                {
                    Vector3 pos = cell_manager_.GetCellCenter(row, col);
                    if (Geometry.PointInPolygon(pos, polygon, 0, polygon.Length) > 0)
                    {
                        blockPos = pos;
                        ret = true;
                        LogSystem.Debug("CheckRoadBlock {0} -> {1} find roadblock {2}", start.ToString(), end.ToString(), pos.ToString());
                        return false;
                    }
                }
                return true;
            });
            roadBlock = blockPos;
            return ret;
        }

        public bool CheckCollide(ISpaceObject hiter, ISpaceObject dest)
        {
            Shape srcShape = hiter.GetCollideShape();
            Shape destShape = dest.GetCollideShape();
            if (null == srcShape)
            {
                LogSystem.Error("CheckCollide:hiter obj({0}[{1}])'s shape is null", hiter.GetID(), hiter.GetObjType());
                return false;
            }
            if (null == destShape)
            {
                LogSystem.Error("CheckCollide:dest obj({0}[{1}])'s shape is null", dest.GetID(), dest.GetObjType());
                return false;
            }
            bool is_collide = collide_detector_.Intersect(srcShape, destShape);
            bool is_allready_collide = IsAllreadyCollide(hiter, dest);

            if (is_collide)
            {     // 碰撞
                if (!is_allready_collide)
                {  // 之前没有碰撞,记录下相撞的物体
                   //LogSystem.Info("{4} spatial collide obj({0},{1}) with obj({2},{3})",
                   //           hiter.GetID(), hiter.GetObjType(), dest.GetID(), 
                   //           dest.GetObjType(), current_spatial_id_);
                    hiter.OnCollideObject(dest);
                    dest.OnCollideObject(hiter);
                }
            }
            else if (is_allready_collide)
            {  // 当前没有碰撞，但是之前碰撞，即分离的情况
               //LogSystem.Info("{4} spatial depart obj({0},{1}) with obj({2},{3})",
               //           hiter.GetID(), hiter.GetObjType(),
               //           dest.GetID(), dest.GetObjType(), current_spatial_id_);
                hiter.OnDepartObject(dest);
                dest.OnDepartObject(hiter);
            }
            return is_collide;
        }

        public bool CanPass(ISpaceObject obj, Vector3 to)
        {
            return CanPass(obj.GetPosition(), to);
        }
        public bool CanPass(Vector3 curPos, Vector3 to)
        {
            CellPos cur_cell;
            cell_manager_.GetCell(curPos, out cur_cell.row, out cur_cell.col);
            CellPos to_cell;
            cell_manager_.GetCell(to, out to_cell.row, out to_cell.col);
            if (cur_cell.row == to_cell.row && cur_cell.col == to_cell.col)
            {
                return true;
            }
            byte curStatus = cell_manager_.GetCellStatus(cur_cell.row, cur_cell.col);
            byte block = BlockType.GetBlockType(curStatus);
            byte subtype = BlockType.GetBlockSubType(curStatus);
            if (block != BlockType.NOT_BLOCK && subtype != BlockType.SUBTYPE_ENERGYWALL)
            {
                return true;
            }
            byte status = cell_manager_.GetCellStatus(to_cell.row, to_cell.col);
            block = BlockType.GetBlockType(status);
            subtype = BlockType.GetBlockSubType(status);
            if (block != BlockType.NOT_BLOCK && subtype != BlockType.SUBTYPE_ENERGYWALL)
            {
                //LogSystem.Debug("CanPass ({0},{1})->({2},{3}), target is blocked {4}", cur_cell.row, cur_cell.col, to_cell.row, to_cell.col, block);
                return false;
            }
            if (Math.Abs(cur_cell.row - to_cell.row) >= 1 || Math.Abs(cur_cell.col - to_cell.col) >= 1)
            {
                Vector3 from = cell_manager_.GetCellCenter(cur_cell.row, cur_cell.col);
                to = cell_manager_.GetCellCenter(to_cell.row, to_cell.col);

                bool ret = true;
                cell_manager_.VisitCellsCrossByLine(from, to, (row, col) =>
                {
                    status = cell_manager_.cells_arr_[row, col];
                    block = BlockType.GetBlockType(status);
                    subtype = BlockType.GetBlockSubType(status);
                    if (block != BlockType.NOT_BLOCK && subtype != BlockType.SUBTYPE_ENERGYWALL)
                    {
                        ret = false;
                        //LogSystem.Debug("CanPass ({0},{1})->({2},{3}), ({4},{5}) is blocked {6}", cur_cell.row, cur_cell.col, to_cell.row, to_cell.col, row, col, block);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                });
                return ret;
            }
            return true;
        }

        public bool CanPass(Vector3 pos, int depth)
        {
            return GetCellMapView(depth).CanPass(pos);
        }

        public bool CanShoot(Vector3 pos, int depth)
        {
            return GetCellMapView(depth).CanShoot(pos);
        }

        public bool CanLeap(Vector3 pos, int depth)
        {
            return GetCellMapView(depth).CanLeap(pos);
        }

        public bool CanSee(Vector3 pos, int depth)
        {
            return GetCellMapView(depth).CanSee(pos);
        }

        public List<Vector3> FindPath(Vector3 from, Vector3 to, int bodysize)
        {
            List<Vector3> result = new List<Vector3>();
            CellPos start, end;
            ICellMapView view = cell_manager_.GetCellMapView(bodysize);
            CellPos start0;
            view.GetCell(from, out start0.row, out start0.col);
            view.GetCell(to, out end.row, out end.col);
            start = view.GetFirstWalkableCell(from, to);
            if (start.row >= 0 && start.col >= 0)
            {
                List<CellPos> path = PathFinder.FindPath(cell_manager_.GetCellMapView(bodysize), start, end);
                if (start0.row != start.row || start0.col != start.col)
                    result.Add(view.GetCellCenter(start0.row, start0.col));
                for (int ix = path.Count - 1; ix >= 0; --ix)
                {
                    CellPos pos = path[ix];
                    result.Add(view.GetCellCenter(pos.row, pos.col));
                }
            }
            else
            {
                result.Add(view.GetCellCenter(start0.row, start0.col));
                result.Add(view.GetCellCenter(end.row, end.col));
            }
            return result;
        }

        public List<Vector3> FindPath(Vector3 from, Vector3 to, int scope, int bodysize)
        {
            List<Vector3> result = new List<Vector3>();
            CellPos start, end, minCell, maxCell;
            ICellMapView view = cell_manager_.GetCellMapView(bodysize);
            CellPos start0;
            view.GetCell(from, out start0.row, out start0.col);
            view.GetCell(to, out end.row, out end.col);
            start = view.GetFirstWalkableCell(from, to);
            if (start.row >= 0 && start.col >= 0)
            {
                minCell.row = Math.Min(start.row, end.row) - scope;
                minCell.col = Math.Min(start.col, end.col) - scope;
                maxCell.row = Math.Max(start.row, end.row) + scope;
                maxCell.col = Math.Max(start.col, end.col) + scope;
                if (minCell.row < 0)
                    minCell.row = 0;
                if (minCell.col < 0)
                    minCell.col = 0;
                if (maxCell.row >= view.MaxRowCount)
                    maxCell.row = view.MaxRowCount - 1;
                if (maxCell.col >= view.MaxColCount)
                    maxCell.col = view.MaxColCount - 1;
                List<CellPos> path = PathFinder.FindPath(cell_manager_.GetCellMapView(bodysize), start, end, minCell, maxCell);
                if (start0.row != start.row || start0.col != start.col)
                    result.Add(view.GetCellCenter(start0.row, start0.col));
                for (int ix = path.Count - 1; ix >= 0; --ix)
                {
                    CellPos pos = path[ix];
                    result.Add(view.GetCellCenter(pos.row, pos.col));
                }
            }
            return result;
        }

        public Vector3 ComputeVelocity(ISpaceObject obj, Vector3 prefDir, float stepTime, float maxSpeed, float neighborDist, bool isUsingAvoidanceVelocity)
        {
            Vector3 newVelocity = m_RvoAlgorithm.ComputeNewVelocity(obj, prefDir, stepTime, m_KdTree, m_KdObstacleTree, maxSpeed, neighborDist, isUsingAvoidanceVelocity);
            return newVelocity;
        }

        public bool CanShoot(ISpaceObject obj, Vector3 to)
        {
            Vector3 from = obj.GetPosition();
            return CanShoot(from, to);
        }

        public bool CanShoot(Vector3 from, Vector3 to)
        {
            bool canShoot = true;
            byte status = cell_manager_.GetCellStatus(from);
            byte lvl0 = BlockType.GetBlockLevel(status);
            if (BlockType.GetBlockType(status) == BlockType.NOT_BLOCK)
            {
                cell_manager_.VisitCellsCrossByLine(from, to, (int row, int col) =>
                {
                    status = cell_manager_.cells_arr_[row, col];
                    byte block = BlockType.GetBlockType(status);
                    byte subtype = BlockType.GetBlockSubType(status);
                    byte lvl1 = BlockType.GetBlockLevel(status);
                    if (block != BlockType.NOT_BLOCK && subtype != BlockType.SUBTYPE_ROADBLOCK)
                    {
                        canShoot = false;
                        return false;
                    }
                    /*if (lvl1 > lvl0) {
                      canShoot = false;
                      break;
                    }*/
                    return true;
                });
            }
            return canShoot;
        }

        public bool CanLeap(ISpaceObject obj, Vector3 to)
        {
            Vector3 from = obj.GetPosition();
            return CanLeap(from, to);
        }

        public bool CanLeap(Vector3 from, Vector3 to)
        {
            bool canLeap = true;
            byte status = cell_manager_.GetCellStatus(from);
            if (BlockType.GetBlockType(status) == BlockType.NOT_BLOCK)
            {
                cell_manager_.VisitCellsCrossByLine(from, to, (int row, int col) =>
                {
                    status = cell_manager_.cells_arr_[row, col];
                    byte blockType = BlockType.GetBlockType(status);
                    byte subtype = BlockType.GetBlockSubType(status);
                    if (blockType != BlockType.NOT_BLOCK && subtype == BlockType.SUBTYPE_OBSTACLE)
                    {
                        canLeap = false;
                        return false;
                    }
                    return true;
                });
            }
            return canLeap;
        }

        public bool CanSee(ISpaceObject obj, Vector3 to)
        {
            Vector3 from = obj.GetPosition();
            return CanSee(from, to);
        }

        public bool CanSee(Vector3 from, Vector3 to)
        {
            bool canSee = true;
            byte status = cell_manager_.GetCellStatus(from);
            byte lvl0 = BlockType.GetBlockLevel(status);
            cell_manager_.VisitCellsCrossByLine(from, to, (row, col) =>
            {
                status = cell_manager_.cells_arr_[row, col];
                byte blockType = BlockType.GetBlockType(status);
                byte blinding = BlockType.GetBlockBlinding(status);
                byte lvl1 = BlockType.GetBlockLevel(status);
                if (BlockType.BLINDING_BLINDING == blinding)
                {
                    canSee = false;
                    return false;
                }
                if (lvl1 > lvl0)
                {
                    canSee = false;
                    return false;
                }
                return true;
            });
            return canSee;
        }

        public bool RayCastForPass(Vector3 from, Vector3 to, out Vector3 hitpoint)
        {
            return RayCast(from, to, true, true, true, false, out hitpoint);
        }

        public bool RayCastForShoot(Vector3 from, Vector3 to, out Vector3 hitpoint)
        {
            return RayCast(from, to, false, true, true, false, out hitpoint);
        }

        public bool RayCastForLeap(Vector3 from, Vector3 to, out Vector3 hitpoint)
        {
            return RayCast(from, to, false, false, true, false, out hitpoint);
        }

        private bool RayCast(Vector3 from, Vector3 to, bool includeCanShoot, bool includeCanLeap, bool includeCantLeap, bool includeLevel, out Vector3 hitpoint)
        {
            CellPos cell;
            bool r = cell_manager_.RayCast(from, to, includeCanShoot, includeCanLeap, includeCantLeap, includeLevel, out cell);
            if (r)
            {
                hitpoint = cell_manager_.GetCellCenter(cell.row, cell.col);
            }
            else
            {
                hitpoint = new Vector3();
            }
            return r;
        }

        public ICellMapView GetCellMapView(int radius)
        {
            return cell_manager_.GetCellMapView(radius);
        }
        public byte GetCellStatus(int row, int col)
        {
            return cell_manager_.GetCellStatus(row, col);
        }
        public void SetCellStatus(int row, int col, byte status)
        {
            cell_manager_.SetCellStatus(row, col, status);
        }
        public List<CellPos> GetCellsInPolygon(IList<Vector3> pts)
        {
            return cell_manager_.GetCellsInPolygon(pts);
        }
        public void VisitCellsInPolygon(IList<Vector3> pts, MyAction<int, int> visitor)
        {
            cell_manager_.VisitCellsInPolygon(pts, visitor);
        }
        public List<CellPos> GetCellsCrossByLine(Vector3 start, Vector3 end)
        {
            return cell_manager_.GetCellsCrossByLine(start, end);
        }
        public void VisitCellsCrossByLine(Vector3 start, Vector3 end, MyAction<int, int> visitor)
        {
            cell_manager_.VisitCellsCrossByLine(start, end, visitor);
        }
        public void VisitCellsCrossByLine(Vector3 start, Vector3 end, MyFunc<int, int, bool> visitor)
        {
            cell_manager_.VisitCellsCrossByLine(start, end, visitor);
        }
        public List<CellPos> GetCellsCrossByPolyline(IList<Vector3> pts)
        {
            return cell_manager_.GetCellsCrossByPolyline(pts);
        }
        public void VisitCellsCrossByPolyline(IList<Vector3> pts, MyAction<int, int> visitor)
        {
            cell_manager_.VisitCellsCrossByPolyline(pts, visitor);
        }
        public List<CellPos> GetCellsCrossByPolygon(IList<Vector3> pts)
        {
            return cell_manager_.GetCellsCrossByPolygon(pts);
        }
        public void VisitCellsCrossByPolygon(IList<Vector3> pts, MyAction<int, int> visitor)
        {
            cell_manager_.VisitCellsCrossByPolygon(pts, visitor);
        }
        public List<CellPos> GetCellsInCircle(Vector3 center, float radius)
        {
            return cell_manager_.GetCellsInCircle(center, radius);
        }
        public void VisitCellsInCircle(Vector3 center, float radius, MyAction<int, int> visitor)
        {
            cell_manager_.VisitCellsInCircle(center, radius, visitor);
        }
        public List<CellPos> GetCellsCrossByCircle(Vector3 center, float radius)
        {
            return cell_manager_.GetCellsCrossByCircle(center, radius);
        }
        public void VisitCellsCrossByCircle(Vector3 center, float radius, MyAction<int, int> visitor)
        {
            cell_manager_.VisitCellsCrossByCircle(center, radius, visitor);
        }
        public Vector3 CalcNearstReachablePoint(Vector3 src, float radius)
        {
            Vector3 ret = src;
            float minDistPow = float.MaxValue;
            m_ReachableSet.Query(src, radius, (float distSq, Vector3 pt) =>
            {
                float distPow = Geometry.DistanceSquare(src, pt);
                if (distPow < minDistPow)
                {
                    ret = pt;
                    minDistPow = distPow;
                }
            });
            return ret;
        }
        public Vector3 CalcNearstReachablePoint(Vector3 src, Vector3 target, float radius)
        {
            Vector3 ret = target;
            float minDistPow = float.MaxValue;
            m_ReachableSet.Query(src, radius, (float distSq, Vector3 pt) =>
            {
                if (Geometry.DotMultiply(src, target, pt) < 0)
                {
                    float distPow = Geometry.DistanceSquare(src, pt);
                    if (distPow < minDistPow)
                    {
                        ret = pt;
                        minDistPow = distPow;
                    }
                }
            });
            return ret;
        }

        public CellManager GetCellManager() { return cell_manager_; }
        public KdTree KdTree
        {
            get { return m_KdTree; }
        }
        public KdObstacleTree KdObstacleTree
        {
            get { return m_KdObstacleTree; }
        }

        // private functions------------------------------------------------------
        private bool IsPassableCollide(ISpaceObject hiter, ISpaceObject dest)
        {
            if ((hiter.GetObjType() == SpatialObjType.kNPC || hiter.GetObjType() == SpatialObjType.kUser) &&
                (dest.GetObjType() == SpatialObjType.kNPC || dest.GetObjType() == SpatialObjType.kUser))
            {
                return false;
            }
            if (hiter.GetID() == dest.GetID() && hiter.GetObjType() == dest.GetObjType())
            {
                return false;
            }
            return true;
        }

        private bool IsAllreadyCollide(ISpaceObject hiter, ISpaceObject dest)
        {
            foreach (ISpaceObject collide_obj in hiter.GetCollideObjects())
            {
                if (collide_obj.GetID() == dest.GetID() && collide_obj.GetObjType() == dest.GetObjType())
                {
                    return true;
                }
            }
            return false;
        }

        // 取得被线段穿过的第一个阻挡区域
        private float GetFirstBlockCellDistance(Vector3 start, Vector3 end, bool includeCanShoot, bool includeCanLeap, bool includeCantLeap, bool includeLevel)
        {
            float min_distance = -1;
            CellPos cell;
            if (cell_manager_.RayCast(start, end, includeCanShoot, includeCanLeap, includeCantLeap, includeLevel, out cell))
            {
                min_distance = GetDistanceWithCell(start, cell);
            }
            return min_distance;
        }

        private float GetDistanceWithCell(Vector3 pos, CellPos cell)
        {
            Vector3 center = cell_manager_.GetCellCenter(cell.row, cell.col);
            return (float)(Math.Pow((center.x - pos.x), 2) + Math.Pow((center.z - pos.z), 2));
        }

        private float GetDistanceWithPos(ISpaceObject obj, Vector3 pos)
        {
            return (float)Math.Pow((obj.GetPosition().x - pos.x), 2) + (float)Math.Pow((obj.GetPosition().z - pos.z), 2);
        }

        // private attributes-----------------------------------------------------
        private Dictionary<uint, ISpaceObject> space_obj_collection_dict_ = new Dictionary<uint, ISpaceObject>();
        private Dictionary<uint, ISpaceObject> delete_obj_buffer_ = new Dictionary<uint, ISpaceObject>();
        private Dictionary<uint, ISpaceObject> add_obj_buffer_ = new Dictionary<uint, ISpaceObject>();
        private Collide collide_detector_ = new Collide();
        private CellManager cell_manager_ = new CellManager();
        private string map_file_ = "";
        private int current_spatial_id_ = 0;
        private KdTree m_KdTree = new KdTree();
        private KdObstacleTree m_KdObstacleTree = new KdObstacleTree();
        private RvoAlgorithm m_RvoAlgorithm = new RvoAlgorithm();
        private PointKdTree m_ReachableSet = new PointKdTree();
        private static int spatial_system_id_ = 0;

        private const long c_CountInterval = 10000;
        private long m_LastCountTime = 0;

        private uint CalcSpaceObjectKey(ISpaceObject obj)
        {
            //注意：这里假设obj.GetID()<0x10000000，应该不会有超过这个的id
            return ((uint)obj.GetObjType() << 24) | obj.GetID();
        }
    }
} // namespace StarWars
