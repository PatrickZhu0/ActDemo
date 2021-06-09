/**
 * @file CharacterProperty.cs
 * @brief 角色属性类
 *          用于技能系统中的属性变化
 *
 * @author 
 * @version
 * @date 
 */

using System;
using System.Collections.Generic;

namespace StarWars
{
    /**
     * @brief 角色属性类
     */
    public sealed class CharacterProperty
    {
        /**
         * @brief 构造函数
         *
         * @param owner
         *
         * @return 
         */
        public CharacterProperty()
        { }

        /**
         * @brief 移动速度
         */
        public float MoveSpeed
        {
            get { return m_MoveSpeed; }
        }
        /**
         * @brief 行走速度
         */
        public float WalkSpeed
        {
            get { return m_WalkSpeed; }
        }
        /**
         * @brief 跑路速度
         */
        public float RunSpeed
        {
            get { return m_RunSpeed; }
        }
        /**
         * @brief 最大血量
         */
        public int HpMax
        {
            get { return m_HpMax; }
        }

        /**
         * @brief 最大血量
         */
        public int RageMax
        {
            get { return m_RageMax; }
        }

        /**
         * @brief 最大能量
         */
        public int EnergyMax
        {
            get { return m_EnergyMax; }
        }

        /**
         * @brief 生命值回复速度
         */
        public float HpRecover
        {
            get { return m_HpRecover; }
        }

        /**
         * @brief 能量值回复速度
         */
        public float EnergyRecover
        {
            get { return m_EnergyRecover; }
        }

        /**
         * @brief 基础攻击力
         */
        public int AttackBase
        {
            get { return m_AttackBase; }
        }

        /**
         * @brief 基础防御力
         */
        public int ADefenceBase
        {
            get { return m_ADefenceBase; }
        }

        /**
         * @brief 基础防御力
         */
        public int MDefenceBase
        {
            get { return m_MDefenceBase; }
        }

        /**
         * @brief 暴击率
         */
        public float Critical
        {
            get { return m_Critical; }
        }

        /**
         * @brief 暴击额外伤害比率
         */
        public float CriticalPow
        {
            get { return m_CriticalPow; }
        }

        /**
         * @brief 背击时额外造成伤害的比率
         */
        public float CriticalBackHitPow
        {
            get { return m_CriticalBackHitPow; }
        }

        /**
         * @brief 破招时额外造成伤害的比率
         */
        public float CriticalCrackPow
        {
            get { return m_CriticalCrackPow; }
        }

        /**
         * @brief 火伤害
         */
        public float FireDamage
        {
            get { return m_FireDam; }
        }

        /**
         * @brief 火抗性
         */
        public float FireERD
        {
            get { return m_FireERD; }
        }

        /**
         * @brief 冰伤害
         */
        public float IceDamage
        {
            get { return m_IceDam; }
        }

        /**
         * @brief 冰抗性
         */
        public float IceERD
        {
            get { return m_IceERD; }
        }

        /**
         * @brief 毒伤害
         */
        public float PoisonDamage
        {
            get { return m_PoisonDam; }
        }

        /**
         * @brief 毒抗性
         */
        public float PoisonERD
        {
            get { return m_PoisonERD; }
        }

        /**
         * @brief 重量
         */
        public float Weight
        {
            get { return m_Weight; }
        }

        /**
         * @brief 攻击速度
         */
        public float Rps
        {
            get { return m_Rps; }
        }

