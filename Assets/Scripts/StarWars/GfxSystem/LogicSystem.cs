using System;
using System.Threading;
using UnityEngine;

namespace StarWars
{
    public static class LogicSystem
    {
        //public static MissionConfig GetMissionDataById(int id)
        //{
        //    return MissionConfigProvider.Instance.GetDataById(id);
        //}
        //public static ItemConfig GetItemDataById(int id)
        //{
        //    return ItemConfigProvider.Instance.GetDataById(id);
        //}
        public static void SetLoadingBarScene(string name)
        {
            GfxSystem.Instance.SetLoadingBarScene(name);
        }
        public static float SampleTerrainHeight(float x, float z)
        {
            return GfxSystem.Instance.SampleTerrainHeight(x, z);
        }
        public static GameObject GetGameObject(int id)
        {
            return GfxSystem.Instance.GetGameObject(id);
        }
        public static SharedGameObjectInfo GetSharedGameObjectInfo(int id)
        {
            return GfxSystem.Instance.GetSharedGameObjectInfo(id);
        }
        public static SharedGameObjectInfo GetSharedGameObjectInfo(GameObject obj)
        {
            return GfxSystem.Instance.GetSharedGameObjectInfo(obj);
        }
        public static bool ExistGameObject(GameObject obj)
        {
            return GfxSystem.Instance.ExistGameObject(obj);
        }
        public static void VisitGameObject(MyAction<GameObject, SharedGameObjectInfo> visitor)
        {
            GfxSystem.Instance.VisitGameObject(visitor);
        }
        public static GameObject PlayerSelf
        {
            get { return GfxSystem.Instance.PlayerSelf; }
        }
        public static SharedGameObjectInfo PlayerSelfInfo
        {
            get { return GfxSystem.Instance.PlayerSelfInfo; }
        }
        public static bool IsLastHitUi
        {
            get { return GfxSystem.Instance.IsLastHitUi; }
            set { GfxSystem.Instance.IsLastHitUi = value; }
        }
        public static void LogicLog(string format, params object[] args)
        {
            GfxSystem.Instance.CallLogicLog(format, args);
        }
        public static void LogicErrorLog(string format, params object[] args)
        {
            GfxSystem.Instance.CallLogicErrorLog(format, args);
        }
        public static void GfxLog(string format, params object[] args)
        {
            GfxSystem.Instance.CallGfxLog(string.Format(format, args));
        }
        public static void GfxErrorLog(string format, params object[] args)
        {
            GfxSystem.Instance.CallGfxErrorLog(string.Format(format, args));
        }
        public static float RadianToDegree(float dir)
        {
            return GfxSystem.Instance.RadianToDegree(dir);
        }
        public static void SetScenePrepared(bool val)
        {
            GfxSystem.Instance.SceneResourcePrepared = val;
        }
        public static void SetScenePreparedProgress(float val)
        {
            GfxSystem.Instance.SceneResourcePreparedProgress = val;
        }
        public static void BeginLoading()
        {
            GfxSystem.Instance.BeginLoading();
        }
        public static void EndLoading()
        {
            GfxSystem.Instance.EndLoading();
        }
        public static void UpdateLoadingProgress(float progress)
        {
            GfxSystem.Instance.UpdateLoadingProgress(progress);
        }
        public static void UpdateLoadingTip(string tip)
        {
            GfxSystem.Instance.UpdateLoadingTip(tip);
        }
        public static void UpdateVersinoInfo(string info)
        {
            GfxSystem.Instance.UpdateVersionInfo(info);
        }
        public static float GetLoadingProgress()
        {
            return GfxSystem.Instance.GetLoadingProgress();
        }
        public static string GetLoadingTip()
        {
            return GfxSystem.Instance.GetLoadingTip();
        }
        public static string GetVersionInfo()
        {
            return GfxSystem.Instance.GetVersionInfo();
        }
        public static Transform FindChildRecursive(Transform parent, string bonePath)
        {
            return GfxSystem.Instance.FindChildRecursive(parent, bonePath);
        }
        public static void FireGestureEvent(GestureArgs args)
        {
            GfxSystem.Instance.OnGesture(args);
        }
        public static void SetJoystickInfo(GestureArgs args)
        {
            GfxSystem.Instance.SetJoystickInfoImpl(args);
        }
        public static void QueueLogicAction(MyAction action)
        {
            QueueLogicActionWithDelegation(action);
        }
        public static void QueueLogicAction<T1>(MyAction<T1> action, T1 t1)
        {
            QueueLogicActionWithDelegation(action, t1);
        }
        public static void QueueLogicAction<T1, T2>(MyAction<T1, T2> action, T1 t1, T2 t2)
        {
            QueueLogicActionWithDelegation(action, t1, t2);
        }
        public static void QueueLogicAction<T1, T2, T3>(MyAction<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3);
        }
        public static void QueueLogicAction<T1, T2, T3, T4>(MyAction<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5>(MyAction<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6>(MyAction<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7>(MyAction<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
        }
        public static void QueueLogicAction<R>(MyFunc<R> action)
        {
            QueueLogicActionWithDelegation(action);
        }
        public static void QueueLogicAction<T1, R>(MyFunc<T1, R> action, T1 t1)
        {
            QueueLogicActionWithDelegation(action, t1);
        }
        public static void QueueLogicAction<T1, T2, R>(MyFunc<T1, T2, R> action, T1 t1, T2 t2)
        {
            QueueLogicActionWithDelegation(action, t1, t2);
        }
        public static void QueueLogicAction<T1, T2, T3, R>(MyFunc<T1, T2, T3, R> action, T1 t1, T2 t2, T3 t3)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, R>(MyFunc<T1, T2, T3, T4, R> action, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, R>(MyFunc<T1, T2, T3, T4, T5, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, R>(MyFunc<T1, T2, T3, T4, T5, T6, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
        {
            QueueLogicAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
        }
        public static void QueueLogicAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16)
        {
            QueueLogicActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
        }
        public static void QueueLogicActionWithDelegation(Delegate action, params object[] args)
        {
            GfxSystem.Instance.QueueLogicActionWithDelegation(action, args);
        }
        public static void PublishLogicEvent(string evt, string group, params object[] args)
        {
            GfxSystem.Instance.PublishLogicEvent(evt, group, args);
        }
        public static void ProxyPublishLogicEvent(string evt, string group, params object[] args)
        {
            GfxSystem.Instance.ProxyPublishLogicEvent(evt, group, args);
        }
        public static PublishSubscribeSystem EventChannelForGfx
        {
            get
            {
                return GfxSystem.Instance.EventChannelForGfx;
            }
        }

        public static BeforeLoadSceneDelegation OnBeforeLoadScene
        {
            get
            {
                return GfxSystem.Instance.OnBeforeLoadScene;
            }
            set
            {
                GfxSystem.Instance.OnBeforeLoadScene = value;
            }
        }

        public static AfterLoadSceneDelegation OnAfterLoadScene
        {
            get
            {
                return GfxSystem.Instance.OnAfterLoadScene;
            }
            set
            {
                GfxSystem.Instance.OnAfterLoadScene = value;
            }
        }

        public static void SendStoryMessage(string msgId, params object[] args)
        {
            if (null != GfxSystem.Instance.GameLogicNotification)
            {
                QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxSendStoryMessage, msgId, args);
            }
        }
        public static void StartStory(int storyId)
        {
            if (null != GfxSystem.Instance.GameLogicNotification)
            {
                QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxStartStory, storyId);
            }
        }

        public static void NotifyGfxAnimationStart(GameObject obj, bool isSkill)
        {
            SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
            if (null != info)
            {
                if (isSkill)
                {
                    info.IsSkillGfxAnimation = true;
                }
                else
                {
                    info.IsImpactGfxAnimation = true;
                }
                GfxLog("NotifyGfxAnimationStart:{0}", info.m_LogicObjectId);
            }
            else
            {
                GfxLog("NotifyGfxAnimationStart:{0}, can't find object !", obj.name);
            }
        }
        public static void NotifyGfxAnimationFinish(GameObject obj, bool isSkill)
        {
            SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
            if (null != info)
            {
                if (isSkill)
                {
                    info.IsSkillGfxAnimation = false;
                }
                else
                {
                    info.IsImpactGfxAnimation = false;
                }

                GfxLog("NotifyGfxAnimationFinish:{0}", info.m_LogicObjectId);
            }
            else
            {
                GfxLog("NotifyGfxAnimationFinish:{0}, can't find object !", obj.name);
            }
        }
        public static void NotifyGfxMoveControlStart(GameObject obj, int id, bool isSkill)
        {
            SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
            if (null != info)
            {
                if (isSkill)
                {
                    info.IsSkillGfxMoveControl = true;
                }
                else
                {
                    info.IsImpactGfxMoveControl = true;
                    info.IsImpactGfxRotateControl = true;
                }
                if (null != GfxSystem.Instance.GameLogicNotification)
                {
                    QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxControlMoveStart, info.m_LogicObjectId, id, isSkill);
                }

                GfxLog("NotifyGfxMoveControlStart:{0}", info.m_LogicObjectId);
            }
            else
            {
                GfxLog("NotifyGfxMoveControlStart:{0}, can't find object !", obj.name);
            }
        }
        public static void NotifyGfxMoveControlFinish(GameObject obj, int id, bool isSkill)
        {
            SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
            if (null != info)
            {
                if (isSkill)
                {
                    info.IsSkillGfxMoveControl = false;
                    info.IsSkillGfxRotateControl = false;
                }
                else
                {
                    info.IsImpactGfxMoveControl = false;
                    info.IsImpactGfxRotateControl = false;
                }

                if (null != GfxSystem.Instance.GameLogicNotification)
                {
                    QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxControlMoveStop, info.m_LogicObjectId, id, isSkill);
                }

                GfxLog("NotifyGfxMoveControlFinish:{0}", info.m_LogicObjectId);
            }
            else
            {
                GfxLog("NotifyGfxMoveControlFinish:{0}, can't find object !", obj.name);
            }
        }
        public static void NotifyGfxUpdatePosition(GameObject obj, float x, float y, float z)
        {
            lock (GfxSystem.SyncLock)
            {
                SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
                if (null != info)
                {
                    info.x = x;
                    info.y = y;
                    info.z = z;
                    info.DataChangedByGfx = true;
                }
                else
                {
                    GfxLog("NotifyGfxUpdatePosition:{0} {1} {2} {3}, can't find object !", obj.name, x, y, z);
                }
            }
        }
        public static void NotifyGfxUpdatePosition(GameObject obj, float x, float y, float z, float rx, float ry, float rz)
        {
            lock (GfxSystem.SyncLock)
            {
                SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
                if (null != info)
                {
                    info.x = x;
                    info.y = y;
                    info.z = z;
                    info.FaceDir = ry;
                    info.DataChangedByGfx = true;
                }
                else
                {
                    GfxLog("NotifyGfxUpdatePosition:{0} {1} {2} {3} {4} {5} {6}, can't find object !", obj.name, x, y, z, rx, ry, rz);
                }
            }
        }

        public static void NotifyGfxChangedWantDir(GameObject obj, float ry)
        {
            lock (GfxSystem.SyncLock)
            {
                SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
                if (null != info)
                {
                    info.WantFaceDir = ry;
                    info.WantDirChangedByGfx = true;
                }
                else
                {
                    GfxLog("NotifyGfxUpdatePosition:{0} {1}, can't find object !", obj.name, ry);
                }
            }
        }

        public static void NotifyGfxHitTarget(GameObject src, int impactId, GameObject target, int hitCount, int skillId, int duration, Vector3 srcPos, float srcDir)
        {
            SharedGameObjectInfo srcInfo = GfxSystem.Instance.GetSharedGameObjectInfo(src);
            SharedGameObjectInfo targetInfo = GfxSystem.Instance.GetSharedGameObjectInfo(target);
            if (null != srcInfo && null != targetInfo)
            {
                if (null != GfxSystem.Instance.GameLogicNotification)
                {
                    QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxHitTarget, srcInfo.m_LogicObjectId, impactId, targetInfo.m_LogicObjectId, hitCount, skillId, duration, srcPos.x, srcPos.y, srcPos.z, srcDir);
                }
            }
            else
            {
                GfxLog("NotifyGfxHitTarget:{0} {1} {2} {3}, can't find object !", src.name, impactId, target.name, hitCount);
            }
        }

