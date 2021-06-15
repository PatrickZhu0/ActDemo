using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using StarWars;

public class GameLogic : MonoBehaviour
{
    internal void Awake()
    {
        GlobalVariables.Instance.IsClient = true;
        DontDestroyOnLoad(this.gameObject);
    }
    // Use this for initialization
    internal void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        QualitySettings.SetQualityLevel(1);
        Application.runInBackground = true;
        try
        {
            if (!GameControler.IsInited)
            {
                //AnalyticsManager.Init();


                //HardWareQuality.Clear();
                //HardWareQuality.ComputeHardwarePerformance();

                //HardWareQuality.SetQualityAll();

                if (GlobalVariables.Instance.IsPublish)
                {
                    //ResUpdateControler.InitContext();
                }

                string dataPath = Application.dataPath;
                string persistentDataPath = Application.persistentDataPath + "/DataFile";
                string streamingAssetsPath = Application.streamingAssetsPath;
                string tempPath = Application.temporaryCachePath;
                //LogicSystem.LogicLog("dataPath:{0} persistentDataPath:{1} streamingAssetsPath:{2} tempPath:{3}", dataPath, persistentDataPath, streamingAssetsPath, tempPath);
                Debug.Log(string.Format("dataPath:{0} persistentDataPath:{1} streamingAssetsPath:{2} tempPath:{3}", dataPath, persistentDataPath, streamingAssetsPath, tempPath));
                if (GlobalVariables.Instance.IsPublish)
                {
                    GameControler.Init(tempPath, persistentDataPath);
                }
                else
                {
#if UNITY_ANDROID
	      GameControler.Init(tempPath, persistentDataPath);
#elif UNITY_IPHONE
	      GameControler.Init(tempPath, persistentDataPath);
#else
                    if (Application.isEditor)
                        GameControler.Init(tempPath, streamingAssetsPath);
                    else
                        GameControler.Init(dataPath, persistentDataPath);
#endif
                }
            }
        }
        catch (Exception ex)
        {
            //LogicSystem.LogicLog("GameLogic.Start throw exception:{0}\n{1}", ex.Message, ex.StackTrace);
            Debug.Log(string.Format("GameLogic.Start throw exception:{0}\n{1}", ex.Message, ex.StackTrace));
        }
    }

    // Update is called once per frame
    internal void Update()
    {
        try
        {
            if (!m_IsDataFileExtracted && !m_IsDataFileExtractedPaused)
            {
                StartCoroutine(ExtractDataFileAndStartGame());
                m_IsDataFileExtracted = true;
            }
            if (!m_IsInit && m_LoadLevelAsync != null && m_LoadLevelAsync.isDone)
            {
                m_LoadLevelAsync = null;
                m_IsInit = true;
                //AssetExManager.Instance.ClearAllAssetBundle();
            }
            if (!m_IsSettingModified)
            {
                QualitySettings.vSyncCount = 1;
                if (QualitySettings.vSyncCount == 1)
                {
                    m_IsSettingModified = true;
                }
            }
            m_IsInit = true;
            if (m_IsInit)
            {
                //bool isLastHitUi = (UICamera.lastHit.collider != null);
                //LogicSystem.IsLastHitUi = isLastHitUi;
                //DebugConsole.IsLastHitUi = isLastHitUi;
                GameControler.TickGame();
            }
            //AssetExManager.Instance.Update();
        }
        catch (Exception ex)
        {
            //LogicSystem.LogicLog("GameLogic.Update throw exception:{0}\n{1}", ex.Message, ex.StackTrace);
            Debug.Log(string.Format("GameLogic.Update throw exception:{0}\n{1}", ex.Message, ex.StackTrace));
        }
    }
    internal void OnApplicationPause(bool isPause)
    {
        Debug.LogWarning("OnApplicationPause:" + isPause);
        GameControler.PauseLogic(isPause);
    }

    internal void OnApplicationQuit()
    {
        Debug.LogWarning("OnApplicationQuit");
        GameControler.StopLogic();
        GameControler.Release();
        //AssetExManager.Instance.ClearAllAssetEx();
        Resources.UnloadUnusedAssets();
    }

    private void LogToConsole(string msg)
    {
        //DebugConsole.Log(msg);
        UnityEngine.Debug.Log(msg);
    }

    private void OnResetDsl(string script)
    {
        //SkillSystem.SkillConfigManager.Instance.Clear();
        //GfxModule.Skill.GfxSkillSystem.Instance.ClearSkillInstancePool();
        //LogicSystem.PublishLogicEvent("ge_resetdsl", "game");
    }

    private void OnExecScript(string script)
    {
        //LogicSystem.PublishLogicEvent("ge_execscript", "game", script);
    }

    private void OnExecCommand(string command)
    {
        //LogicSystem.PublishLogicEvent("ge_execcommand", "game", command);
    }
    public void ShowUi(bool show)
    {
        //UIManager.Instance.SetAllUiVisible(show);
    }
    public void StartCountDown(int countDownTime)
    {
        //StarWars.LogicSystem.EventChannelForGfx.Publish("ge_pvp_counttime", "ui", countDownTime);
    }
    public void TriggerStory(int storyId)
    {
        //StoryDlg.StoryDlgInfo storyInfo = StoryDlg.StoryDlgManager.Instance.GetStoryInfoByID(storyId);
        //if (null != storyInfo)
        //{
        //    if (storyInfo.DlgType == StoryDlgPanel.StoryDlgType.Small)
        //    {
        //        GameObject obj = UIManager.Instance.GetWindowGoByName("StoryDlgSmall");
        //        if (null != obj)
        //        {
        //            StoryDlgPanel dlg = obj.GetComponent<StoryDlgPanel>();
        //            dlg.OnTriggerStory(storyInfo);
        //        }
        //    }
        //    else
        //    {
        //        GameObject obj = UIManager.Instance.GetWindowGoByName("StoryDlgBig");
        //        if (null != obj)
        //        {
        //            StoryDlgPanel dlg = obj.GetComponent<StoryDlgPanel>();
        //            dlg.OnTriggerStory(storyInfo);
        //        }
        //    }
        //}
        //else
        //{
        //    Debug.LogError("Wrong Story id = " + storyId);
        //}
    }

    private IEnumerator ExtractDataFileAndStartGame()
    {
        //LogicSystem.BeginLoading();
        if (GlobalVariables.Instance.IsPublish)
        {
            //AssetExManager.Instance.Cleanup();

            //// 更新资源
            //ResUpdateControler.InitUpdate();
            //ResUpdateControler.HandleUpdateFailed = ReExtractDataFileAndStartGame;

            //ResUpdateControler.OnUpdateProgress(0, "加载客户端版本信息...");
            //ResAsyncInfo loadClientVersionInfo = ResUpgrader.RequestClientVersion();
            //yield return loadClientVersionInfo.CurCoroutine;
            //if (loadClientVersionInfo.IsError)
            //{
            //    UnityEngine.Debug.Log("加载客户端版本信息错误");
            //    ReExtractDataFileAndStartGame();
            //    yield break;
            //}

            //ResUpdateControler.SetUpdateProgressRange(0, 1.0f, 1);
            //int targetChapter = Mathf.Max(ResUpdateControler.CurChapter, 1);
            //ResAsyncInfo startUpdateInfo = ResUpdateControler.StartUpdate(targetChapter);
            //yield return startUpdateInfo.CurCoroutine;
            //if (ResUpdateControler.IsNeedPauseUpdate)
            //{
            //    PauseExtractDataFileAndStartGame();
            //    yield break;
            //}
            //if (startUpdateInfo.IsError)
            //{
            //    ReExtractDataFileAndStartGame();
            //    yield break;
            //}

            //LogicSystem.UpdateLoadingProgress(0.0f);
            //LogicSystem.UpdateLoadingTip("加载场景不费流量");
            //ResUpdateControler.SetUpdateProgressRange(0.0f, 1.0f, 1);
            //List<ResCacheConfig> cacheConfigList = new List<ResCacheConfig>();
            //cacheConfigList = new List<ResCacheConfig>();
            //cacheConfigList.Add(new ResCacheConfig(ResCacheType.level, ResUpdateControler.s_LoadSceneId));
            //ResAsyncInfo loadCacheResForMainMenuInfo = ResUpdateControler.CacheResByConfig(cacheConfigList);
            //yield return loadCacheResForMainMenuInfo.CurCoroutine;
            //if (loadCacheResForMainMenuInfo.IsError)
            //{
            //    ReExtractDataFileAndStartGame();
            //    yield break;
            //}

            //ResUpdateControler.ExitUpdate();
        }
        else if (!Application.isEditor)
        {
            // 加载txt资源
            //LogicSystem.UpdateLoadingTip("加载配置数据");
            string srcPath = Application.streamingAssetsPath;
            string destPath = Application.persistentDataPath + "/DataFile";
            Debug.Log(srcPath);
            Debug.Log(destPath);

            if (!srcPath.Contains("://"))
                srcPath = "file://" + srcPath;
            string listPath = srcPath + "/list.txt";
            WWW listData = new WWW(listPath);
            //Debug.Log("wait for www " + listPath + " done");
            yield return listData;
            //Debug.Log("www " + listPath + " is done");
            string listTxt = listData.text;
            if (null != listTxt)
            {
                //Debug.Log(listTxt);
                using (StringReader sr = new StringReader(listTxt))
                {
                    string numStr = sr.ReadLine();
                    float totalNum = 50;
                    if (null != numStr)
                    {
                        numStr = numStr.Trim();
                        totalNum = (float)int.Parse(numStr);
                        if (totalNum <= 0)
                            totalNum = 50;
                    }
                    for (float num = 1; ; num += 1)
                    {
                        string path = sr.ReadLine();
                        if (null != path)
                        {
                            path = path.Trim();
                            string url = srcPath + "/" + path;
                            //Debug.Log("extract " + url);
                            string filePath = Path.Combine(destPath, path);
                            string dir = Path.GetDirectoryName(filePath);
                            if (!Directory.Exists(dir))
                                Directory.CreateDirectory(dir);
                            WWW temp = new WWW(url);
                            yield return temp;
                            if (null != temp.bytes)
                            {
                                File.WriteAllBytes(filePath, temp.bytes);
                            }
                            else
                            {
                                //Debug.Log(path + " can't load");
                            }
                            temp = null;
                        }
                        else
                        {
                            break;
                        }

                        //LogicSystem.UpdateLoadingProgress(0.8f + 0.2f * num / totalNum);
                    }
                    sr.Close();
                }
                listData = null;
            }
            else
            {
                Debug.Log("Can't load list.txt");
            }
        }
        //LogicSystem.EndLoading();

        Debug.LogError("GameLogic ---  StartLogic");
        StartLogic();
    }
    private void ReExtractDataFileAndStartGame()
    {
        //ResUpdateControler.IncReconnectNum();
        //m_IsDataFileExtractedPaused = true;
        //string info = "网络连接错误,请重试连接";
        //Action<bool> fun = new Action<bool>(delegate (bool selected)
        //{
        //    if (selected)
        //    {
        //        m_IsDataFileExtractedPaused = false;
        //        ResUpdateControler.ExitUpdate();
        //        m_IsDataFileExtracted = false;
        //        m_IsInit = false;
        //    }
        //});
        //StarWars.LogicSystem.EventChannelForGfx.Publish("ge_show_yesornot", "ui", info, fun);
    }
    private void PauseExtractDataFileAndStartGame()
    {
        //m_IsDataFileExtractedPaused = true;
        //ResUpdateControler.ExitUpdate();
        m_IsDataFileExtracted = false;
        m_IsInit = false;
    }
    private void UpdateProgress(float progress, string tip)
    {
        LogicSystem.UpdateLoadingTip(tip);
        LogicSystem.UpdateLoadingProgress(progress);
    }
    private void StartLogic()
    {
        GameControler.InitLogic();
        GameControler.StartLogic();
        //LogicSystem.SetLoadingBarScene("LoadingBar");
        //if (GlobalVariables.Instance.IsPublish)
        //{
        //    Resources.UnloadUnusedAssets();
        //    m_LoadLevelAsync = Application.LoadLevelAsync("loading");
        //    m_IsInit = false;
        //}
        //else
        //{
        //    Application.LoadLevel("Loading");
        //    m_IsInit = true;
        //}

        StarWars.LogicSystem.EventChannelForGfx.Publish("ge_show_login", "ui");
    }
    internal void RestartLocgic()
    {
        //LogicSystem.SetLoadingBarScene("LoadingBar");
        //Application.LoadLevel("Loading");
        //StarWars.LogicSystem.PublishLogicEvent("ge_change_scene", "game", 0);
        m_IsInit = true;
    }
    private bool m_IsDataFileExtracted = false;
    private bool m_IsDataFileExtractedPaused = false;
    private bool m_IsSettingModified = false;
    private bool m_IsInit = false;
    private AsyncOperation m_LoadLevelAsync = null;
}
