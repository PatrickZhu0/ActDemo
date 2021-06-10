using System;
using System.Collections.Generic;
using UnityEngine;
using StarWarsSpatial;

namespace StarWars
{
    public delegate void DamageDelegation(int receiver, int caster, bool isShootDamage, bool isCritical, int hpDamage, int npDamage);
    public delegate void GainMoneyDelegation(int receiver, int money);
    public delegate void OnBeginAttackEvent();

    public interface IShootTarget
    {
        uint GetActorID();
    }

    //！！！注意，不要在这里添加属于CharacterProperty管理的内容，只能加人物状态数据，不能加属性数据，属性由属性计算得到
    public enum PropertyIndex
    {
        IDX_HP = 0,
        IDX_MP,
        IDX_STATE,
    }

    public enum HilightType
    {
        kNone,
        kBurnType,
        kFrozenType,
        kShineType,
    }

    public enum CharacterState_Type
    {
        CST_FixedPosition = 1 << 0,  // 定身，不能移动
        CST_Silence = 1 << 1,  // 沉默，不能释放技能
        CST_Invincible = 1 << 2,  // 无敌
        CST_BODY = 1 << 3,  // ？
        CST_Sleep = 1 << 4,  // 昏迷，不能移动，不能攻击，不能放技能
        CST_DamageAbsorb = 1 << 5,  // 伤害吸收，在计算伤害时需要考虑
        CST_Hidden = 1 << 6,  // 隐身的
        CST_Opened = 1 << 7,  // 用于npc带物品的，表明已经捡过了
        CST_Disarm = 1 << 8,  // 缴械，不能开枪
    }


    /**
     * @brief 角色基类
     */
    public class CharacterInfo : IShootTarget
    {
        public struct AttackerInfo
        {
            public int m_AttackerType;
            public long m_AttackTime;
            public int m_HpDamage;
            public int m_NpDamage;
        }

        // implement ISpaceObject interface
        public class SpaceObjectImpl : ISpaceObject
        {
            public uint GetID() { return (uint)m_CharacterInfo.GetId(); }
            public SpatialObjType GetObjType() { return m_ObjType; }
            public Vector3 GetPosition()
            {
                Vector3 v = m_CharacterInfo.GetMovementStateInfo().GetPosition3D();
                return v;
            }
            public float GetRadius() { return m_CharacterInfo.GetRadius(); }
            public Vector3 GetVelocity()
            {
                Vector3 ret;
                if (!m_CharacterInfo.GetMovementStateInfo().IsMoving)
                {
                    ret = new Vector3();
                }
                else
                {
                    ret = m_CharacterInfo.GetMovementStateInfo().GetMoveDir3D() * (float)(m_CharacterInfo.GetActualProperty().MoveSpeed * m_CharacterInfo.VelocityCoefficient);
                }
                return ret;
            }
            public bool IsAvoidable()
            {
                bool ret = true;
                //if (SpatialObjType.kNPC == m_ObjType)
                //{
                //    NpcInfo npc = m_CharacterInfo.CastNpcInfo();
                //    if (null != npc)
                //    {
                //        ret = (npc.NpcType != (int)NpcTypeEnum.Skill &&
                //          npc.NpcType != (int)NpcTypeEnum.AutoPickItem);
                //    }
                //}
                return ret;
            }
            public Shape GetCollideShape()
            {
                return m_CharacterInfo.Shape;
            }
            public List<ISpaceObject> GetCollideObjects() { return m_CollideObjects; }
            public void OnCollideObject(ISpaceObject obj)
            {
                m_CollideObjects.Add(obj);
            }
            public void OnDepartObject(ISpaceObject obj)
            {
                m_CollideObjects.Remove(obj);
            }
            public object RealObject
            {
                get
                {
                    return m_CharacterInfo;
                }
            }
            public SpaceObjectImpl(CharacterInfo info, SpatialObjType objType)
            {
                m_CharacterInfo = info;
                m_ObjType = objType;
            }

            private CharacterInfo m_CharacterInfo = null;
            private SpatialObjType m_ObjType = SpatialObjType.kNPC;
            private List<ISpaceObject> m_CollideObjects = new List<ISpaceObject>(); // 与当前物体碰撞的物体
        }
        public StarWarsSpatial.CellPos SightCell
        {
            get { return m_SightCell; }
            set { m_SightCell = value; }
        }
        public bool CurBlueCanSeeMe
        {
            get { return m_CurBlueCanSeeMe; }
            set { m_CurBlueCanSeeMe = value; }
        }
        public bool LastBlueCanSeeMe
        {
            get { return m_LastBlueCanSeeMe; }
        }
        public bool CurRedCanSeeMe
        {
            get { return m_CurRedCanSeeMe; }
            set { m_CurRedCanSeeMe = value; }
        }
        public bool LastRedCanSeeMe
        {
            get { return m_LastRedCanSeeMe; }
        }
        public void PrepareUpdateSight()
        {
            m_LastBlueCanSeeMe = m_CurBlueCanSeeMe;
            m_CurBlueCanSeeMe = false;
            m_LastRedCanSeeMe = m_CurRedCanSeeMe;
            m_CurRedCanSeeMe = false;
        }
        public bool IsNpc
        {
            get
            {
                return false;
            }
        }
        public bool IsUser
        {
            get
            {
                return m_CastUserInfo != null;
            }
        }

        public UserInfo CastUserInfo()
        {
            return m_CastUserInfo;
        }

