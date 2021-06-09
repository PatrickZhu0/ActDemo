/**
 * @file   Shape.cs
 * @author 
 * @date   
 * 
 * @brief  碰撞形状定义
 * 
 * 
 */

using System;
using System.Collections.Generic;

using UnityEngine;

namespace StarWarsSpatial
{
    public enum ShapeType
    {
        kShapePolygon = 0,
        kShapeCircle,
        kLine,
        kShapeUnknow
    };

    public enum PolygonType
    {
        kPolygonRect = 0,
        kPolygonUnknow
    };

    public class Shape : ICloneable
    {
        public virtual ShapeType GetShapeType()
        {
            return ShapeType.kShapeUnknow;
        }
        public virtual Vector3 GetCenter()
        {
            return new Vector3(0, 0, 0);
        }
        public virtual float GetRadius()
        {
            return 1.0f;
        }

        /// <summary>
        /// 获取在坐标轴上的投影
        /// </summary>
        /// <param name="axis_vector"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public virtual void GetShadowOnAxis(Vector3 axis_vector, out Vector3 min, out Vector3 max)
        {
            min = new Vector3(0, 0, 0);
            max = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// 坐标变换
        /// </summary>
        /// <param name="sys_pos"></param>
        /// <param name="relative_pos"></param>
        /// <param name="cos_angle"></param>
        /// <param name="sin_angle"></param>
        /// <returns></returns>
        public static Vector3 TransformToWorldPos(Vector3 sys_pos,
                                                Vector3 relative_pos, float cos_angle, float sin_angle)
        {
            Vector3 new_pos = new Vector3(0, sys_pos.y, 0);
            new_pos.x = (float)(relative_pos.x * cos_angle + relative_pos.z * sin_angle + sys_pos.x);
            new_pos.y = sys_pos.y + relative_pos.y;
            new_pos.z = (float)(relative_pos.z * cos_angle - relative_pos.x * sin_angle + sys_pos.z);
            return new_pos;
        }

        public virtual void IdenticalTransform()
        {
        }

        public virtual void Transform(Vector3 owner_sys_pos, float cos_angle, float sin_angle)
        {
        }

        public virtual object Clone()
        {
            return new Shape();
        }
    }

    public class Line : Shape
    {
        public Line(Vector3 start, Vector3 end)
        {
            start_ = start;
            end_ = end;
            center_ = (start_ + end_) / 2;
            radius_ = Vector3.Distance(start_, end_) / 2;
        }

        public override ShapeType GetShapeType()
        {
            return ShapeType.kLine;
        }
        public override Vector3 GetCenter()
        {
            return center_;
        }
        public override float GetRadius()
        {
            return radius_;
        }

        public override void IdenticalTransform()
        {
            world_vertex_.Clear();
            world_vertex_.Add(start_);
            world_vertex_.Add(end_);
        }

        public override void Transform(Vector3 owner_sys_pos, float cos_angle, float sin_angle)
        {
            world_vertex_.Clear();
            world_vertex_.Add(Shape.TransformToWorldPos(owner_sys_pos, start_, cos_angle, sin_angle));
            world_vertex_.Add(Shape.TransformToWorldPos(owner_sys_pos, end_, cos_angle, sin_angle));

            center_ = (world_vertex_[0] + world_vertex_[1]) / 2;
        }

        public override void GetShadowOnAxis(Vector3 axis_vector, out Vector3 min, out Vector3 max)
        {
            float p00 = 1 - (float)Math.Pow(axis_vector.x, 2);
            float p01 = -axis_vector.x * axis_vector.z;
            float p10 = p01;
            float p11 = 1 - (float)Math.Pow(axis_vector.z, 2);

            List<Vector3> one_list = world_vertex_;
            min = new Vector3(0, 0, 0);
            max = new Vector3(0, 0, 0);
            for (int i = 0; i < one_list.Count; i++)
            {
                Vector3 pass_pos = new Vector3(0, 0, 0);
                pass_pos.x = (float)(one_list[i].x * p00 + one_list[i].z * p10);
                pass_pos.z = (float)(one_list[i].x * p01 + one_list[i].z * p11);
                if (i == 0)
                {
                    min = pass_pos;
                    max = pass_pos;
                }
                if (pass_pos.x < min.x ||
                    (pass_pos.x == min.x && pass_pos.z < min.z))
                {
                    min = pass_pos;
                }
                if (pass_pos.x > max.x ||
                    (pass_pos.x == max.x && pass_pos.z > max.z))
                {
                    max = pass_pos;
                }
            }
        }

