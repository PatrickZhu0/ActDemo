using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWars
{
    public enum IdleState
    {
        kNotIdle, //未进入
        kReady, //空闲开始计时
        kBegin, //开始播放空闲动画
    }

    public sealed class CharacterAnimationInfo
    {
        public bool IsPlayChangeWeapon;
        public bool IsPlayDead;
        public bool IsPlayTaunt;

        public bool IsMoving;
        public bool CanMove;
        public MovementMode MoveMode;
        public bool IsCombat;
        public float m_Speed;

        public void Reset()
        {
            IsPlayChangeWeapon = false;
            IsPlayDead = false;
            IsPlayTaunt = false;
            IsMoving = false;
            CanMove = true;
            IsCombat = false;
            m_Speed = 0;
        }
        public bool IsIdle()
        {
            return !IsMoving && !IsPlayChangeWeapon && !IsPlayDead && !IsPlayTaunt;
        }
    }

    //public delegate void AnimationStopCallback(string anim_name);

    public partial class CharacterView
    {
        public bool IsCombatState
        {
            get { return m_IsCombatState; }
        }

        //public void PlayAnimation(Animation_Type type)
        //{
        //    PlayAnimation(type, 1.0f);
        //}

        //    public void PlayAnimation(Animation_Type type, float speed)
        //    {
        //        string name = GetAnimationNameByType(type);
        //        if (string.IsNullOrEmpty(name))
        //        {
        //            return;
        //        }

        //        GfxSystem.CrossFadeAnimation(m_Actor, name);
        //        GfxSystem.SetAnimationSpeed(m_Actor, name, speed);
        //    }

        //    public void PlayQueuedAnimation(Animation_Type type)
        //    {
        //        PlayQueuedAnimation(type, 1.0f);
        //    }

        //    public void PlayQueuedAnimation(Animation_Type type, float speed)
        //    {
        //        string name = GetAnimationNameByType(type);
        //        if (string.IsNullOrEmpty(name))
        //        {
        //            return;
        //        }

        //        GfxSystem.PlayQueuedAnimation(m_Actor, name);
        //        GfxSystem.SetAnimationSpeed(m_Actor, name, speed);
        //    }

        //    public void StopAnimation(Animation_Type type)
        //    {
        //        string name = GetAnimationNameByType(type);
        //        if (string.IsNullOrEmpty(name))
        //        {
        //            return;
        //        }
        //        GfxSystem.StopAnimation(m_Actor, name);
        //    }

        //    protected void InitAnimationSets()
        //    {
        //        List<int> action_list = GetOwner().GetActionList();
        //        for (int i = 0; i < action_list.Count; i++)
        //        {
        //            m_CurActionConfig = ActionConfigProvider.Instance.GetDataById(action_list[i]);
        //            m_ObjectInfo.AnimConfigId = action_list[i];
        //        }
        //    }

        //    protected void UpdateMoveAnimation()
        //    {
        //        CharacterInfo charObj = GetOwner();
        //        if (null == charObj)
        //            return;
        //        if (charObj.GetMovementStateInfo().IsMoving && !charObj.GetMovementStateInfo().IsSkillMoving)
        //        {
        //            if (!m_CharacterAnimationInfo.IsMoving)
        //            {
        //                m_CharacterAnimationInfo.IsMoving = true;
        //                m_CharacterAnimationInfo.MoveMode = charObj.GetMovementStateInfo().MovementMode;
        //                StartMoveAnimation();
        //            }
        //            else if (IsMoveStateChange())
        //            {
        //                StopMoveAnimation();
        //                UpdateAnimInfo();
        //                StartMoveAnimation();
        //            }
        //        }
        //        else
        //        {
        //            if (m_CharacterAnimationInfo.IsMoving)
        //            {
        //                m_CharacterAnimationInfo.IsMoving = false;
        //                StopMoveAnimation();
        //            }
        //        }
        //    }

        //    private bool IsMoveStateChange()
        //    {
        //        if (m_CharacterAnimationInfo.MoveMode != GetOwner().GetMovementStateInfo().MovementMode)
        //        {
        //            return true;
        //        }
        //        if (m_CharacterAnimationInfo.IsCombat != IsCombatState)
        //        {
        //            return true;
        //        }
        //        if (m_CharacterAnimationInfo.m_Speed != GetOwner().GetActualProperty().MoveSpeed)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }

        //    private void UpdateAnimInfo()
        //    {
        //        m_CharacterAnimationInfo.MoveMode = GetOwner().GetMovementStateInfo().MovementMode;
        //        m_CharacterAnimationInfo.IsCombat = IsCombatState;
        //        m_CharacterAnimationInfo.m_Speed = GetOwner().GetActualProperty().MoveSpeed;
        //    }

        //    public void PlayBeCoolAnimation()
        //    {
        //        GetOwner().SkillController.PushSkill(SkillCategory.kBeCool, Vector3.Zero);
        //    }

        //    protected void UpdateDead()
        //    {
        //        CharacterInfo charObj = GetOwner();
        //        if (null == charObj)
        //            return;
        //        if (charObj.IsDead() && !charObj.GetSkillStateInfo().IsImpactControl() && charObj.DeadTime > 0 && !charObj.IsHaveGfxStateFlag(GfxCharacterState_Type.KnockDown))
        //        {
        //            if (!m_CharacterAnimationInfo.IsPlayDead)
        //            {
        //                m_CharacterAnimationInfo.IsPlayDead = true;
        //                string name = GetAnimationNameByType(Animation_Type.AT_Dead);
        //                if (!string.IsNullOrEmpty(name))
        //                {
        //                    GfxSystem.CrossFadeAnimation(m_Actor, name);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (m_CharacterAnimationInfo.IsPlayDead)
        //            {
        //                m_CharacterAnimationInfo.IsPlayDead = false;
        //            }
        //        }
        //    }

        //    protected virtual void UpdateIdle()
        //    {
        //        if (!GetOwner().IsDead() && m_CharacterAnimationInfo.IsIdle())
        //        {
        //            if (m_IdleState == IdleState.kNotIdle)
        //            {
        //                Animation_Type at = m_IsCombatState ? Animation_Type.AT_CombatStand : Animation_Type.AT_Stand;
        //                string name = GetAnimationNameByType(at);
        //                if (!string.IsNullOrEmpty(name))
        //                {
        //                    float fadetime = 0.5f;
        //                    if (GetOwner() != null && GetOwner().GetSkillStateInfo() != null)
        //                    {
        //                        fadetime = GetOwner().GetSkillStateInfo().CrossToStandTime;
        //                        GetOwner().ResetCross2StandRunTime();
        //                    }
        //                    GfxSystem.CrossFadeAnimation(m_Actor, name, fadetime);
        //                }

        //                m_BeginIdleTime = TimeUtility.GetServerMilliseconds();
        //                m_IdleState = IdleState.kReady;
        //                m_IdleInterval = new Random().Next(1, 3) * 1000;
        //            }
        //            else if (m_IdleState == IdleState.kReady)
        //            {
        //                if (TimeUtility.GetServerMilliseconds() - m_BeginIdleTime > m_IdleInterval)
        //                {
        //                    Animation_Type at = m_IsCombatState ? Animation_Type.AT_CombatStand : Animation_Type.AT_Stand;
        //                    string name = GetAnimationNameByType(at);
        //                    if (!string.IsNullOrEmpty(name))
        //                    {
        //                        float fadetime = 0.5f;
        //                        if (GetOwner() != null && GetOwner().GetSkillStateInfo() != null)
        //                        {
        //                            fadetime = GetOwner().GetSkillStateInfo().CrossToStandTime;
        //                        }
        //                        GfxSystem.CrossFadeAnimation(m_Actor, name, fadetime);
        //                    }
        //                    m_BeginIdleTime = TimeUtility.GetServerMilliseconds();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            m_IdleState = IdleState.kNotIdle;
        //        }
        //    }

        protected virtual CharacterInfo GetOwner() { return null; }

        //    protected void GetAnimationDirAndSpeed(MovementMode mode, float move_speed, out Animation_Type at, out float speed_factor)
        //    {
        //        Data_ActionConfig action_config = m_CurActionConfig;

        //        if (mode == MovementMode.LowSpeed)
        //        {
        //            at = Animation_Type.AT_SlowMove;
        //        }
        //        else if (mode == MovementMode.HighSpeed)
        //        {
        //            at = Animation_Type.AT_FastMove;
        //        }
        //        else
        //        {
        //            at = Animation_Type.AT_RunForward;
        //        }
        //        if (action_config != null)
        //        {
        //            if (mode == MovementMode.LowSpeed)
        //            {
        //                speed_factor = move_speed / action_config.m_SlowStdSpeed;
        //            }
        //            else if (mode == MovementMode.HighSpeed)
        //            {
        //                speed_factor = move_speed / action_config.m_FastStdSpeed;
        //            }
        //            else
        //            {
        //                speed_factor = move_speed / action_config.m_ForwardStdSpeed;
        //            }
        //        }
        //        else
        //        {
        //            speed_factor = 1.0f;
        //        }
        //    }

        //    protected Data_ActionConfig GetCurActionConfig()
        //    {
        //        return ActionConfigProvider.Instance.GetCharacterCurActionConfig(GetOwner().GetActionList());
        //    }

        //    protected string GetAnimationNameByType(Animation_Type type)
        //    {
        //        if (m_CurActionConfig != null)
        //        {
        //            Data_ActionConfig.Data_ActionInfo action = m_CurActionConfig.GetRandomActionByType(type);
        //            if (action != null)
        //            {
        //                return action.m_AnimName;
        //            }
        //        }
        //        return null;
        //    }

        //    private void StartMoveAnimation()
        //    {
        //        Animation_Type type = Animation_Type.AT_None;
        //        float speed_factor;
        //        float move_speed = GetOwner().GetActualProperty().MoveSpeed;
        //        GetAnimationDirAndSpeed(m_CharacterAnimationInfo.MoveMode,
        //                                move_speed, out type, out speed_factor);
        //        if (type == Animation_Type.AT_RunForward && m_IsCombatState)
        //        {
        //            type = Animation_Type.AT_CombatRun;
        //            if (m_CurActionConfig != null)
        //            {
        //                speed_factor = move_speed / m_CurActionConfig.m_CombatStdSpeed;
        //            }
        //        }
        //        string name = GetAnimationNameByType(type);
        //        if (!string.IsNullOrEmpty(name))
        //        {
        //            float fadetime = 0.3f;
        //            if (GetOwner() != null && GetOwner().GetSkillStateInfo() != null)
        //            {
        //                fadetime = GetOwner().GetSkillStateInfo().CrossToRunTime;
        //                GetOwner().ResetCross2StandRunTime();
        //            }
        //            GfxSystem.SetAnimationSpeed(m_Actor, name, speed_factor);
        //            GfxSystem.CrossFadeAnimation(m_Actor, name, fadetime);
        //        }
        //    }

        //    private void StopMoveAnimation()
        //    {
        //        FadeToStand();
        //    }

        //    private void FadeToStand()
        //    {
        //        string name = GetAnimationNameByType(Animation_Type.AT_Stand);
        //        if (!string.IsNullOrEmpty(name))
        //        {
        //            GfxSystem.CrossFadeAnimation(m_Actor, name);
        //        }
        //    }

        //    private void FadeToMoveOrHold()
        //    {
        //        if (m_CharacterAnimationInfo.IsMoving)
        //        {
        //            MovementMode moveMode = GetOwner().GetMovementStateInfo().MovementMode;
        //            RelMoveDir moveDir = GetOwner().GetMovementStateInfo().RelativeMoveDir;
        //            StartMoveAnimation();
        //        }
        //        else
        //        {
        //            FadeToStand();
        //        }

        protected bool m_IsCombatState = false;
        //protected Data_ActionConfig m_CurActionConfig = null;

        protected CharacterAnimationInfo m_CharacterAnimationInfo = new CharacterAnimationInfo();
        protected IdleState m_IdleState = IdleState.kNotIdle;
        protected long m_BeginIdleTime = 0;
        protected long m_IdleInterval = 0;
    }
}