using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using UnityEngine;

namespace StarWars
{
    public class ResAsyncInfo
    {
        public float Progress = 0;
        public bool IsDone = false;
        public bool IsError = false;
        public string Tip = string.Empty;
        public Coroutine CurCoroutine = null;
        public System.Object Target = null;
    }

    public enum ResCacheTypeMask
    {

    }

    public enum ResCacheType
    {
        none = 0,
        // ui
        ui = 1 << 1,
        // monster
        monster = 1 << 5,
        monster_effect = 1 << 6,
        // level
        level = 1 << 10,
        level_obj = 1 << 11,
        // async
        async = 1 << 15,
        // combile
        all = 0x7FFFFFFF,
    }
    public class ResCacheConfig
    {
        public const int S_ALL_RES = -1;
        public ResCacheType CacheType { get; private set; }
        public int ResId { get; private set; }
        public HashSet<int> LinkLimitList { get; set; }
        public ResCacheConfig(ResCacheType cacheType, int resId = S_ALL_RES)
        {
            CacheType = cacheType;
            ResId = resId;
        }
    }
    public class ResUpdateHandler
    {
        #region Resource Upgrade
        public delegate ResAsyncInfo EventStartUpdate(int targetChapter);
        public delegate ResAsyncInfo EventStartUpdateChapter(int targetChapter);
        public delegate void EventInitUpdate();
        public delegate void EventExitUpdate();
        public delegate void EventSetUpdateProgressRange(float start, float max, int progressBarType = 0);
        public delegate ResAsyncInfo EventCacheResByConfig(List<ResCacheConfig> cacheConfigList);
        public delegate bool EventGetEnableResServerSkip();
        public delegate int EventGetReconnectSkipNumMax();
        public delegate int EventGetReconnectNum();
        public delegate void EventIncReconnectNum();
        public delegate void EventResetReconnectNum();

        public static EventStartUpdate HandleStartUpdate;
        public static EventStartUpdateChapter HandleStartUpdateChapter;
        public static EventInitUpdate HandleInitUpdate;
        public static EventExitUpdate HandleExitUpdate;
        public static EventSetUpdateProgressRange HandleSetUpdateProgressRange;
        public static EventCacheResByConfig HandleCacheResByConfig;

        public static EventGetEnableResServerSkip HandleGetEnableResServerSkip;
        public static EventGetReconnectSkipNumMax HandleGetReconnectSkipNumMax;
        public static EventGetReconnectNum HandleGetReconnectNum;
        public static EventIncReconnectNum HandleIncReconnectNum;
        public static EventResetReconnectNum HandleResetReconnectNum;

        #endregion

        #region Resouces Load
        public delegate UnityEngine.Object EventLoadAssetFromABWithoutExtention(string assetNameWithoutExtention);
        public delegate void EventReleaseAllAssetBundle();
        public delegate void EventCleanup();

        public static EventLoadAssetFromABWithoutExtention HandleLoadAssetFromABWithoutExtention;
        public static EventReleaseAllAssetBundle HandleReleaseAllAssetBundle;
        public static EventCleanup HandleCleanup;
        #endregion

        #region Runtime Getter
        public delegate void EventGetPlayerCurSkillInfo(ref List<int> userList, ref List<int> userSkillList);
        public static EventGetPlayerCurSkillInfo HandleGetPlayerCurSkillInfo;
        #endregion

        public static ResAsyncInfo StartUpdate(int targetChapter)
        {
            if (HandleStartUpdate != null)
            {
                return HandleStartUpdate(targetChapter);
            }
            return null;
        }
        public static ResAsyncInfo StartUpdateChapter(int targetChapter)
        {
            if (HandleStartUpdateChapter != null)
            {
                return HandleStartUpdateChapter(targetChapter);
            }
            return null;
        }
        public static void InitUpdate()
        {
            if (HandleInitUpdate != null)
            {
                HandleInitUpdate();
            }
        }
        public static void ExitUpdate()
        {
            if (HandleExitUpdate != null)
            {
                HandleExitUpdate();
            }
        }
        public static ResAsyncInfo CacheResByConfig(List<ResCacheConfig> cacheConfigList)
        {
            if (HandleCacheResByConfig != null)
            {
                return HandleCacheResByConfig(cacheConfigList);
            }
            return null;
        }
        public static void SetUpdateProgressRange(float start, float max, int progressBarType = 0)
        {
            if (HandleSetUpdateProgressRange != null)
            {
                HandleSetUpdateProgressRange(start, max, progressBarType);
            }
        }
        public static UnityEngine.Object LoadAssetFromABWithoutExtention(string assetNameWithoutExtention)
        {
            if (HandleLoadAssetFromABWithoutExtention != null)
            {
                return HandleLoadAssetFromABWithoutExtention(assetNameWithoutExtention);
            }
            return null;
        }
        public static void Cleanup()
        {
            if (HandleCleanup != null)
            {
                HandleCleanup();
            }
        }
        public static void ReleaseAllAssetBundle()
        {
            if (HandleReleaseAllAssetBundle != null)
            {
                HandleReleaseAllAssetBundle();
            }
        }
        public static void GetPlayerInfoForCache(ref List<int> userList, ref List<int> userSkillList)
        {
            if (HandleGetPlayerCurSkillInfo != null)
            {
                HandleGetPlayerCurSkillInfo(ref userList, ref userSkillList);
            }
        }
        public static bool GetEnableResServerSkip()
        {
            if (HandleGetEnableResServerSkip != null)
            {
                return HandleGetEnableResServerSkip();
            }
            return false;
        }
        public static int GetReconnectSkipNumMax()
        {
            if (HandleGetReconnectSkipNumMax != null)
            {
                return HandleGetReconnectSkipNumMax();
            }
            return 0;
        }
        public static int GetReconnectNum()
        {
            if (HandleGetReconnectNum != null)
            {
                return HandleGetReconnectNum();
            }
            return 0;
        }
        public static void IncReconnectNum()
        {
            if (HandleIncReconnectNum != null)
            {
                HandleIncReconnectNum();
            }
        }
        public static void ResetReconnectNum()
        {
            if (HandleResetReconnectNum != null)
            {
                HandleResetReconnectNum();
            }
        }
    }
}