        public override object Clone()
        {
            return new Line(start_, end_);
        }

        public Vector3 GetStartPos() { return start_; }
        public Vector3 GetEndPos() { return end_; }

        public virtual List<Vector3> world_vertex() { return world_vertex_; }
        protected List<Vector3> world_vertex_ = new List<Vector3>();
        private Vector3 start_;
        private Vector3 end_;
        private Vector3 center_;
        private float radius_ = 0;
    }

    public class Polygon : Shape
    {
        public override ShapeType GetShapeType()
        {
            return ShapeType.kShapePolygon;
        }
        public override Vector3 GetCenter()
        {
            RecalcCenterAndRadius();
            return center_;
        }
        public override float GetRadius()
        {
            RecalcCenterAndRadius();
            return radius_;
        }

        public virtual PolygonType GetPolygonType()
        {
            return PolygonType.kPolygonUnknow;
        }

        public void AddVertex(Vector3 pos)
        {
            if (relation_vertex_ == null)
            {
                relation_vertex_ = new List<Vector3>();
            }
            relation_vertex_.Add(pos);
            MarkRecalc();
        }

        public void AddVertex(float x, float y)
        {
            if (relation_vertex_ == null)
            {
                relation_vertex_ = new List<Vector3>();
            }
            relation_vertex_.Add(new Vector3((float)x, 0, (float)y));
            MarkRecalc();
        }

        public override void IdenticalTransform()
        {
            if (world_vertex_ == null)
            {
                world_vertex_ = new List<Vector3>();
            }
            world_vertex_.Clear();
            world_vertex_.AddRange(relation_vertex_);
        }

        public override void Transform(Vector3 owner_sys_pos, float cos_angle, float sin_angle)
        {
            if (world_vertex_ == null)
            {
                world_vertex_ = new List<Vector3>();
            }
            world_vertex_.Clear();
            for (int i = 0; i < relation_vertex_.Count; i++)
            {
                world_vertex_.Add(Shape.TransformToWorldPos(
                                                          owner_sys_pos, relation_vertex_[i], cos_angle, sin_angle));
            }
            MarkRecalc();
            RecalcCenterAndRadius();
            center_ = Shape.TransformToWorldPos(owner_sys_pos, center_, cos_angle, sin_angle);
        }

        public override void GetShadowOnAxis(Vector3 axis_vector, out Vector3 min, out Vector3 max)
        {
            float p00 = 1 - (float)Math.Pow(axis_vector.x, 2);
            float p01 = -axis_vector.x * axis_vector.z;
            float p10 = p01;
            float p11 = 1 - (float)Math.Pow(axis_vector.z, 2);

            List<Vector3> one_list = world_vertex_;
            min = new Vector3(0, 0, 0);
            max = new Vector3(0, 0, 0);
            for (int i = 0; i < one_list.Count; i++)
            {
                Vector3 pass_pos = new Vector3(0, 0, 0);
                pass_pos.x = (float)(one_list[i].x * p00 + one_list[i].z * p10);
                pass_pos.z = (float)(one_list[i].x * p01 + one_list[i].z * p11);
                if (i == 0)
                {
                    min = pass_pos;
                    max = pass_pos;
                }
                if (pass_pos.x < min.x ||
                    (pass_pos.x == min.x && pass_pos.z < min.z))
                {
                    min = pass_pos;
                }
                if (pass_pos.x > max.x ||
                    (pass_pos.x == max.x && pass_pos.z > max.z))
                {
                    max = pass_pos;
                }
            }
        }

        public override object Clone()
        {
            Polygon pl = new Polygon();
            for (int i = 0; i < relation_vertex_.Count; i++)
            {
                pl.AddVertex(relation_vertex_[i]);
            }
            return pl;
        }

        public virtual List<Vector3> world_vertex() { return world_vertex_; }

        protected void MarkRecalc()
        {
            need_recalc_ = true;
        }
        protected void RecalcCenterAndRadius()
        {
            if (need_recalc_)
            {
                float x = 0, y = 0, z = 0;
                foreach (Vector3 pt in relation_vertex_)
                {
                    x += pt.x;
                    y += pt.y;
                    z += pt.z;
                }
                int ct = relation_vertex_.Count;
                center_ = new Vector3(x / ct, y / ct, z / ct);
                radius_ = 0;
                foreach (Vector3 pt in relation_vertex_)
                {
                    radius_ = Math.Max(radius_, Vector3.Distance(center_, pt));
                }
                need_recalc_ = false;
            }
        }

