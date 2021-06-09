using System;
using System.Collections.Generic;

namespace StarWars
{
    public sealed class TimeUtility
    {
        public static double CurTimestamp
        {
            get { return (DateTime.Now.AddHours(-8) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds; }
        }
        public static long AverageRoundtripTime
        {
            get { return s_AverageRoundtripTime; }
            set { s_AverageRoundtripTime = value; }
        }
        public static long RemoteTimeOffset
        {
            get { return s_RemoteTimeOffset; }
            set { s_RemoteTimeOffset = value; }
        }

        public static long GetServerMilliseconds()
        {
            long val = GetLocalMilliseconds();
            //if (GlobalVariables.Instance.IsClient)
            //{
            //    return val + s_RemoteTimeOffset;
            //}
            //else
            //{
            //    return val;
            //}
            return val;
        }
        public static long GetLocalMilliseconds()
        {
            return (GetElapsedTimeUs() - s_Instance.m_StartTimeUs) / 1000;
        }
        public static long GetElapsedTimeUs()
        {
            return DateTime.Now.Ticks / 10;
        }
        public static void SampleClientTick()
        {
            long curTime = GetElapsedTimeUs();
            s_Instance.m_ClientDeltaTime = curTime - s_Instance.m_ClientTickTimeUs;
            s_Instance.m_ClientTickTimeUs = curTime;
        }
        public static long GetClientDeltaTime()
        {
            return s_Instance.m_ClientDeltaTime;
        }

        private static long s_AverageRoundtripTime = 0;
        private static long s_RemoteTimeOffset = 0;
        private static TimeUtility s_Instance = new TimeUtility();

        private TimeUtility()
        {
            m_StartTimeUs = GetElapsedTimeUs();
        }
        private long m_StartTimeUs = 0;
        private long m_ClientTickTimeUs = 0;
        private long m_ClientDeltaTime = 0;
    }

    public sealed class TimeSnapshot
    {
        public static void Start()
        {
            Instance.Start_();
        }
        public static long End()
        {
            return Instance.End_();
        }
        public static long DoCheckPoint()
        {
            return Instance.DoCheckPoint_();
        }

        private void Start_()
        {
            m_LastSnapshotTime = TimeUtility.GetElapsedTimeUs();
            m_StartTime = m_LastSnapshotTime;
        }
        private long End_()
        {
            m_EndTime = TimeUtility.GetElapsedTimeUs();
            return m_EndTime - m_StartTime;
        }
        private long DoCheckPoint_()
        {
            long curTime = TimeUtility.GetElapsedTimeUs();
            long ret = curTime - m_LastSnapshotTime;
            m_LastSnapshotTime = curTime;
            return ret;
        }

        private long m_StartTime = 0;
        private long m_LastSnapshotTime = 0;
        private long m_EndTime = 0;

        private static TimeSnapshot Instance
        {
            get
            {
                if (null == s_Instance)
                {
                    s_Instance = new TimeSnapshot();
                }
                return s_Instance;
            }
        }

        [ThreadStatic]
        private static TimeSnapshot s_Instance = null;
    }
}
