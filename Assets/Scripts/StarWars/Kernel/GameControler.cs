using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace StarWars
{
    /// <summary>
    /// 游戏控制器
    /// </summary>
    public sealed class GameControler
    {

        public sealed class LogicLogger : IDisposable
        {
            public void Log(string format, params object[] args)
            {
                string msg = string.Format(format, args);
#if USE_DISK_LOG
        m_LogStream.WriteLine(msg);
        m_LogStream.Flush();
#else
                m_LogQueue.Enqueue(msg);
                if (m_LogQueue.Count >= c_FlushCount)
                {
                    m_LastFlushTime = TimeUtility.GetLocalMilliseconds();

                    RequestFlush();
                }
#endif
            }
            public void Init(string logPath)
            {
                string logFile = string.Format("{0}/Game_{1}.log", logPath, DateTime.Now.ToString("yyyy-MM-dd"));
                m_LogStream = new StreamWriter(logFile, true);
#if !USE_DISK_LOG
                m_LogQueue = m_LogQueues[m_CurQueueIndex];
                m_Thread.OnQuitEvent = OnThreadQuit;
                m_Thread.Start();
#endif
                Log("======GameLog Start ({0}, {1})======", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
            }
            public void Dispose()
            {
                Release();
            }
            public void Tick()
            {
#if !USE_DISK_LOG
                long curTime = TimeUtility.GetLocalMilliseconds();
                if (m_LastFlushTime + 10000 < curTime)
                {
                    m_LastFlushTime = curTime;

                    RequestFlush();
                }
#endif
            }
            private void Release()
            {
#if !USE_DISK_LOG
                m_Thread.Stop();
#endif
                m_LogStream.Close();
                m_LogStream.Dispose();
            }
#if !USE_DISK_LOG
            private void RequestFlush()
            {
                lock (m_LogQueueLock)
                {
                    m_Thread.QueueActionWithDelegation((MyAction<Queue<string>>)FlushToFile, m_LogQueue);
                    m_CurQueueIndex = 1 - m_CurQueueIndex;
                    m_LogQueue = m_LogQueues[m_CurQueueIndex];
                }
            }
            private void OnThreadQuit()
            {
                FlushToFile(m_LogQueue);
            }
            private void FlushToFile(Queue<string> logQueue)
            {
                lock (m_LogQueueLock)
                {
                    while (logQueue.Count > 0)
                    {
                        string msg = logQueue.Dequeue();
                        m_LogStream.WriteLine(msg);
                    }
                    m_LogStream.Flush();
                }
            }

            private Queue<string>[] m_LogQueues = new Queue<string>[] { new Queue<string>(), new Queue<string>() };
            private MyThread m_Thread = new MyThread();
            private int m_CurQueueIndex = 0;
            private Queue<string> m_LogQueue;
            private object m_LogQueueLock = new object();

            private long m_LastFlushTime = 0;
            private const int c_FlushCount = 4096;
#endif
            private StreamWriter m_LogStream;
        }

        //----------------------------------------------------------------------
        // 标准接口
        //----------------------------------------------------------------------
        public static bool IsInited
        {
            get { return s_IsInited; }
        }

        public static void Init(string logPath, string dataPath)
        {
            s_IsInited = true;

            UnityEngine.Debug.LogError(" logPath = " + logPath);
            s_LogicLogger.Init(logPath);
            HomePath.CurHomePath = dataPath;
            GlobalVariables.Instance.IsDebug = false;

            string key = "防君子不防小人";
            byte[] xor = Encoding.UTF8.GetBytes(key);




            //GfxSystem.Init();

        }

        public static void TickGame()
        {
            //这里是在渲染线程执行的tick，逻辑线程的tick在GameLogicThread.cs文件里执行。
            //GfxSystem.Tick();
            //GfxModule.Skill.GfxSkillSystem.Instance.Tick();
            //GfxModule.Impact.GfxImpactSystem.Instance.Tick();
        }

        public static void InitLogic()
        {
            //GfxSystem.GfxLog("GameControler.InitLogic");
            EntityManager.Instance.Init();

            //WorldSystem.Instance.Init();
            //WorldSystem.Instance.LoadData();
            //ClientStorySystem.Instance.Init();
            //GmCommands.ClientGmStorySystem.Instance.Init();

            PlayerControl.Instance.Init();
            //LobbyNetworkSystem.Instance.Init(s_LogicThread);
            //NetworkSystem.Instance.Init();
            //AiViewManager.Instance.Init();
            //SceneLogicViewManager.Instance.Init();
            //ImpactViewManager.Instance.Init();
        }

        public static void StartLogic()
        {
            //GfxSystem.GfxLog("GameControler.StartLogic");
            s_LogicThread.Start();
        }
        public static void ChangeScene(int sceneId)
        {
            LogSystem.Info("GameControler.ChangeScene {0}", sceneId);
            //WorldSystem.Instance.ChangeScene(sceneId);
        }
        public static void PauseLogic(bool isPause)
        {
            s_IsPaused = isPause;
        }
        public static void StopLogic()
        {
            //GfxSystem.GfxLog("GameControler.StopLogic");
            s_LogicThread.Stop();
            //LobbyNetworkSystem.Instance.QuitClient();
            //NetworkSystem.Instance.QuitClient();
        }
        public static void Release()
        {
            //GfxSystem.GfxLog("GameControler.Release");

            //WorldSystem.Instance.Release();
            EntityManager.Instance.Release();
            //NetworkSystem.Instance.Release();
            //GfxSystem.Release();
            s_LogicLogger.Dispose();
        }


        internal static LogicLogger LogicLoggerInstance
        {
            get
            {
                return s_LogicLogger;
            }
        }

        internal static bool IsPaused
        {
            get
            {
                return s_IsPaused;
            }
        }

        private static LogicLogger s_LogicLogger = new LogicLogger();
        private static GameLogicThread s_LogicThread = new GameLogicThread();
        private static bool s_IsInited = false;
        private static bool s_IsPaused = false;
    }
}