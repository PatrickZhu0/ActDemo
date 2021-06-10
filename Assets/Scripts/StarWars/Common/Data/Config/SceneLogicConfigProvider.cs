using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWars
{
  public class SceneLogicConfigProvider
  {
    public SceneLogicConfig GetSceneLogicConfig(int resId, int id)
    {
      SceneLogicConfig cfg = null;
      MapDataProvider mapData = SceneConfigProvider.Instance.GetMapDataBySceneResId(resId);
      if (null != mapData) {
        cfg = mapData.m_SceneLogicMgr.GetDataById(id);
      }
      return cfg;
    }

    private SceneLogicConfigProvider()
    { }
    private DataDictionaryMgr<SceneLogicConfig> sceneLogicConfigMgr = new DataDictionaryMgr<SceneLogicConfig>();
    
    public static SceneLogicConfigProvider Instance
    {
      get { return s_Instance; }
    }
    private static SceneLogicConfigProvider s_Instance = new SceneLogicConfigProvider();
  }
}