        /**
         * @brief 构造函数
         *
         * @param id
         *
         * @return 
         */
        public CharacterInfo(int id)
        {
            m_Id = id;
            m_UnitId = 0;
            m_LinkId = 0;
            m_AIEnable = true;
            m_BaseProperty = new CharacterProperty();
            m_ActualProperty = new CharacterProperty();

            m_ReleaseTime = 0;
            IsFlying = false;
        }

        /**
         * @brief 获取id
         *
         * @return 
         */
        public int GetId()
        {
            return m_Id;
        }

        /**
         * @brief 单位id
         *
         * @return 
         */
        public int GetUnitId()
        {
            return m_UnitId;
        }

        /**
         * @brief 设置单位id
         *
         * @return 
         */
        public void SetUnitId(int id)
        {
            m_UnitId = id;
        }

        public int GetLinkId()
        {
            return m_LinkId;
        }

        public void SetLinkId(int id)
        {
            m_LinkId = id;
        }

        /**
         * @brief 设置名字
         *
         * @param name
         *
         * @return 
         */
        public void SetName(string name)
        {
            m_Name = name;
        }

        /**
         * @brief 获取名字
         *
         * @return 
         */
        public string GetName()
        {
            return m_Name;
        }

        public void SetBeAttack(bool bAttack)
        {
            m_BeAttack = bAttack;
        }

        public bool GetBeAttack()
        {
            return m_BeAttack;
        }

        public void SetCurEnemyInfo(int arg)
        {
            m_CurEnemyId = arg;
        }

        public int GetCurEnemyId()
        {
            return m_CurEnemyId;
        }
        public int GetMeetEnemyImpact()
        {
            return m_MeetEnemyImpact;
        }

        public long ReleaseTime
        {
            get { return m_ReleaseTime; }
            set { m_ReleaseTime = value; }
        }

        public long DeadTime
        {
            get
            {
                return m_DeadTime;
            }
            set
            {
                m_DeadTime = value;
            }
        }
        public long EmptyBloodTime
        {
            get
            {
                return m_EmptyBloodTime;
            }
            set
            {
                m_EmptyBloodTime = value;
            }
        }
        public bool CauseStiff
        {
            get { return m_CauseStiff; }
        }
        public bool AcceptStiff
        {
            get { return m_AcceptStiff; }
            set { m_AcceptStiff = value; }
        }
        public bool AcceptStiffEffect
        {
            get { return m_AcceptStiffEffect; }
            set { m_AcceptStiffEffect = value; }
        }

        /**
         * @brief 获取等级
         *
         * @return 
         */
        public int GetLevel()
        {
            return m_Level;
        }

        /**
         * @brief 设置等级
         *
         * @param money
         *
         * @return 
         */
        public void SetLevel(int level)
        {
            m_Level = level;
            m_LevelChanged = true;
        }

        public int GetTotalSkillPoint()
        {
            return GetLevel() + 100;
        }

        public float FightingScore
        {
            get { return m_FightingScore; }
            set
            {
                m_FightingScore = value;
                if (null != mFightingScoreChangeCB)
                {
                    mFightingScoreChangeCB(m_FightingScore);
                }
            }
        }
        private static MyAction<float> mFightingScoreChangeCB;
        public static void AddPropertyInfoChangeCB(MyAction<float> cb)
        {
            mFightingScoreChangeCB += cb;
        }

        /**
         * @brief 
         */
        public int Hp
        {
            get { return m_Hp; }
        }

        /**
         * @brief 
         */
        public int Rage
        {
            get { return m_Rage; }
        }

        /**
         * @brief 
         */
        public int Energy
        {
            get { return m_Energy; }
        }

        public bool GfxDead
        {
            get { return m_GfxDead; }
            set { m_GfxDead = value; }
        }

        public bool SuperArmor
        {
            get { return m_SuperArmor; }
            set
            {
                if (m_SuperArmor != value)
                {
                    m_IsArmorChanged = true;
                    m_SuperArmor = value;
                }
            }
        }
        public bool UltraArmor
        {
            get
            {
                if (m_UltraArmor || !m_AcceptStiff)
                    return true;
                else
                    return false;
            }
            set
            {
                if (m_UltraArmor != value)
                {
                    m_IsArmorChanged = true;
                    m_UltraArmor = value;
                }
            }
        }
        public bool IsArmorChanged
        {
            get { return m_IsArmorChanged; }
            set { m_IsArmorChanged = value; }
        }

        public float HpMaxCoefficient
        {
            get { return m_HpMaxCoefficient; }
            set { m_HpMaxCoefficient = value; }
        }
        public float EnergyMaxCoefficient
        {
            get { return m_EnergyMaxCoefficient; }
            set { m_EnergyMaxCoefficient = value; }
        }

        /**
         * 攻击范围系数（在0~1之间） 
         */
        public float AttackRangeCoefficient
        {
            get { return m_AttackRangeCoefficient; }
            set { m_AttackRangeCoefficient = value; }
        }

        /**
         * 对象的速度系数（在0~1之间）
         */
        public float VelocityCoefficient
        {
            get { return m_VelocityCoefficient; }
            set { m_VelocityCoefficient = value; }
        }

        /**
         * @brief 视野范围
         */
        public float ViewRange
        {
            get { return m_ViewRange; }
            set { m_ViewRange = value; }
        }

        public float GohomeRange
        {
            get { return m_GohomeRange; }
            set { m_GohomeRange = value; }
        }

        public bool IsMoving { set; get; }
        public bool IsFlying { set; get; }
        public Vector3 FlyStartPos { set; get; }