        protected List<Vector3> relation_vertex_;
        protected List<Vector3> world_vertex_;
        private bool need_recalc_ = true;
        private Vector3 center_;
        private float radius_ = 0;
    }

    public class Rect : Polygon
    {
        public Rect(float width, float height)
        {
            this.width_ = width;
            this.height_ = height;
            relation_vertex_ = new List<Vector3>();
            world_vertex_ = new List<Vector3>();
            AddVertex(-width / 2, height / 2);
            AddVertex(-width / 2, -height / 2);
            AddVertex(width / 2, -height / 2);
            AddVertex(width / 2, height / 2);
        }

        /// <summary>
        /// 创建以pos为中心，width为宽度， height为高度的矩形
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Rect(Vector3 pos, float width, float height)
        {
            this.width_ = width;
            this.height_ = height;
            relation_vertex_ = new List<Vector3>();
            world_vertex_ = new List<Vector3>();
            AddVertex(pos.x - width / 2, pos.z + height / 2);
            AddVertex(pos.x - width / 2, pos.z - height / 2);
            AddVertex(pos.x + width / 2, pos.z - height / 2);
            AddVertex(pos.x + width / 2, pos.z + height / 2);
        }

        public override PolygonType GetPolygonType()
        {
            return PolygonType.kPolygonRect;
        }

        public override object Clone()
        {
            return new Rect(width_, height_);
        }

        public float width() { return width_; }
        public float height() { return height_; }

        private float width_;
        private float height_;
    }

    public class Circle : Shape
    {
        public Circle(Vector3 centerpos, float radius)
        {
            relate_center_pos_ = centerpos;
            world_center_pos_ = relate_center_pos_;
            radius_ = radius;
        }

        public override ShapeType GetShapeType()
        {
            return ShapeType.kShapeCircle;
        }
        public override Vector3 GetCenter()
        {
            return world_center_pos_;
        }
        public override float GetRadius()
        {
            return radius_;
        }

        public override void IdenticalTransform()
        {
            world_center_pos_ = relate_center_pos_;
        }

        public override void Transform(Vector3 owner_sys_pos, float cos_angle, float sin_angle)
        {
            world_center_pos_ = Shape.TransformToWorldPos(owner_sys_pos,
                                                        relate_center_pos_, cos_angle, sin_angle);
        }

        public override void GetShadowOnAxis(Vector3 axis_vector, out Vector3 min, out Vector3 max)
        {
            float p00 = 1 - (float)Math.Pow(axis_vector.x, 2);
            float p01 = -axis_vector.x * axis_vector.z;
            float p10 = p01;
            float p11 = 1 - (float)Math.Pow(axis_vector.z, 2);

            //计算圆的最大最小透影
            min = new Vector3(0, 0, 0);
            max = new Vector3(0, 0, 0);
            Vector3 center = world_center_pos_;
            Vector3 center_pass = new Vector3(0, 0, 0);
            center_pass.x = (float)(center.x * p00 + center.z * p10);
            center_pass.z = (float)(center.x * p01 + center.z * p11);
            float half_x = radius_ * Math.Abs(axis_vector.z);
            min.x = (float)(center_pass.x - half_x);
            max.x = (float)(center_pass.x + half_x);
            float half_y = radius_ * Math.Abs(axis_vector.x);
            min.z = (float)(center_pass.z - half_y);
            max.z = (float)(center_pass.z + half_y);
        }

        public override object Clone()
        {
            return new Circle(relate_center_pos_, radius_);
        }

        public void set_relate_center_pos(Vector3 pos) { relate_center_pos_ = pos; }
        public Vector3 relate_center_pos() { return relate_center_pos_; }
        public void set_world_center_pos(Vector3 pos) { world_center_pos_ = pos; }
        public Vector3 world_center_pos() { return world_center_pos_; }
        public float radius() { return radius_; }

        private Vector3 relate_center_pos_;    //相对的圆心位置
        private Vector3 world_center_pos_;     //世界圆心位置
        private float radius_ = 0;
    }
}
