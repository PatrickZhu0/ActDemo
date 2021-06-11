using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarWars
{
    public enum AiStateId : int
    {
        Invalid = 0,
        Idle,
        Combat,
        Pursuit,
        GoHome,
        Escape,
        Patrol,
        Guard,
        Move,
        Wait,
        MoveCommand,
        PatrolCommand,
        PursuitCommand,
        MaxNum
    }
    public class AiStateInfo
    {
        public int CurState
        {
            get
            {
                int state = (int)AiStateId.Invalid;
                if (m_StateStack.Count > 0)
                    state = m_StateStack.Peek();
                return state;
            }
        }
        public void PushState(int state)
        {
            m_StateStack.Push(state);
        }
        public int PopState()
        {
            int ret = (int)AiStateId.Invalid;
            if (m_StateStack.Count > 0)
                ret = m_StateStack.Pop();
            return ret;
        }
        public void ChangeToState(int state)
        {
            if (m_StateStack.Count > 0)
                m_StateStack.Pop();
            m_StateStack.Push(state);
        }
        public void CloneAiStates(IEnumerable<int> states)
        {
            m_StateStack = new Stack<int>(states);
        }
        public int[] CloneAiStates()
        {
            return m_StateStack.ToArray();
        }
        public void Reset()
        {
            m_StateStack.Clear();
            while (m_CommandQueue.Count > 0)
            {
                IAiCommand cmd = m_CommandQueue.Dequeue();
                cmd.Recycle();
            }
        }
        public int AiLogic
        {
            get { return m_AiLogic; }
            set { m_AiLogic = value; }
        }
        public string[] AiParam
        {
            get { return m_AiParam; }
        }
        public List<int> AiActions
        {
            get { return m_AiActions; }
        }
        public TypedDataCollection AiDatas
        {
            get { return m_AiDatas; }
        }
        public Queue<IAiCommand> CommandQueue
        {
            get { return m_CommandQueue; }
        }
        public long Time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }

        private Stack<int> m_StateStack = new Stack<int>();
        private int m_AiLogic = 0;
        private string[] m_AiParam = new string[Data_Unit.c_MaxAiParamNum];
        private List<int> m_AiActions = new List<int>();
        private TypedDataCollection m_AiDatas = new TypedDataCollection();
        private Queue<IAiCommand> m_CommandQueue = new Queue<IAiCommand>();
        private long m_Time = 0;
    }
    public class NpcAiStateInfo : AiStateInfo
    {
        public Vector3 HomePos
        {
            get { return m_HomePos; }
            set { m_HomePos = value; }
        }
        public int Target
        {
            get { return m_Target; }
            set
            {
                m_Target = value;
                m_IsExternalTarget = false;
            }
        }
        public int HateTarget
        {
            get { return m_HateTarget; }
            set { m_HateTarget = value; }
        }
        public bool IsExternalTarget
        {
            get { return m_IsExternalTarget; }
        }
        public void SetExternalTarget(int target)
        {
            m_Target = target;
            m_IsExternalTarget = true;
        }
        public StarWars.AiActionInfo CurActionInfo
        {
            get { return m_CurActionInfo; }
            set { m_CurActionInfo = value; }
        }
        public List<AiActionInfo> ActionInfos
        {
            get { return m_ActionInfos; }
        }

        private Vector3 m_HomePos = new Vector3();
        private int m_Target = 0;
        private int m_HateTarget = 0;
        private bool m_IsExternalTarget = false;
        private List<AiActionInfo> m_ActionInfos = new List<AiActionInfo>();
        private AiActionInfo m_CurActionInfo;
    }
    public class UserAiStateInfo : AiStateInfo
    {
        public Vector3 HomePos
        {
            get { return m_HomePos; }
            set { m_HomePos = value; }
        }
        public int Target
        {
            get
            {
                return m_Target;
            }
            set
            {
                m_Target = value;
            }
        }
        public Vector3 TargetPos
        {
            get
            {
                return m_TargetPos;
            }
            set
            {
                m_TargetPos = value;
            }
        }
        public bool IsAttacked
        {
            get
            {
                return m_IsAttacked;
            }
            set
            {
                m_IsAttacked = value;
            }
        }

        public float AttackRange
        {
            get
            {
                return m_attackRange;
            }
            set
            {
                m_attackRange = value;
            }
        }

        private float m_attackRange = 0;
        private bool m_IsAttacked = false;
        public bool IsTargetPosChanged { get; set; }
        private Vector3 m_HomePos = new Vector3();
        private int m_Target = 0;
        private Vector3 m_TargetPos = Vector3.zero;
    }
}
