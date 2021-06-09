using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarWars
{
    public enum MovementMode : int
    {
        Normal = 0,
        LowSpeed,
        HighSpeed
    }

    public class MovementStateInfo
    {
        static RelMoveDir[] RelMoveDirRange = new RelMoveDir[] {
            RelMoveDir.Forward,
            RelMoveDir.Forward | RelMoveDir.Rightward,
            RelMoveDir.Rightward,
            RelMoveDir.Rightward | RelMoveDir.Backward,
            RelMoveDir.Backward,
            RelMoveDir.Leftward | RelMoveDir.Backward,
            RelMoveDir.Leftward,
            RelMoveDir.Leftward | RelMoveDir.Forward
        };

        public bool IsMoving
        {
            get { return m_IsMoving; }
            set
            {
                m_IsMoving = value;
                if (m_IsMoving)
                    m_IsMoveMeetObstacle = false;
            }
        }
        public bool IsSkillMoving
        {
            get { return m_IsSkillMoving; }
            set { m_IsSkillMoving = value; }
        }
        public MovementMode MovementMode
        {
            get { return m_MovementMode; }
            set { m_MovementMode = value; }
        }
        public bool IsMoveMeetObstacle
        {
            get { return m_IsMoveMeetObstacle; }
            set { m_IsMoveMeetObstacle = value; }
        }
        public Vector3 TargetPosition
        {
            get { return m_TargetPosition; }
            set { m_TargetPosition = value; }
        }
        public float PositionX
        {
            get { return m_Position.x; }
            set { m_Position.x = value; }
        }
        public float PositionY
        {
            get { return m_Position.y; }
            set { m_Position.y = value; }
        }
        public float PositionZ
        {
            get { return m_Position.z; }
            set { m_Position.z = value; }
        }
        public float MoveDirCosAngle
        {
            get { return m_MoveDirCosAngle; }
        }
        public float MoveDirSinAngle
        {
            get { return m_MoveDirSinAngle; }
        }
        public float FaceDirCosAngle
        {
            get { return m_FaceDirCosAngle; }
        }
        public float FaceDirSinAngle
        {
            get { return m_FaceDirSinAngle; }
        }
        public float CalcDistancSquareToTarget()
        {
            return Geometry.DistanceSquare(m_Position, m_TargetPosition);
        }
        public void SetPosition(float x, float y, float z)
        {
            m_Position.x = x;
            m_Position.y = y;
            m_Position.z = z;
        }
        public void SetPosition(Vector3 pos)
        {
            m_Position = pos;
        }
        public Vector3 GetPosition3D()
        {
            return m_Position;
        }
        public void SetPosition2D(float x, float y)
        {
            m_Position.x = x;
            m_Position.z = y;
        }
        public void SetPosition2D(Vector2 pos)
        {
            m_Position.x = pos.x;
            m_Position.z = pos.y;
        }
        public Vector2 GetPosition2D()
        {
            return new Vector2(m_Position.x, m_Position.z);
        }
        public void SetFaceDir(float rot)
        {
            m_FaceDir = rot;
            m_FaceDirCosAngle = (float)Math.Cos(rot);
            m_FaceDirSinAngle = (float)Math.Sin(rot);
        }
        public float GetFaceDir()
        {
            return m_FaceDir;
        }
        public void SetMoveDir(float dir)
        {
            m_MoveDir = dir;
            m_MoveDirCosAngle = (float)Math.Cos(dir);
            m_MoveDirSinAngle = (float)Math.Sin(dir);
        }

        public float GetMoveDir()
        {
            return m_MoveDir;
        }
        public Vector3 GetMoveDir3D()
        {
            float dir = GetMoveDir();
            return new Vector3((float)Math.Sin(dir), 0, (float)Math.Cos(dir));
        }

        public void SetWantFaceDir(float dir)
        {
            m_WantFaceDir = dir;
        }
        public float GetWantFaceDir()
        {
            return m_WantFaceDir;
        }

        public Vector3 GetFaceDir3D()
        {
            float dir = GetFaceDir();
            return new Vector3((float)Math.Sin(dir), 0, (float)Math.Cos(dir));
        }
        public void StartMove()
        {
            IsMoving = true;
        }
        public void StopMove()
        {
            IsMoving = false;
        }
        public MovementStateInfo()
        {
            m_Position = new Vector3();
        }
        public void Reset()
        {
            m_Position = new Vector3();
            m_TargetPosition = new Vector3();
            m_IsMoving = false;
            m_IsSkillMoving = false;
            m_IsMoveMeetObstacle = false;
            m_FaceDir = 0;
            m_MoveDir = 0;
            m_WantFaceDir = 0;
            m_MovementMode = MovementMode.Normal;
        }

        public RelMoveDir RelativeMoveDir
        {
            get
            {
                return RelMoveDir.Forward;
            }
        }

        private bool IsIn(float num, float left, float right)
        {
            if (num >= left && num <= right)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool m_IsMoving = false;
        private bool m_IsSkillMoving = false;
        private bool m_IsMoveMeetObstacle = false;
        private Vector3 m_Position;
        private Vector3 m_TargetPosition;
        private float m_FaceDir = 0;
        private float m_WantFaceDir = 0;
        private float m_FaceDirCosAngle = 1;
        private float m_FaceDirSinAngle = 0;
        private float m_MoveDir = 0;
        private float m_MoveDirCosAngle = 1;
        private float m_MoveDirSinAngle = 0;
        private MovementMode m_MovementMode = MovementMode.Normal;
    }
}
