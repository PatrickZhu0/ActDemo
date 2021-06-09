/**
 * @file   Collide.cs
 * @author
 * @date   
 * 
 * @brief  碰撞检测算法类，提供多边形，圆等凸多边形的碰撞检测算法
 * 
 * 
 */


using System;
using System.Collections.Generic;

using UnityEngine;

namespace StarWarsSpatial
{
    public sealed class Collide
    {
        //判断两个多边形是否分离
        public bool SeperatePolygon(Vector3 axis_vector, Polygon one, Polygon two)
        {
            float p00 = 1 - (float)Math.Pow(axis_vector.x, 2);
            float p01 = -axis_vector.x * axis_vector.z;
            float p10 = p01;
            float p11 = 1 - (float)Math.Pow(axis_vector.z, 2);
            List<Vector3> one_list = one.world_vertex();
            Vector3 one_min = new Vector3(0, 0, 0);
            Vector3 one_max = new Vector3(0, 0, 0);
            for (int i = 0; i < one_list.Count; i++)
            {
                Vector3 pass_pos = new Vector3(0, 0, 0);
                pass_pos.x = (float)(one_list[i].x * p00 + one_list[i].z * p10);
                pass_pos.z = (float)(one_list[i].x * p01 + one_list[i].z * p11);
                if (i == 0)
                {
                    one_min = pass_pos;
                    one_max = pass_pos;
                }
                if (pass_pos.x < one_min.x ||
                    (pass_pos.x == one_min.x && pass_pos.z < one_min.z))
                {
                    one_min = pass_pos;
                }
                if (pass_pos.x > one_max.x ||
                    (pass_pos.x == one_max.x && pass_pos.z > one_max.z))
                {
                    one_max = pass_pos;
                }
            }

            List<Vector3> two_list = two.world_vertex();
            Vector3 two_min = new Vector3(0, 0, 0);
            Vector3 two_max = new Vector3(0, 0, 0);
            for (int i = 0; i < two_list.Count; i++)
            {
                Vector3 pass_pos = new Vector3(0, 0, 0);
                pass_pos.x = (float)(two_list[i].x * p00 + two_list[i].z * p10);
                pass_pos.z = (float)(two_list[i].x * p01 + two_list[i].z * p11);
                if (i == 0)
                {
                    two_min = pass_pos;
                    two_max = pass_pos;
                }
                if (pass_pos.x < two_min.x ||
                    (pass_pos.x == two_min.x && pass_pos.z < two_min.z))
                {
                    two_min = pass_pos;
                }
                if (pass_pos.x > two_max.x ||
                    (pass_pos.x == two_max.x && pass_pos.z > two_max.z))
                {
                    two_max = pass_pos;
                }
            }

            if (one_min.x > two_max.x ||
                (one_min.x == two_max.x && one_min.z > two_max.z))
            {
                return true;
            }
            if (one_max.x < two_min.x ||
                (one_max.x == two_min.x && one_max.z < two_min.z))
            {
                return true;
            }
            return false;
        }

        //判断矩形和圆是否分离
        public bool SeperatePolygonCircle(Vector3 axis_vector, Polygon rect, Circle circle)
        {
            float p00 = 1 - (float)Math.Pow(axis_vector.x, 2);
            float p01 = -axis_vector.x * axis_vector.z;
            float p10 = p01;
            float p11 = 1 - (float)Math.Pow(axis_vector.z, 2);

            //计算多边形的最大最小透影
            List<Vector3> one_list = rect.world_vertex();
            Vector3 one_min = new Vector3(0, 0, 0);
            Vector3 one_max = new Vector3(0, 0, 0);
            for (int i = 0; i < one_list.Count; i++)
            {
                Vector3 pass_pos = new Vector3(0, 0, 0);
                pass_pos.x = (float)(one_list[i].x * p00 + one_list[i].z * p10);
                pass_pos.z = (float)(one_list[i].x * p01 + one_list[i].z * p11);
                if (i == 0)
                {
                    one_min = pass_pos;
                    one_max = pass_pos;
                }
                if (pass_pos.x < one_min.x ||
                    (pass_pos.x == one_min.x && pass_pos.z < one_min.z))
                {
                    one_min = pass_pos;
                }
                if (pass_pos.x > one_max.x ||
                    (pass_pos.x == one_max.x && pass_pos.z > one_max.z))
                {
                    one_max = pass_pos;
                }
            }

            //计算圆的最大最小透影
            float two_min_x, two_min_y,
                  two_max_x, two_max_y;
            Vector3 center = circle.world_center_pos();
            Vector3 center_pass = new Vector3(0, 0, 0);
            center_pass.x = (float)(center.x * p00 + center.z * p10);
            center_pass.z = (float)(center.x * p01 + center.z * p11);
            float half_x = circle.radius() * Math.Abs(axis_vector.z);
            two_min_x = center_pass.x - half_x;
            two_max_x = center_pass.x + half_x;
            float half_y = circle.radius() * Math.Abs(axis_vector.x);
            two_min_y = center_pass.z - half_y;
            two_max_y = center_pass.z + half_y;

            //判断是否分离
            if (one_min.x > two_max_x ||
                (one_min.x == two_max_x && one_min.z > two_max_y))
            {
                return true;
            }
            if (one_max.x < two_min_x ||
                (one_max.x == two_min_x && one_max.z < two_min_y))
            {
                return true;
            }
            return false;
        }

