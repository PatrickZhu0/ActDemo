using System;
using System.Collections.Generic;
using System.Text;

namespace StarWars
{
    public enum SkillCategory
    {
        kNone = 0,
        kAttack = 1,
        kSkillA = 2,
        kSkillB = 3,
        kSkillC = 4,
        kSkillD = 5,
        kSkillQ = 6,
        kSkillE = 7,
        kRoll = 8,
        kHold = 9,
        kEx = 10,
        kFlashAttack = 11,
        kCombat2Idle = 12,
        kBeCool = 13,
    }
    /**
   * @brief
   *   skill data
   */
    public class SkillLogicData : IData
    {
        public enum Passivity
        {
            NOT_PASSIVE_SKILL = 0,  // 主动
            PASSIVE_SKILL = 1,      // 被动
        }

        public enum TargSelectType
        {
            SELECT_TYPE_SELECT = 1,    // 选择目标
            SELECT_TYPE_POSITION = 2,  // 选择地点
            SELECT_TYPE_NONE = 3,      // 无需选择
            SELECT_TYPE_MOVE_DIR = 4,  // 选择移动方向
        }

        public enum TargType
        {
            ANY = 0,      // 任意目标
            PARTNER = 1,  // 友军
            ENEMY = 2,    // 敌军
            SELF = 3,     // 自己
        }

        // basic attributes
        public int SkillId;                           // 技能ID
        public string SkillDataFile;                  // 技能逻辑描述文件
        public string SkillIcon;                      // 技能图标
        public int ActivateLevel;                     // 激活的最小等级
        public string SkillDescription;               // 技能名称
        public string SkillTrueDescription;           // 技能描述
        public int SkillLevel;                        // 技能等级
        public float SkillRangeMin;                   // 可释放技能的最近距离。
        public float SkillRangeMax;                   // 可释放技能的最远距离。
        public float SkillCoefficient;                // 技能系数
        public Passivity SkillPassivity;              // 0-主动，1-被动
        public TargSelectType TargetSelectType;       // 1-选择目标，2-选择地点，3-无需选择
        public TargType TargetType;                   // 0-对任意目标有效，1-对友军使用，2-对敌人使用，3-对自己使用

        public float CoolDownTime;                    // 冷却时间，单位：秒
        public int CostHp;                            // 释放技能需要消耗的HP
        public int CostRage;                          // 消耗怒气
        public int CostEnergy;                        // 释放技能需要消耗的Energy
        public int CostEnergyCore;                    // 释放技能需要消耗的能量豆
        public int CostItemId;                        // 释放技能需要消耗的物品id

        public int LevelUpCostType;                  // 技能升级花费类型

        //--------start(UI显示)
        public string ShowName = "";
        public string ShowDescription = "";
        public float ShowCd;                            //
        public int ShowCostEnergy;
        public int ShowSteps = 0;//技能阶数
        public string ShowSteps2Des = "";
        public string ShowSteps3Des = "";
        public string ShowSteps4Des = "";
        public float DamagePerLevel;
        public float ShowBaseDamage = 0.0f;
        public string ShowAtlasPath;
        public string ShowIconName;
        //-------end

        public bool CanStartWhenStiffness;
        public bool CanStartWhenHitFly;
        public bool CanStartWhenKnockDown;
        public int BreakType;
        public SkillCategory Category;
        public int NextSkillId;
        public int LiftSkillId;
        public List<int> LiftCostItemList = new List<int>();
        public List<int> LiftCostItemNumList = new List<int>();
        public int QSkillId;
        public int ESkillId;
        public float LockInputTime;
        public float NextInputTime;
        public float TargetChooseRange;

