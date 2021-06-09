using System;

namespace StarWars
{
    /// 
    /// @brief
    ///   根据配置修正角色的属性值
    ///   配置文件为 FilePathDefine.cs中C_BuffConfig所对应的文件
    ///
    class BuffRefixProperty
    {
        public static void RefixCharacterProperty(CharacterInfo entity, int buffId, float factor)
        {
            BuffConfig buffConfig = BuffConfigProvider.Instance.GetDataById(buffId);
            if (null == buffConfig) return;

            float aMoveSpeed = entity.GetActualProperty().MoveSpeed;
            float aWalkSpeed = entity.GetActualProperty().WalkSpeed;
            int aHpMax = entity.GetActualProperty().HpMax;
            int aEnergyMax = entity.GetActualProperty().EnergyMax;
            float aHpRecover = entity.GetActualProperty().HpRecover;
            float aEnergyRecover = entity.GetActualProperty().EnergyRecover;
            int aAttackBase = entity.GetActualProperty().AttackBase;
            int aADefenceBase = entity.GetActualProperty().ADefenceBase;
            int aMDefenceBase = entity.GetActualProperty().MDefenceBase;
            float aCritical = entity.GetActualProperty().Critical;
            float aCriticalPow = entity.GetActualProperty().CriticalPow;
            float aCriticalBackHitPow = entity.GetActualProperty().CriticalBackHitPow;
            float aCriticalCrackPow = entity.GetActualProperty().CriticalCrackPow;
            float aFireDamage = entity.GetActualProperty().FireDamage;
            float aFireERD = entity.GetActualProperty().FireERD;
            float aIceDamage = entity.GetActualProperty().IceDamage;
            float aIceERD = entity.GetActualProperty().IceERD;
            float aPoisonDamage = entity.GetActualProperty().PoisonDamage;
            float aPoisonERD = entity.GetActualProperty().PoisonERD;
            float aWeight = entity.GetActualProperty().Weight;

            aMoveSpeed += AddFactor(buffConfig.m_AttrData.GetAddSpd(aMoveSpeed, entity.GetLevel()), factor);
            aWalkSpeed += AddFactor(buffConfig.m_AttrData.GetAddWalkSpd(aWalkSpeed, entity.GetLevel()), factor);
            aHpMax += (int)AddFactor(buffConfig.m_AttrData.GetAddHpMax(aHpMax, entity.GetLevel()), factor);
            aEnergyMax += (int)AddFactor(buffConfig.m_AttrData.GetAddEpMax(aEnergyMax, entity.GetLevel()), factor);
            aHpRecover += AddFactor(buffConfig.m_AttrData.GetAddHpRecover(aHpRecover, entity.GetLevel()), factor);
            aEnergyRecover += AddFactor(buffConfig.m_AttrData.GetAddEpRecover(aEnergyRecover, entity.GetLevel()), factor);
            aAttackBase += (int)AddFactor(buffConfig.m_AttrData.GetAddAd(aAttackBase, entity.GetLevel()), factor);
            aADefenceBase += (int)AddFactor(buffConfig.m_AttrData.GetAddADp(aADefenceBase, entity.GetLevel()), factor);
            aMDefenceBase += (int)AddFactor(buffConfig.m_AttrData.GetAddMDp(aMDefenceBase, entity.GetLevel()), factor);
            aCritical += AddFactor(buffConfig.m_AttrData.GetAddCri(aCritical, entity.GetLevel()), factor);
            aCriticalPow += AddFactor(buffConfig.m_AttrData.GetAddPow(aCriticalPow, entity.GetLevel()), factor);
            aCriticalBackHitPow += AddFactor(buffConfig.m_AttrData.GetAddBackHitPow(aCriticalBackHitPow, entity.GetLevel()), factor);
            aCriticalCrackPow += AddFactor(buffConfig.m_AttrData.GetAddCrackPow(aCriticalCrackPow, entity.GetLevel()), factor);
            aFireDamage += AddFactor(buffConfig.m_AttrData.GetAddFireDam(aFireDamage, entity.GetLevel()), factor);
            aFireERD += AddFactor(buffConfig.m_AttrData.GetAddFireErd(aFireERD, entity.GetLevel()), factor);
            aIceDamage += AddFactor(buffConfig.m_AttrData.GetAddIceDam(aIceDamage, entity.GetLevel()), factor);
            aIceERD += AddFactor(buffConfig.m_AttrData.GetAddIceErd(aIceERD, entity.GetLevel()), factor);
            aPoisonDamage += AddFactor(buffConfig.m_AttrData.GetAddPoisonDam(aPoisonDamage, entity.GetLevel()), factor);
            aPoisonERD += AddFactor(buffConfig.m_AttrData.GetAddPoisonErd(aPoisonERD, entity.GetLevel()), factor);
            aWeight += AddFactor(buffConfig.m_AttrData.GetAddWeight(aWeight, entity.GetLevel()), factor);

            entity.GetActualProperty().SetMoveSpeed(Operate_Type.OT_Absolute, aMoveSpeed);
            entity.GetActualProperty().SetWalkSpeed(Operate_Type.OT_Absolute, aWalkSpeed);
            entity.GetActualProperty().SetHpMax(Operate_Type.OT_Absolute, aHpMax);
            entity.GetActualProperty().SetEnergyMax(Operate_Type.OT_Absolute, aEnergyMax);
            entity.GetActualProperty().SetHpRecover(Operate_Type.OT_Absolute, aHpRecover);
            entity.GetActualProperty().SetEnergyRecover(Operate_Type.OT_Absolute, aEnergyRecover);
            entity.GetActualProperty().SetAttackBase(Operate_Type.OT_Absolute, aAttackBase);
            entity.GetActualProperty().SetADefenceBase(Operate_Type.OT_Absolute, aADefenceBase);
            entity.GetActualProperty().SetMDefenceBase(Operate_Type.OT_Absolute, aMDefenceBase);
            entity.GetActualProperty().SetCritical(Operate_Type.OT_Absolute, aCritical);
            entity.GetActualProperty().SetCriticalPow(Operate_Type.OT_Absolute, aCriticalPow);
            entity.GetActualProperty().SetCriticalBackHitPow(Operate_Type.OT_Absolute, aCriticalBackHitPow);
            entity.GetActualProperty().SetCriticalCrackPow(Operate_Type.OT_Absolute, aCriticalCrackPow);
            entity.GetActualProperty().SetFireDamage(Operate_Type.OT_Absolute, aFireDamage);
            entity.GetActualProperty().SetFireERD(Operate_Type.OT_Absolute, aFireERD);
            entity.GetActualProperty().SetIceDamage(Operate_Type.OT_Absolute, aIceDamage);
            entity.GetActualProperty().SetIceERD(Operate_Type.OT_Absolute, aIceERD);
            entity.GetActualProperty().SetPoisonDamage(Operate_Type.OT_Absolute, aPoisonDamage);
            entity.GetActualProperty().SetPoisonERD(Operate_Type.OT_Absolute, aPoisonERD);
            entity.GetActualProperty().SetWeight(Operate_Type.OT_Absolute, aWeight);
        }
        private static int AddFactor(int input, float factor)
        {
            return (int)(factor * input);
        }
        private static float AddFactor(float input, float factor)
        {
            return factor * input;
        }
    }
}