        public static void NotifyGfxStopImpact(GameObject src, int impactId, GameObject target)
        {
            if (null != src && null != target)
            {
                SharedGameObjectInfo srcInfo = GfxSystem.Instance.GetSharedGameObjectInfo(src);
                SharedGameObjectInfo targetInfo = GfxSystem.Instance.GetSharedGameObjectInfo(target);
                if (null != srcInfo && null != targetInfo)
                {
                    if (null != GfxSystem.Instance.GameLogicNotification)
                    {
                        QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxStopImpact, targetInfo.m_LogicObjectId, impactId);
                    }
                }
            }
        }

        public static void NotifyGfxMoveMeetObstacle(GameObject obj)
        {
            SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
            if (null != info)
            {
                if (null != GfxSystem.Instance.GameLogicNotification)
                {
                    QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxMoveMeetObstacle, info.m_LogicObjectId);
                }
            }
        }
        public static void NotifyGfxStartSkill(GameObject obj, SkillCategory category, Vector3 targetpos)
        {
            if (null != obj)
            {
                SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
                if (null != info)
                {
                    if (null != GfxSystem.Instance.GameLogicNotification)
                    {
                        QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxStartSkill, info.m_LogicObjectId,
                                                                                   category, targetpos.x,
                                                                                   targetpos.y, targetpos.z);
                        GfxLog("NotifyGfxStartSkill:{0} {1}", obj.name, category);
                    }
                }
                else
                {
                    GfxLog("NotifyGfxStartSkill:{0} {1}, can't find object !", obj.name, category);
                }
            }
        }
        public static void NotifyGfxForceStartSkill(GameObject obj, int skillId)
        {
            if (null != obj)
            {
                SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
                if (null != info)
                {
                    if (null != GfxSystem.Instance.GameLogicNotification)
                    {
                        QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxForceStartSkill, info.m_LogicObjectId, skillId);
                        //GfxLog("NotifyGfxStartSkill:{0} {1}", obj.name, category);
                    }
                }
                else
                {
                    GfxLog("NotifyGfxForceStartSkill:{0} {1}, can't find object !", obj.name, skillId);
                }
            }
        }
        public static void NotifyGfxStopSkill(GameObject obj, int skillId)
        {
            if (null != obj)
            {
                SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
                if (null != info)
                {
                    if (null != GfxSystem.Instance.GameLogicNotification)
                    {
                        QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxStopSkill, info.m_LogicObjectId, skillId);

                        GfxLog("NotifyGfxStopSkill:{0} {1}", obj.name, skillId);
                    }
                }
                else
                {
                    GfxLog("NotifyGfxStopSkill:{0} {1}, can't find object !", obj.name, skillId);
                }
            }
        }

        public static void NotifyGfxStartAttack(GameObject obj, float x, float y, float z)
        {
            SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
            if (null != info)
            {
                if (null != GfxSystem.Instance.GameLogicNotification)
                {
                    QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxStartAttack, info.m_LogicObjectId, x, y, z);
                }
            }
        }

        public static void NotifyGfxStopAttack(GameObject obj)
        {
            SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
            if (null != info)
            {
                if (null != GfxSystem.Instance.GameLogicNotification)
                {
                    QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxStopAttack, info.m_LogicObjectId);
                }
            }
        }

        public static void NotifyGfxSkillBreakSection(GameObject obj, int skillid, int breaktype, int starttime, int endtime, bool isinterrupt)
        {
            SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
            if (null != info)
            {
                if (null != GfxSystem.Instance.GameLogicNotification)
                {
                    QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxSkillBreakSection, info.m_LogicObjectId, skillid, breaktype, starttime, endtime, isinterrupt);
                }
            }
        }

        public static void NotifyGfxSetCrossFadeTime(GameObject obj, string fadeTargetAnim, float fadeTime)
        {
            SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
            if (null != info)
            {
                if (null != GfxSystem.Instance.GameLogicNotification)
                {
                    QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxSetCrossFadeTime, info.m_LogicObjectId, fadeTargetAnim, fadeTime);
                }
            }
        }

        public static void NotifyGfxAddLockInputTime(GameObject obj, SkillCategory category, float lockinputtime)
        {
            SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
            if (null != info)
            {
                if (null != GfxSystem.Instance.GameLogicNotification)
                {
                    QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxAddLockInputTime, info.m_LogicObjectId, category, lockinputtime);
                }
            }
        }

        public static void NotifyGfxSummonNpc(GameObject obj, int owner_skillid, int npc_type_id, string model, int skillid,
                                              float pos_x, float pos_y, float pos_z)
        {
            SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
            if (null != info)
            {
                if (null != GfxSystem.Instance.GameLogicNotification)
                {
                    QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxSummonNpc, info.m_LogicObjectId, owner_skillid, npc_type_id, model, skillid,
                                                                                                     pos_x, pos_y, pos_z);
                }
            }
        }

        public static void NotifyGfxDestroyObj(GameObject obj)
        {
            SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
            if (null != info)
            {
                if (null != GfxSystem.Instance.GameLogicNotification)
                {
                    QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxDestroyObj, info.m_LogicObjectId);
                }
            }
        }

        public static void NotifyGfxDestroySummonObject(GameObject obj)
        {
            SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
            if (null != info)
            {
                if (null != GfxSystem.Instance.GameLogicNotification)
                {
                    QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxDestroySummonObject, info.m_LogicObjectId);
                }
            }
        }

        public static void NotifyGfxSetObjLifeTime(GameObject obj, long life_remain_time)
        {
            SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
            if (null != info)
            {
                if (null != GfxSystem.Instance.GameLogicNotification)
                {
                    QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxSetObjLifeTime, info.m_LogicObjectId, life_remain_time);
                }
            }
        }

        public static void NotifyGfxSimulateMove(GameObject obj)
        {
            SharedGameObjectInfo info = GfxSystem.Instance.GetSharedGameObjectInfo(obj);
            if (null != info)
            {
                if (null != GfxSystem.Instance.GameLogicNotification)
                {
                    QueueLogicAction(GfxSystem.Instance.GameLogicNotification.OnGfxSimulateMove, info.m_LogicObjectId);
                }
            }
        }

    }
}