        /**
         * @brief 提取数据
         *
         * @param node
         *
         * @return 
         */
        public bool CollectDataFromDBC(DBC_Row node)
        {
            SkillId = DBCUtil.ExtractNumeric<int>(node, "Id", 0, true);
            SkillDataFile = DBCUtil.ExtractString(node, "LogicDataFile", "", false);
            SkillDescription = DBCUtil.ExtractString(node, "Description", "", true);
            SkillTrueDescription = DBCUtil.ExtractString(node, "TrueDescription", "", false);
            ActivateLevel = DBCUtil.ExtractNumeric<int>(node, "ActivateLevel", 0, true);
            SkillPassivity = (Passivity)DBCUtil.ExtractNumeric<int>(node, "Passivity", 0, true);
            SkillCoefficient = DBCUtil.ExtractNumeric<float>(node, "SkillCoefficient", 0, true);
            SkillRangeMin = DBCUtil.ExtractNumeric<float>(node, "RangeMin", 0.0f, false);
            SkillRangeMax = DBCUtil.ExtractNumeric<float>(node, "RangeMax", 3.0f, false);
            TargetType = (TargType)DBCUtil.ExtractNumeric<int>(node, "TargetType", 0, true);
            TargetSelectType = (TargSelectType)DBCUtil.ExtractNumeric<int>(node, "TargetSelectType", 0, true);
            CoolDownTime = DBCUtil.ExtractNumeric<float>(node, "CD", 0, true);
            CostHp = DBCUtil.ExtractNumeric<int>(node, "CostHp", 0, false);
            CostRage = DBCUtil.ExtractNumeric<int>(node, "CostRage", 0, false);
            CostEnergy = DBCUtil.ExtractNumeric<int>(node, "CostEnergy", 0, false);
            CostEnergyCore = DBCUtil.ExtractNumeric<int>(node, "CostEnergyCore", 0, false);
            CostItemId = DBCUtil.ExtractNumeric<int>(node, "CostItemId", 0, false);

            LevelUpCostType = DBCUtil.ExtractNumeric<int>(node, "LevelUpCostType", 1, false);

            CanStartWhenStiffness = DBCUtil.ExtractBool(node, "CanStartWhenStiffness", false, false);
            CanStartWhenHitFly = DBCUtil.ExtractBool(node, "CanStartWhenHitFly", false, false);
            CanStartWhenKnockDown = DBCUtil.ExtractBool(node, "CanStartWhenKnockDown", false, false);
            BreakType = DBCUtil.ExtractNumeric<int>(node, "BreakType", 0, false);
            Category = (SkillCategory)DBCUtil.ExtractNumeric<int>(node, "Category", 0, false);
            NextSkillId = DBCUtil.ExtractNumeric<int>(node, "NextSkillId", -1, false);
            LiftSkillId = DBCUtil.ExtractNumeric<int>(node, "LiftSkillId", -1, false);
            LiftCostItemList = DBCUtil.ExtractNumericList<int>(node, "LiftCostItem", 0, false);
            LiftCostItemNumList = DBCUtil.ExtractNumericList<int>(node, "LiftCostItemNum", 0, false);
            QSkillId = DBCUtil.ExtractNumeric<int>(node, "QSKillId", -1, false);
            ESkillId = DBCUtil.ExtractNumeric<int>(node, "ESKillId", -1, false);
            LockInputTime = DBCUtil.ExtractNumeric<float>(node, "LockInputTime", 0, false); ;
            NextInputTime = DBCUtil.ExtractNumeric<float>(node, "NextInputTime", 0, false); ;
            TargetChooseRange = DBCUtil.ExtractNumeric<float>(node, "TargetChooseRange", 0, false); ;
            ShowName = DBCUtil.ExtractString(node, "ShowName", "", false);
            ShowDescription = DBCUtil.ExtractString(node, "ShowDescription", "", false);
            ShowCd = DBCUtil.ExtractNumeric<float>(node, "ShowCD", 0, false);            //
            ShowCostEnergy = DBCUtil.ExtractNumeric<int>(node, "ShowCostEnergy", 0, false);
            ShowSteps = DBCUtil.ExtractNumeric<int>(node, "ShowSteps", 0, false);
            ShowSteps2Des = DBCUtil.ExtractString(node, "ShowSteps2Dec", "", false);
            ShowSteps3Des = DBCUtil.ExtractString(node, "ShowSteps3Dec", "", false);
            ShowSteps4Des = DBCUtil.ExtractString(node, "ShowSteps4Dec", "", false);
            DamagePerLevel = DBCUtil.ExtractNumeric<float>(node, "ShowDamagePerLevel", 0f, false);
            ShowBaseDamage = DBCUtil.ExtractNumeric<float>(node, "ShowDamage", 0f, false);
            ShowIconName = DBCUtil.ExtractString(node, "ShowIconName", "", false);
            ShowAtlasPath = DBCUtil.ExtractString(node, "ShowAtlasPath", "", false);
            return true;
        }

