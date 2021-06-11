using System;
using System.Collections.Generic;

namespace StarWars
{
    public class AiActionConfig : IData
    {
        public int Id;
        public int AiActionType;
        public int ActionParam;
        public float DisMin;
        public float DisMax;
        public float TargetHpMin;
        public float TargetHpMax;
        public float SelfHpMin;
        public float SelfHpMax;
        public float Cooldown;
        public float Weight;
        public float LastTime;

        public bool CollectDataFromDBC(DBC_Row node)
        {
            Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0, true);
            AiActionType = DBCUtil.ExtractNumeric<int>(node, "ActionType", 1, true);
            ActionParam = DBCUtil.ExtractNumeric<int>(node, "ActionParam", 0, true);
            DisMin = DBCUtil.ExtractNumeric<float>(node, "DisMin", 0.0f, true);
            DisMax = DBCUtil.ExtractNumeric<float>(node, "DisMax", 0.0f, true);
            TargetHpMin = DBCUtil.ExtractNumeric<float>(node, "TargetHpMin", 0.0f, true);
            TargetHpMax = DBCUtil.ExtractNumeric<float>(node, "TargetHpMax", 0.0f, true);
            SelfHpMin = DBCUtil.ExtractNumeric<float>(node, "SelfHpMin", 0.0f, true);
            SelfHpMax = DBCUtil.ExtractNumeric<float>(node, "SelfHpMax", 0.0f, true);
            Cooldown = DBCUtil.ExtractNumeric<float>(node, "Cooldown", 0.0f, true);
            Weight = DBCUtil.ExtractNumeric<float>(node, "Weight", 0.0f, true);
            LastTime = DBCUtil.ExtractNumeric<float>(node, "LastTime", 0.0f, true);
            return true;
        }

        public int GetId()
        {
            return Id;
        }
    }

    public class AiActionConfigProvider
    {
        public AiActionConfig GetDataById(int id)
        {
            return m_AiActionConfigMrg.GetDataById(id);
        }
        public int GetDataCount()
        {
            return m_AiActionConfigMrg.GetDataCount();
        }
        public void Load(string file, string root)
        {
            m_AiActionConfigMrg.CollectDataFromDBC(file, root);
        }
        public static AiActionConfigProvider Instance
        {
            get { return s_Instance; }
        }
        private DataDictionaryMgr<AiActionConfig> m_AiActionConfigMrg = new DataDictionaryMgr<AiActionConfig>();
        private static AiActionConfigProvider s_Instance = new AiActionConfigProvider();
    }
}
