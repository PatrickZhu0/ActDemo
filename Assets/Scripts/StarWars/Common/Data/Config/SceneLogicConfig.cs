using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWars
{
  public class SceneLogicConfig : IData
  {
    public int m_Id = 0;
    public int m_LogicId = 0;
    public bool m_IsClient = false;
    public bool m_IsServer = false;
    public int m_ParamNum = 0;
    public string[] m_Params = null;

    public bool CollectDataFromDBC(DBC_Row node)
    {
      m_Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0, true);
      m_LogicId = DBCUtil.ExtractNumeric<int>(node, "LogicId", 0, true);
      m_IsClient = DBCUtil.ExtractBool(node, "IsClient", false, true);
      m_IsServer = DBCUtil.ExtractBool(node, "IsServer", false, true);
      m_ParamNum = DBCUtil.ExtractNumeric<int>(node, "ParamNum", 0, true);
      if (m_ParamNum > 0) {
        m_Params = new string[m_ParamNum];
        for (int i = 0; i < m_ParamNum; ++i) {
          m_Params[i] = DBCUtil.ExtractString(node, "Param" + i, "", false);
        }
      }
      return true;
    }

    public int GetId()
    {
      return m_Id;
    }
  }
}