        /**
         * @brief
         *   get skill ID
         *
         * @return 
         */
        public int GetId()
        {
            return SkillId;
        }

        /**
         *  @brief
         *    dump all data
         */
        public void Dump()
        {
            LogSystem.Debug("Id={0}", SkillId);
            LogSystem.Debug("Icon={0}", SkillIcon);
            LogSystem.Debug("Name={0}", SkillDescription);
            LogSystem.Debug("Level={0}", SkillLevel);
            LogSystem.Debug("Passivity={0}", SkillPassivity);
            LogSystem.Debug("TargetType={0}", TargetType);
            LogSystem.Debug("TargetSelectType={0}", TargetSelectType);
            LogSystem.Debug("Cooldown={0}", CoolDownTime);
            LogSystem.Debug("CostHp={0}", CostHp);
            LogSystem.Debug("CostEnergy={0}", CostEnergy);
            LogSystem.Debug("CostEnergyCore={0}", CostEnergyCore);
            LogSystem.Debug("CostItemId={0}", CostItemId);
        }
    }

    /**
   * @brief 效果数据
   */
    public class ImpactLogicData : IData
    {
        public int ImpactId;                  // 效果ID
        public int ImpactLogicId;             // 效果逻辑ID
        public int ImpactGfxLogicId;          // 表现逻辑ID
        public string ImpactDescription;      // 效果说明
        public int ImpactType;                // 效果类型，1-持续效果  2-瞬发效果
        public bool BreakSuperArmor;       // 是否能破霸体
        public int MaxRank;                   // 效果可以叠加的最大层数
        public string AnimationInfo;          // 动作播放信息
        public string LockFrameInfo;          // 定帧信息
        public string CurveMoveInfo;          // 移动信息
        public int MoveMode;                  // 位移模式
        public string AdjustPoint;            // 校准点
        public float AdjustAppend;            // 校准附加
        public float AdjustDegreeXZ;          // 校准度
        public float AdjustDegreeY;
        public float MaxDist;                 // 最大位移
        public int DamageType;                // 伤害类型
        public int ElementType;               // 元素类型
        public int DamageValue;               // 伤害数值
        public float DamageRate;              // 伤害系数
        public float LevelRate;               // 等级系数
        public int ImpactTime;                // 效果持续时间，单位：毫秒 （瞬发效果无用）
        public int BuffDataId;                // 效果发作的间歇时间(瞬发效果无用)
        public float FallDownTime;            // 倒地时间
        public float OnGroundTime;            // 倒地时间
        public float GetUpTime;               // 起身时间
        public bool IsDeadDisappear;          // 死后是否可以保留
        public bool CanBeCancel;              // 是否可以被玩家自己手动取消（右键点击）
        public bool IsAbsorbHurt;             // 伤害能否被吸收
        public bool IsReflect;                // 效果能否被反射
        public bool IsDamageDisappear;        // 受到伤害是否会消失
        public bool IsFightDisappear;         // 进入战斗状态是否会消失
        public bool IsShootDisappear;         // 射击时是否消失
        public bool IsSkillDisappear;         // 释放技能时是否消失
        public const int EffectCount = 5;
        // 特效播放
        public List<List<int>> EffectList = new List<List<int>>();
        public List<int> ActionList = new List<int>();
        public string action;
        // 扩展数据项
        public int ParamNum = 0;
        public List<string> ExtraParams = new List<string>();

        public float CalcMaxMoveDistanceSqr()
        {
            /*
            //"starttime, [movetime, speedx, speedy, speedz, accelx, accely, accelz]+"
            List<float> list = Converter.ConvertNumericList<float>(CurveMoveInfo);
            float x = 0;
            float z = 0;
            for (int i = 1; i < list.Count - 6; i += 7) {
              float time = list[i];
              float sx = list[i + 1];
              float sz = list[i + 3];
              float ax = list[i + 4];
              float az = list[i + 6];
              float timeSqr_div_2 = time * time / 2;
              x += sx * time + timeSqr_div_2 * ax;
              z += sz * time + timeSqr_div_2 * az;
            }
            return x * x + z * z;
            */
            return MaxDist * MaxDist;
        }

