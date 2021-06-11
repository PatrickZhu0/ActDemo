using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWars
{
    public sealed partial class GfxSystem
    {
        //引擎线程调用的方法，不要在逻辑线程调用
        public static void Init()
        {
            s_Instance.InitImpl();
        }
        public static void Release()
        {
            s_Instance.ReleaseImpl();
        }
        public static void Tick()
        {
            s_Instance.TickImpl();
        }
        //注册异步处理接口（这个是渲染线程向逻辑线程返回信息的底层机制：逻辑线程向渲染线程注册处理，渲染线程完成请求后将此处理发回逻辑线程执行）
        public static void SetLogicInvoker(IActionQueue processor)
        {
            s_Instance.SetLogicInvokerImpl(processor);
        }
        public static void SetLogicLogCallback(MyAction<bool, string, object[]> callback)
        {
            s_Instance.SetLogicLogCallbackImpl(callback);
        }
        public static void SetGameLogicNotification(IGameLogicNotification notification)
        {
            s_Instance.SetGameLogicNotificationImpl(notification);
        }
        //输入状态，允许跨线程读取
        public static float GetMouseX()
        {
            return s_Instance.GetMouseXImpl();
        }
        public static float GetMouseY()
        {
            return s_Instance.GetMouseYImpl();
        }
        public static float GetMouseZ()
        {
            return s_Instance.GetMouseZImpl();
        }
        public static float GetMouseRayPointX()
        {
            return s_Instance.GetMouseRayPointXImpl();
        }
        public static float GetMouseRayPointY()
        {
            return s_Instance.GetMouseRayPointYImpl();
        }
        public static float GetMouseRayPointZ()
        {
            return s_Instance.GetMouseRayPointZImpl();
        }
        public static bool IsTouchPosChanged()
        {
            return s_Instance.IsTouchPosChangedImpl();
        }
        public static float GetTouchPointX()
        {
            return s_Instance.GetTouchXImpl();
        }
        public static float GetTouchPointY()
        {
            return s_Instance.GetTouchYImpl();
        }
        public static float GetTouchPointZ()
        {
            return s_Instance.GetTouchZImpl();
        }
        public static float GetTouchRayPointX()
        {
            return s_Instance.GetTouchRayPointXImpl();
        }
        public static float GetTouchRayPointY()
        {
            return s_Instance.GetTouchRayPointYImpl();
        }
        public static float GetTouchRayPointZ()
        {
            return s_Instance.GetTouchRayPointZImpl();
        }
        public static float GetJoystickDir()
        {
            return s_Instance.GetJoystickDirImpl();
        }
        public static float GetJoystickTargetPosX()
        {
            return s_Instance.GetJoystickTargetPosXImpl();
        }
        public static float GetJoystickTargetPosY()
        {
            return s_Instance.GetJoystickTargetPosYImpl();
        }
        public static float GetJoystickTargetPosZ()
        {
            return s_Instance.GetJoystickTargetPosZImpl();
        }
        public static bool IsButtonPressed(Mouse.Code c)
        {
            return s_Instance.IsButtonPressedImpl(c);
        }
        public static bool IsKeyPressed(Keyboard.Code c)
        {
            return s_Instance.IsKeyPressedImpl(c);
        }
        //指出需要查询状态与处理事件的键列表（仅在初始化时调用【第一次tick前】，一般是初始化时，可多次设置不同的键）
        public static void ListenKeyPressState(params Keyboard.Code[] keys)
        {
            s_Instance.ListenKeyPressStateImpl(keys);
        }
        //事件注册,事件处理会通过IActionQueue在游戏逻辑线程处理（仅在初始化时调用【第一次tick前】)
        public static void ListenKeyboardEvent(Keyboard.Code c, MyAction<int, int> handler)
        {
            s_Instance.ListenKeyboardEventImpl(c, handler);
        }
        public static void ListenMouseEvent(Mouse.Code c, MyAction<int, int> handler)
        {
            s_Instance.ListenMouseEventImpl(c, handler);
        }
        public static void ListenTouchEvent(TouchEvent c, MyAction<int, GestureArgs> handler)
        {
            s_Instance.ListenTouchEventImpl(c, handler);
        }
        public static void ResetInputState()
        {
            QueueGfxAction(s_Instance.ResetInputStateImpl);
        }
        //供逻辑线程调用的异步命令
        public static void LoadScene(string sceneName, int chapter, int sceneId)
        {
            LoadScene(sceneName, chapter, sceneId, null, null);
        }
        public static void LoadScene(string sceneName, int chapter, int sceneId, HashSet<int> limitList, MyAction onFinish)
        {
            QueueGfxAction(s_Instance.LoadSceneImpl, sceneName, chapter, sceneId, limitList, onFinish);
        }
        public static void MarkPlayerSelf(int id)
        {
            QueueGfxAction(s_Instance.MarkPlayerSelfImpl, id);
        }
        public static void CreateGameObject(int id, string resource, SharedGameObjectInfo info)
        {
            QueueGfxAction(s_Instance.CreateGameObjectImpl, id, resource, info);
        }
        public static void CreateGameObject(int id, string resource, float x, float y, float z, float rx, float ry, float rz, bool attachTerrain)
        {
            QueueGfxAction(s_Instance.CreateGameObjectImpl, id, resource, x, y, z, rx, ry, rz, attachTerrain);
        }
        public static void CreateGameObjectWithMeshData(int id, List<float> vertices, List<int> triangles, uint color, string mat, bool attachTerrain)
        {
            QueueGfxAction(s_Instance.CreateGameObjectWithMeshDataImpl, id, vertices, triangles, color, mat, attachTerrain);
        }
        public static void CreateGameObjectWithMeshData(int id, List<float> vertices, List<float> uvs, List<int> triangles, uint color, string mat, bool attachTerrain)
        {
            QueueGfxAction(s_Instance.CreateGameObjectWithMeshDataImpl, id, vertices, uvs, triangles, color, mat, attachTerrain);
        }
        public static void CreateGameObjectWithMeshData(int id, List<float> vertices, List<int> triangles, string matRes, bool attachTerrain)
        {
            QueueGfxAction(s_Instance.CreateGameObjectWithMeshDataImpl, id, vertices, triangles, matRes, attachTerrain);
        }
        public static void CreateGameObjectWithMeshData(int id, List<float> vertices, List<float> uvs, List<int> triangles, string matRes, bool attachTerrain)
        {
            QueueGfxAction(s_Instance.CreateGameObjectWithMeshDataImpl, id, vertices, uvs, triangles, matRes, attachTerrain);
        }
        public static void CreateAndAttachGameObject(string resource, int parentId, string path, float recycleTime = -1)
        {
            QueueGfxAction(s_Instance.CreateAndAttachGameObjectImpl, resource, parentId, path, recycleTime);
        }
        public static void CreateGameObjectForAttach(int id, string resource)
        {
            QueueGfxAction(s_Instance.CreateGameObjectForAttachImpl, id, resource);
        }
        public static void DestroyGameObject(int id)
        {
            QueueGfxAction(s_Instance.DestroyGameObjectImpl, id);
        }
        public static object SyncLock
        {
            get
            {
                return s_Instance.m_SyncLock;
            }
        }
        public static void UpdateGameObjectLocalPosition(int id, float x, float y, float z)
        {
            QueueGfxAction(s_Instance.UpdateGameObjectLocalPositionImpl, id, x, y, z);
        }
        public static void UpdateGameObjectLocalPosition2D(int id, float x, float z)
        {
            UpdateGameObjectLocalPosition2D(id, x, z, true);
        }
        public static void UpdateGameObjectLocalPosition2D(int id, float x, float z, bool attachTerrain)
        {
            QueueGfxAction(s_Instance.UpdateGameObjectLocalPosition2DImpl, id, x, z, attachTerrain);
        }
        public static void UpdateGameObjectLocalRotate(int id, float rx, float ry, float rz)
        {
            QueueGfxAction(s_Instance.UpdateGameObjectLocalRotateImpl, id, rx, ry, rz);
        }
        public static void UpdateGameObjectLocalRotateY(int id, float ry)
        {
            QueueGfxAction(s_Instance.UpdateGameObjectLocalRotateYImpl, id, ry);
        }
        public static void UpdateGameObjectLocalScale(int id, float sx, float sy, float sz)
        {
            QueueGfxAction(s_Instance.UpdateGameObjectLocalScaleImpl, id, sx, sy, sz);
        }
        public static void AttachGameObject(int id, int parentId)
        {
            AttachGameObject(id, parentId, 0, 0, 0, 0, 0, 0);
        }
        public static void AttachGameObject(int id, int parentId, float x, float y, float z, float rx, float ry, float rz)
        {
            QueueGfxAction(s_Instance.AttachGameObjectImpl, id, parentId, x, y, z, rx, ry, rz);
        }
        public static void AttachGameObject(int id, int parentId, string path)
        {
            AttachGameObject(id, parentId, path, 0, 0, 0, 0, 0, 0);
        }
        public static void AttachGameObject(int id, int parentId, string path, float x, float y, float z, float rx, float ry, float rz)
        {
            QueueGfxAction(s_Instance.AttachGameObjectImpl, id, parentId, path, x, y, z, rx, ry, rz);
        }
        public static void DetachGameObject(int id)
        {
            QueueGfxAction(s_Instance.DetachGameObjectImpl, id);
        }
        public static void SetGameObjectVisible(int id, bool visible)
        {
            QueueGfxAction(s_Instance.SetGameObjectVisibleImpl, id, visible);
        }
        public static void PlayAnimation(int id)
        {
            PlayAnimation(id, false);
        }
        public static void PlayAnimation(int id, bool isStopAll)
        {
            QueueGfxAction(s_Instance.PlayAnimationImpl, id, isStopAll);
        }
        public static void PlayAnimation(int id, string animationName)
        {
            PlayAnimation(id, animationName, false);
        }
        public static void PlayAnimation(int id, string animationName, bool isStopAll)
        {
            QueueGfxAction(s_Instance.PlayAnimationImpl, id, animationName, isStopAll);
        }
        public static void StopAnimation(int id, string animationName)
        {
            QueueGfxAction(s_Instance.StopAnimationImpl, id, animationName);
        }
        public static void StopAnimation(int id)
        {
            QueueGfxAction(s_Instance.StopAnimationImpl, id);
        }
        public static void BlendAnimation(int id, string animationName)
        {
            BlendAnimation(id, animationName, 1, 0.3f);
        }
        public static void BlendAnimation(int id, string animationName, float weight)
        {
            BlendAnimation(id, animationName, weight, 0.3f);
        }
        public static void BlendAnimation(int id, string animationName, float weight, float fadeLength)
        {
            QueueGfxAction(s_Instance.BlendAnimationImpl, id, animationName, weight, fadeLength);
        }
        public static void CrossFadeAnimation(int id, string animationName)
        {
            CrossFadeAnimation(id, animationName, 0.3f, false);
        }
        public static void CrossFadeAnimation(int id, string animationName, float fadeLength)
        {
            CrossFadeAnimation(id, animationName, fadeLength, false);
        }
        public static void CrossFadeAnimation(int id, string animationName, float fadeLength, bool isStopAll)
        {
            QueueGfxAction(s_Instance.CrossFadeAnimationImpl, id, animationName, fadeLength, isStopAll);
        }
        public static void PlayQueuedAnimation(int id, string animationName)
        {
            PlayQueuedAnimation(id, animationName, false, false);
        }
        public static void PlayQueuedAnimation(int id, string animationName, bool isPlayNow)
        {
            PlayQueuedAnimation(id, animationName, isPlayNow, false);
        }
        public static void PlayQueuedAnimation(int id, string animationName, bool isPlayNow, bool isStopAll)
        {
            QueueGfxAction(s_Instance.PlayQueuedAnimationImpl, id, animationName, isPlayNow, isStopAll);
        }
        public static void CrossFadeQueuedAnimation(int id, string animationName)
        {
            CrossFadeQueuedAnimation(id, animationName, 0.3f, false, false);
        }
        public static void CrossFadeQueuedAnimation(int id, string animationName, float fadeLength)
        {
            CrossFadeQueuedAnimation(id, animationName, fadeLength, false, false);
        }
        public static void CrossFadeQueuedAnimation(int id, string animationName, float fadeLength, bool isPlayNow)
        {
            CrossFadeQueuedAnimation(id, animationName, fadeLength, isPlayNow, false);
        }
        public static void CrossFadeQueuedAnimation(int id, string animationName, float fadeLength, bool isPlayNow, bool isStopAll)
        {
            QueueGfxAction(s_Instance.CrossFadeQueuedAnimationImpl, id, animationName, fadeLength, isPlayNow, isStopAll);
        }
        public static void RewindAnimation(int id, string animationName)
        {
            QueueGfxAction(s_Instance.RewindAnimationImpl, id, animationName);
        }
        public static void RewindAnimation(int id)
        {
            QueueGfxAction(s_Instance.RewindAnimationImpl, id);
        }
        public static void SetAnimationSpeed(int id, string animationName, float speed)
        {
            QueueGfxAction(s_Instance.SetAnimationSpeedImpl, id, animationName, speed);
        }
        public static void SetAnimationSpeedByTime(int id, string animationName, float time)
        {
            QueueGfxAction(s_Instance.SetAnimationSpeedByTimeImpl, id, animationName, time);
        }
        public static void SetAnimationWeight(int id, string animationName, float weight)
        {
            QueueGfxAction(s_Instance.SetAnimationWeightImpl, id, animationName, weight);
        }
        public static void SetAnimationLayer(int id, string animationName, int layer)
        {
            QueueGfxAction(s_Instance.SetAnimationLayerImpl, id, animationName, layer);
        }
        public static void SetAnimationBlendMode(int id, string animationName, int blendMode)
        {
            QueueGfxAction(s_Instance.SetAnimationBlendModeImpl, id, animationName, blendMode);
        }
        public static void AddMixingTransformAnimation(int id, string animationName, string path)
        {
            AddMixingTransformAnimation(id, animationName, path, true);
        }
        public static void AddMixingTransformAnimation(int id, string animationName, string path, bool recursive)
        {
            QueueGfxAction(s_Instance.AddMixingTransformAnimationImpl, id, animationName, path, recursive);
        }
        public static void RemoveMixingTransformAnimation(int id, string animationName, string path)
        {
            QueueGfxAction(s_Instance.RemoveMixingTransformAnimationImpl, id, animationName, path);
        }
        // sound
        public static void PlaySound(int id, string audio_source_obj_name, float pitch)
        {
            QueueGfxAction(s_Instance.PlaySoundImpl, id, audio_source_obj_name, pitch);
        }
        public static void SetSoundPitch(int id, string audio_source_obj_name, float pitch)
        {
            QueueGfxAction(s_Instance.SetAudioSourcePitchImpl, id, audio_source_obj_name, pitch);
        }
        public static void StopSound(int id, string audio_source_obj_name)
        {
            QueueGfxAction(s_Instance.StopSoundImpl, id, audio_source_obj_name);
        }
        public static void SetShader(int id, string shaderPath)
        {
            QueueGfxAction(s_Instance.SetShaderImpl, id, shaderPath);
        }
        public static void SetBlockedShader(int id, uint rimColor, float rimPower, float cutValue)
        {
            QueueGfxAction(s_Instance.SetBlockedShaderImpl, id, rimColor, rimPower, cutValue);
        }
        public static void RestoreMaterial(int id)
        {
            QueueGfxAction(s_Instance.RestoreMaterialImpl, id);
        }
        public static void SetTimeScale(float scale)
        {
            QueueGfxAction(s_Instance.SetTimeScaleImpl, scale);
        }
        //日志
        public static void GfxLog(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            QueueGfxAction(s_Instance.GfxLogImpl, msg);
        }
        public static void GfxErrorLog(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            QueueGfxAction(s_Instance.GfxErrorLogImpl, msg);
        }
        //逻辑层与unity3d脚本交互函数
        public static void QueueGfxAction(MyAction action)
        {
            QueueGfxActionWithDelegation(action);
        }
        public static void QueueGfxAction<T1>(MyAction<T1> action, T1 t1)
        {
            QueueGfxActionWithDelegation(action, t1);
        }
        public static void QueueGfxAction<T1, T2>(MyAction<T1, T2> action, T1 t1, T2 t2)
        {
            QueueGfxActionWithDelegation(action, t1, t2);
        }
        public static void QueueGfxAction<T1, T2, T3>(MyAction<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3);
        }
        public static void QueueGfxAction<T1, T2, T3, T4>(MyAction<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5>(MyAction<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6>(MyAction<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7>(MyAction<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
        }
        public static void QueueGfxAction<R>(MyFunc<R> action)
        {
            QueueGfxActionWithDelegation(action);
        }
        public static void QueueGfxAction<T1, R>(MyFunc<T1, R> action, T1 t1)
        {
            QueueGfxActionWithDelegation(action, t1);
        }
        public static void QueueGfxAction<T1, T2, R>(MyFunc<T1, T2, R> action, T1 t1, T2 t2)
        {
            QueueGfxActionWithDelegation(action, t1, t2);
        }
        public static void QueueGfxAction<T1, T2, T3, R>(MyFunc<T1, T2, T3, R> action, T1 t1, T2 t2, T3 t3)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, R>(MyFunc<T1, T2, T3, T4, R> action, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, R>(MyFunc<T1, T2, T3, T4, T5, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, R>(MyFunc<T1, T2, T3, T4, T5, T6, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
        }
        public static void QueueGfxAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16)
        {
            QueueGfxActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
        }
        public static void QueueGfxActionWithDelegation(Delegate action, params object[] args)
        {
            if (null != s_Instance.m_GfxInvoker)
            {
                s_Instance.m_GfxInvoker.QueueActionWithDelegation(action, args);
            }
        }
        public static void PublishGfxEvent(string evt, string group, params object[] args)
        {
            QueueGfxAction(s_Instance.PublishGfxEventImpl, evt, group, args);
        }
        public static void ProxyPublishGfxEvent(string evt, string group, params object[] args)
        {
            QueueGfxAction(s_Instance.ProxyPublishGfxEventImpl, evt, group, args);
        }
        public static PublishSubscribeSystem EventChannelForLogic
        {
            get { return s_Instance.m_EventChannelForLogic; }
        }
        public static void SendMessage(string objname, string msg, object arg)
        {
            SendMessage(objname, msg, arg, false);
        }
        public static void SendMessage(string objname, string msg, object arg, bool needReceiver)
        {
            QueueGfxAction(s_Instance.SendMessageImpl, objname, msg, arg, needReceiver);
        }
        public static void SendMessage(int objid, string msg, object arg)
        {
            SendMessage(objid, msg, arg, false);
        }
        public static void SendMessage(int objid, string msg, object arg, bool needReceiver)
        {
            QueueGfxAction(s_Instance.SendMessageByIdImpl, objid, msg, arg, needReceiver);
        }
        public static void SendMessageWithTag(string objtag, string msg, object arg)
        {
            SendMessageWithTag(objtag, msg, arg, false);
        }
        public static void SendMessageWithTag(string objtag, string msg, object arg, bool needReceiver)
        {
            QueueGfxAction(s_Instance.SendMessageWithTagImpl, objtag, msg, arg, needReceiver);
        }

        internal static GfxSystem Instance
        {
            get
            {
                return s_Instance;
            }
        }
        private static GfxSystem s_Instance = new GfxSystem();
    }
}