        /**
         * @brief 攻击距离
         */
        public float AttackRange
        {
            get { return m_AttackRange; }
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetMoveSpeed(Operate_Type opType, float tVal)
        {
            m_MoveSpeed = UpdateAttr(m_MoveSpeed, m_MoveSpeed, opType, tVal);
        }
        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetWalkSpeed(Operate_Type opType, float tVal)
        {
            m_WalkSpeed = UpdateAttr(m_WalkSpeed, m_WalkSpeed, opType, tVal);
        }
        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetRunSpeed(Operate_Type opType, float tVal)
        {
            m_RunSpeed = UpdateAttr(m_RunSpeed, m_RunSpeed, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetHpMax(Operate_Type opType, int tVal)
        {
            m_HpMax = (int)UpdateAttr(m_HpMax, m_HpMax, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetRageMax(Operate_Type opType, int tVal)
        {
            m_RageMax = (int)UpdateAttr(m_RageMax, m_RageMax, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetEnergyMax(Operate_Type opType, int tVal)
        {
            m_EnergyMax = (int)UpdateAttr(m_EnergyMax, m_EnergyMax, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetHpRecover(Operate_Type opType, float tVal)
        {
            m_HpRecover = UpdateAttr(m_HpRecover, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetEnergyRecover(Operate_Type opType, float tVal)
        {
            m_EnergyRecover = UpdateAttr(m_EnergyRecover, opType, tVal);
        }
        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetAttackBase(Operate_Type opType, int tVal)
        {
            m_AttackBase = (int)UpdateAttr(m_AttackBase, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetADefenceBase(Operate_Type opType, int tVal)
        {
            m_ADefenceBase = (int)UpdateAttr(m_ADefenceBase, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetMDefenceBase(Operate_Type opType, int tVal)
        {
            m_MDefenceBase = (int)UpdateAttr(m_MDefenceBase, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetCritical(Operate_Type opType, float tVal)
        {
            m_Critical = UpdateAttr(m_Critical, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetCriticalPow(Operate_Type opType, float tVal)
        {
            m_CriticalPow = UpdateAttr(m_CriticalPow, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetCriticalBackHitPow(Operate_Type opType, float tVal)
        {
            m_CriticalBackHitPow = UpdateAttr(m_CriticalBackHitPow, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetCriticalCrackPow(Operate_Type opType, float tVal)
        {
            m_CriticalCrackPow = UpdateAttr(m_CriticalCrackPow, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetFireDamage(Operate_Type opType, float tVal)
        {
            m_FireDam = UpdateAttr(m_FireDam, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetFireERD(Operate_Type opType, float tVal)
        {
            m_FireERD = UpdateAttr(m_FireERD, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetIceDamage(Operate_Type opType, float tVal)
        {
            m_IceDam = UpdateAttr(m_IceDam, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetIceERD(Operate_Type opType, float tVal)
        {
            m_IceERD = UpdateAttr(m_IceERD, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetPoisonDamage(Operate_Type opType, float tVal)
        {
            m_PoisonDam = UpdateAttr(m_PoisonDam, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetPoisonERD(Operate_Type opType, float tVal)
        {
            m_PoisonERD = UpdateAttr(m_PoisonERD, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetRps(Operate_Type opType, float tVal)
        {
            m_Rps = UpdateAttr(m_Rps, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetAttackRange(Operate_Type opType, float tVal)
        {
            m_AttackRange = UpdateAttr(m_AttackRange, opType, tVal);
        }

        /**
         * @brief 角色属性修改
         *
         * @param optype 操作类型
         * @param val 值
         *
         */
        public void SetWeight(Operate_Type opType, float tVal)
        {
            m_Weight = UpdateAttr(m_Weight, opType, tVal);
        }

        public static float UpdateAttr(float val, float maxVal, Operate_Type opType, float tVal)
        {
            float ret = val;
            if (opType == Operate_Type.OT_PercentMax)
            {
                float t = maxVal * (tVal / 100.0f);
                ret = t;
            }
            else
            {
                ret = UpdateAttr(val, opType, tVal);
            }
            return ret;
        }

        public static float UpdateAttr(float val, Operate_Type opType, float tVal)
        {
            float ret = val;
            if (opType == Operate_Type.OT_Absolute)
            {
                ret = tVal;
            }
            else if (opType == Operate_Type.OT_Relative)
            {
                float t = (ret + tVal);
                if (t < 0)
                {
                    t = 0;
                }
                ret = t;
            }
            else if (opType == Operate_Type.OT_PercentCurrent)
            {
                float t = (ret * (tVal / 100.0f));
                ret = t;
            }
            return ret;
        }

        /**
         * @brief 奔跑速度
         */
        private float m_MoveSpeed;
        /**
         * @brief 走路速度
         */
        private float m_WalkSpeed;
        /**
         * @brief 跑路速度
         */
        private float m_RunSpeed;
        /**
         * @brief 最大生命值
         */
        private int m_HpMax;
        /**
         * @brief 最大怒气值
         */
        private int m_RageMax;
        /**
         * @brief 最大能量值
         */
        private int m_EnergyMax;
        /**
         * @brief 生命值回复速度
         */
        private float m_HpRecover;
        /**
         * @brief 能量值回复速度
         */
        private float m_EnergyRecover;
        /**
         * @brief 基础攻击力
         */
        private int m_AttackBase;
        /**
         * @brief 物理防御力
         */
        private int m_ADefenceBase;
        /**
         * @brief 魔法防御力
         */
        private int m_MDefenceBase;
        /**
         * @brief 暴击率
         */
        private float m_Critical;
        /**
         * @brief 暴击额外伤害比率
         */
        private float m_CriticalPow;
        /**
         * @brief 背击时额外造成伤害的比率
         */
        private float m_CriticalBackHitPow;
        /**
         * @brief 破招时额外造成伤害的比率
         */
        private float m_CriticalCrackPow;
        /**
         * @brief 火伤害
         */
        private float m_FireDam;
        /**
         * @brief 火抗性
         */
        private float m_FireERD;
        /**
         * @brief 冰伤害
         */
        private float m_IceDam;
        /**
         * @brief 冰抗性
         */
        private float m_IceERD;
        /**
         * @brief 毒伤害
         */
        private float m_PoisonDam;
        /**
         * @brief 毒抗性
         */
        private float m_PoisonERD;
        /**
         * @brief 重量
         */
        private float m_Weight;
        /**
         * @brief 攻击速度
         */
        private float m_Rps;
        /**
         * @brief 攻击距离
         */
        private float m_AttackRange;
    }
}