        /**
         * @brief 提取数据
         *
         * @param node
         *
         * @return 
         */
        public bool CollectDataFromDBC(DBC_Row node)
        {
            ImpactId = DBCUtil.ExtractNumeric<int>(node, "Id", -1, true);
            ImpactLogicId = DBCUtil.ExtractNumeric<int>(node, "LogicId", -1, true);
            ImpactGfxLogicId = DBCUtil.ExtractNumeric<int>(node, "GfxLogicId", -1, true);
            ImpactDescription = DBCUtil.ExtractString(node, "Description", "", false);
            ImpactType = DBCUtil.ExtractNumeric<int>(node, "ImpactType", -1, true);
            BreakSuperArmor = DBCUtil.ExtractBool(node, "BreakSuperArmor", false, false);
            ImpactTime = DBCUtil.ExtractNumeric<int>(node, "ImpactTime", -1, false);
            BuffDataId = DBCUtil.ExtractNumeric<int>(node, "BuffDataId", -1, false);
            MaxRank = DBCUtil.ExtractNumeric<int>(node, "MaxRank", -1, false);
            AnimationInfo = DBCUtil.ExtractString(node, "AnimationInfo", "", false);
            LockFrameInfo = DBCUtil.ExtractString(node, "LockFrameInfo", "", false);
            CurveMoveInfo = DBCUtil.ExtractString(node, "CurveMove", "", false);
            MoveMode = DBCUtil.ExtractNumeric<int>(node, "MoveMode", -1, false);
            AdjustPoint = DBCUtil.ExtractString(node, "AdjustPoint", "", false);
            AdjustAppend = DBCUtil.ExtractNumeric<float>(node, "AdjustAppend", 0.0f, false);
            AdjustDegreeXZ = DBCUtil.ExtractNumeric<float>(node, "AdjustDegreeXZ", 0.0f, false);
            AdjustDegreeY = DBCUtil.ExtractNumeric<float>(node, "AdjustDegreeY", 0.0f, false);
            MaxDist = DBCUtil.ExtractNumeric<float>(node, "MaxDist", 0.0f, false);
            DamageType = DBCUtil.ExtractNumeric<int>(node, "DamageType", 0, false);
            ElementType = DBCUtil.ExtractNumeric<int>(node, "ElementType", 0, false);
            DamageValue = DBCUtil.ExtractNumeric<int>(node, "DamageValue", 0, false);
            DamageRate = DBCUtil.ExtractNumeric<float>(node, "DamageRate", 1.0f, false);
            LevelRate = DBCUtil.ExtractNumeric<float>(node, "LevelRate", 0.0f, false);
            IsDeadDisappear = DBCUtil.ExtractBool(node, "IsDeadDisappear", false, false);
            CanBeCancel = DBCUtil.ExtractBool(node, "CanBeCancel", false, false);
            IsAbsorbHurt = DBCUtil.ExtractBool(node, "IsAbsorbHurt", false, false);
            IsReflect = DBCUtil.ExtractBool(node, "IsReflect", false, false);
            FallDownTime = DBCUtil.ExtractNumeric<float>(node, "FallDownTime", -1.0f, false);
            OnGroundTime = DBCUtil.ExtractNumeric<float>(node, "OnGroundTime", 0.3f, false);
            GetUpTime = DBCUtil.ExtractNumeric<float>(node, "GetUpTime", -1.0f, false);
            IsDamageDisappear = DBCUtil.ExtractBool(node, "IsDamageDisappear", false, false);
            IsFightDisappear = DBCUtil.ExtractBool(node, "IsFightDisappear", false, false);
            IsShootDisappear = DBCUtil.ExtractBool(node, "IsShootDisappear", false, false);
            IsSkillDisappear = DBCUtil.ExtractBool(node, "IsSkillDisappear", false, false);

            for (int i = 0; i < EffectCount; ++i)
            {
                string key = "Effect" + i.ToString();
                EffectList.Add(Converter.ConvertNumericList<int>(DBCUtil.ExtractString(node, key, "", false)));
            }
            ActionList = DBCUtil.ExtractNumericList<int>(node, "AnimationType", 0, false);
            ParamNum = DBCUtil.ExtractNumeric<int>(node, "ParamNum", 0, false);
            ExtraParams.Clear();
            if (ParamNum > 0)
            {
                for (int i = 0; i < ParamNum; ++i)
                {
                    string key = "Param" + i.ToString();
                    ExtraParams.Insert(i, DBCUtil.ExtractString(node, key, "", false));
                }
            }

            return true;
        }

