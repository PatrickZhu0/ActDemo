using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarWars
{
    public sealed partial class GfxSystem
    {
        public bool IsLastHitUi
        {
            get { return m_IsLastHitUi; }
            internal set { m_IsLastHitUi = value; }
        }
        private float GetMouseXImpl()
        {
            return m_CurMousePos.x;
        }
        private float GetMouseYImpl()
        {
            return m_CurMousePos.y;
        }
        private float GetMouseZImpl()
        {
            return m_CurMousePos.z;
        }
        private float GetMouseRayPointXImpl()
        {
            return m_MouseRayPoint.x;
        }
        private float GetMouseRayPointYImpl()
        {
            return m_MouseRayPoint.y;
        }
        private float GetMouseRayPointZImpl()
        {
            return m_MouseRayPoint.z;
        }
        private bool IsButtonPressedImpl(Mouse.Code c)
        {
            return m_ButtonPressed[(int)c];
        }
        private bool IsKeyPressedImpl(Keyboard.Code c)
        {
            return m_KeyPressed[(int)c];
        }
        private void ListenKeyPressStateImpl(Keyboard.Code[] codes)
        {
            foreach (Keyboard.Code c in codes)
            {
                if (!m_KeysForListen.Contains((int)c))
                {
                    m_KeysForListen.Add((int)c);
                }
            }
        }
        private void ListenKeyboardEventImpl(Keyboard.Code c, MyAction<int, int> handler)
        {
            if (m_KeyboardHandlers.ContainsKey((int)c))
            {
                m_KeyboardHandlers[(int)c] = handler;
            }
            else
            {
                m_KeyboardHandlers.Add((int)c, handler);
            }
        }
        private void ListenMouseEventImpl(Mouse.Code c, MyAction<int, int> handler)
        {
            if (m_MouseHandlers.ContainsKey((int)c))
            {
                m_MouseHandlers[(int)c] = handler;
            }
            else
            {
                m_MouseHandlers.Add((int)c, handler);
            }
        }

        private void ResetInputStateImpl()
        {
            for (int i = 0; i < (int)Keyboard.Code.MaxNum; ++i)
            {
                m_KeyPressed[i] = false;
            }
            for (int i = 0; i < (int)Mouse.Code.MaxNum; ++i)
            {
                m_ButtonPressed[i] = false;
            }
        }

        private void HandleInput()
        {
            Debug.LogError("HandleInput");
            m_LastMousePos = m_CurMousePos;
            m_CurMousePos = Input.mousePosition;

            if ((m_CurMousePos - m_LastMousePos).sqrMagnitude >= 1 && null != Camera.main)
            {
                Ray ray = Camera.main.ScreenPointToRay(m_CurMousePos);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    m_MouseRayPoint = hitInfo.point;
                }
            }

            foreach (int c in m_KeysForListen)
            {
                if (Input.GetKeyDown((KeyCode)c))
                {
                    m_KeyPressed[c] = true;
                    FireKeyboard(c, (int)Keyboard.Event.Down);
                    Debug.LogError("GetKeyDown = " + (KeyCode)c);
                }
                else if (Input.GetKeyUp((KeyCode)c))
                {
                    m_KeyPressed[c] = false;
                    FireKeyboard(c, (int)Keyboard.Event.Up);
                }
            }
        }
        private void FireKeyboard(int c, int e)
        {
            if (null != m_LogicInvoker && m_KeyboardHandlers.ContainsKey(c))
            {
                MyAction<int, int> handler = m_KeyboardHandlers[c];
                QueueLogicActionWithDelegation(handler, c, e);
            }
        }
        private void FireMouse(int c, int e)
        {
            if (null != m_LogicInvoker && m_MouseHandlers.ContainsKey(c))
            {
                MyAction<int, int> handler = m_MouseHandlers[c];
                QueueLogicActionWithDelegation(handler, c, e);
            }
        }

        private bool m_IsLastHitUi;
        private Vector3 m_LastMousePos;
        private Vector3 m_CurMousePos;
        private Vector3 m_MouseRayPoint;

        private bool[] m_KeyPressed = new bool[(int)Keyboard.Code.MaxNum];
        private bool[] m_ButtonPressed = new bool[(int)Mouse.Code.MaxNum];
        private HashSet<int> m_KeysForListen = new HashSet<int>();

        private Dictionary<int, MyAction<int, int>> m_KeyboardHandlers = new Dictionary<int, MyAction<int, int>>();
        private Dictionary<int, MyAction<int, int>> m_MouseHandlers = new Dictionary<int, MyAction<int, int>>();
    }
}
