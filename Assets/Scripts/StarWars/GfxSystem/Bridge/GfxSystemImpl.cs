using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarWars
{
    //public delegate void LogicLogCallbackDelegation(bool isError, string format, object[] args);
    public delegate void BeforeLoadSceneDelegation(string curName, string targetName, int targetSceneId);
    public delegate void AfterLoadSceneDelegation(string targetName, int targetSceneId);

    /// <summary>
    /// 显示层 引擎渲染实施类
    /// 
    /// </summary>
    public sealed partial class GfxSystem
    {
        private class GameObjectInfo
        {
            public GameObject ObjectInstance;
            public SharedGameObjectInfo ObjectInfo;

            public GameObjectInfo(GameObject o, SharedGameObjectInfo i)
            {
                ObjectInstance = o;
                ObjectInfo = i;
            }
        }
        //初始化阶段调用的函数
        private void InitImpl()
        {
            m_EventChannelForLogic.RunInLogicThread = true;
            m_EventChannelForGfx.RunInLogicThread = false;
        }
        private void TickImpl()
        {
            long curTime = TimeUtility.GetLocalMilliseconds();
            if (m_LastLogTime + 10000 < curTime)
            {
                m_LastLogTime = curTime;

                if (m_GfxInvoker.CurActionNum > 10)
                {
                    CallGfxLog("GfxSystem.Tick actionNum:{0}", m_GfxInvoker.CurActionNum);
                }

                m_GfxInvoker.DebugPoolCount((string msg) =>
                {
                    CallGfxLog("GfxActionQueue {0}", msg);
                });
            }
            HandleSync();
            HandleInput();
            HandleLoadingProgress();
            ResourceManager.Instance.Tick();
            m_GfxInvoker.HandleActions(4096);
        }
        private void ReleaseImpl()
        {

        }
        private void SetLogicInvokerImpl(IActionQueue processor)
        {
            m_LogicInvoker = processor;
        }
        private void SetLogicLogCallbackImpl(MyAction<bool, string, object[]> callback)
        {
            m_LogicLogCallback = callback;
        }
        private void SetGameLogicNotificationImpl(IGameLogicNotification notification)
        {
            m_GameLogicNotification = notification;
        }
        //Gfx线程执行的函数，供游戏逻辑线程异步调用
        private void LoadSceneImpl(string name, int chapter, int sceneId, HashSet<int> limitList, MyAction onFinish)
        {
            CallLogicLog("Begin LoadScene:{0}", name);
            m_TargetScene = name;
            m_TargetChapter = chapter;
            m_TargetSceneId = sceneId;
            m_TargetSceneLimitList = limitList;
            BeginLoading();
            if (null == m_LoadingBarAsyncOperation)
            {
                m_LoadingBarAsyncOperation = Application.LoadLevelAsync(m_LoadingBarScene);
                m_LevelLoadedCallback = onFinish;
            }
        }
        private void MarkPlayerSelfImpl(int id)
        {
            GameObjectInfo info = GetGameObjectInfo(id);
            if (null != info)
            {
                m_PlayerSelf = info;
                if (null != info.ObjectInstance)
                {
                    int layer = LayerMask.NameToLayer("Character");
                    if (layer >= 0)
                    {
                        info.ObjectInstance.layer = layer;
                    }
                }
            }
        }
        private void CreateGameObjectImpl(int id, string resource, SharedGameObjectInfo info)
        {
            if (null != info)
            {
                try
                {
                    Vector3 pos = new Vector3(info.x, info.y, info.z);
                    if (!info.IsFloat)
                        pos.y = SampleTerrainHeight(pos.x, pos.z);
                    Quaternion q = Quaternion.Euler(0, RadianToDegree(info.FaceDir), 0);
                    GameObject obj = ResourceManager.Instance.NewObject(resource) as GameObject;
                    if (null != obj)
                    {
                        if (null != obj.transform)
                        {
                            obj.transform.position = pos;
                            obj.transform.localRotation = q;
                            obj.transform.localScale = new Vector3(info.sx, info.sy, info.sz);
                        }
                        RememberGameObject(id, obj, info);
                        obj.SetActive(true);
                    }
                    else
                    {
                        CallLogicErrorLog("CreateGameObject {0} can't load resource", resource);
                    }
                }
                catch (Exception ex)
                {
                    CallGfxErrorLog("CreateGameObject {0} throw exception:{1}\n{2}", resource, ex.Message, ex.StackTrace);
                }
            }
        }
        private void CreateGameObjectImpl(int id, string resource, float x, float y, float z, float rx, float ry, float rz, bool attachTerrain)
        {
            try
            {
                if (attachTerrain)
                    y = SampleTerrainHeight(x, z);
                Vector3 pos = new Vector3(x, y, z);
                Quaternion q = Quaternion.Euler(RadianToDegree(rx), RadianToDegree(ry), RadianToDegree(rz));
                GameObject obj = ResourceManager.Instance.NewObject(resource) as GameObject;
                if (null != obj)
                {
                    obj.transform.position = pos;
                    obj.transform.localRotation = q;
                    RememberGameObject(id, obj);
                    obj.SetActive(true);
                }
                else
                {
                    CallLogicErrorLog("CreateGameObject {0} can't load resource", resource);
                }
            }
            catch (Exception ex)
            {
                CallGfxErrorLog("CreateGameObject {0} throw exception:{1}\n{2}", resource, ex.Message, ex.StackTrace);
            }
        }
        private void CreateGameObjectWithMeshDataImpl(int id, List<float> vertices, List<int> triangles, uint color, string mat, bool attachTerrain)
        {
            if (vertices.Count >= 3)
            {
                List<float> uvs = new List<float>();
                int count = vertices.Count / 3;
                for (int i = 0; i < count; ++i)
                {
                    int ix = i % 4;
                    switch (ix)
                    {
                        case 0:
                            uvs.Add(0);
                            uvs.Add(0);
                            break;
                        case 1:
                            uvs.Add(0);
                            uvs.Add(1);
                            break;
                        case 2:
                            uvs.Add(1);
                            uvs.Add(0);
                            break;
                        case 3:
                            uvs.Add(1);
                            uvs.Add(1);
                            break;
                    }
                }
                CreateGameObjectWithMeshDataImpl(id, vertices, uvs, triangles, color, mat, attachTerrain);
            }
        }
        private void CreateGameObjectWithMeshDataImpl(int id, List<float> vertices, List<float> uvs, List<int> triangles, uint color, string mat, bool attachTerrain)
        {
            byte a = (byte)((color & 0xff000000) >> 24);
            byte r = (byte)((color & 0x0ff0000) >> 16);
            byte g = (byte)((color & 0x0ff00) >> 8);
            byte b = (byte)(color & 0x0ff);
            Color32 c = new Color32(r, g, b, a);

            Material material = null;
            Shader shader = Shader.Find(mat);
            if (null != shader)
            {
                material = new Material(shader);
                material.color = c;
            }
            else
            {
                material = new Material(mat);
                material.color = c;
            }

            CreateGameObjectWithMeshDataHelper(id, vertices, uvs, triangles, material, attachTerrain);
        }
        private void CreateGameObjectWithMeshDataImpl(int id, List<float> vertices, List<int> triangles, string matRes, bool attachTerrain)
        {
            if (vertices.Count >= 3)
            {
                List<float> uvs = new List<float>();
                int count = vertices.Count / 3;
                for (int i = 0; i < count; ++i)
                {
                    int ix = i % 4;
                    switch (ix)
                    {
                        case 0:
                            uvs.Add(0);
                            uvs.Add(0);
                            break;
                        case 1:
                            uvs.Add(0);
                            uvs.Add(1);
                            break;
                        case 2:
                            uvs.Add(1);
                            uvs.Add(0);
                            break;
                        case 3:
                            uvs.Add(1);
                            uvs.Add(1);
                            break;
                    }
                }
                CreateGameObjectWithMeshDataImpl(id, vertices, uvs, triangles, matRes, attachTerrain);
            }
        }
        private void CreateGameObjectWithMeshDataImpl(int id, List<float> vertices, List<float> uvs, List<int> triangles, string matRes, bool attachTerrain)
        {
            UnityEngine.Object matObj = ResourceManager.Instance.GetSharedResource(matRes);
            Material material = matObj as Material;
            if (null != material)
            {
                CreateGameObjectWithMeshDataHelper(id, vertices, uvs, triangles, material, attachTerrain);
            }
            else
            {
                CallLogicErrorLog("CreateGameObjectWithMeshData {0} can't load resource", matRes);
            }
        }
        private void CreateGameObjectForAttachImpl(int id, string resource)
        {
            try
            {
                GameObject obj = ResourceManager.Instance.NewObject(resource) as GameObject;
                if (null != obj)
                {
                    RememberGameObject(id, obj);
                    obj.SetActive(true);
                }
                else
                {
                    CallLogicErrorLog("CreateGameObject {0} can't load resource", resource);
                }
            }
            catch (Exception ex)
            {
                CallGfxErrorLog("CreateGameObject {0} throw exception:{1}\n{2}", resource, ex.Message, ex.StackTrace);
            }
        }
        private void CreateAndAttachGameObjectImpl(string resource, int parentId, string path, float recycleTime)
        {
            try
            {
                GameObject obj = ResourceManager.Instance.NewObject(resource, recycleTime) as GameObject;
                GameObject parent = GetGameObject(parentId);
                if (null != obj)
                {
                    obj.SetActive(true);
                    if (null != obj.transform && null != parent && null != parent.transform)
                    {
                        Transform t = parent.transform;
                        if (!String.IsNullOrEmpty(path))
                        {
                            t = FindChildRecursive(parent.transform, path);
                        }
                        if (null != t)
                        {
                            obj.transform.parent = t;
                            obj.transform.localPosition = new Vector3(0, 0, 0);
                        }
                        else
                        {
                            CallLogicErrorLog("Obj {0} CreateAndAttachGameObject {1} can't find bone {2}", resource, parentId, path);
                        }
                    }
                }
                else
                {
                    CallLogicErrorLog("CreateAndAttachGameObject {0} can't load resource", resource);
                }
            }
            catch (Exception ex)
            {
                CallGfxErrorLog("CreateAndAttachGameObject {0} throw exception:{1}\n{2}", resource, ex.Message, ex.StackTrace);
            }
        }
        private void DestroyGameObjectImpl(int id)
        {
            try
            {
                GameObject obj = GetGameObject(id);
                if (null != obj)
                {
                    ForgetGameObject(id, obj);
                    obj.SetActive(false);
                    if (!ResourceManager.Instance.RecycleObject(obj))
                    {
                        GameObject.Destroy(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                CallGfxErrorLog(string.Format("DestroyGameObject:{0} failed:{1}\n{2}", id, ex.Message, ex.StackTrace));
            }
        }
        private void UpdateGameObjectLocalPositionImpl(int id, float x, float y, float z)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
            {
                obj.transform.localPosition = new Vector3(x, y, z);
            }
        }
        private void UpdateGameObjectLocalPosition2DImpl(int id, float x, float z, bool attachTerrain)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
            {
                float y = 0;
                if (attachTerrain)
                    y = SampleTerrainHeight(x, z);
                else
                    y = obj.transform.localPosition.y;
                obj.transform.localPosition = new Vector3(x, y, z);
            }
        }
        private void UpdateGameObjectLocalRotateImpl(int id, float rx, float ry, float rz)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
            {
                obj.transform.localRotation = Quaternion.Euler(RadianToDegree(rx), RadianToDegree(ry), RadianToDegree(rz));
            }
        }
        private void UpdateGameObjectLocalRotateYImpl(int id, float ry)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
            {
                float rx = obj.transform.localRotation.eulerAngles.x;
                float rz = obj.transform.localRotation.eulerAngles.z;
                obj.transform.localRotation = Quaternion.Euler(rx, RadianToDegree(ry), rz);
            }
        }
        private void UpdateGameObjectLocalScaleImpl(int id, float sx, float sy, float sz)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
            {
                obj.transform.localScale = new Vector3(sx, sy, sz);
            }
        }
        private void AttachGameObjectImpl(int id, int parentId, float x, float y, float z, float rx, float ry, float rz)
        {
            GameObject obj = GetGameObject(id);
            GameObject parent = GetGameObject(parentId);
            if (null != obj && null != obj.transform && null != parent && null != parent.transform)
            {
                obj.transform.parent = parent.transform;
                obj.transform.localPosition = new Vector3(x, y, z);
                obj.transform.localRotation = Quaternion.Euler(RadianToDegree(rx), RadianToDegree(ry), RadianToDegree(rz));
            }
        }
        private void AttachGameObjectImpl(int id, int parentId, string path, float x, float y, float z, float rx, float ry, float rz)
        {
            GameObject obj = GetGameObject(id);
            GameObject parent = GetGameObject(parentId);
            if (null != obj && null != obj.transform && null != parent && null != parent.transform)
            {
                Transform t = FindChildRecursive(parent.transform, path);
                if (null != t)
                {
                    obj.transform.parent = t;
                    obj.transform.localPosition = new Vector3(x, y, z);
                    obj.transform.localRotation = Quaternion.Euler(RadianToDegree(rx), RadianToDegree(ry), RadianToDegree(rz));
                }
                else
                {
                    CallLogicLog("Obj {0} AttachGameObject {1} can't find bone {2}", id, parentId, path);
                }
            }
        }
        private void DetachGameObjectImpl(int id)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj && null != obj.transform)
            {
                obj.transform.parent = null;
            }
        }
        private void SetGameObjectVisibleImpl(int id, bool visible)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
            {
                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; ++i)
                {
                    renderers[i].enabled = visible;
                }
            }
        }
        private void PlayAnimationImpl(int id, bool isStopAll)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            if (null != obj && null != animation)
            {
                try
                {
                    animation.Play(isStopAll ? PlayMode.StopAll : PlayMode.StopSameLayer);
                }
                catch
                {
                }
            }
        }
        private void PlayAnimationImpl(int id, string animationName, bool isStopAll)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            if (null != obj && null != animation)
            {
                try
                {
                    if (null != animation[animationName])
                    {
                        animation.Play(animationName, isStopAll ? PlayMode.StopAll : PlayMode.StopSameLayer);
                        //CallLogicLog("Obj {0} PlayerAnimation {1} clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                    else
                    {
                        CallLogicErrorLog("Obj {0} PlayerAnimation {1} AnimationState is null, clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                }
                catch
                {
                }
            }
        }
        private void StopAnimationImpl(int id, string animationName)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            if (null != obj && null != animation)
            {
                try
                {
                    if (null != animation[animationName])
                    {
                        animation.Stop(animationName);
                        //CallLogicLog("Obj {0} StopAnimation {1} clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                    else
                    {
                        CallLogicErrorLog("Obj {0} StopAnimation {1} AnimationState is null, clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                }
                catch
                {
                }
            }
        }
        private void StopAnimationImpl(int id)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            if (null != obj && null != animation)
            {
                try
                {
                    animation.Stop();
                }
                catch
                {
                }
            }
        }
        private void BlendAnimationImpl(int id, string animationName, float weight, float fadeLength)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            if (null != obj && null != animation)
            {
                try
                {
                    AnimationState state = animation[animationName];
                    if (null != state)
                    {
                        animation.Blend(animationName, weight, fadeLength);
                        //CallLogicLog("Obj {0} BlendAnimation {1} clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                    else
                    {
                        CallLogicErrorLog("Obj {0} BlendAnimation {1} AnimationState is null, clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                }
                catch
                {
                }
            }
        }
        private void CrossFadeAnimationImpl(int id, string animationName, float fadeLength, bool isStopAll)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            SharedGameObjectInfo obj_info = GetSharedGameObjectInfo(id);
            if (null != obj && null != animation)
            {
                try
                {
                    if (null != animation[animationName] && obj_info != null && !obj_info.IsGfxAnimation)
                    {
                        animation.CrossFade(animationName, fadeLength, isStopAll ? PlayMode.StopAll : PlayMode.StopSameLayer);
                        //CallLogicLog("Obj {0} CrossFadeAnimation {1} clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                    else
                    {
                        if (null == animation[animationName])
                        {
                            CallLogicErrorLog("Obj {0} CrossFadeAnimation {1} AnimationState is null, clipcount {2}", id, animationName, animation.GetClipCount());
                        }
                        if (null == obj_info)
                        {
                            CallLogicErrorLog("Obj {0} CrossFadeAnimation {1} obj_info is null, obj name {2}", id, animationName, obj.name);
                        }
                    }
                }
                catch
                {
                }
            }
        }
        private void PlayQueuedAnimationImpl(int id, string animationName, bool isPlayNow, bool isStopAll)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            if (null != obj && null != animation)
            {
                try
                {
                    if (null != animation[animationName])
                    {
                        animation.PlayQueued(animationName, isPlayNow ? QueueMode.PlayNow : QueueMode.CompleteOthers, isStopAll ? PlayMode.StopAll : PlayMode.StopSameLayer);
                        //CallLogicLog("Obj {0} PlayQueuedAnimation {1} clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                    else
                    {
                        CallLogicErrorLog("Obj {0} PlayQueuedAnimation {1} AnimationState is null, clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                }
                catch
                {
                }
            }
        }
        private void CrossFadeQueuedAnimationImpl(int id, string animationName, float fadeLength, bool isPlayNow, bool isStopAll)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            if (null != obj && null != animation)
            {
                try
                {
                    if (null != animation[animationName])
                    {
                        animation.CrossFadeQueued(animationName, fadeLength, isPlayNow ? QueueMode.PlayNow : QueueMode.CompleteOthers, isStopAll ? PlayMode.StopAll : PlayMode.StopSameLayer);
                        //CallLogicLog("Obj {0} CrossFadeQueuedAnimation {1} clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                    else
                    {
                        CallLogicErrorLog("Obj {0} CrossFadeQueuedAnimation {1} AnimationState is null, clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                }
                catch
                {
                }
            }
        }
        private void RewindAnimationImpl(int id, string animationName)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            if( null != animation)
            {
                try
                {
                    if (null != animation[animationName])
                    {
                        animation.Rewind(animationName);
                    }
                    else
                    {
                        CallLogicErrorLog("Obj {0} RewindAnimation {1} AnimationState is null, clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                }
                catch
                {
                }
            }
        }
        private void RewindAnimationImpl(int id)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            if (null != obj && null != animation)
            {
                try
                {
                    animation.Rewind();
                }
                catch
                {
                }
            }
        }
        private void SetAnimationSpeedImpl(int id, string animationName, float speed)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            if (null != obj && null != animation)
            {
                try
                {
                    AnimationState state = animation[animationName];
                    if (null != state)
                    {
                        state.speed = speed;
                    }
                    else
                    {
                        CallLogicErrorLog("Obj {0} SetAnimationSpeed {1} AnimationState is null, clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                }
                catch
                {
                }
            }
        }
        private void SetAnimationSpeedByTimeImpl(int id, string animationName, float time)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            if (null != obj && null != animation)
            {
                try
                {
                    AnimationState state = animation[animationName];
                    if (null != state)
                    {
                        state.speed = state.length / state.time;
                    }
                    else
                    {
                        CallLogicErrorLog("Obj {0} SetAnimationSpeedByTime {1} AnimationState is null, clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                }
                catch
                {
                }
            }
        }
        private void SetAnimationWeightImpl(int id, string animationName, float weight)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            if (null != obj && null != animation)
            {
                try
                {
                    AnimationState state = animation[animationName];
                    if (null != state)
                    {
                        state.weight = weight;
                    }
                    else
                    {
                        CallLogicErrorLog("Obj {0} SetAnimationWeight {1} AnimationState is null, clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                }
                catch
                {
                }
            }
        }
        private void SetAnimationLayerImpl(int id, string animationName, int layer)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            if (null != obj && null != animation)
            {
                try
                {
                    AnimationState state = animation[animationName];
                    if (null != state)
                    {
                        state.layer = layer;
                    }
                    else
                    {
                        CallLogicErrorLog("Obj {0} SetAnimationLayer {1} AnimationState is null, clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                }
                catch
                {
                }
            }
        }
        private void SetAnimationBlendModeImpl(int id, string animationName, int blendMode)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            if (null != obj && null != animation)
            {
                try
                {
                    AnimationState state = animation[animationName];
                    if (null != state)
                    {
                        state.blendMode = (AnimationBlendMode)blendMode;
                    }
                    else
                    {
                        CallLogicErrorLog("Obj {0} SetAnimationBlendMode {1} AnimationState is null, clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                }
                catch
                {
                }
            }
        }
        private void AddMixingTransformAnimationImpl(int id, string animationName, string path, bool recursive)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            if (null != obj && null != animation && null != obj.transform)
            {
                try
                {
                    AnimationState state = animation[animationName];
                    if (null != state)
                    {
                        Transform t = obj.transform.Find(path);
                        if (null != t)
                        {
                            state.AddMixingTransform(t, recursive);
                        }
                        else
                        {
                            CallLogicErrorLog("Obj {0} AddMixingTransformAnimation {1} Can't find bone {2}", id, animationName, path);
                        }
                    }
                    else
                    {
                        CallLogicErrorLog("Obj {0} AddMixingTransformAnimation {1} AnimationState is null, clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                }
                catch
                {
                }
            }
        }
        private void RemoveMixingTransformAnimationImpl(int id, string animationName, string path)
        {
            GameObject obj = GetGameObject(id);
            if (null != obj)
                return;

            Animation animation = obj.GetComponent<Animation>();
            if (null != obj && null != animation && null != obj.transform)
            {
                try
                {
                    AnimationState state = animation[animationName];
                    if (null != state)
                    {
                        Transform t = obj.transform.Find(path);
                        if (null != t)
                        {
                            state.RemoveMixingTransform(t);
                        }
                        else
                        {
                            CallLogicErrorLog("Obj {0} RemoveMixingTransformAnimation {1} Can't find bone {2}", id, animationName, path);
                        }
                    }
                    else
                    {
                        CallLogicErrorLog("Obj {0} RemoveMixingTransformAnimation {1} AnimationState is null, clipcount {2}", id, animationName, animation.GetClipCount());
                    }
                }
                catch
                {
                }
            }
        }
        private AudioSource GetAudioSource(GameObject obj, string source_obj_name)
        {
            if (obj == null)
            {
                return null;
            }
            AudioSource[] audiosources = obj.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource audio in audiosources)
            {
                if (audio.gameObject.name.Equals(source_obj_name))
                {
                    return audio;
                }
            }
            return null;
        }
        private void PlaySoundImpl(int id, string audiosource, float pitch)
        {
            GameObject obj = GetGameObject(id);
            AudioSource target_audio_source = GetAudioSource(obj, audiosource);
            if (target_audio_source == null)
            {
                CallLogicErrorLog("id={0} obj can't find audiosource {1}! can't play sound!", id, audiosource);
                return;
            }
            target_audio_source.pitch = pitch;
            target_audio_source.Play();
        }
        private void SetAudioSourcePitchImpl(int id, string audiosource, float pitch)
        {
            GameObject obj = GetGameObject(id);
            AudioSource target_audio_source = GetAudioSource(obj, audiosource);
            if (target_audio_source == null)
            {
                CallLogicErrorLog("id={0} obj can't find audiosource {1}! can't set sound pitch!", id, audiosource);
                return;
            }
            target_audio_source.pitch = pitch;
        }
        private void StopSoundImpl(int id, string audiosource)
        {
            GameObject obj = GetGameObject(id);
            AudioSource target_audio_source = GetAudioSource(obj, audiosource);
            if (target_audio_source == null)
            {
                CallLogicErrorLog("id={0} obj can't find audiosource {1}! can't stop sound!", id, audiosource);
                return;
            }
            target_audio_source.Stop();
        }
        private void SetShaderImpl(int id, string shaderPath)
        {
            GameObject obj = GetGameObject(id);
            if (null == obj)
            {
                return;
            }
            Shader shader = Shader.Find(shaderPath);
            if (null == shader)
            {
                CallLogicErrorLog("id={0} obj can't find shader {1}!", id, shaderPath);
                return;
            }
            SkinnedMeshRenderer[] renderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                if (renderers[i].material.shader != shader)
                {
                    renderers[i].material.shader = shader;
                }
            }
        }
        private void SetBlockedShaderImpl(int id, uint rimColor, float rimPower, float cutValue)
        {
            GameObjectInfo objInfo = GetGameObjectInfo(id);
            if (null == objInfo || null == objInfo.ObjectInstance || null == objInfo.ObjectInfo)
            {
                return;
            }
            bool needChange = false;
            SkinnedMeshRenderer[] skinnedRenderers = objInfo.ObjectInstance.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    string name = mat.shader.name;
                    if (0 != name.CompareTo("DFM/Blocked") && 0 != name.CompareTo("DFM/NotBlocked"))
                    {
                        needChange = true;
                    }
                }
            }
            MeshRenderer[] meshRenderers = objInfo.ObjectInstance.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in meshRenderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    string name = mat.shader.name;
                    if (0 != name.CompareTo("DFM/Blocked") && 0 != name.CompareTo("DFM/NotBlocked"))
                    {
                        needChange = true;
                    }
                }
            }
            if (needChange)
            {
                byte rb = (byte)((rimColor & 0xFF000000) >> 24);
                byte gb = (byte)((rimColor & 0x00FF0000) >> 16);
                byte bb = (byte)((rimColor & 0x0000FF00) >> 8);
                byte ab = (byte)(rimColor & 0x000000FF);
                float r = (float)rb / 255.0f;
                float g = (float)gb / 255.0f;
                float b = (float)bb / 255.0f;
                float a = (float)ab / 255.0f;
                Color c = new Color(r, g, b, a);

                Shader blocked = Shader.Find("DFM/Blocked");
                Shader notBlocked = Shader.Find("DFM/NotBlocked");
                if (null == blocked)
                {
                    CallLogicLog("id={0} obj can't find shader DFM/Blocked !", id);
                    return;
                }
                if (null == notBlocked)
                {
                    CallLogicLog("id={0} obj can't find shader DFM/NotBlocked !", id);
                    return;
                }
                foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
                {
                    objInfo.ObjectInfo.m_SkinedMaterialChanged = true;
                    Texture texture = renderer.material.mainTexture;

                    Material blockedMat = new Material(blocked);
                    Material notBlockedMat = new Material(notBlocked);
                    Material[] mats = new Material[]{
            notBlockedMat,
            blockedMat
          };
                    blockedMat.SetColor("_RimColor", c);
                    blockedMat.SetFloat("_RimPower", rimPower);
                    blockedMat.SetFloat("_CutValue", cutValue);
                    notBlockedMat.SetTexture("_MainTex", texture);

                    renderer.materials = mats;
                }
                foreach (MeshRenderer renderer in meshRenderers)
                {
                    objInfo.ObjectInfo.m_MeshMaterialChanged = true;
                    Texture texture = renderer.material.mainTexture;

                    Material blockedMat = new Material(blocked);
                    Material notBlockedMat = new Material(notBlocked);
                    Material[] mats = new Material[]{
            notBlockedMat,
            blockedMat
          };
                    blockedMat.SetColor("_RimColor", c);
                    blockedMat.SetFloat("_RimPower", rimPower);
                    blockedMat.SetFloat("_CutValue", cutValue);
                    notBlockedMat.SetTexture("_MainTex", texture);

                    renderer.materials = mats;
                }
            }
        }
        private void RestoreMaterialImpl(int id)
        {
            GameObjectInfo objInfo = GetGameObjectInfo(id);
            if (null == objInfo)
            {
                return;
            }
            GameObject obj = objInfo.ObjectInstance;
            SharedGameObjectInfo info = objInfo.ObjectInfo;
            if (null != obj && null != info)
            {
                if (info.m_SkinedMaterialChanged)
                {
                    SkinnedMeshRenderer[] renderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
                    int ix = 0;
                    int ct = info.m_SkinedOriginalMaterials.Count;
                    foreach (SkinnedMeshRenderer renderer in renderers)
                    {
                        if (ix < ct)
                        {
                            renderer.materials = info.m_SkinedOriginalMaterials[ix] as Material[];
                            ++ix;
                        }
                    }
                    info.m_SkinedMaterialChanged = false;
                }
                if (info.m_MeshMaterialChanged)
                {
                    MeshRenderer[] renderers = obj.GetComponentsInChildren<MeshRenderer>();
                    int ix = 0;
                    int ct = info.m_MeshOriginalMaterials.Count;
                    foreach (MeshRenderer renderer in renderers)
                    {
                        if (ix < ct)
                        {
                            renderer.materials = info.m_MeshOriginalMaterials[ix] as Material[];
                            ++ix;
                        }
                    }
                    info.m_MeshMaterialChanged = false;
                }
            }
        }
        private void SetTimeScaleImpl(float scale)
        {
            Time.timeScale = scale;
        }

        private void GfxLogImpl(string msg)
        {
            SendMessageImpl("GfxGameRoot", "LogToConsole", msg, false);
        }
        private void GfxErrorLogImpl(string error)
        {
            SendMessageImpl("GfxGameRoot", "LogToConsole", error, false);
        }
        private void PublishGfxEventImpl(string evt, string group, object[] args)
        {
            m_EventChannelForGfx.Publish(evt, group, args);
        }
        private void ProxyPublishGfxEventImpl(string evt, string group, object[] args)
        {
            m_EventChannelForGfx.ProxyPublish(evt, group, args);
        }
        private void SendMessageImpl(string objname, string msg, object arg, bool needReceiver)
        {
            GameObject obj = GameObject.Find(objname);
            if (null != obj)
            {
                try
                {
                    obj.SendMessage(msg, arg, needReceiver ? SendMessageOptions.RequireReceiver : SendMessageOptions.DontRequireReceiver);
                }
                catch
                {

                }
            }
        }
        private void SendMessageByIdImpl(int objid, string msg, object arg, bool needReceiver)
        {
            GameObject obj = GetGameObject(objid);
            if (null != obj)
            {
                try
                {
                    obj.SendMessage(msg, arg, needReceiver ? SendMessageOptions.RequireReceiver : SendMessageOptions.DontRequireReceiver);
                }
                catch
                {

                }
            }
        }
        private void SendMessageWithTagImpl(string objtag, string msg, object arg, bool needReceiver)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(objtag);
            if (null != objs)
            {
                foreach (GameObject obj in objs)
                {
                    try
                    {
                        obj.SendMessage(msg, arg, needReceiver ? SendMessageOptions.RequireReceiver : SendMessageOptions.DontRequireReceiver);
                    }
                    catch
                    {
                    }
                }
            }
        }
        //游戏逻辑层执行的函数，供Gfx线程异步调用
        private void PublishLogicEventImpl(string evt, string group, object[] args)
        {
            m_EventChannelForLogic.Publish(evt, group, args);
        }
        private void ProxyPublishLogicEventImpl(string evt, string group, object[] args)
        {
            m_EventChannelForLogic.ProxyPublish(evt, group, args);
        }

        //Gfx线程执行的函数，对游戏逻辑线程的异步调用由这里发起
        internal float SampleTerrainHeight(float x, float z)
        {
            float y = c_MinTerrainHeight;
            if (null != Terrain.activeTerrain)
            {
                y = Terrain.activeTerrain.SampleHeight(new Vector3(x, c_MinTerrainHeight, z));
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(x, c_MinTerrainHeight * 2, z), Vector3.down, out hit, c_MinTerrainHeight * 2, 1 << LayerMask.NameToLayer("Terrains")))
                {
                    y = hit.point.y;
                }
            }
            return y;
        }
        internal void SetLoadingBarScene(string name)
        {
            m_LoadingBarScene = name;
        }
        internal GameObject GetGameObject(int id)
        {
            GameObject ret = null;
            if (m_GameObjects.Contains(id))
                ret = m_GameObjects[id].ObjectInstance;
            return ret;
        }
        internal SharedGameObjectInfo GetSharedGameObjectInfo(int id)
        {
            SharedGameObjectInfo ret = null;
            if (m_GameObjects.Contains(id))
                ret = m_GameObjects[id].ObjectInfo;
            return ret;
        }
        internal SharedGameObjectInfo GetSharedGameObjectInfo(GameObject obj)
        {
            int id = GetGameObjectId(obj);
            return GetSharedGameObjectInfo(id);
        }
        internal bool ExistGameObject(GameObject obj)
        {
            int id = GetGameObjectId(obj);
            return id > 0;
        }
        internal GameObject PlayerSelf
        {
            get
            {
                if (null != m_PlayerSelf)
                    return m_PlayerSelf.ObjectInstance;
                else
                    return null;
            }
        }
        internal SharedGameObjectInfo PlayerSelfInfo
        {
            get
            {
                if (null != m_PlayerSelf)
                    return m_PlayerSelf.ObjectInfo;
                else
                    return null;
            }
        }
        internal void CallLogicLog(string format, params object[] args)
        {
            QueueLogicActionWithDelegation(m_LogicLogCallback, false, format, args);
        }
        internal void CallLogicErrorLog(string format, params object[] args)
        {
            QueueLogicActionWithDelegation(m_LogicLogCallback, true, format, args);
        }
        internal void CallGfxLog(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            GfxLogImpl(msg);
        }
        internal void CallGfxErrorLog(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            GfxErrorLogImpl(msg);
        }
        internal float RadianToDegree(float dir)
        {
            return (float)(dir * 180 / Math.PI);
        }
        internal bool SceneResourcePrepared
        {
            get { return m_SceneResourcePrepared; }
            set { m_SceneResourcePrepared = value; }
        }
        internal float SceneResourcePreparedProgress
        {
            get { return m_SceneResourcePreparedProgress; }
            set { m_SceneResourcePreparedProgress = value; }
        }
        internal void BeginLoading()
        {
            m_LoadingProgress = 0;
            EventChannelForGfx.Publish("ge_loading_start", "ui");
        }
        internal void EndLoading()
        {
            m_LoadingProgress = 1;
            //延迟处理，在逻辑层逻辑处理之后通知loading条结束，同时也让loading条能走完（视觉效果）。
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueAction(NotifyGfxEndloading);
            }
        }
        internal void UpdateLoadingProgress(float progress)
        {
            m_LoadingProgress = progress;
        }
        internal void UpdateLoadingTip(string tip)
        {
            m_LoadingTip = tip;
        }
        internal void UpdateVersionInfo(string info)
        {
            m_VersionInfo = info;
        }
        internal float GetLoadingProgress()
        {
            return m_LoadingProgress;
        }
        internal string GetLoadingTip()
        {
            return m_LoadingTip;
        }
        internal string GetVersionInfo()
        {
            return m_VersionInfo;
        }
        internal Transform FindChildRecursive(Transform parent, string bonePath)
        {
            Transform t = parent.Find(bonePath);
            if (null != t)
            {
                return t;
            }
            else
            {
                int ct = parent.childCount;
                for (int i = 0; i < ct; ++i)
                {
                    t = FindChildRecursive(parent.GetChild(i), bonePath);
                    if (null != t)
                    {
                        return t;
                    }
                }
            }
            return null;
        }
        internal IActionQueue LogicInvoker
        {
            get { return m_LogicInvoker; }
        }
        internal void QueueLogicActionWithDelegation(Delegate action, params object[] args)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueActionWithDelegation(action, args);
            }
        }
        internal void PublishLogicEvent(string evt, string group, object[] args)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueActionWithDelegation((MyAction<string, string, object[]>)PublishLogicEventImpl, evt, group, args);
            }
        }
        internal void ProxyPublishLogicEvent(string evt, string group, object[] args)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueActionWithDelegation((MyAction<string, string, object[]>)ProxyPublishLogicEventImpl, evt, group, args);
            }
        }
        internal PublishSubscribeSystem EventChannelForGfx
        {
            get { return m_EventChannelForGfx; }
        }
        internal IGameLogicNotification GameLogicNotification
        {
            get { return m_GameLogicNotification; }
        }
        internal BeforeLoadSceneDelegation OnBeforeLoadScene
        {
            get { return m_OnBeforeLoadScene; }
            set { m_OnBeforeLoadScene = value; }
        }
        internal AfterLoadSceneDelegation OnAfterLoadScene
        {
            get { return m_OnAfterLoadScene; }
            set { m_OnAfterLoadScene = value; }
        }
        internal void VisitGameObject(MyAction<GameObject, SharedGameObjectInfo> visitor)
        {
            if (Monitor.TryEnter(m_SyncLock))
            {
                try
                {
                    for (LinkedListNode<GameObjectInfo> node = m_GameObjects.FirstValue; null != node; node = node.Next)
                    {
                        GameObjectInfo info = node.Value;
                        if (null != info && null != info.ObjectInstance)
                        {
                            visitor(info.ObjectInstance, info.ObjectInfo);
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(m_SyncLock);
                }
            }
        }

        private void HandleSync()
        {
            if (Monitor.TryEnter(m_SyncLock))
            {
                try
                {
                    for (LinkedListNode<GameObjectInfo> node = m_GameObjects.FirstValue; null != node; node = node.Next)
                    {
                        GameObjectInfo info = node.Value;
                        if (null != info && null != info.ObjectInstance && null != info.ObjectInfo)
                        {
                            if (info.ObjectInfo.DataChangedByLogic)
                            {
                                Vector3 pos = new Vector3(info.ObjectInfo.x, info.ObjectInfo.y, info.ObjectInfo.z);
                                //if (!info.ObjectInfo.IsFloat && pos.y <= c_MinTerrainHeight)
                                //  pos.y = SampleTerrainHeight(pos.x, pos.z);
                                GameObject obj = info.ObjectInstance;
                                Vector3 old = obj.transform.position;
                                CharacterController ctrl = obj.GetComponent<CharacterController>();
                                if (null != ctrl)
                                {
                                    ctrl.Move(pos - old);
                                }
                                else
                                {
                                    info.ObjectInstance.transform.position = pos;
                                }
                                info.ObjectInstance.transform.rotation = Quaternion.Euler(0, RadianToDegree(info.ObjectInfo.FaceDir), 0);

                                info.ObjectInfo.DataChangedByLogic = false;
                            }
                            else
                            {
                                if (!info.ObjectInfo.IsGfxMoveControl)
                                {
                                    if (info.ObjectInfo.IsLogicMoving)
                                    {
                                        GameObject obj = info.ObjectInstance;
                                        Vector3 old = obj.transform.position;
                                        Vector3 pos;
                                        float distance = info.ObjectInfo.MoveSpeed * Time.deltaTime;
                                        if (distance * distance < info.ObjectInfo.MoveTargetDistanceSqr)
                                        {
                                            float dz = distance * info.ObjectInfo.MoveCos;
                                            float dx = distance * info.ObjectInfo.MoveSin;

                                            if (info.ObjectInfo.CurTime + Time.deltaTime < info.ObjectInfo.TotalTime)
                                            {
                                                info.ObjectInfo.CurTime += Time.deltaTime;
                                                float scale = Time.deltaTime / info.ObjectInfo.TotalTime;
                                                dx += info.ObjectInfo.AdjustDx * scale;
                                                dz += info.ObjectInfo.AdjustDz * scale;
                                            }
                                            else
                                            {
                                                info.ObjectInfo.TotalTime = 0;
                                            }

                                            CharacterController ctrl = obj.GetComponent<CharacterController>();
                                            if (null != ctrl)
                                            {
                                                ctrl.Move(new Vector3(dx, 0, dz));
                                                pos = obj.transform.position;
                                                //if (!info.ObjectInfo.IsFloat && pos.y <= c_MinTerrainHeight) {
                                                //  pos.y = SampleTerrainHeight(pos.x, pos.z);
                                                //  obj.transform.position = pos;
                                                //}
                                                if (info == m_PlayerSelf && ctrl.collisionFlags == CollisionFlags.Sides)
                                                {
                                                    if (null != m_GameLogicNotification && null != m_LogicInvoker)
                                                    {
                                                        m_LogicInvoker.QueueActionWithDelegation((MyAction<int>)m_GameLogicNotification.OnGfxMoveMeetObstacle, info.ObjectInfo.m_LogicObjectId);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                pos = old + new Vector3(dx, 0, dz);
                                                //if (!info.ObjectInfo.IsFloat && pos.y <= c_MinTerrainHeight)
                                                //  pos.y = SampleTerrainHeight(pos.x, pos.z);
                                                //info.ObjectInstance.transform.position = pos;
                                            }

                                            info.ObjectInfo.x = pos.x;
                                            info.ObjectInfo.y = pos.y;
                                            info.ObjectInfo.x = pos.z;
                                            info.ObjectInfo.DataChangedByGfx = true;
                                        }
                                    }
                                    Vector3 nowPos = info.ObjectInstance.transform.position;
                                    float terrainHeight = SampleTerrainHeight(nowPos.x, nowPos.z);
                                    if (!info.ObjectInfo.IsFloat && nowPos.y > terrainHeight)
                                    {
                                        float cur_height = nowPos.y + info.ObjectInfo.VerticlaSpeed * Time.deltaTime - 9.8f * Time.deltaTime * Time.deltaTime / 2;
                                        if (cur_height < terrainHeight)
                                        {
                                            cur_height = terrainHeight;
                                        }
                                        info.ObjectInfo.VerticlaSpeed += -9.8f * Time.deltaTime;
                                        CharacterController cc = info.ObjectInstance.GetComponent<CharacterController>();
                                        if (null != cc)
                                        {
                                            cc.Move(new Vector3(nowPos.x, cur_height, nowPos.z) - nowPos);
                                        }
                                        else
                                        {
                                            info.ObjectInstance.transform.position = new Vector3(nowPos.x, cur_height, nowPos.z);
                                        }
                                        info.ObjectInfo.y = cur_height;
                                        info.ObjectInfo.DataChangedByGfx = true;
                                    }
                                    else
                                    {
                                        info.ObjectInfo.VerticlaSpeed = 0;
                                    }
                                    info.ObjectInstance.transform.rotation = Quaternion.Euler(RadianToDegree(0), RadianToDegree(info.ObjectInfo.FaceDir), RadianToDegree(0));
                                }
                            }
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(m_SyncLock);
                }
            }
        }
        private void HandleLoadingProgress()
        {
            if (GlobalVariables.Instance.IsPublish && m_LoadScenePaused)
            {
                return;
            }
            //先等待loading bar加载完成,发起对目标场景的加载
            if (null != m_LoadingBarAsyncOperation)
            {
                if (m_LoadingBarAsyncOperation.isDone)
                {
                    m_LoadingBarAsyncOperation = null;

                    CallLogicLog("HandleLoadingProgress m_LoadingBarAsyncOperation.isDone");

                    if (null != m_OnBeforeLoadScene)
                    {
                        m_OnBeforeLoadScene(Application.loadedLevelName, m_TargetScene, m_TargetSceneId);
                    }
                    if (GlobalVariables.Instance.IsPublish)
                    {
                        ResUpdateHandler.Cleanup();
                    }
                    ResourceManager.Instance.CleanupResourcePool();
                    if (GlobalVariables.Instance.IsPublish)
                    {
                        ResUpdateHandler.InitUpdate();
                        ResUpdateHandler.SetUpdateProgressRange(0, 1.0f, 1);
                        m_UpdateChapterInfo = ResUpdateHandler.StartUpdateChapter(m_TargetChapter);
                    }
                    else
                    {
                        m_LoadingLevelAsyncOperation = Application.LoadLevelAsync(m_TargetScene);
                        UpdateLoadingTip("加载场景不费流量");
                    }
                }
            }
            else if (m_UpdateChapterInfo != null)
            {
                if (m_UpdateChapterInfo.IsDone)
                {
                    m_UpdateChapterInfo = null;
                    CallLogicLog("HandleLoadingProgress m_UpdateChapterInfo.IsDone");

                    UpdateLoadingProgress(0.0f);
                    UpdateLoadingTip("加载场景不费流量");
                    ResUpdateHandler.SetUpdateProgressRange(0.0f, 0.5f, 1);
                    List<ResCacheConfig> cacheConfigList = new List<ResCacheConfig>();
                    ResCacheConfig levelConfig = new ResCacheConfig(ResCacheType.level, m_TargetSceneId);
                    cacheConfigList.Add(levelConfig);
                    if (m_TargetSceneLimitList != null)
                    {
                        levelConfig.LinkLimitList = m_TargetSceneLimitList;
                    }
                    m_LoadCacheResInfo = ResUpdateHandler.CacheResByConfig(cacheConfigList);
                }
                else if (m_UpdateChapterInfo.IsError)
                {

                    CallLogicLog("HandleLoadingProgress m_UpdateChapterInfo.IsError");

                    ReStartLoad();
                }
            }
            else if (m_LoadCacheResInfo != null)
            {
                if (m_LoadCacheResInfo.IsDone)
                {
                    m_LoadCacheResInfo = null;
                    ResUpdateHandler.ExitUpdate();

                    CallLogicLog("HandleLoadingProgress m_LoadCacheResInfo.IsDone");

                    string levelName = m_TargetScene;
                    if (GlobalVariables.Instance.IsPublish)
                    {
                        levelName = levelName.ToLower();
                    }
                    m_LoadingLevelAsyncOperation = Application.LoadLevelAsync(levelName);
                    //UpdateLoadingTip("加载场景数据...");
                }
                else if (m_LoadCacheResInfo.IsError)
                {

                    CallLogicLog("HandleLoadingProgress m_LoadCacheResInfo.IsError");

                    ReStartLoad();
                }
            }
            else if (null != m_LoadingLevelAsyncOperation)
            {//再等待目标场景加载
                if (m_LoadingLevelAsyncOperation.isDone)
                {
                    if (GlobalVariables.Instance.IsPublish)
                    {
                        ResUpdateHandler.ReleaseAllAssetBundle();
                    }
                    m_LoadingLevelAsyncOperation = null;

                    CallLogicLog("HandleLoadingProgress m_LoadingLevelAsyncOperation.IsDone");

                    if (null != m_LogicInvoker && null != m_LevelLoadedCallback)
                    {
                        QueueLogicActionWithDelegation(m_LevelLoadedCallback);
                        m_LevelLoadedCallback = null;
                    }
                    //UpdateLoadingTip("场景加载完成...");
                    Resources.UnloadUnusedAssets();
                    EndLoading();
                    CallLogicLog("End LoadScene:{0}", m_TargetScene);

                    if (null != m_OnAfterLoadScene)
                    {
                        m_OnAfterLoadScene(m_TargetScene, m_TargetSceneId);
                    }
                }
                else
                {
                    UpdateLoadingProgress(0.5f + m_LoadingLevelAsyncOperation.progress * 0.5f);
                }
            }

        }
        private void ReStartLoad()
        {
            ResUpdateHandler.IncReconnectNum();
            m_LoadScenePaused = true;
            string info = "网络连接错误,请重试连接";
            Action<bool> fun = new Action<bool>(delegate (bool selected)
            {
                if (selected)
                {
                    m_LoadScenePaused = false;
                    m_LoadCacheResInfo = null;
                    m_UpdateChapterInfo = null;
                    m_LoadingBarAsyncOperation = null;
                    m_LoadingLevelAsyncOperation = null;
                    ResUpdateHandler.ExitUpdate();
                    LoadSceneImpl(m_TargetScene, m_TargetChapter, m_TargetSceneId, m_TargetSceneLimitList, m_LevelLoadedCallback);
                }
            });
            StarWars.LogicSystem.EventChannelForGfx.Publish("ge_show_yesornot", "ui", info, fun);
        }
        private GameObjectInfo GetGameObjectInfo(int id)
        {
            GameObjectInfo ret = null;
            if (m_GameObjects.Contains(id))
                ret = m_GameObjects[id];
            return ret;
        }
        private int GetGameObjectId(GameObject obj)
        {
            int ret = 0;
            if (m_GameObjectIds.ContainsKey(obj))
            {
                ret = m_GameObjectIds[obj];
            }
            return ret;
        }
        private void RememberGameObject(int id, GameObject obj)
        {
            RememberGameObject(id, obj, null);
        }
        private void RememberGameObject(int id, GameObject obj, SharedGameObjectInfo info)
        {
            if (m_GameObjects.Contains(id))
            {
                GameObject oldObj = m_GameObjects[id].ObjectInstance;
                oldObj.SetActive(false);
                m_GameObjectIds.Remove(oldObj);
                GameObject.Destroy(oldObj);
                m_GameObjects[id] = new GameObjectInfo(obj, info);
            }
            else
            {
                m_GameObjects.AddLast(id, new GameObjectInfo(obj, info));
            }
            if (null != info)
            {
                if (!info.m_SkinedMaterialChanged)
                {
                    SkinnedMeshRenderer[] renderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (SkinnedMeshRenderer renderer in renderers)
                    {
                        info.m_SkinedOriginalMaterials.Add(renderer.materials);
                    }
                }
                if (!info.m_MeshMaterialChanged)
                {
                    MeshRenderer[] renderers = obj.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer renderer in renderers)
                    {
                        info.m_MeshOriginalMaterials.Add(renderer.materials);
                    }
                }
            }
            m_GameObjectIds.Add(obj, id);
        }
        private void ForgetGameObject(int id, GameObject obj)
        {
            SharedGameObjectInfo info = GetSharedGameObjectInfo(id);
            if (null != info)
            {
                RestoreMaterialImpl(id);
                info.m_SkinedOriginalMaterials.Clear();
                info.m_MeshOriginalMaterials.Clear();
            }
            m_GameObjects.Remove(id);
            m_GameObjectIds.Remove(obj);
        }
        private void CreateGameObjectWithMeshDataHelper(int id, List<float> vertices, List<float> uvs, List<int> triangles, Material mat, bool attachTerrain)
        {
            GameObject obj = new GameObject();
            MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
            MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
            Mesh mesh = new Mesh();

            Vector3[] _vertices = new Vector3[vertices.Count / 3];
            for (int i = 0; i < _vertices.Length; ++i)
            {
                float x = vertices[i * 3];
                float y = vertices[i * 3 + 1];
                float z = vertices[i * 3 + 2];
                if (attachTerrain)
                    y = SampleTerrainHeight(x, z) + 0.01f;
                _vertices[i] = new Vector3(x, y, z);
            }
            Vector2[] _uvs = new Vector2[uvs.Count / 2];
            for (int i = 0; i < _uvs.Length; ++i)
            {
                float u = uvs[i * 2];
                float v = uvs[i * 2 + 1];
                _uvs[i] = new Vector2(u, v);
            }

            mesh.vertices = _vertices;
            mesh.uv = _uvs;
            mesh.triangles = triangles.ToArray();
            mesh.Optimize();

            meshFilter.mesh = mesh;
            renderer.material = mat;

            RememberGameObject(id, obj);
            obj.SetActive(true);
        }

        private void NotifyGfxEndloading()
        {
            GfxSystem.PublishGfxEvent("ge_loading_finish", "ui");
        }

        private GfxSystem() { }

        private object m_SyncLock = new object();
        private LinkedListDictionary<int, GameObjectInfo> m_GameObjects = new LinkedListDictionary<int, GameObjectInfo>();
        private Dictionary<GameObject, int> m_GameObjectIds = new Dictionary<GameObject, int>();
        private MyAction<bool, string, object[]> m_LogicLogCallback;

        private IActionQueue m_LogicInvoker;
        private AsyncActionProcessor m_GfxInvoker = new AsyncActionProcessor();

        private PublishSubscribeSystem m_EventChannelForLogic = new PublishSubscribeSystem();
        private PublishSubscribeSystem m_EventChannelForGfx = new PublishSubscribeSystem();

        private bool m_SceneResourcePrepared = false;
        private bool m_SceneResourceStartPrepare = false;
        private float m_SceneResourcePreparedProgress = 0;
        private ResAsyncInfo m_LoadCacheResInfo = null;
        private ResAsyncInfo m_UpdateChapterInfo = null;
        private bool m_LoadScenePaused = false;
        private UnityEngine.AsyncOperation m_LoadingBarAsyncOperation = null;
        private UnityEngine.AsyncOperation m_LoadingLevelAsyncOperation = null;
        private MyAction m_LevelLoadedCallback = null;

        private IGameLogicNotification m_GameLogicNotification = null;
        private GameObjectInfo m_PlayerSelf = null;

        private BeforeLoadSceneDelegation m_OnBeforeLoadScene;
        private AfterLoadSceneDelegation m_OnAfterLoadScene;

        private string m_LoadingBarScene = "";
        private int m_TargetSceneId = 0;
        private HashSet<int> m_TargetSceneLimitList = null;
        private string m_TargetScene = "";
        private int m_TargetChapter = 0;
        private float m_LoadingProgress = 0;
        private string m_LoadingTip = "";
        private string m_VersionInfo = "";

        private long m_LastLogTime = 0;

        private float c_MinTerrainHeight = 120.0f;
    }
}