        //判断两个圆是否碰撞
        public bool IntersectCircle(Circle one, Circle two)
        {
            Vector3 one_center = one.world_center_pos();
            Vector3 two_center = two.world_center_pos();
            float center_distance_square = one_center.sqrMagnitude + two_center.sqrMagnitude;
            float radius = one.radius() + two.radius();
            if (radius * radius < center_distance_square)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //取得两点的垂直单位向量
        public Vector3 GetVerticalVector(Vector3 one, Vector3 two)
        {
            Vector3 vertical_vect = new Vector3(0, 0, 0);
            float length = (float)Math.Sqrt(
                                      Math.Pow((one.x - two.x), 2) +
                                      Math.Pow((one.z - two.z), 2));
            vertical_vect.x = (float)((two.z - one.z) / length);
            vertical_vect.z = (float)((one.x - two.x) / length);
            return vertical_vect;
        }

        public bool IsLineCrossShape(Line line, Shape shape)
        {
            Vector3 axis = GetVerticalVector(line.GetStartPos(), line.GetEndPos());
            if (IsSeperateByAxis(axis, line, shape))
            {
                return false;
            }
            axis = GetVerticalVector(new Vector3(0, 0, 0), axis);
            if (IsSeperateByAxis(axis, line, shape))
            {
                return false;
            }
            if (shape.GetShapeType() == ShapeType.kShapeCircle)
            {
                Vector3 center = ((Circle)shape).world_center_pos();
                axis = GetVerticalVector(center, line.GetStartPos());
                if (IsSeperateByAxis(axis, line, shape))
                {
                    return false;
                }
                axis = GetVerticalVector(line.GetEndPos(), center);
                if (IsSeperateByAxis(axis, line, shape))
                {
                    return false;
                }
            }
            if (shape.GetShapeType() == ShapeType.kShapePolygon)
            {
                List<Vector3> rect_list = ((Polygon)shape).world_vertex();
                for (int i = 0; i < rect_list.Count; i++)
                {
                    if (i == rect_list.Count - 1)
                    {
                        axis = GetVerticalVector(rect_list[i], rect_list[0]);
                    }
                    else
                    {
                        axis = GetVerticalVector(rect_list[i], rect_list[i + 1]);
                    }
                    if (IsSeperateByAxis(axis, line, shape))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public List<Vector3> GetCollidePoint(Line line, Shape shape)
        {
            List<Vector3> collide_points = new List<Vector3>();
            Vector3 pos = new Vector3();
            if (shape.GetShapeType() == ShapeType.kShapePolygon)
            {
                List<Vector3> vect_list = ((Polygon)shape).world_vertex();
                for (int i = 0; i < vect_list.Count; i++)
                {
                    Line edge;
                    if (i == vect_list.Count - 1)
                    {
                        edge = new Line(vect_list[i], vect_list[0]);
                    }
                    else
                    {
                        edge = new Line(vect_list[i], vect_list[i + 1]);
                    }
                    if (GetHitPoint(line, edge, out pos))
                    {
                        collide_points.Add(pos);
                    }
                }
            }
            Vector3 startpos = line.GetStartPos();
            collide_points.Sort((one, two) =>
            {
                if (Math.Abs(one.x - startpos.x) < Math.Abs(two.x - startpos.x) ||
                  (Math.Abs(one.x - startpos.x) <= 0.0001 && Math.Abs(one.z - startpos.z) < Math.Abs(two.z - startpos.z)))
                {
                    return -1;
                }
                else if (Math.Abs(one.x - two.x) < 0.0001 && Math.Abs(one.z - two.z) < 0.0001)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            });
            return collide_points;
        }


        public bool Intersect(Shape one, Shape two)
        {
            if (one.GetShapeType() == ShapeType.kShapeCircle &&
                two.GetShapeType() == ShapeType.kShapeCircle)
            {//两个都是圆
                return IntersectCircle((Circle)one, (Circle)two);
            }
            else if (one.GetShapeType() == ShapeType.kLine)
            {
                return IsLineCrossShape((Line)one, two);
            }
            else if (two.GetShapeType() == ShapeType.kLine)
            {
                return IsLineCrossShape((Line)two, one);
            }
            else if (one.GetShapeType() == ShapeType.kShapeCircle)
            {//第一个是圆，第二个是矩形
                List<Vector3> rect_list = ((Polygon)two).world_vertex();
                Vector3 center_pos = ((Circle)one).world_center_pos();
                for (int i = 0; i < rect_list.Count; i++)
                {
                    Vector3 vector = new Vector3(0, 0, 0);
                    if (i == rect_list.Count - 1)
                    {
                        vector = GetVerticalVector(rect_list[i], rect_list[0]);
                    }
                    else
                    {
                        vector = GetVerticalVector(rect_list[i], rect_list[i + 1]);
                    }
                    if (SeperatePolygonCircle(vector, (Polygon)two, (Circle)one))
                    {
                        return false;
                    }
                }
                for (int i = 0; i < rect_list.Count; i++)
                {
                    Vector3 vector = new Vector3(0, 0, 0);
                    vector = GetVerticalVector(center_pos, rect_list[i]);
                    if (SeperatePolygonCircle(vector, (Polygon)two, (Circle)one))
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (two.GetShapeType() == ShapeType.kShapeCircle)
            {//第一个是矩形，第二个是圆
                List<Vector3> rect_list = ((Polygon)one).world_vertex();
                Vector3 center_pos = ((Circle)two).world_center_pos();
                for (int i = 0; i < rect_list.Count; i++)
                {
                    Vector3 vector = new Vector3(0, 0, 0);
                    if (i == rect_list.Count - 1)
                    {
                        vector = GetVerticalVector(rect_list[i], rect_list[0]);
                    }
                    else
                    {
                        vector = GetVerticalVector(rect_list[i], rect_list[i + 1]);
                    }
                    if (SeperatePolygonCircle(vector, (Polygon)one, (Circle)two))
                    {
                        return false;
                    }
                }
                for (int i = 0; i < rect_list.Count; i++)
                {
                    Vector3 vector = new Vector3(0, 0, 0);
                    vector = GetVerticalVector(center_pos, rect_list[i]);
                    if (SeperatePolygonCircle(vector, (Polygon)one, (Circle)two))
                    {
                        return false;
                    }
                }
                return true;
            }

            //两个都矩形
            //判断第一个矩形边的透影
            List<Vector3> one_list = ((Polygon)one).world_vertex();
            for (int i = 0; i < one_list.Count; i++)
            {
                Vector3 vector = new Vector3(0, 0, 0);
                if (i == one_list.Count - 1)
                {
                    vector = GetVerticalVector(one_list[i], one_list[0]);
                }
                else
                {
                    vector = GetVerticalVector(one_list[i], one_list[i + 1]);
                }
                if (SeperatePolygon(vector, (Polygon)one, (Polygon)two))
                {
                    return false;
                }
            }

            //判断第二个矩形边的透影
            List<Vector3> two_list = ((Polygon)two).world_vertex();
            for (int i = 0; i < two_list.Count; i++)
            {
                Vector3 vector = new Vector3(0, 0, 0);
                if (i == two_list.Count - 1)
                {
                    vector = GetVerticalVector(two_list[i], two_list[0]);
                }
                else
                {
                    vector = GetVerticalVector(two_list[i], two_list[i + 1]);
                }
                if (SeperatePolygon(vector, (Polygon)one, (Polygon)two))
                {
                    return false;
                }
            }
            return true;
        }

        // y = kx + c
        // ax + by = d
        private bool GetHitPoint(Line one, Line two, out Vector3 hitpos)
        {
            hitpos = new Vector3();
            Vector3 vd1 = new Vector3(one.GetEndPos().x - one.GetStartPos().x, 0, one.GetEndPos().z - one.GetStartPos().z);
            float a1 = vd1.z;
            float b1 = -vd1.x;
            float d1 = one.GetStartPos().x * vd1.z - one.GetStartPos().z * vd1.x;
            Vector3 vd2 = new Vector3(two.GetEndPos().x - two.GetStartPos().x, 0, two.GetEndPos().z - two.GetStartPos().z);
            float a2 = vd2.z;
            float b2 = -vd2.x;
            float d2 = two.GetStartPos().x * vd2.z - two.GetStartPos().z * vd2.x;
            float denominator = a1 * b2 - a2 * b1;
            if (Math.Abs(denominator) >= 0.0001)
            {
                hitpos.x = (b2 * d1 - b1 * d2) / denominator;
                hitpos.z = (a1 * d2 - a2 * d1) / denominator;
                if (IsPointInLine(hitpos, one) && IsPointInLine(hitpos, two))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            float c1, c2;
            if (Math.Abs(a1) <= 0.0001)
            {
                c1 = d1 / b1;
                c2 = d2 / b2;
            }
            else
            {
                c1 = d1 / a1;
                c2 = d2 / a2;
            }
            // parallel
            if (Math.Abs(c1 - c2) > 0.0001)
            {
                return false;
            }
            //overlaped
            bool isstartin = IsPointInLine(two.GetStartPos(), one);
            bool isendin = IsPointInLine(two.GetEndPos(), one);
            if (isstartin && isendin)
            {
                if (Math.Abs(two.GetStartPos().x - one.GetStartPos().x) < Math.Abs(two.GetEndPos().x - one.GetStartPos().x) ||
                  (Math.Abs(one.GetStartPos().x - one.GetEndPos().x) <= 0.0001 &&
                  Math.Abs(two.GetStartPos().z - one.GetStartPos().z) < Math.Abs(two.GetEndPos().z - one.GetStartPos().z)))
                {
                    hitpos = two.GetStartPos();
                    return true;
                }
                else
                {
                    hitpos = two.GetEndPos();
                    return true;
                }
            }
            else if (isstartin)
            {
                hitpos = two.GetStartPos();
                return true;
            }
            else if (isendin)
            {
                hitpos = two.GetEndPos();
                return true;
            }
            else
            {
                return false;
            }
        }

        // constraint: the point must on the straight line but maybe not bettwen the line.start and line.end
        // this is judge whether it's bettwen or not
        private bool IsPointInLine(Vector3 pos, Line line)
        {
            Vector3 min, max;
            if (line.GetStartPos().x < line.GetEndPos().x ||
                (Math.Abs(line.GetStartPos().x - line.GetEndPos().x) <= 0.0001 &&
                line.GetStartPos().z < line.GetEndPos().z))
            {
                min = line.GetStartPos();
                max = line.GetEndPos();
            }
            else
            {
                min = line.GetEndPos();
                max = line.GetStartPos();
            }
            return !IsShadowSeperate(pos, pos, min, max);
        }

        private bool IsSeperateByAxis(Vector3 axis, Line line, Shape shape)
        {
            Vector3 line_shadow_min, line_shadow_max;
            Vector3 shape_shadow_min, shape_shadow_max;
            line.GetShadowOnAxis(axis, out line_shadow_min, out line_shadow_max);
            shape.GetShadowOnAxis(axis, out shape_shadow_min, out shape_shadow_max);
            return IsShadowSeperate(line_shadow_min, line_shadow_max, shape_shadow_min, shape_shadow_max);
        }

        private bool IsShadowSeperate(Vector3 one_min, Vector3 one_max, Vector3 two_min, Vector3 two_max)
        {
            //判断是否分离
            if (one_min.x > two_max.x ||
                (one_min.x == two_max.x && one_min.z > two_max.z))
            {
                return true;
            }
            if (one_max.x < two_min.x ||
                (one_max.x == two_min.x && one_max.z < two_min.z))
            {
                return true;
            }
            return false;
        }
    }
}
