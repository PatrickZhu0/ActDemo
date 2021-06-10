using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWars
{
  public class Data_PlayerConfig : IData
  {
    // 基础属性
    public int m_Id;
    public string m_Name;
    public float m_Scale = 1.0f;

    // 战斗属性
    public AttrDataConfig m_AttrData = new AttrDataConfig();

    public bool m_SuperArmor = false;
    public float m_ViewRange = 10;
    public long m_ReleaseTime = 1000;
    public int m_CostType = 0;

    public List<int> m_PreSkillList = new List<int>();
    public List<int> m_FixedSkillList = new List<int>();
    public List<int> m_ActionList = new List<int>();
    public List<int> m_NoviceEquipList = new List<int>();

    // 动画
    public string m_Model;
    public string m_DeathModel;
    public string m_ActionFile;
    public string m_AnimPath;
    //头像
    public string m_Portrait;

    public float m_Radius;
    public int m_AvoidanceRadius;

    public int m_AiLogic;
    //推荐装备
    public int[] m_RecommendEquipment = null;
    //电脑ai用到的数据
    public int[] m_AiEquipment = null;
    public int[] m_AiAttackSkill = null;
    public int[] m_AiMoveSkill = null;
    public int[] m_AiControlSkill = null;
    public int[] m_AiSelfAssitSkill = null;
    public int[] m_AiTeamAssitSkill = null;

    public float m_Cross2StandTime = 0.5f;
    public float m_Cross2RunTime = 0.3f;

    public float m_Combat2IdleTime = 3;
    public int m_Combat2IdleSkill = 0;
    public string m_Idle2CombatWeaponMoves = "";
    public string m_IndicatorEffect = "";
    public float m_IndicatorShowDis = 10.0f;
    public string m_HeroIntroduce1 = null;
    public string m_HeroIntroduce2 = null;
    /**
     * @brief 提取数据
     *
     * @param node
     *
     * @return 
     */
    public bool CollectDataFromDBC(DBC_Row node)
    {
      m_Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0, true);
      m_Name = DBCUtil.ExtractString(node, "Name", "", true);
      m_Scale = DBCUtil.ExtractNumeric<float>(node, "Scale", 1.0f, false);
      m_AiLogic = DBCUtil.ExtractNumeric<int>(node, "AiLogic", 0, false);

      m_AttrData.CollectDataFromDBC(node);
      //m_SuperArmor = DBCUtil.ExtractNumeric<bool>(node, "SuperArmor", false, false);
      m_ViewRange = DBCUtil.ExtractNumeric<float>(node, "ViewRange", -1, true);
      m_ReleaseTime = DBCUtil.ExtractNumeric<long>(node, "ReleaseTime", 0, false);
      m_CostType = DBCUtil.ExtractNumeric<int>(node, "CostType", 0, false);

      m_PreSkillList = DBCUtil.ExtractNumericList<int>(node, "PreSkillList", 0, false);
      m_FixedSkillList = DBCUtil.ExtractNumericList<int>(node, "FixedSkillList", 0, false);
      m_ActionList = DBCUtil.ExtractNumericList<int>(node, "ActionId", 0, false);
      m_NoviceEquipList = DBCUtil.ExtractNumericList<int>(node, "NoviceEquipList", 0, false);

      m_Model = DBCUtil.ExtractString(node, "Model", "", false);
      m_DeathModel = DBCUtil.ExtractString(node, "DeathModel", "", false);
      m_ActionFile = DBCUtil.ExtractString(node, "ActionFile", "", false);
      m_AnimPath = DBCUtil.ExtractString(node, "AnimPath", "", false);

      m_Portrait = DBCUtil.ExtractString(node, "Portrait", "", false);
      m_Radius = DBCUtil.ExtractNumeric<float>(node, "Radius", 1.0f, false);
      m_AvoidanceRadius = DBCUtil.ExtractNumeric<int>(node, "AvoidanceRadius", 1, false);

      List<int> list = DBCUtil.ExtractNumericList<int>(node, "RecommendEquipment", 0, false);
      if (list.Count == 6) {
        m_RecommendEquipment = list.ToArray();
      } else {
        m_RecommendEquipment = new int[] { 0, 0, 0, 0, 0, 0 };
      }

      list = DBCUtil.ExtractNumericList<int>(node, "AiEquipment", 0, false);
      if (list.Count == 6)
        m_AiEquipment = list.ToArray();
      else
        m_AiEquipment = new int[] { 0, 0, 0, 0, 0, 0 };
      list = DBCUtil.ExtractNumericList<int>(node, "AiAttackSkill", 0, false);
      if (list.Count > 0)
        m_AiAttackSkill = list.ToArray();
      list = DBCUtil.ExtractNumericList<int>(node, "AiMoveSkill", 0, false);
      if (list.Count > 0)
        m_AiMoveSkill = list.ToArray();
      list = DBCUtil.ExtractNumericList<int>(node, "AiControlSkill", 0, false);
      if (list.Count > 0)
        m_AiControlSkill = list.ToArray();
      list = DBCUtil.ExtractNumericList<int>(node, "AiSelfAssitSkill", 0, false);
      if (list.Count > 0)
        m_AiSelfAssitSkill = list.ToArray();
      list = DBCUtil.ExtractNumericList<int>(node, "AiTeamAssitSkill", 0, false);
      if (list.Count > 0)
        m_AiTeamAssitSkill = list.ToArray();

      m_Cross2StandTime = DBCUtil.ExtractNumeric<float>(node, "Cross2StandTime", 0.5f, false);
      m_Cross2RunTime = DBCUtil.ExtractNumeric<float>(node, "Cross2RunTime", 0.3f, false);

      m_Combat2IdleTime = DBCUtil.ExtractNumeric<float>(node, "Comabat2IdleTime", 3, false);
      m_Combat2IdleSkill = DBCUtil.ExtractNumeric<int>(node, "Combat2IdleSkill", 0, false);
      m_Idle2CombatWeaponMoves = DBCUtil.ExtractString(node, "Idle2CombatWeaponMoves", "", false);
      m_IndicatorEffect = DBCUtil.ExtractString(node, "IndicatorEffect", "", false);
      m_IndicatorShowDis = DBCUtil.ExtractNumeric<float>(node, "IndicatorDis", 10.0f, false);
      m_HeroIntroduce1 = DBCUtil.ExtractString(node, "HeroIntroduce1", "", false);
      m_HeroIntroduce2 = DBCUtil.ExtractString(node, "HeroIntroduce2", "", false);
      return true;
    }

    /**
     * @brief 获取数据ID
     *
     * @return 
     */
    public int GetId()
    {
      return m_Id;
    }

    private int findFromList(List<int> l, int id)
    {
      return l.FindIndex(
        delegate(int v) {
          return v == id;
        }
        );
    }
  }

  public class PlayerLevelupExpConfig : IData
  {
    public int m_Level = 0;
    public int m_ConsumeExp = 0;
    public int m_RebornTime = 0;

    public bool CollectDataFromDBC(DBC_Row node)
    {
      m_Level = DBCUtil.ExtractNumeric<int>(node, "Level", 0, true);
      m_ConsumeExp = DBCUtil.ExtractNumeric<int>(node, "ConsumeExp", 0, true);
      m_RebornTime = DBCUtil.ExtractNumeric<int>(node, "RebornTime", 0, true);
      return true;
    }

    public int GetId()
    {
      return m_Level;
    }
  }

  public class PlayerConfigProvider
  {
    public DataDictionaryMgr<Data_PlayerConfig> PlayerConfigMgr
    {
      get { return m_PlayerConfigMgr; }
    }
    public Data_PlayerConfig GetPlayerConfigById(int id)
    {
      return m_PlayerConfigMgr.GetDataById(id);
    }
    public void LoadPlayerConfig(string file, string root)
    {
      m_PlayerConfigMgr.CollectDataFromDBC(file, root);
    }

    private DataDictionaryMgr<Data_PlayerConfig> m_PlayerConfigMgr = new DataDictionaryMgr<Data_PlayerConfig>();

    public DataDictionaryMgr<LevelupConfig> PlayerLevelupConfigMgr
    {
      get { return m_PlayerLevelupConfigMgr; }
    }
    public LevelupConfig GetPlayerLevelupConfigById(int id)
    {
      return m_PlayerLevelupConfigMgr.GetDataById(id);
    }
    public void LoadPlayerLevelupConfig(string file, string root)
    {
      m_PlayerLevelupConfigMgr.CollectDataFromDBC(file, root);
    }

    private DataDictionaryMgr<LevelupConfig> m_PlayerLevelupConfigMgr = new DataDictionaryMgr<LevelupConfig>();

    public DataDictionaryMgr<PlayerLevelupExpConfig> PlayerLevelupExpConfigMgr
    {
      get { return m_PlayerLevelupExpConfigMgr; }
    }
    public PlayerLevelupExpConfig GetPlayerLevelupExpConfigById(int id)
    {
      return m_PlayerLevelupExpConfigMgr.GetDataById(id);
    }
    public void LoadPlayerLevelupExpConfig(string file, string root)
    {
      m_PlayerLevelupExpConfigMgr.CollectDataFromDBC(file, root);
    }
    public void Clear()
    {
      m_PlayerLevelupExpConfigMgr.Clear();
      m_PlayerConfigMgr.Clear();
    }

    private DataDictionaryMgr<PlayerLevelupExpConfig> m_PlayerLevelupExpConfigMgr = new DataDictionaryMgr<PlayerLevelupExpConfig>();

    public static PlayerConfigProvider Instance
    {
      get { return s_Instance; }
    }
    private static PlayerConfigProvider s_Instance = new PlayerConfigProvider();
  }
}