        public float Combat2IdleTime
        {
            get { return m_Combat2IdleTime; }
            set { m_Combat2IdleTime = value; }
        }
        public int Combat2IdleSkill
        {
            get { return m_Combat2IdleSkill; }
            set { m_Combat2IdleSkill = value; }
        }
        public string Idle2CombatWeaponMoves
        {
            get { return m_Idle2CombatWeaponMoves; }
            set { m_Idle2CombatWeaponMoves = value; }
        }
        /**
         * @brief 获取模型名
         *
         * @return 
         */
        public string GetModel()
        {
            return m_Model;
        }

        /**
         * @brief 设置模型名
         *
         * @return 
         */
        public void SetModel(string model)
        {
            m_Model = model;
        }
        /**
         * @brief 获取出生特效
         *
         * @return 
         */
        public string GetBornEffect()
        {
            return m_BornEffect;
        }
        /**
         * @brief 设置出生特效
         *
         * @return 
         */
        public void SetBornEffect(string effect)
        {
            m_BornEffect = effect;
        }

        /**
         * @brief 获取ai列表
         *
         * @return 
         */
        public bool GetAIEnable()
        {
            return m_AIEnable;
        }

        /**
         * @brief 设置ai列表
         *
         * @return 
         */
        public void SetAIEnable(bool enable)
        {
            m_AIEnable = enable;
            if (IsUser)
            {
                //LogSystem.Debug("SetAIEnable:{0} user:{1} heroid:{2} name:{3}", enable, GetId(), GetLinkId(), GetName());
            }
        }

        /**
         * @brief 获取动作id
         *
         * @return 
         */
        public List<int> GetActionList()
        {
            return m_ActionList;
        }

        //public Data_ActionConfig GetCurActionConfig()
        //{
        //    return ActionConfigProvider.Instance.GetCharacterCurActionConfig(GetActionList());
        //}

        /**
         * @brief 设置动作id
         *
         * @return 
         */
        public void SetActionList(List<int> id_list)
        {
            m_ActionList.Clear();
            m_ActionList.AddRange(id_list);
        }

        /**
         * @brief 阵营ID	
         */
        public int GetCampId()
        {
            if (null != m_ControllerObject)
            {
                return m_ControllerObject.GetCampId();
            }
            return m_CampId;
        }

        /**
         * @brief 阵营ID	
         */
        public void SetCampId(int val)
        {
            if (null != m_ControllerObject)
            {
                m_ControllerObject.SetCampId(val);
            }
            m_CampId = val;
        }

        public int KillerId
        {
            get { return m_KillerId; }
            set { m_KillerId = value; }
        }

        public long LastAttackedTime
        {
            get { return m_LastAttackedTime; }
            set { m_LastAttackedTime = value; }
        }

        public long LastAttackTime
        {
            get { return m_LastAttackTime; }
            set { m_LastAttackTime = value; }
        }

        public Dictionary<int, AttackerInfo> AttackerInfos
        {
            get { return m_AttackerInfos; }
        }
        /// <summary>
        /// 避让半径（1=1格，2=3格，3=5格，...）
        /// </summary>
        public int AvoidanceRadius
        {
            get { return m_AvoidanceRadius; }
            set { m_AvoidanceRadius = value; }
        }
        /// <summary>
        /// 掩体ID（使用掩体场景逻辑ID）
        /// </summary>
        public int BlindageId
        {
            get { return m_BlindageId; }
            set { m_BlindageId = value; }
        }
        /// <summary>
        /// 对象所处的掩体区域，如果不在掩体中，则为null
        /// </summary>
        public Vector3[] Blindage
        {
            get { return m_Blindage; }
            set { m_Blindage = value; }
        }
        public long BlindageLeftTime
        {
            get { return m_BlindageLeftTime; }
            set { m_BlindageLeftTime = value; }
        }

        public OnBeginAttackEvent OnBeginAttack;

        public Shape Shape
        {
            get { return m_Shape; }
            set { m_Shape = value; }
        }

        public float GetRadius()
        {
            float radius = 0;
            if (null != m_Shape)
                radius = m_Shape.GetRadius();
            return radius;
        }

        public bool IsHaveStateFlag(CharacterState_Type type)
        {
            return (m_StateFlag & ((int)type)) != 0;
        }

        public bool IsHaveGfxStateFlag(GfxCharacterState_Type type)
        {
            return (m_GfxStateFlag & ((int)type)) != 0;
        }

        public void SetStateFlag(Operate_Type opType, CharacterState_Type mask)
        {
            if (opType == Operate_Type.OT_AddBit)
            {
                m_StateFlag |= (int)mask;
            }
            else if (opType == Operate_Type.OT_RemoveBit)
            {
                m_StateFlag &= ~((int)mask);
            }
        }

        public void SetGfxStateFlag(Operate_Type opType, GfxCharacterState_Type mask)
        {
            if (opType == Operate_Type.OT_AddBit)
            {
                m_StateFlag |= (int)mask;
            }
            else if (opType == Operate_Type.OT_RemoveBit)
            {
                m_StateFlag &= ~((int)mask);
            }
        }

        public int StateFlag
        {
            get { return m_StateFlag; }
            set { m_StateFlag = value; }
        }

        public int GfxStateFlag
        {
            get { return m_GfxStateFlag; }
            set { m_GfxStateFlag = value; }
        }

        /**
         * @brief 预估移动时间
         *
         * @param pos
         *
         * @return 
         */
        public float PredictMoveDuration(Vector2 pos)
        {
            float distance = Vector2.Distance(GetMovementStateInfo().GetPosition2D(), pos);
            float duration = distance / GetActualProperty().MoveSpeed;
            //duration += GameDefines.C_PredictMoveDurationRefix;

            return duration;
        }

        /**
         * @brief 基础属性值
         */
        public CharacterProperty GetBaseProperty()
        {
            return m_BaseProperty;
        }

