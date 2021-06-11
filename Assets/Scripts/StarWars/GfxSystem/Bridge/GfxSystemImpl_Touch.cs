using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarWars
{
    public sealed partial class GfxSystem
    {
        public delegate void FingerStatus(GestureArgs e);
        public static FingerStatus OnFingerDown;
        public static FingerStatus OnFingerUp;
        private bool IsTouchPosChangedImpl()
        {
            if (m_LastTouchPos.x == m_CurTouchPos.x && m_LastTouchPos.y == m_CurTouchPos.y)
            {
                return false;
            }
            return true;
        }
        private float GetTouchXImpl()
        {
            return m_CurTouchPos.x;
        }
        private float GetTouchYImpl()
        {
            return m_CurTouchPos.y;
        }
        private float GetTouchZImpl()
        {
            return m_CurTouchPos.z;
        }
        private float GetTouchRayPointXImpl()
        {
            return m_TouchRayPoint.x;
        }
        private float GetTouchRayPointYImpl()
        {
            return m_TouchRayPoint.y;
        }
        private float GetTouchRayPointZImpl()
        {
            return m_TouchRayPoint.z;
        }
        private void ListenTouchEventImpl(TouchEvent c, MyAction<int, GestureArgs> handler)
        {
            if (m_TouchHandlers.ContainsKey((int)c))
            {
                m_TouchHandlers[(int)c] = handler;
            }
            else
            {
                m_TouchHandlers.Add((int)c, handler);
            }
        }
        private void Fire(int c, GestureArgs e)
        {
            if (null != m_LogicInvoker && m_TouchHandlers.ContainsKey(c))
            {
                MyAction<int, GestureArgs> handler = m_TouchHandlers[c];
                QueueLogicActionWithDelegation(handler, c, e);
            }
        }
        internal void OnGesture(GestureArgs e)
        {
            if (null != e)
            {
                m_LastTouchPos = m_CurTouchPos;
                m_CurTouchPos = new Vector3(e.positionX, e.positionY, 0);
                m_TouchRayPoint = new Vector3(e.gamePosX, e.gamePosY, e.gamePosZ);
            }
            Fire((int)TouchEvent.Cesture, e);
            ///
            string ename = e.name;
            if (GestureEvent.OnFingerDown.ToString() == ename)
            {
                if (null != OnFingerDown)
                {
                    OnFingerDown(e);
                }
            }
            else if (GestureEvent.OnFingerUp.ToString() == ename)
            {
                if (null != OnFingerUp)
                {
                    OnFingerUp(e);
                }
            }
        }
        /// Joystick
        internal void SetJoystickInfoImpl(GestureArgs e)
        {
            if (null != e)
            {
                m_CurJoyDir = e.towards;
                m_CurJoyTargetPos.x = e.airWelGamePosX;
                m_CurJoyTargetPos.y = e.airWelGamePosY;
                m_CurJoyTargetPos.z = e.airWelGamePosZ;
            }
        }
        private float GetJoystickDirImpl()
        {
            return m_CurJoyDir;
        }
        private float GetJoystickTargetPosXImpl()
        {
            return m_CurJoyTargetPos.x;
        }
        private float GetJoystickTargetPosYImpl()
        {
            return m_CurJoyTargetPos.y;
        }
        private float GetJoystickTargetPosZImpl()
        {
            return m_CurJoyTargetPos.z;
        }

        ///
        private Vector3 m_LastTouchPos;
        private Vector3 m_CurTouchPos;
        private Vector3 m_TouchRayPoint;
        /// Joystick
        private float m_CurJoyDir;
        private Vector3 m_CurJoyTargetPos;

        private Dictionary<int, MyAction<int, GestureArgs>> m_TouchHandlers = new Dictionary<int, MyAction<int, GestureArgs>>();
    }
}
