using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarWars
{
    public partial class CharacterView
    {
        public int Actor
        {
            get { return m_Actor; }
        }
        public int ObjId
        {
            get { return m_ObjId; }
        }
        public SharedGameObjectInfo ObjectInfo
        {
            get { return m_ObjectInfo; }
        }

        public bool Visible
        {
            get { return m_Visible; }
            set
            {
                m_Visible = UpdateVisible(value);
            }
        }

        public bool CanAffectPlayerSelf
        {
            get { return m_CanAffectPlayerSelf; }
            set { m_CanAffectPlayerSelf = value; }
        }

        public void PlayParticle(string particlename, bool enable)
        {
            if (enable && !CanAffectPlayerSelf) return;
        }

        public void PlayParticle(string particlename, float x, float y, float z, float duration)
        {
            if (!CanAffectPlayerSelf) return;
            //TemporaryEffectArgs args = new TemporaryEffectArgs(particlename, duration, x, y, z);
            //GfxSystem.SendMessage("GfxGameRoot", "AddTemporaryEffect", args);
        }

        public void AddAttachedObject(int objectId, string path)
        {
            //GfxSystem.AttachGameObject(objectId, m_Actor, path);
        }

        public void RemoveAttachedObject(int objectId, bool isDestroy = false)
        {
            //GfxSystem.DetachGameObject(objectId);
            //if (isDestroy)
            //{
            //    GfxSystem.DestroyGameObject(objectId);
            //}
        }
        public void PlaySound(string filename, Vector3 pos)
        {
            if (!CanAffectPlayerSelf) return;
        }

        public void SetVisible(bool bVis, string model = null)
        {
            if (m_Actor == 0)
            {
                return;
            }
        }

        public void SetMaterial(string material_name)
        {
            if (!CanAffectPlayerSelf) return;
        }

        public void ResetMaterial()
        {
        }

        protected void CreateActor(int objId, string model, Vector3 pos, float dir, float scale = 1.0f)
        {
            Init();

            m_ObjId = objId;
            m_Actor = GameObjectIdManager.Instance.GenNextId();
            m_ObjectInfo.m_ActorId = m_Actor;
            m_ObjectInfo.m_LogicObjectId = objId;
            m_ObjectInfo.x = pos.x;
            m_ObjectInfo.y = pos.y;
            m_ObjectInfo.z = pos.z;
            m_ObjectInfo.FaceDir = dir;
            m_ObjectInfo.sx = scale;
            m_ObjectInfo.sy = scale;
            m_ObjectInfo.sz = scale;
            GfxSystem.CreateGameObject(m_Actor, model, m_ObjectInfo);
        }

        protected void CreateBornEffect(int parentActor, string effect)
        {
            if (!String.IsNullOrEmpty(effect))
            {
                //GfxSystem.CreateAndAttachGameObject(effect, parentActor, "");
            }
        }

        protected void DestroyActor()
        {
            //GfxSystem.DestroyGameObject(m_Actor);
            Release();
        }

        protected virtual bool UpdateVisible(bool visible)
        {
            //GfxSystem.SetGameObjectVisible(m_Actor, visible);
            return visible;
        }

        public void OnCombat2IdleSkillOver()
        {
            m_IsCombatState = false;
            m_IsWeaponMoved = false;
            m_IdleState = IdleState.kNotIdle;
        }

        protected bool IsInCombatState()
        {
            SkillStateInfo state = GetOwner().GetSkillStateInfo();
            if (state == null)
            {
                return false;
            }
            //if (state.IsSkillActivated() && state.GetCurSkillInfo().SkillId != GetOwner().Combat2IdleSkill)
            //{
            //    return true;
            //}
            if (state.IsImpactActive())
            {
                return true;
            }
            return false;
        }

        protected void UpdateState()
        {
            if (GetOwner().IsDead())
            {
                return;
            }
            long now = TimeUtility.GetServerMilliseconds();
            if (IsInCombatState())
            {
                m_LastLeaveCombatTime = now;
                m_IsCombat2IdleChanging = false;
                m_IsCombatState = true;
            }
            else if (m_IsCombatState)
            {
                if (GetOwner().GetMovementStateInfo().IsMoving)
                {
                    m_LastLeaveCombatTime = now;
                    m_IsCombat2IdleChanging = false;
                }
            }
            if (GetOwner().GetId() == WorldSystem.Instance.GetPlayerSelf().GetId())
            {
                if (m_LastLeaveCombatTime + GetOwner().Combat2IdleTime * 1000 <= now && !m_IsCombat2IdleChanging)
                {
                    //GetOwner().SkillController.PushSkill(SkillCategory.kCombat2Idle, Vector3.Zero);
                    m_IsCombat2IdleChanging = true;
                }
            }
            if (m_IsCombatState && !m_IsWeaponMoved)
            {
                EnterCombatState();
            }
        }

        public void EnterCombatState()
        {
            m_IsCombatState = true;
            string[] weapon_moves = GetOwner().Idle2CombatWeaponMoves.Split('|');
            for (int i = 1; i < weapon_moves.Length; i += 2)
            {
                string child = weapon_moves[i - 1];
                string node = weapon_moves[i];
                //GfxSystem.QueueGfxAction(GfxModule.Skill.Trigers.TriggerUtil.MoveChildToNode, Actor, child, node);
            }
            m_IsWeaponMoved = true;
        }

        protected void UpdateMovement()
        {
            CharacterInfo obj = GetOwner();
            if (null != obj && !obj.IsDead() && null != ObjectInfo)
            {
                //if (obj.IsNpc && !obj.CastNpcInfo().CanMove) return;
                MovementStateInfo msi = obj.GetMovementStateInfo();
                ObjectInfo.FaceDir = msi.GetFaceDir();
                ObjectInfo.WantFaceDir = msi.GetWantFaceDir();
                if (msi.IsMoving)
                {
                    Vector3 pos = msi.GetPosition3D();
                    ObjectInfo.MoveCos = (float)msi.MoveDirCosAngle;
                    ObjectInfo.MoveSin = (float)msi.MoveDirSinAngle;
                    ObjectInfo.MoveSpeed = (float)obj.GetActualProperty().MoveSpeed * (float)obj.VelocityCoefficient;

                    if (obj is UserInfo)
                    {
                        if (msi.TargetPosition.sqrMagnitude < Geometry.c_FloatPrecision)
                        {
                            ObjectInfo.MoveTargetDistanceSqr = 100.0f;
                        }
                        else
                        {
                            ObjectInfo.MoveTargetDistanceSqr = msi.CalcDistancSquareToTarget();
                        }
                    }
                    else
                    {
                        ObjectInfo.MoveTargetDistanceSqr = msi.CalcDistancSquareToTarget();
                    }

                    ObjectInfo.IsLogicMoving = true;
                }
                else
                {
                    ObjectInfo.IsLogicMoving = false;
                }
            }
            else
            {
                ObjectInfo.IsLogicMoving = false;
            }
        }

        protected void UpdateAffectPlayerSelf(Vector3 pos)
        {
            if (null != WorldSystem.Instance.GetPlayerSelf())
            {
                Vector3 myselfPos = WorldSystem.Instance.GetPlayerSelf().GetMovementStateInfo().GetPosition3D();
                if (Geometry.DistanceSquare(pos, myselfPos) < c_AffectPlayerSelfDistanceSquare)
                {
                    CanAffectPlayerSelf = true;
                }
                else
                {
                    CanAffectPlayerSelf = false;
                }
            }
            else
            {
                CanAffectPlayerSelf = false;
            }
        }

        private void Init()
        {
            m_NormalColor = new Color(1, 1, 1, 1);
            m_BurnColor = new Color(0.75f, 0.2f, 0.2f, 1);
            m_FrozonColor = new Color(0.2f, 0.2f, 0.75f, 1);
            m_ShineColor = new Color(0.2f, 0.75f, 0.2f, 1);
            m_Actor = 0;

            //m_CurActionConfig = null;
        }

        private void Release()
        {
            List<string> keyList = effect_map_.Keys.ToList();
            if (keyList != null && keyList.Count > 0)
            {
                foreach (string model in keyList)
                {
                    //DetachActor(model);
                }
            }
            CurWeaponList.Clear();
        }

        public List<string> CurWeaponList
        {
            get
            {
                return m_CurWeaponName;
            }
        }

        public string Cylinder
        {
            get { return c_CylinderName; }
        }

        private int m_Actor = 0;
        private int m_ObjId = 0;
        private SharedGameObjectInfo m_ObjectInfo = new SharedGameObjectInfo();

        protected long m_LastLeaveCombatTime = 0;
        protected bool m_IsCombat2IdleChanging = true;
        protected bool m_IsWeaponMoved = false;

        private const string c_CylinderName = "1_Cylinder";
        private const float c_AffectPlayerSelfDistanceSquare = 900;
        private List<string> m_CurWeaponName = new List<string>();

        private bool m_Visible = true;
        private bool m_CanAffectPlayerSelf = true;

        private Color m_NormalColor = new Color(1, 1, 1, 1);
        private Color m_BurnColor = new Color(0.75f, 0.2f, 0.2f, 1);
        private Color m_FrozonColor = new Color(0.2f, 0.2f, 0.75f, 1);
        private Color m_ShineColor = new Color(0.2f, 0.75f, 0.2f, 1);
        private Dictionary<string, uint> effect_map_ = new Dictionary<string, uint>();
    }
}