        /**
         * @brief 获取数据ID
         *
         * @return 
         */
        public int GetId()
        {
            return ImpactId;
        }
    }

    /**
   * @brief 特效效果数据
   */
    public class EffectLogicData : IData
    {
        public int EffectId;                   // 声音ID
        public string EffectDescription;      // 声音说明

        public string EffectPath;
        public float EffectDelay;
        public float PlayTime;
        public string MountPoint;
        public string RelativePos;
        public bool RotateWithTarget;
        public string RelativeRotation;
        public bool DelWithImpact;
        // 扩展数据项
        public int ParamNum = 0;
        public List<string> ExtraParams = new List<string>();

        /**
         * @brief 提取数据
         *
         * @param node
         *
         * @return 
         */
        public bool CollectDataFromDBC(DBC_Row node)
        {
            EffectId = DBCUtil.ExtractNumeric<int>(node, "Id", -1, true);
            EffectDescription = DBCUtil.ExtractString(node, "Description", "", false);
            EffectPath = DBCUtil.ExtractString(node, "Prefab", "", false);
            EffectDelay = DBCUtil.ExtractNumeric<float>(node, "Delay", 0.0f, false);
            PlayTime = DBCUtil.ExtractNumeric<float>(node, "PlayTime", 0.0f, false);
            MountPoint = DBCUtil.ExtractString(node, "MountPoint", "", false);
            RelativePos = DBCUtil.ExtractString(node, "RelativePos", "0 0 0", false);
            RotateWithTarget = DBCUtil.ExtractBool(node, "RotateWithTarget", false, false);
            RelativeRotation = DBCUtil.ExtractString(node, "RelativeRotation", "0 0 0", false);
            DelWithImpact = DBCUtil.ExtractBool(node, "DelWithImpact", false, false);
            return true;
        }

        /**
         * @brief 获取数据ID
         *
         * @return 
         */
        public int GetId()
        {
            return EffectId;
        }
    }
    /**
   * @brief 声音效果数据
   */
    public class SoundLogicData : IData
    {
        public int SoundId;                   // 声音ID
        public string SoundDescription;      // 声音说明

        // 武器声音资源
        public const int skillSoundCount = 6;
        public System.Collections.Generic.List<string> m_SoundList = new System.Collections.Generic.List<string>(skillSoundCount);

        // 扩展数据项
        public int ParamNum = 0;
        public List<string> ExtraParams = new List<string>();

        public SoundLogicData()
        {
            m_SoundList.Clear();
        }

        /**
         * @brief 提取数据
         *
         * @param node
         *
         * @return 
         */
        public bool CollectDataFromDBC(DBC_Row node)
        {
            SoundId = DBCUtil.ExtractNumeric<int>(node, "Id", -1, true);
            SoundDescription = DBCUtil.ExtractString(node, "Description", "", false);

            for (int i = 0; i < skillSoundCount; ++i)
            {
                string NodeName = "Sound" + i.ToString();
                m_SoundList.Add(DBCUtil.ExtractString(node, NodeName, "", false));
            }

            ParamNum = DBCUtil.ExtractNumeric<int>(node, "ParamNum", 0, false);
            ExtraParams.Clear();
            if (ParamNum > 0)
            {
                for (int i = 0; i < ParamNum; ++i)
                {
                    string key = "Param" + i.ToString();
                    ExtraParams.Insert(i, DBCUtil.ExtractString(node, key, "", false));
                }
            }

            return true;
        }

        /**
         * @brief 获取数据ID
         *
         * @return 
         */
        public int GetId()
        {
            return SoundId;
        }
    }
}