        /**
         * @brief 当前属性值
         */
        public CharacterProperty GetActualProperty()
        {
            return m_ActualProperty;
        }

        public bool IsDead()
        {
            return Hp <= 0;
        }

        //临时加的，不要调用
        public uint GetActorID()
        {
            return 0;
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetHp(Operate_Type opType, int tVal)
        {
            m_Hp = (int)CharacterProperty.UpdateAttr(m_Hp, m_ActualProperty.HpMax, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetRage(Operate_Type opType, int tVal)
        {
            int result = (int)CharacterProperty.UpdateAttr(m_Rage, m_ActualProperty.RageMax, opType, tVal);
            if (result > m_ActualProperty.RageMax)
            {
                result = m_ActualProperty.RageMax;
            }
            else if (result < 0)
            {
                result = 0;
            }
            m_Rage = result;
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetEnergy(Operate_Type opType, int tVal)
        {
            int result = (int)CharacterProperty.UpdateAttr(m_Energy, m_ActualProperty.EnergyMax, opType, tVal);
            if (result > m_ActualProperty.EnergyMax)
            {
                result = m_ActualProperty.RageMax;
            }
            else if (result < 0)
            {
                result = 0;
            }
            m_Energy = result;
        }

        public void ResetAttackerInfo()
        {
            KillerId = 0;
            AttackerInfos.Clear();
            m_LastAttackedTime = 0;
            m_LastAttackTime = 0;
        }

        public void SetAttackerInfo(int attackId, int attackerType, bool isKiller, bool isShootDamage, bool isCritical, int hpDamage, int npDamage)
        {
            if (isKiller)
                KillerId = attackId;
            long curTime = TimeUtility.GetServerMilliseconds();
            LastAttackedTime = curTime;
            if (!AttackerInfos.ContainsKey(attackId))
            {
                AttackerInfos.Add(attackId, new AttackerInfo { m_AttackerType = attackerType, m_AttackTime = curTime, m_HpDamage = hpDamage, m_NpDamage = npDamage });
            }
            else
            {
                AttackerInfo info = AttackerInfos[attackId];
                info.m_AttackTime = curTime;
                info.m_HpDamage += hpDamage;
                info.m_NpDamage += npDamage;
                AttackerInfos[attackId] = info;
            }
            if (IsNpc)
            {
                //NpcManager.FireDamageEvent(GetId(), attackId, isShootDamage, isCritical, hpDamage, npDamage);
            }
            else
            {
                //UserManager.FireDamageEvent(GetId(), attackId, isShootDamage, isCritical, hpDamage, npDamage);
            }
        }
        public void SetAttackTime()
        {
            LastAttackTime = TimeUtility.GetServerMilliseconds();
        }
        public void CalcBaseAttr()
        {
            float aMoveSpeed = GetBaseProperty().MoveSpeed;
            float aWalkSpeed = GetBaseProperty().WalkSpeed;
            float aRunSpeed = GetBaseProperty().RunSpeed;
            int aHpMax = GetBaseProperty().HpMax;
            int aEnergyMax = GetBaseProperty().EnergyMax;
            float aHpRecover = GetBaseProperty().HpRecover;
            float aEnergyRecover = GetBaseProperty().EnergyRecover;
            int aAttackBase = GetBaseProperty().AttackBase;
            int aADefenceBase = GetBaseProperty().ADefenceBase;
            int aMDefenceBase = GetBaseProperty().MDefenceBase;
            float aCritical = GetBaseProperty().Critical;
            float aCriticalPow = GetBaseProperty().CriticalPow;
            float aCriticalBackHitPow = GetBaseProperty().CriticalBackHitPow;
            float aCriticalCrackPow = GetBaseProperty().CriticalCrackPow;
            float aFireDamage = GetBaseProperty().FireDamage;
            float aFireErd = GetBaseProperty().FireERD;
            float aIceDamage = GetBaseProperty().IceDamage;
            float aIceErd = GetBaseProperty().IceERD;
            float aPoisonDamage = GetBaseProperty().PoisonDamage;
            float aPoisonErd = GetBaseProperty().PoisonERD;
            float aWeight = GetBaseProperty().Weight;
            float aRps = GetBaseProperty().Rps;
            float aAttackRange = GetBaseProperty().AttackRange;


            //if (null != m_LevelupConfig)
            //{
            //    int rate = (m_Level > 0 ? m_Level : 0);
            //    aMoveSpeed += m_LevelupConfig.m_AttrData.GetAddSpd(0, 0) * rate;
            //    aHpMax += (int)(m_LevelupConfig.m_AttrData.GetAddHpMax(0, 0) * rate);
            //    aEnergyMax += (int)(m_LevelupConfig.m_AttrData.GetAddEpMax(0, 0) * rate);
            //    aHpRecover += m_LevelupConfig.m_AttrData.GetAddHpRecover(0, 0) * rate;
            //    aEnergyRecover += m_LevelupConfig.m_AttrData.GetAddEpRecover(0, 0) * rate;
            //    aAttackBase += (int)(m_LevelupConfig.m_AttrData.GetAddAd(0, 0) * rate);
            //    aADefenceBase += (int)(m_LevelupConfig.m_AttrData.GetAddADp(0, 0) * rate);
            //    aMDefenceBase += (int)(m_LevelupConfig.m_AttrData.GetAddMDp(0, 0) * rate);
            //    aCritical += m_LevelupConfig.m_AttrData.GetAddCri(0, 0) * rate;
            //    aCriticalPow += m_LevelupConfig.m_AttrData.GetAddPow(0, 0) * rate;
            //    aCriticalBackHitPow += m_LevelupConfig.m_AttrData.GetAddBackHitPow(0, 0) * rate;
            //    aCriticalCrackPow += m_LevelupConfig.m_AttrData.GetAddCrackPow(0, 0) * rate;
            //    aFireDamage += m_LevelupConfig.m_AttrData.GetAddFireDam(0, 0) * rate;
            //    aFireErd += m_LevelupConfig.m_AttrData.GetAddFireErd(0, 0) * rate;
            //    aIceDamage += m_LevelupConfig.m_AttrData.GetAddIceDam(0, 0) * rate;
            //    aIceErd += m_LevelupConfig.m_AttrData.GetAddIceErd(0, 0) * rate;
            //    aPoisonDamage += m_LevelupConfig.m_AttrData.GetAddPoisonDam(0, 0) * rate;
            //    aPoisonErd += m_LevelupConfig.m_AttrData.GetAddPoisonErd(0, 0) * rate;
            //    aWeight += m_LevelupConfig.m_AttrData.GetAddWeight(0, 0) * rate;
            //    aRps += m_LevelupConfig.m_AttrData.GetAddRps(0, 0) * rate;
            //    aAttackRange += m_LevelupConfig.m_AttrData.GetAddAttackRange(0, 0) * rate;
            //}

            GetActualProperty().SetMoveSpeed(Operate_Type.OT_Absolute, aMoveSpeed);
            GetActualProperty().SetWalkSpeed(Operate_Type.OT_Absolute, aWalkSpeed);
            GetActualProperty().SetRunSpeed(Operate_Type.OT_Absolute, aRunSpeed);
            GetActualProperty().SetHpMax(Operate_Type.OT_Absolute, aHpMax);
            GetActualProperty().SetRageMax(Operate_Type.OT_Absolute, GetBaseProperty().RageMax);
            GetActualProperty().SetEnergyMax(Operate_Type.OT_Absolute, aEnergyMax);
            GetActualProperty().SetHpRecover(Operate_Type.OT_Absolute, aHpRecover);
            GetActualProperty().SetEnergyRecover(Operate_Type.OT_Absolute, aEnergyRecover);
            GetActualProperty().SetAttackBase(Operate_Type.OT_Absolute, aAttackBase);
            GetActualProperty().SetADefenceBase(Operate_Type.OT_Absolute, aADefenceBase);
            GetActualProperty().SetMDefenceBase(Operate_Type.OT_Absolute, aMDefenceBase);
            GetActualProperty().SetCritical(Operate_Type.OT_Absolute, aCritical);
            GetActualProperty().SetCriticalPow(Operate_Type.OT_Absolute, aCriticalPow);
            GetActualProperty().SetCriticalBackHitPow(Operate_Type.OT_Absolute, aCriticalBackHitPow);
            GetActualProperty().SetCriticalCrackPow(Operate_Type.OT_Absolute, aCriticalCrackPow);
            GetActualProperty().SetFireDamage(Operate_Type.OT_Absolute, aFireDamage);
            GetActualProperty().SetFireERD(Operate_Type.OT_Absolute, aFireErd);
            GetActualProperty().SetIceDamage(Operate_Type.OT_Absolute, aIceDamage);
            GetActualProperty().SetIceERD(Operate_Type.OT_Absolute, aIceErd);
            GetActualProperty().SetPoisonDamage(Operate_Type.OT_Absolute, aPoisonDamage);
            GetActualProperty().SetPoisonERD(Operate_Type.OT_Absolute, aPoisonErd);
            GetActualProperty().SetWeight(Operate_Type.OT_Absolute, aWeight);
            GetActualProperty().SetRps(Operate_Type.OT_Absolute, aRps);
            GetActualProperty().SetAttackRange(Operate_Type.OT_Absolute, aAttackRange);
        }

        public void TransformShape()
        {
            if (null != m_Shape)
            {
                m_Shape.Transform(GetMovementStateInfo().GetPosition3D(), GetMovementStateInfo().FaceDirCosAngle, GetMovementStateInfo().FaceDirSinAngle);
            }
        }

        public bool LevelChanged
        {
            get { return m_LevelChanged; }
            set { m_LevelChanged = value; }
        }

        /*
         * 一个对象控制另一个对象时，对控制对象的操作（武器、装备、技能）将转移到受控制对象（数据与表现）。
         * 1、位置在移动模块里让受控对象跟随控制对象（开始控制时将控制对象移动到受控对象位置）。
         * 2、逻辑上二个对象仍然是分离的，仅仅是控制的目标发生转移（为了保证体系上一致，这种转移在控制层处理【目前对应于客户端输入、客户端与服务器消息处理】）。
         * 3、只影响交互式操作的状态数据不转移，仍然影响控制对象而不是影响受控对象(这里需要仔细考虑是否逻辑概念)。
         * 4、玩家对控制对象的操作所基于的逻辑判断由各操作所需的信息决定（即如果操作依赖武器装备与技能则使用受控制对象的数据，否则用控制对象的数据）。
         * 5、表现上，受控对象成为显示主体，控制对象根据配置挂接到受控对象上或隐藏。
         * 6、战斗骑乘特殊，此时控制对象的操作不转移到受控对象，仅仅是位置跟随与表现上挂接（如果在骑乘时不允许战斗，则可以用没有战斗配置的机甲实现）。
         */
        public SkillStateInfo GetRealControlledSkillStateInfo()
        {
            if (null != m_ControlledObject && !m_ControlledObject.m_IsHorse)
                return m_ControlledObject.GetRealControlledSkillStateInfo();
            return m_SkillStateInfo;
        }
        //public EquipmentStateInfo GetRealControlledEquipmentStateInfo()
        //{
        //    if (null != m_ControlledObject && !m_ControlledObject.m_IsHorse)
        //        return m_ControlledObject.GetRealControlledEquipmentStateInfo();
        //    return m_EquipmentStateInfo;
        //}
        public CharacterInfo GetRealControlledObject()
        {
            if (null != m_ControlledObject && !m_ControlledObject.m_IsHorse)
                return m_ControlledObject.GetRealControlledObject();
            return this;
        }
        public CharacterInfo ControllerObject
        {
            get { return m_ControllerObject; }
            set { m_ControllerObject = value; }
        }
        public CharacterInfo ControlledObject
        {
            get { return m_ControlledObject; }
            set { m_ControlledObject = value; }
        }
        public bool CanControl
        {
            get { return m_IsMecha || m_IsHorse; }
        }
        public bool IsTask
        {
            get
            {
                return m_IsTask;
            }
        }
        public bool IsPvpTower
        {
            get
            {
                return m_IsPvpTower;
            }
        }
        public bool IsControlMecha
        {
            get
            {
                bool ret = false;
                if (null != m_ControlledObject && m_ControlledObject.m_IsMecha)
                {
                    ret = true;
                }
                return ret;
            }
        }
        public bool IsControlHorse
        {
            get
            {
                bool ret = false;
                if (null != m_ControlledObject && m_ControlledObject.m_IsHorse)
                {
                    ret = true;
                }
                return ret;
            }
        }

        public MovementStateInfo GetMovementStateInfo()
        {
            return m_MovementStateInfo;
        }
        public SkillStateInfo GetSkillStateInfo()
        {
            return m_SkillStateInfo;
        }
        //public ISkillController SkillController
        //{
        //    get { return m_SkillController; }
        //    set { m_SkillController = value; }
        //}
        //public EquipmentStateInfo GetEquipmentStateInfo()
        //{
        //    return m_EquipmentStateInfo;
        //}
        //public LegacyStateInfo GetLegacyStateInfo()
        //{
        //    return m_LegacyStateInfo;
        //}
        public StarWarsSpatial.ISpaceObject SpaceObject
        {
            get { return m_SpaceObject; }
        }
        public SceneContextInfo SceneContext
        {
            get { return m_SceneContext; }
            set { m_SceneContext = value; }
        }
        public ISpatialSystem SpatialSystem
        {
            get
            {
                ISpatialSystem sys = null;
                if (null != m_SceneContext)
                {
                    sys = m_SceneContext.SpatialSystem;
                }
                return sys;
            }
        }
        //public SceneLogicInfoManager SceneLogicInfoManager
        //{
        //    get
        //    {
        //        SceneLogicInfoManager mgr = null;
        //        if (null != m_SceneContext)
        //        {
        //            mgr = m_SceneContext.SceneLogicInfoManager;
        //        }
        //        return mgr;
        //    }
        //}
        //public NpcManager NpcManager
        //{
        //    get
        //    {
        //        NpcManager mgr = null;
        //        if (null != m_SceneContext)
        //        {
        //            mgr = m_SceneContext.NpcManager;
        //        }
        //        return mgr;
        //    }
        //}
        public UserManager UserManager
        {
            get
            {
                UserManager mgr = null;
                if (null != m_SceneContext)
                {
                    mgr = m_SceneContext.UserManager;
                }
                return mgr;
            }
        }
        public BlackBoard BlackBoard
        {
            get
            {
                BlackBoard blackBoard = null;
                if (null != m_SceneContext)
                {
                    blackBoard = m_SceneContext.BlackBoard;
                }
                return blackBoard;
            }
        }

        public float Cross2StandTime
        {
            get { return m_Cross2StandTime; }
        }
        public float Cross2RunTime
        {
            get { return m_Cross2RunTime; }
        }
        public float DeadAnimTime
        {
            get { return m_DeadAnimTime; }
        }

        public int OwnerId
        {
            set { m_OwnerId = value; }
            get { return m_OwnerId; }
        }

        public bool IsUnderControl()
        {
            if (IsHaveGfxStateFlag(GfxCharacterState_Type.GetUp) ||
               IsHaveGfxStateFlag(GfxCharacterState_Type.HitFly) ||
               IsHaveGfxStateFlag(GfxCharacterState_Type.KnockDown) ||
               IsHaveGfxStateFlag(GfxCharacterState_Type.Stiffness))
            {
                return true;
            }
            return false;
        }
        public void ResetCross2StandRunTime()
        {
            if (GetSkillStateInfo() == null)
            {
                return;
            }
            SkillStateInfo skillstateinfo = GetSkillStateInfo();
            skillstateinfo.CrossToRunTime = Cross2RunTime;
            skillstateinfo.CrossToStandTime = Cross2StandTime;
        }

        public void ResetSkill()
        {
            //if (null != SkillController)
            //{
            //    SkillController.Init();
            //}
        }

        public void SetCanUseSkill(bool can_use_skill) { m_CanUseSkill = can_use_skill; }
        public bool CanUseSkill()
        {
            if (!m_CanUseSkill)
            {
                return false;
            }
            if (IsDead())
            {
                return false;
            }
            if (IsHaveStateFlag(CharacterState_Type.CST_Sleep))
            {
                return false;
            }
            if (IsHaveStateFlag(CharacterState_Type.CST_Silence))
            {
                return false;
            }
            if (GetSkillStateInfo().IsSkillActivated())
            {
                return false;
            }
            if (GetSkillStateInfo().IsImpactControl())
            {
                return false;
            }
            if (null != m_ControlledObject)
            {
                return m_ControlledObject.CanUseSkill();
            }
            return true;
        }

        protected void ResetCharacterInfo()
        {
            m_OwnerId = -1;
            SetAIEnable(true);
            DeadTime = 0;
            EmptyBloodTime = 0;

            IsMoving = false;
            m_CampId = 0;

            m_Blindage = null;
            m_BlindageId = 0;
            m_BlindageLeftTime = 0;

            m_BeAttack = false;
            m_CurEnemyId = 0;
            m_MeetEnemyImpact = 0;
            IsFlying = false;

            m_ControllerObject = null;
            m_ControlledObject = null;

            m_FightingScore = 0;
            m_ActionList.Clear();

            m_Level = 0;
            m_Hp = 0;
            m_Energy = 0;

            m_HpMaxCoefficient = 1;
            m_EnergyMaxCoefficient = 1;
            m_AttackRangeCoefficient = 1;
            m_VelocityCoefficient = 1;
            m_StateFlag = 0;
            OnBeginAttack = null;
            m_GfxDead = false;
            m_SuperArmor = false;
            m_IsArmorChanged = true;

            m_IsMecha = false;
            m_IsHorse = false;
            m_IsTask = false;
            m_IsPvpTower = false;

            m_CanUseSkill = true;
            m_KillerId = 0;

            m_CurBlueCanSeeMe = false;
            m_CurRedCanSeeMe = false;
            m_LastBlueCanSeeMe = false;
            m_LastRedCanSeeMe = false;

            SetHp(Operate_Type.OT_Absolute, GetActualProperty().HpMax);
            SetRage(Operate_Type.OT_Absolute, 0);
            SetEnergy(Operate_Type.OT_Absolute, GetActualProperty().EnergyMax);

            ResetAttackerInfo();

            GetMovementStateInfo().Reset();
            GetSkillStateInfo().Reset();
            //GetEquipmentStateInfo().Reset();
        }

        protected int m_Id = 0;
        /**
         * @brief 单位ID，在地图中赋予的id，用于剧情查找
         */
        protected int m_UnitId = 0;
        protected int m_LinkId = 0;
        protected int m_OwnerId = -1;
        protected string m_Name = "";
        protected int m_Level = 1;

        protected float m_Combat2IdleTime = 3;
        protected int m_Combat2IdleSkill = 0;
        protected string m_Idle2CombatWeaponMoves = "";

        private bool m_LevelChanged = false;
        private int m_Hp = 0;
        private int m_Rage = 0;
        private int m_Energy = 0;
        private float m_HpMaxCoefficient = 1;
        private float m_EnergyMaxCoefficient = 1;
        private float m_AttackRangeCoefficient = 1;
        private float m_VelocityCoefficient = 1;
        private float m_ViewRange = 0;
        private float m_GohomeRange = 0;
        private bool m_GfxDead = false;
        private bool m_SuperArmor = false;  // 可被破除的霸体
        private bool m_UltraArmor = false;  // 不可被破除的霸体
        private bool m_IsArmorChanged = true;
        protected string m_Model = "";
        protected string m_BornEffect = "";

        protected bool m_AIEnable = true;
        protected List<int> m_ActionList = new List<int>();
        private int m_CampId = 0;

        private bool m_CanUseSkill = true;
        private int m_KillerId = 0;

        private float m_FightingScore = 0;
        /************************************************************************/
        /* 助攻列表                                                             */
        /************************************************************************/
        private Dictionary<int, AttackerInfo> m_AttackerInfos = new Dictionary<int, AttackerInfo>();
        private long m_LastAttackedTime = 0;
        private long m_LastAttackTime = 0;
        //掩体信息
        private int m_BlindageId = 0;
        private Vector3[] m_Blindage = null;
        private long m_BlindageLeftTime = 0;

        private Shape m_Shape = null;
        private int m_AvoidanceRadius = 1;
        protected bool m_BeAttack = false;
        private int m_StateFlag = 0;
        private int m_GfxStateFlag = 0;

        private long m_ReleaseTime = 0;  //尸体存在时间
        private long m_EmptyBloodTime = 0;
        private long m_DeadTime = 0;
        protected bool m_CauseStiff = true;
        protected bool m_AcceptStiff = true;
        protected bool m_AcceptStiffEffect = true;
        /**
         * @brief 基础属性值
         */
        protected CharacterProperty m_BaseProperty;
        /**
         * @brief 当前属性值
         */
        protected CharacterProperty m_ActualProperty;

        private MovementStateInfo m_MovementStateInfo = new MovementStateInfo();
        private SkillStateInfo m_SkillStateInfo = new SkillStateInfo();
        //private ISkillController m_SkillController = null;
        //private EquipmentStateInfo m_EquipmentStateInfo = new EquipmentStateInfo();
        //private LegacyStateInfo m_LegacyStateInfo = new LegacyStateInfo();
        private SceneContextInfo m_SceneContext = null;
        protected ISpaceObject m_SpaceObject = null;
        //protected LevelupConfig m_LevelupConfig = null;

        protected int m_CurEnemyId = 0;
        protected int m_MeetEnemyImpact = 0;

        protected float m_Cross2StandTime = 0.5f;
        protected float m_Cross2RunTime = 0.3f;
        protected float m_DeadAnimTime = 1.4f;

        private CellPos m_SightCell;
        private bool m_CurBlueCanSeeMe = true;    //移动版本不计算视野，默认都可见
        private bool m_LastBlueCanSeeMe = false;
        private bool m_CurRedCanSeeMe = true;     //移动版本不计算视野，默认都可见
        private bool m_LastRedCanSeeMe = false;

        private CharacterInfo m_ControllerObject = null;
        private CharacterInfo m_ControlledObject = null;

        protected bool m_IsMecha = false;
        protected bool m_IsHorse = false;
        protected bool m_IsTask = false;
        protected bool m_IsPvpTower = false;

        //protected NpcInfo m_CastNpcInfo = null;
        protected UserInfo m_CastUserInfo = null;

        //阵营可为Friendly、Hostile、Blue、Red
        //Friendly 全部友好
        //Hostile 全部敌对
        //Blue 与Hostile与Red敌对
        //Red与Hostile与Blue敌对
        public static CharacterRelation GetRelation(CharacterInfo pObj_A, CharacterInfo pObj_B)
        {
            if (pObj_A == null || pObj_B == null)
            {
                return CharacterRelation.RELATION_ENEMY;
            }

            if (pObj_A == pObj_B)
            {
                return CharacterRelation.RELATION_FRIEND;
            }

            int campA = pObj_A.GetCampId();
            int campB = pObj_B.GetCampId();
            return GetRelation(campA, campB);
        }
        public static CharacterRelation GetRelation(int campA, int campB)
        {
            CharacterRelation relation = CharacterRelation.RELATION_INVALID;
            if ((int)CampIdEnum.Unkown != campA && (int)CampIdEnum.Unkown != campB)
            {
                if (campA == campB)
                    relation = CharacterRelation.RELATION_FRIEND;
                else if (campA == (int)CampIdEnum.Friendly || campB == (int)CampIdEnum.Friendly)
                    relation = CharacterRelation.RELATION_FRIEND;
                else if (campA == (int)CampIdEnum.Hostile || campB == (int)CampIdEnum.Hostile)
                    relation = CharacterRelation.RELATION_ENEMY;
                else
                    relation = CharacterRelation.RELATION_ENEMY;
            }
            return relation;
        }
        public static void ControlObject(CharacterInfo controller, CharacterInfo controlled)
        {
            if (null != controller && null != controlled)
            {
                ReleaseControlObject(controller, controlled);
                controller.ControlledObject = controlled;
                controlled.ControllerObject = controller;
            }
        }
        public static void ReleaseControlObject(CharacterInfo controller, CharacterInfo controlled)
        {
            ReleaseControlledObject(controller);
            ReleaseControllerObject(controlled);
        }
        public static void ReleaseControlledObject(CharacterInfo controller)
        {
            if (null != controller)
            {
                CharacterInfo controlled = controller.ControlledObject;
                controller.ControlledObject = null;
                if (null != controlled)
                    controlled.ControllerObject = null;
            }
        }
        public static void ReleaseControllerObject(CharacterInfo controlled)
        {
            if (null != controlled)
            {
                CharacterInfo controller = controlled.ControllerObject;
                controlled.ControllerObject = null;
                if (null != controller)
                    controller.ControlledObject = null;
            }
        }
        public static bool CanSee(CharacterInfo source, CharacterInfo target)
        {
            bool ret = false;
            if (null != source && null != target)
            {
                Vector3 pos1 = source.GetMovementStateInfo().GetPosition3D();
                Vector3 pos2 = target.GetMovementStateInfo().GetPosition3D();
                float distSqr = StarWars.Geometry.DistanceSquare(pos1, pos2);
                return CanSee(source, target, distSqr, pos1, pos2);
            }
            return ret;
        }
        public static bool CanSee(CharacterInfo source, CharacterInfo target, float distSqr, Vector3 pos1, Vector3 pos2)
        {
            bool ret = false;
            if (null != source && null != target)
            {
                //一、先判断距离
                if (distSqr < source.ViewRange * source.ViewRange)
                {
                    //二、再判断逻辑
                    //后面修改的同学注意下：
                    //1、我们目前的object层是数据接口层，是不需要使用多态的。概念变化的可能性比功能变化的可能性要小很多，所以我们将多态机制应用到Logic里。
                    //2、逻辑上的影响可能是对象buff或类型产生，如果判断逻辑比较复杂，采用结构化编程的风格拆分成多个函数即可。
                    //3、另一个不建议用多态理由是这个函数的调用频率会很高。
                    if (source.GetCampId() == target.GetCampId() ||
                      (!target.IsHaveStateFlag(CharacterState_Type.CST_Hidden)))
                    {//隐身状态判断（未考虑反隐）
                        ret = true;//移动版本不计算视野，只考虑逻辑上的几个点供ai用
                        /*
                        //判断掩体（草丛）,目标不在草丛或者与源在同一草丛
                        if (null == target.Blindage || source.BlindageId == target.BlindageId) {
                          //三、最后判断空间关系
                          ret = source.SpatialSystem.CanSee(pos1, pos2);
                          if (ret && source.IsUser && target.IsNpc) {
                            int row1, col1, row2, col2;
                            target.SpatialSystem.GetCellMapView(1).GetCell(pos1, out row1, out col1);
                            target.SpatialSystem.GetCellMapView(1).GetCell(pos2, out row2, out col2);
                            byte status1 = target.SpatialSystem.GetCellStatus(row1, col1);
                            byte status2 = target.SpatialSystem.GetCellStatus(row2, col2);
                            byte lvl1 = BlockType.GetBlockLevel(status1);
                            byte lvl2 = BlockType.GetBlockLevel(status2);
                            if (lvl1 < lvl2) {
                              LogSystem.Debug("user {0}({1},{2}:{3}) can see user {4}({5},{6}:{7})", source.GetLinkId(), row1, col1, status1, target.GetLinkId(), row2, col2, status2);
                              ret = source.SpatialSystem.CanSee(pos1, pos2);
                            }
                          }
                        }
                        */
                    }
                }
            }
            return ret;
        }
    }
}
