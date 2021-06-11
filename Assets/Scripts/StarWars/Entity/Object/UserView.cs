using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarWars
{
    public class UserView : CharacterView
    {
        public UserInfo User
        {
            get { return m_User; }
        }

        public void Create(UserInfo user)
        {
            Init();
            if (null != user)
            {
                m_User = user;
                ObjectInfo.CampId = m_User.GetCampId();
                MovementStateInfo msi = m_User.GetMovementStateInfo();
                Vector3 pos = msi.GetPosition3D();
                float dir = msi.GetFaceDir();
                CreateActor(m_User.GetId(), m_User.GetModel(), pos, dir, m_User.Scale);
                CreateIndicatorActor(m_User.GetId(), m_User.GetIndicatorModel());
                //InitAnimationSets();
                ObjectInfo.IsPlayer = true;
                if (user.GetId() == WorldSystem.Instance.PlayerSelfId)
                {
                //    GfxSystem.MarkPlayerSelf(Actor);
                }
            }
        }
        public void Destroy()
        {
            DestroyActor();
            Release();
        }
        public void Update()
        {
            UpdateAttr();
            UpdateSpatial();
            UpdateAnimation();
            UpdateIndicator();
        }

        public void SetIndicatorInfo(bool visible, float dir, int targetType)
        {
            m_IndicatorVisible = visible;
            m_IndicatorDir = dir;
            m_IndicatorTargetType = targetType;
        }
        public void SetIndicatorTargetType(int targetType)
        {
            //GfxSystem.SendMessage(m_IndicatorActor, "SetIndicatorTarget", targetType);
        }

        protected override bool UpdateVisible(bool visible)
        {
            SetVisible(visible);
            return visible;
        }
        private void CreateIndicatorActor(int objId, string model)
        {
            m_IndicatorActor = GameObjectIdManager.Instance.GenNextId();
            //GfxSystem.CreateGameObject(m_IndicatorActor, model, 0, 0, 0, 0, 0, 0, false);
            //GfxSystem.CreateGameObjectForAttach(m_IndicatorActor, model);
            //GfxSystem.AttachGameObject(m_IndicatorActor, Actor);
            //GfxSystem.SetGameObjectVisible(m_IndicatorActor, false);
            //GfxSystem.SendMessage(m_IndicatorActor, "SetOwner", Actor);
            //GfxSystem.UpdateGameObjectLocalRotateY(m_IndicatorActor, m_IndicatorDir);
        }

        private void UpdateAttr()
        {
            if (null != m_User)
            {
                ObjectInfo.Blood = m_User.Hp;
                ObjectInfo.MaxBlood = m_User.GetActualProperty().HpMax;
                ObjectInfo.Energy = m_User.Energy;
                ObjectInfo.MaxEnergy = m_User.GetActualProperty().EnergyMax;
                ObjectInfo.Rage = m_User.Rage;
                ObjectInfo.MaxRage = m_User.GetActualProperty().RageMax;
                ObjectInfo.IsSuperArmor = (m_User.SuperArmor || m_User.UltraArmor);
                m_User.GfxStateFlag = ObjectInfo.GfxStateFlag;
            }
        }

        private void UpdateSpatial()
        {
            if (null != m_User)
            {
                MovementStateInfo msi = m_User.GetMovementStateInfo();
                if (ObjectInfo.IsGfxMoveControl)
                {
                    if (ObjectInfo.DataChangedByGfx)
                    {
                        msi.PositionX = ObjectInfo.x;
                        msi.PositionY = ObjectInfo.y;
                        msi.PositionZ = ObjectInfo.z;
                        msi.SetFaceDir(ObjectInfo.FaceDir);
                        ObjectInfo.DataChangedByGfx = false;
                    }
                    if (ObjectInfo.WantDirChangedByGfx)
                    {
                        msi.SetWantFaceDir(ObjectInfo.WantFaceDir);
                        ObjectInfo.WantDirChangedByGfx = false;
                    }
                }
                else
                {
                    if (ObjectInfo.DataChangedByGfx)
                    {
                        msi.PositionX = ObjectInfo.x;
                        msi.PositionY = ObjectInfo.y;
                        msi.PositionZ = ObjectInfo.z;
                        //msi.SetFaceDir(ObjectInfo.FaceDir);
                        ObjectInfo.DataChangedByGfx = false;
                    }
                    UpdateMovement();
                }
                ObjectInfo.WantFaceDir = msi.GetWantFaceDir();
                SimulateDir(ObjectInfo.WantFaceDir);
            }
        }

        private void SimulateDir(float dir)
        {
            //List<NpcInfo> summons = m_User.GetSkillStateInfo().GetSummonObject();
            //foreach (NpcInfo npc in summons)
            //{
            //    if (npc.IsSimulateMove)
            //    {
            //        npc.GetMovementStateInfo().SetWantFaceDir(dir);
            //    }
            //}
        }

        private void UpdateAnimation()
        {
            if (!CanAffectPlayerSelf) return;
            if (null != m_User)
            {
                UpdateState();
                if (ObjectInfo.IsGfxAnimation)
                {
                    m_CharacterAnimationInfo.Reset();
                    m_IdleState = IdleState.kNotIdle;
                    return;
                }
                //UpdateMoveAnimation();
                //UpdateDead();
                //UpdateIdle();
            }
        }

        private void UpdateIndicator()
        {
            if (null != m_User)
            {
                //GfxSystem.SetGameObjectVisible(m_IndicatorActor, m_IndicatorVisible);
                //GfxSystem.SendMessage(m_IndicatorActor, "SetIndicatorDir", m_IndicatorDir);
                //GfxSystem.SendMessage(m_IndicatorActor, "SetIndicatorTarget", m_IndicatorTargetType);
            }
        }

        private void Init()
        {
            m_User = null;
            old_color_ = Color.white;
        }

        private void Release()
        {

        }

        protected override CharacterInfo GetOwner()
        {
            return m_User;
        }

        private UserInfo m_User = null;
        private Color old_color_;
        private int m_IndicatorActor = 0;
        private float m_IndicatorDir = 0;
        private bool m_IndicatorVisible = false;
        private int m_IndicatorTargetType = 1;
    }
}
