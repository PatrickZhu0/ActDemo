
/**
 * @file GameSystem.cs
 * @brief 游戏系统
 *          负责：
 *                  切换场景
 *                  预加载资源
 *
 * @author 
 * @version 1.0.0
 * @date 2012-12-16
 */

using System;
using System.Collections.Generic;
using System.Text;
//using System.Diagnostics;
using StarWarsSpatial;
using UnityEngine;
//using DashFireMessage;
//using DashFire.Network;
//using ScriptRuntime;

namespace StarWars
{
    public class WorldSystemProfiler
    {
        public long sceneTickTime;
        public long entityMgrTickTime;
        public long controlSystemTickTime;
        public long movementSystemTickTime;
        public long spatialSystemTickTime;
        public long aiSystemTickTime;
        public long sceneLogicSystemTickTime;
        public long storySystemTickTime;
        public long usersTickTime;
        public long npcsTickTime;
        public long combatSystemTickTime;

        public string GenerateLogString(long tickTime)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("WorldSystem.Tick consume ").Append(tickTime).AppendLine();

            builder.Append("=>sceneTickTime:").Append(sceneTickTime).AppendLine();
            builder.Append("=>entityMgrTickTime:").Append(entityMgrTickTime).AppendLine();
            builder.Append("=>controlSystemTickTime:").Append(controlSystemTickTime).AppendLine();
            builder.Append("=>movementSystemTickTime:").Append(movementSystemTickTime).AppendLine();
            builder.Append("=>spatialSystemTickTime:").Append(spatialSystemTickTime).AppendLine();
            builder.Append("=>aiSystemTickTime:").Append(aiSystemTickTime).AppendLine();
            builder.Append("=>sceneLogicSystemTickTime:").Append(sceneLogicSystemTickTime).AppendLine();
            builder.Append("=>storySystemTickTime:").Append(storySystemTickTime).AppendLine();
            builder.Append("=>usersTickTime:").Append(usersTickTime).AppendLine();
            builder.Append("=>npcsTickTime:").Append(npcsTickTime).AppendLine();
            builder.Append("=>combatSystemTickTime:").Append(combatSystemTickTime).AppendLine();

            return builder.ToString();
        }
    }
    /**
     * @brief 游戏系统
     */
    public class WorldSystem
    {

        #region Singleton
        private static WorldSystem s_Instance = new WorldSystem();
        public static WorldSystem Instance
        {
            get { return s_Instance; }
        }

        #endregion

        //---------------------------------------------------------
        // 标准接口
        //---------------------------------------------------------

        /**
         * @brief 初始化
         *
         * @return 
         */
        public void Init()
        {
            m_IsObserver = false;
            m_CurScene = null;

            GfxSystem.EventChannelForLogic.Subscribe("ge_change_hero", "game", ChangeHeroFromGfx);
            GfxSystem.EventChannelForLogic.Subscribe<int>("ge_change_player_movemode", "game", ChangePlayerMoveMode);
            GfxSystem.EventChannelForLogic.Subscribe<int, int>("ge_change_npc_movemode", "game", ChangeNpcMoveMode);
            GfxSystem.EventChannelForLogic.Subscribe<int>("ge_change_scene", "game", ChangeSceneFromGfx);
            GfxSystem.EventChannelForLogic.Subscribe("ge_resetdsl", "game", ResetDsl);
            GfxSystem.EventChannelForLogic.Subscribe<string>("ge_execscript", "game", ExecScript);
            GfxSystem.EventChannelForLogic.Subscribe<string>("ge_execcommand", "game", ExecCommand);

            NpcManager.OnDamage = new DamageDelegation(NpcManager_OnDamage);
            UserManager.OnDamage = new DamageDelegation(UserManager_OnDamage);
            UserManager.OnGainMoney = new GainMoneyDelegation(UserManager_GainMoney);

            ResUpdateHandler.HandleGetPlayerCurSkillInfo = GetPlayerInfoForCache;
        }

        private void UserManager_OnDamage(int receiver, int caster, bool isOrdinaryDamage, bool isCritical, int hpDamage, int npDamage)
        {
            if (IsPvpScene())
            {
                if (caster == PlayerSelfId || receiver == PlayerSelfId)
                {
                    UserInfo charObj = WorldSystem.Instance.GetCharacterById(receiver) as UserInfo;
                    if (null != charObj)
                    {
                        Vector3 pos = charObj.GetMovementStateInfo().GetPosition3D();
                        if (hpDamage != 0)
                        {
                            if (isCritical)
                            {
                                GfxSystem.PublishGfxEvent("ge_npc_cdamage", "ui", pos.X, pos.Y, pos.Z, hpDamage, isOrdinaryDamage);
                            }
                            else
                            {
                                GfxSystem.PublishGfxEvent("ge_npc_odamage", "ui", pos.X, pos.Y, pos.Z, hpDamage, isOrdinaryDamage);
                            }
                        }
                    }
                }
            }
            else
            {
                if (receiver == PlayerSelfId || caster == PlayerSelfId)
                {
                    UserInfo charObj = WorldSystem.Instance.GetCharacterById(receiver) as UserInfo;
                    if (null != charObj)
                    {
                        Vector3 pos = charObj.GetMovementStateInfo().GetPosition3D();
                        if (hpDamage != 0)
                        {
                            GfxSystem.PublishGfxEvent("ge_hero_blood", "ui", pos.X, pos.Y, pos.Z, hpDamage);
                        }
                        if (npDamage != 0)
                        {
                            GfxSystem.PublishGfxEvent("ge_hero_energy", "ui", pos.X, pos.Y, pos.Z, npDamage);
                        }
                    }
                }
            }
        }
        private void NpcManager_OnDamage(int receiver, int caster, bool /*isShootDamage*/isOrdinaryDamage, bool isCritical, int hpDamage, int npDamage)
        {
            if (caster == PlayerSelfId)
            {
                NpcInfo charObj = NpcManager.GetNpcInfo(receiver);

                if (IsPveScene() || IsPureClientScene())
                {
                    int estimateDamage = hpDamage;
                    if (isCritical)
                        estimateDamage /= 2;

                    if (charObj.Hp + estimateDamage <= 0)
                    {
                        if (GetBattleNpcCount() == 1)
                        {
                            ClientStorySystem.Instance.SendMessage("finalblow", receiver);
                        }
                    }
                }

                if (null != charObj && (int)NpcTypeEnum.SceneObject != charObj.NpcType)
                {
                    Vector3 pos = charObj.GetMovementStateInfo().GetPosition3D();
                    if (isCritical)
                    {
                        GfxSystem.PublishGfxEvent("ge_npc_cdamage", "ui", pos.X, pos.Y, pos.Z, hpDamage, isOrdinaryDamage);
                    }
                    else
                    {
                        GfxSystem.PublishGfxEvent("ge_npc_odamage", "ui", pos.X, pos.Y, pos.Z, hpDamage, isOrdinaryDamage);
                    }
                    GfxSystem.PublishGfxEvent("ge_small_monster_healthbar", "ui", charObj.Hp, charObj.GetActualProperty().HpMax, hpDamage);
                }
            }
        }

        private void UserManager_GainMoney(int receiver, int money)
        {
            UserInfo charObj = WorldSystem.Instance.GetCharacterById(receiver) as UserInfo;
            if (null != charObj)
            {
                Vector3 pos = charObj.GetMovementStateInfo().GetPosition3D();
                GfxSystem.PublishGfxEvent("ge_gain_money", "ui", pos.X, pos.Y, pos.Z, money);
            }
        }

        private void ChangeHeroFromGfx()
        {
            try
            {
                ChangeHero();
            }
            catch (Exception ex)
            {
                LogSystem.Error("ExecCommand exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private void ChangePlayerMoveMode(int mode)
        {
            UserInfo player = WorldSystem.Instance.GetPlayerSelf();
            if (null != player)
            {
                if ((int)MovementMode.HighSpeed == mode)
                {
                    player.GetActualProperty().SetMoveSpeed(Operate_Type.OT_Absolute, player.GetBaseProperty().RunSpeed);
                    player.GetMovementStateInfo().MovementMode = MovementMode.Normal;
                }
                else if ((int)MovementMode.LowSpeed == mode)
                {
                    player.GetActualProperty().SetMoveSpeed(Operate_Type.OT_Absolute, player.GetBaseProperty().WalkSpeed);
                    player.GetMovementStateInfo().MovementMode = MovementMode.LowSpeed;
                }
            }
        }
        private void ChangeNpcMoveMode(int objId, int mode)
        {
            CharacterInfo character = WorldSystem.Instance.GetCharacterById(objId);
            if (null != character)
            {
                if ((int)MovementMode.HighSpeed == mode)
                {
                    character.GetActualProperty().SetMoveSpeed(Operate_Type.OT_Absolute, character.GetBaseProperty().RunSpeed);
                    character.GetMovementStateInfo().MovementMode = MovementMode.Normal;
                }
                else if ((int)MovementMode.LowSpeed == mode)
                {
                    character.GetActualProperty().SetMoveSpeed(Operate_Type.OT_Absolute, character.GetBaseProperty().WalkSpeed);
                    character.GetMovementStateInfo().MovementMode = MovementMode.LowSpeed;
                }
            }
        }

        private void ChangeSceneFromGfx(int sceneId)
        {
            try
            {
                if (null == m_CurScene || m_CurScene.IsSuccessEnter)
                {
                    if (0 == sceneId)
                    {
                        if (IsPvpScene() || IsMultiPveScene())
                        {
                            NetworkSystem.Instance.QuitBattle();
                        }
                        LobbyNetworkSystem.Instance.QuitClient();
                    }
                    ChangeScene(sceneId);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("ExecCommand exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private void ResetDsl()
        {
            try
            {
                StorySystem.StoryConfigManager.Instance.Clear();
                ClientStorySystem.Instance.ClearStoryInstancePool();
            }
            catch (Exception ex)
            {
                LogSystem.Error("Exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private void ExecScript(string scriptFile)
        {
            try
            {
                GmCommands.ClientGmStorySystem.Instance.LoadStory(scriptFile);
                GmCommands.ClientGmStorySystem.Instance.StartStory(1);
            }
            catch (Exception ex)
            {
                LogSystem.Error("ExecScript exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private void ExecCommand(string cmd)
        {
            try
            {
                GmCommands.ClientGmStorySystem.Instance.LoadStoryText("story(1){onmessage(\"start\"){" + cmd + "}}");
                GmCommands.ClientGmStorySystem.Instance.StartStory(1);
            }
            catch (Exception ex)
            {
                LogSystem.Error("ExecCommand exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        public void OnRoomServerDisconnected()
        {
            GfxSystem.PublishGfxEvent("ge_ui_connect_hint", "ui", false, true);
        }

        public void OnRoomServerConnected()
        {
            GfxSystem.PublishGfxEvent("ge_ui_connect_hint", "ui", false, false);
        }

        /**
         * @brief 释放
         *
         * @return 
         */
        public void Release()
        {
            if (null != m_CurScene)
            {
                m_CurScene.Release();
            }
        }

        public void QuitBattle()
        {
            OnRoomServerConnected();
            NetworkSystem.Instance.QuitBattle();
        }

        public void QuitClient()
        {
            NetworkSystem.Instance.QuitClient();
            LobbyNetworkSystem.Instance.QuitClient();
        }

        public void ReturnToLogin()
        {
            try
            {
                NetworkSystem.Instance.QuitBattle();
                LobbyNetworkSystem.Instance.QuitClient();
                GfxSystem.PublishGfxEvent("ge_return_login", "ui");
            }
            catch (Exception ex)
            {
                LogSystem.Error("ReturnToLogin exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        public bool IsPureClientScene()
        {
            if (null == m_CurScene)
                return true;
            else
                return m_CurScene.IsPureClientScene;
        }

        public bool IsPveScene()
        {
            if (null == m_CurScene)
                return false;
            else
                return m_CurScene.IsPve;
        }

        public bool IsMultiPveScene()
        {
            if (null == m_CurScene)
                return false;
            else
                return m_CurScene.IsMultiPve;
        }

        public bool IsPvpScene()
        {
            if (null == m_CurScene)
                return false;
            else
                return m_CurScene.IsPvp;
        }

        public bool IsExpeditionScene()
        {
            if (null == m_CurScene)
                return false;
            else
                return m_CurScene.IsExpedition;
        }
        public bool IsELiteScene()
        {
            if (null == m_CurScene)
                return false;
            else
                return m_CurScene.IsELite;
        }
        public bool IsServerSelectScene()
        {
            if (null == m_CurScene)
                return false;
            else
                return m_CurScene.IsServerSelectScene;
        }
        /**
         * @brief 逻辑循环
         */
        public void Tick()
        {
            //逻辑限帧率10帧
            long curTime = TimeUtility.GetLocalMilliseconds();
            if (m_LastLogicTickTime + 40 <= curTime)
            {
                m_LastLogicTickTime = curTime;
            }
            else
            {
                return;
            }
            TimeSnapshot.Start();
            TimeSnapshot.DoCheckPoint();
            if (m_CurScene == null)
            {
                return;
            }
            //处理延迟调用
            m_DelayActionProcessor.HandleActions(100);
            //角色进场景逻辑
            if (!m_CurScene.IsWaitSceneLoad && m_CurScene.IsWaitRoomServerConnect)
            {
                if (this.IsPureClientScene() || this.IsPveScene() || this.IsServerSelectScene() || NetworkSystem.Instance.CanSendMessage)
                {
                    GfxSystem.PublishGfxEvent("ge_enter_scene", "ui", m_CurScene.ResId);
                    //if (this.IsPureClientScene()) {
                    //  GfxSystem.PublishGfxEvent("ge_ShowCYGTSDK", "gt");
                    //} else {
                    //  GfxSystem.PublishGfxEvent("ge_HideCYGTSDK", "gt");
                    //}

                    StorySystem.StoryConfigManager.Instance.Clear();
                    ClientStorySystem.Instance.ClearStoryInstancePool();
                    for (int i = 1; i < 10; ++i)
                    {
                        ClientStorySystem.Instance.PreloadStoryInstance(1);
                    }
                    PlayerControl.Instance.Reset();

                    if (IsObserver)
                    {
                        DestroyHero();
                        CreateSceneLogics();
                        UserInfo myself = CreatePlayerSelf(0x0ffffffe, 1);
                        if (null != myself)
                        {//观战客户端创建一个虚拟玩家（不关联view，血量不要为0，主要目的是为了适应客户端代码里对主角的判断）
                            myself.SetLevel(16);
                            myself.SetHp(Operate_Type.OT_Absolute, 999999);
                        }
                        m_CurScene.NotifyUserEnter();

                        DashFireMessage.Msg_CR_Observer build = new DashFireMessage.Msg_CR_Observer();
                        NetworkSystem.Instance.SendMessage(build);
                        LogSystem.Debug("send Msg_CR_Observer to roomserver");
                    }
                    else if (this.IsPureClientScene() || IsPveScene())
                    {
                        //单机游戏逻辑启动
                        CreateSceneLogics();
                        if (IsExpeditionScene())
                            ExpeditionStartGame();
                        else
                            StartGame();
                        m_CurScene.NotifyUserEnter();
                        ClientStorySystem.Instance.StartStory(1);
                    }
                    else
                    {
                        //下副本时玩家的角色ID与本地客户端的角色不一致，所以下副本前先删掉本地角色
                        DestroyHero();
                        CreateSceneLogics();

                        if (IsPvpScene() || IsMultiPveScene())
                        {
                            DashFireMessage.Msg_CRC_Create build = new DashFireMessage.Msg_CRC_Create();
                            NetworkSystem.Instance.SendMessage(build);
                            LogSystem.Debug("send Msg_CRC_Create to roomserver");
                        }
                    }

                    if (IsPveScene() || IsPureClientScene())
                    {
                        SyncGfxUsersInfo();
                    }

                    m_CurScene.IsWaitRoomServerConnect = false;
                }
            }
            if (!m_CurScene.IsSuccessEnter)
            {
                if (curTime > m_LastTryChangeSceneTime + c_ChangeSceneTimeout)
                {
                    m_LastTryChangeSceneTime = curTime;
                    PromptExceptionAndGotoMainCity();
                }
                return;
            }
            m_Profiler.sceneTickTime = TimeSnapshot.DoCheckPoint();

            EntityManager.Instance.Tick();
            m_Profiler.entityMgrTickTime = TimeSnapshot.DoCheckPoint();

            ControlSystemOperation.Tick();
            m_Profiler.controlSystemTickTime = TimeSnapshot.DoCheckPoint();

            m_Profiler.movementSystemTickTime = TimeSnapshot.DoCheckPoint();

            m_SpatialSystem.Tick();
            m_Profiler.spatialSystemTickTime = TimeSnapshot.DoCheckPoint();
            if (m_Profiler.spatialSystemTickTime > 50000)
            {
                LogSystem.Warn("*** SpatialSystem tick time is {0}", m_Profiler.spatialSystemTickTime);
                for (LinkedListNode<UserInfo> node = UserManager.Users.FirstValue; null != node; node = node.Next)
                {
                    UserInfo userInfo = node.Value;
                    if (null != userInfo)
                    {
                        LogSystem.Warn("===>User:{0} Pos:{1}", userInfo.GetId(), userInfo.GetMovementStateInfo().GetPosition3D().ToString());
                    }
                }
                for (LinkedListNode<NpcInfo> node = NpcManager.Npcs.FirstValue; null != node; node = node.Next)
                {
                    NpcInfo npcInfo = node.Value;
                    if (null != npcInfo)
                    {
                        LogSystem.Warn("===>Npc:{0} Pos:{1}", npcInfo.GetId(), npcInfo.GetMovementStateInfo().GetPosition3D().ToString());
                    }
                }
            }

            TickMoveMeetObstacle();
            //obj特殊逻辑处理
            TickUsers();
            m_Profiler.usersTickTime = TimeSnapshot.DoCheckPoint();

            TickNpcs();
            m_Profiler.npcsTickTime = TimeSnapshot.DoCheckPoint();

            try
            {
                TickSystemByCharacters();
            }
            catch (Exception e)
            {
                LogSystem.Error("Exception:{0}\n{1}", e.Message, e.StackTrace);
            }
            m_Profiler.combatSystemTickTime = TimeSnapshot.DoCheckPoint();

            if (IsPureClientScene() || IsPveScene())
            {
                //TickInteraction();
                TickPve();
            }

            if (IsPveScene())
            {
                TickRecover();
            }

            GmCommands.ClientGmStorySystem.Instance.Tick();

            m_SceneLogicSystem.Tick();
            m_Profiler.sceneLogicSystemTickTime = TimeSnapshot.DoCheckPoint();

            long tickTime = TimeSnapshot.End();
            if (tickTime > 30000)
            {
                LogSystem.Debug("*** PerformanceWarning: {0}", m_Profiler.GenerateLogString(tickTime));
            }
        }

        public void SwitchDebug()
        {
            GlobalVariables.Instance.IsDebug = !GlobalVariables.Instance.IsDebug;
            if (IsPvpScene() || IsMultiPveScene())
            {
                Msg_CR_SwitchDebug builder = new Msg_CR_SwitchDebug();
                builder.is_debug = GlobalVariables.Instance.IsDebug;
                NetworkSystem.Instance.SendMessage(builder);
            }
            if (!GlobalVariables.Instance.IsDebug)
            {
                ShowObstacle(false);
            }
            else
            {
                ShowObstacle(true);
            }
        }
        public void SwitchObserver()
        {
            if (m_IsObserver)
            {
                m_IsFollowObserver = !m_IsFollowObserver;
                if (m_IsFollowObserver)
                {
                    LinkedListNode<UserInfo> node = null;
                    for (node = UserManager.Users.FirstValue; null != node; node = node.Next)
                    {
                        if (node.Value == m_PlayerSelf)
                            continue;
                        if (node.Value.GetId() == m_FollowTargetId)
                        {
                            break;
                        }
                    }
                    if (null == node)
                    {
                        for (node = UserManager.Users.FirstValue; null != node; node = node.Next)
                        {
                            if (node.Value == m_PlayerSelf)
                                continue;
                            else
                                break;
                        }
                    }
                    if (null != node)
                    {
                        m_FollowTargetId = node.Value.GetId();
                        if (!node.Value.IsDead())
                        {
                            UserView view = EntityManager.Instance.GetUserViewById(m_FollowTargetId);
                            if (null != view)
                            {
                                GfxSystem.SendMessage("GfxGameRoot", "CameraFollow", view.Actor);
                            }
                        }
                    }
                }
            }
        }
        public bool InteractObject()
        {
            if (m_IsObserver)
            {
                if (m_IsFollowObserver)
                {
                    LinkedListNode<UserInfo> node = null;
                    for (node = UserManager.Users.FirstValue; null != node; node = node.Next)
                    {
                        if (node.Value == m_PlayerSelf)
                            continue;
                        if (node.Value.GetId() == m_FollowTargetId)
                        {
                            node = node.Next;
                            break;
                        }
                    }
                    if (null == node)
                    {
                        for (node = UserManager.Users.FirstValue; null != node; node = node.Next)
                        {
                            if (node.Value == m_PlayerSelf)
                                continue;
                            else
                                break;
                        }
                    }
                    if (null != node)
                    {
                        m_FollowTargetId = node.Value.GetId();
                    }
                }
                return true;
            }
            bool sendMsg = false;
            if (null != m_PlayerSelf)
            {
                int initiator = m_PlayerSelf.GetId();
                int receiver = 0;

                //释放控制为低优先级操作
                if (!sendMsg && null != m_PlayerSelf.ControlledObject)
                {
                    receiver = m_PlayerSelf.ControlledObject.GetId();
                    sendMsg = true;
                }

                if (sendMsg)
                {
                    Msg_CRC_InteractObject builder = new Msg_CRC_InteractObject();
                    //builder.InitiatorId = initiator;
                    builder.receiver_id = receiver;
                    NetworkSystem.Instance.SendMessage(builder);
                }
            }
            return sendMsg;
        }

        public bool ChangeScene(int sceneId)
        {
            try
            {
                if (null != m_CurScene)
                {
                    if (m_CurScene.ResId == sceneId)
                    {
                        return true;
                    }
                    else if (!m_CurScene.IsSuccessEnter && !this.IsServerSelectScene())
                    {
                        return false;
                    }

                    Reset();
                    m_CurScene.Release();
                    m_CurScene = null;
                }
                else
                {
                    Reset();
                }
                m_LastTryChangeSceneTime = TimeUtility.GetLocalMilliseconds();
                m_CurScene = new SceneResource();
                if (null != m_CurScene)
                {
                    m_CurScene.Init(sceneId);
                    if (null != m_CurScene.SceneConfig)
                    {
                        if (IsServerSelectScene())
                        {
                            //如果是服务器选择场景
                            LobbyClient.Instance.CurrentRole = null;
                        }
                        Data_SceneConfig scene_config = SceneConfigProvider.Instance.GetSceneConfigById(m_CurScene.ResId);
                        m_SpatialSystem.Init(FilePathDefine_Client.C_RootPath + scene_config.m_BlockInfoFile, scene_config.m_ReachableSet);
                        m_SpatialSystem.LoadPatch(FilePathDefine_Client.C_RootPath + scene_config.m_BlockInfoFile + ".patch");
                        m_SpatialSystem.LoadObstacle(FilePathDefine_Client.C_RootPath + scene_config.m_ObstacleFile, 1 / scene_config.m_TiledDataScale);

                        LogSystem.Debug("init SpatialSystem:{0}", FilePathDefine_Client.C_RootPath + scene_config.m_BlockInfoFile);
                        LogSystem.Debug("GameSystem.ChangeNextScene:{0}", m_CurScene.ResId);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
            return false;
        }

        public int GetCurSceneId()
        {
            if (m_CurScene != null)
            {
                return m_CurScene.ResId;
            }
            return 0;
        }
        public SceneResource GetCurScene()
        {
            return m_CurScene;
        }

        public void SetBlockedShader(uint rimColor1, float rimPower1, float rimCutValue1, uint rimColor2, float rimPower2, float rimCutValue2)
        {
            UserInfo myself = GetPlayerSelf();
            if (null != myself)
            {
                UserView myselfView = EntityManager.Instance.GetUserViewById(myself.GetId());
                if (null != myselfView)
                {
                    GfxSystem.SetBlockedShader(myselfView.Actor, rimColor1, rimPower1, rimCutValue1);
                }
                LinkedListDictionary<int, UserInfo> users = UserManager.Users;
                for (LinkedListNode<UserInfo> node = users.FirstValue; null != node; node = node.Next)
                {
                    UserInfo user = node.Value;
                    if (null != user && user != myself)
                    {
                        UserView view = EntityManager.Instance.GetUserViewById(user.GetId());
                        if (null != view)
                        {
                            if (CharacterInfo.GetRelation(myself, user) == CharacterRelation.RELATION_FRIEND)
                            {
                                GfxSystem.SetBlockedShader(view.Actor, rimColor1, rimPower1, rimCutValue1);

                            }
                            else
                            {
                                GfxSystem.SetBlockedShader(view.Actor, rimColor2, rimPower2, rimCutValue2);
                            }
                        }
                    }
                }
                LinkedListDictionary<int, NpcInfo> npcs = NpcManager.Npcs;
                for (LinkedListNode<NpcInfo> node = npcs.FirstValue; null != node; node = node.Next)
                {
                    NpcInfo npc = node.Value;
                    if (null != npc)
                    {
                        NpcView view = EntityManager.Instance.GetNpcViewById(npc.GetId());
                        if (null != view)
                        {
                            if (CharacterInfo.GetRelation(myself, npc) == CharacterRelation.RELATION_FRIEND)
                            {
                                GfxSystem.SetBlockedShader(view.Actor, rimColor1, rimPower1, rimCutValue1);

                            }
                            else
                            {
                                GfxSystem.SetBlockedShader(view.Actor, rimColor2, rimPower2, rimCutValue2);
                            }
                        }
                    }
                }
            }
        }

        public void Reset()
        {
            LogSystem.Debug("WorldSystem.Reset Destory Objects...");
            DestroyObstacleObjects();

            for (LinkedListNode<UserInfo> linkNode = m_UserMgr.Users.FirstValue; null != linkNode; linkNode = linkNode.Next)
            {
                UserInfo info = linkNode.Value;
                if (null != info)
                {
                    EntityManager.Instance.DestroyUserView(info.GetId());
                }
            }
            for (LinkedListNode<NpcInfo> linkNode = m_NpcMgr.Npcs.FirstValue; null != linkNode; linkNode = linkNode.Next)
            {
                NpcInfo info = linkNode.Value;
                if (null != info)
                {
                    EntityManager.Instance.DestroyNpcView(info.GetId());
                }
            }
            LogSystem.Debug("WorldSystem.Reset Destory Objects Finish.");

            m_UserMgr.Reset();
            m_NpcMgr.Reset();
            m_SceneLogicInfoMgr.Reset();

            m_SceneLogicSystem.Reset();
            m_SpatialSystem.Reset();
            m_AiSystem.Reset();
            m_BlackBoard.Reset();

            ControlSystemOperation.Reset();

            ClientStorySystem.Instance.Reset();
            ClientStorySystem.Instance.ClearStoryInstancePool();
            StorySystem.StoryConfigManager.Instance.Clear();
        }
        public void LoadData()
        {
            SceneConfigProvider.Instance.Load(FilePathDefine_Client.C_SceneConfig, "ScenesConfigs");
            SceneConfigProvider.Instance.LoadDropOutConfig(FilePathDefine_Client.C_SceneDropOut, "SceneDropOut");
            SceneConfigProvider.Instance.LoadAllSceneConfig(FilePathDefine_Client.C_RootPath);

            ActionConfigProvider.Instance.Load(FilePathDefine_Client.C_ActionConfig, "ActionConfig");
            NpcConfigProvider.Instance.LoadNpcConfig(FilePathDefine_Client.C_NpcConfig, "NpcConfig");
            NpcConfigProvider.Instance.LoadNpcLevelupConfig(FilePathDefine_Client.C_NpcLevelupConfig, "NpcLevelupConfig");
            PlayerConfigProvider.Instance.LoadPlayerConfig(FilePathDefine_Client.C_PlayerConfig, "PlayerConfig");
            PlayerConfigProvider.Instance.LoadPlayerLevelupConfig(FilePathDefine_Client.C_PlayerLevelupConfig, "PlayerLevelupConfig");
            PlayerConfigProvider.Instance.LoadPlayerLevelupExpConfig(FilePathDefine_Client.C_PlayerLevelupExpConfig, "PlayerLevelupExpConfig");
            CriticalConfigProvider.Instance.Load(FilePathDefine_Client.C_CriticalConfig, "CriticalConfig");
            AttributeScoreConfigProvider.Instance.Load(FilePathDefine_Client.C_AttributeScoreConfig, "AttributeScoreConfig");

            AiActionConfigProvider.Instance.Load(FilePathDefine_Client.C_AiActionConfig, "AiActionConfig");

            ItemConfigProvider.Instance.Load(FilePathDefine_Client.C_ItemConfig, "ItemConfig");
            ItemLevelupConfigProvider.Instance.Load(FilePathDefine_Client.C_ItemLevelupConfig, "ItemLevelupConfig");
            EquipmentConfigProvider.Instance.LoadEquipmentConfig(FilePathDefine_Client.C_EquipmentConfig, "EquipmentConfig");
            BuyStaminaConfigProvider.Instance.Load(FilePathDefine_Client.C_BuyStaminaConfig, "BuyStaminaConfig");
            AppendAttributeConfigProvider.Instance.Load(FilePathDefine_Client.C_AppendAttributeConfig, "AppendAttributeConfig");
            LegacyLevelupConfigProvider.Instance.Load(FilePathDefine_Client.C_LegacyLevelupConfig, "LegacyLevelupConfig");

            SkillConfigProvider.Instance.CollectData(SkillConfigType.SCT_SOUND, FilePathDefine_Client.C_SoundConfig, "SoundConfig");
            SkillConfigProvider.Instance.CollectData(SkillConfigType.SCT_SKILL, FilePathDefine_Client.C_SkillSystemConfig, "SkillConfig");
            SkillConfigProvider.Instance.CollectData(SkillConfigType.SCT_IMPACT, FilePathDefine_Client.C_ImpactSystemConfig, "ImpactConfig");
            SkillConfigProvider.Instance.CollectData(SkillConfigType.SCT_EFFECT, FilePathDefine_Client.C_EffectConfig, "EffectConfig");
            SkillLevelupConfigProvider.Instance.Load(FilePathDefine_Client.C_SkillLevelupConfig, "SkillLevelupConfig");

            BuffConfigProvider.Instance.Load(FilePathDefine_Client.C_BuffConfig, "BuffConfig");

            SoundConfigProvider.Instance.Load(FilePathDefine_Client.C_GlobalSoundConfig, "C_GlobalSoundConfig");
            StrDictionaryProvider.Instance.Load(FilePathDefine_Client.C_StrDictionary, "StrDictionary");

            NewbieGuideProvider.Instance.Load(FilePathDefine_Client.C_NewbieGuide, "NewbieGuide");
            SystemGuideConfigProvider.Instance.Load(FilePathDefine_Client.C_SystemGuideConfig, "SystemGuideConfig");

            UiConfigProvider.Instance.Load(FilePathDefine_Client.C_UiConfig, "UiConfig");
            ServerConfigProvider.Instance.Load(FilePathDefine_Client.C_ServerConfig, "ServerConfig");
            MissionConfigProvider.Instance.Load(FilePathDefine_Client.C_MissionConfig, "MissionConfig");
            WordFilter.Instance.Load(FilePathDefine_Client.C_SensitiveDictionary);
            DynamicSceneConfigProvider.Instance.CollectData(FilePathDefine_Client.C_DynamicSceneConfig, "DynamicSceneConfig");

            ExpeditionMonsterAttrConfigProvider.Instance.Load(FilePathDefine_Client.C_ExpeditionMonsterAttrConfig, "ExpeditionMonsterAttrConfig");
            ExpeditionTollgateConfigProvider.Instance.Load(FilePathDefine_Client.C_ExpeditionTollgateConfig, "ExpeditionTollgateConfig");
            ExpeditionMonsterConfigProvider.Instance.Load(FilePathDefine_Client.C_ExpeditionMonsterConfig, "ExpeditionMonsterConfig");

            BuyMoneyConfigProvider.Instance.Load(FilePathDefine_Client.C_BuyMoneyConfig, "BuyMoneyConfig");
            GowConfigProvider.Instance.LoadForClient();

            VipConfigProvider.Instance.Load(FilePathDefine_Client.C_VipConfig, "VipConfig");
            VersionConfigProvider.Instance.Load(FilePathDefine_Client.C_VersionConfig, "VersionConfig");
        }
        public void ReloadNpc()
        {
            foreach (Data_Unit npcUnit in m_CurScene.StaticData.m_UnitMgr.GetData().Values)
            {
                if (npcUnit.m_IsEnable)
                {
                    NpcInfo npc = m_NpcMgr.GetNpcInfoByUnitId(npcUnit.GetId());
                    if (null == npc)
                    {
                        npc = m_NpcMgr.AddNpc(npcUnit);
                    }
                    if (null != npc)
                    {
                        npc.SetAIEnable(true);
                        EntityManager.Instance.CreateNpcView(npc.GetId());
                    }
                }
            }
        }
        public void ChangeHero()
        {
            try
            {
                UserInfo user = GetPlayerSelf();
                if (null != user)
                {
                    Vector3 pos = user.GetMovementStateInfo().GetPosition3D();
                    float dir = user.GetMovementStateInfo().GetFaceDir();
                    int hp = user.Hp;
                    int rage = user.Rage;

                    EntityManager.Instance.DestroyUserView(user.GetId());
                    DestroyCharacterById(user.GetId());

                    NetworkSystem.Instance.HeroId = (NetworkSystem.Instance.HeroId + 1) % 4;
                    if (NetworkSystem.Instance.HeroId == 0)
                        NetworkSystem.Instance.HeroId = 1;

                    user = CreatePlayerSelf(1, NetworkSystem.Instance.HeroId);
                    user.SetCampId(NetworkSystem.Instance.CampId);
                    /*Data_Unit unit = m_CurScene.StaticData.ExtractData(DataMap_Type.DT_Unit, GlobalVariables.GetUnitIdByCampId(NetworkSystem.Instance.CampId)) as Data_Unit;
                    if (null != unit) {
                      user.GetMovementStateInfo().SetPosition(unit.m_Pos);
                      user.GetMovementStateInfo().SetFaceDir(unit.m_RotAngle);
                      user.SetHp(Operate_Type.OT_Absolute, 1000);
                    }*/
                    user.GetMovementStateInfo().SetPosition(pos);
                    user.GetMovementStateInfo().SetFaceDir(dir);
                    user.SetHp(Operate_Type.OT_Absolute, hp);
                    user.SetRage(Operate_Type.OT_Absolute, rage);
                    EntityManager.Instance.CreatePlayerSelfView(1);
                    UserView view = EntityManager.Instance.GetUserViewById(1);
                    if (null != view)
                    {
                        view.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        public void StartGame()
        {
            m_SceneStartTime = TimeUtility.GetServerMilliseconds();
            UserInfo user = GetPlayerSelf();
            if (null != user)
            {
                EntityManager.Instance.DestroyUserView(user.GetId());
                DestroyCharacterById(user.GetId());
            }
            user = CreatePlayerSelf(1, NetworkSystem.Instance.HeroId);
            user.SetAIEnable(true);
            user.SetCampId(NetworkSystem.Instance.CampId);
            Data_Unit unit = m_CurScene.StaticData.ExtractData(DataMap_Type.DT_Unit, GlobalVariables.GetUnitIdByCampId(NetworkSystem.Instance.CampId)) as Data_Unit;
            if (null != unit)
            {
                user.GetMovementStateInfo().SetPosition(unit.m_Pos);
                user.GetMovementStateInfo().SetFaceDir(unit.m_RotAngle);
                user.SetHp(Operate_Type.OT_Absolute, user.GetActualProperty().HpMax);
                user.SetEnergy(Operate_Type.OT_Absolute, user.GetActualProperty().EnergyMax);
            }
            EntityManager.Instance.CreatePlayerSelfView(1);
            UserView view = EntityManager.Instance.GetUserViewById(1);
            if (null != view)
            {
                view.Visible = true;
            }

            if (null != LobbyClient.Instance.CurrentRole)
            {
                RoleInfo role_info = LobbyClient.Instance.CurrentRole;
                if (role_info.Nickname.Length > 0)
                {
                    user.SetNickName(role_info.Nickname);
                }
                /// level
                if (role_info.Level > 0)
                {
                    user.SetLevel(role_info.Level);
                }
                /// equips
                if (null != role_info.Equips)
                {
                    for (int i = 0; i < role_info.Equips.Length; i++)
                    {
                        if (null != role_info.Equips[i])
                        {
                            int item_id = role_info.Equips[i].ItemId;
                            if (item_id > 0)
                            {
                                ItemDataInfo info = new ItemDataInfo();
                                info.Level = role_info.Equips[i].Level;
                                info.ItemNum = role_info.Equips[i].ItemNum;
                                info.RandomProperty = role_info.Equips[i].RandomProperty;
                                info.ItemConfig = ItemConfigProvider.Instance.GetDataById(item_id);
                                if (null != info.ItemConfig)
                                {
                                    user.GetEquipmentStateInfo().SetEquipmentData(i, info);
                                }
                            }
                        }
                    }
                }
                /// skills
                RefixSkills(user);
                /// legacys
                if (null != role_info.Legacys)
                {
                    for (int i = 0; i < role_info.Legacys.Length; i++)
                    {
                        if (null != role_info.Legacys[i] && role_info.Legacys[i].IsUnlock)
                        {
                            user.GetLegacyStateInfo().ResetLegacyData(i);
                            int item_id = role_info.Legacys[i].ItemId;
                            if (item_id > 0)
                            {
                                ItemDataInfo info = new ItemDataInfo();
                                info.Level = role_info.Legacys[i].Level;
                                info.ItemNum = role_info.Legacys[i].ItemNum;
                                info.RandomProperty = role_info.Legacys[i].RandomProperty;
                                info.ItemConfig = ItemConfigProvider.Instance.GetDataById(item_id);
                                if (null != info.ItemConfig)
                                {
                                    user.GetLegacyStateInfo().SetLegacyData(i, info);
                                }
                            }
                        }
                    }
                }
                ///
                UserAttrCalculator.Calc(user);
                user.SetHp(Operate_Type.OT_Absolute, user.GetActualProperty().HpMax);
                user.SetEnergy(Operate_Type.OT_Absolute, user.GetActualProperty().EnergyMax);
            }

            /// create npc
            foreach (Data_Unit npcUnit in m_CurScene.StaticData.m_UnitMgr.GetData().Values)
            {
                if (npcUnit.m_IsEnable)
                {
                    NpcInfo npc = m_NpcMgr.GetNpcInfoByUnitId(npcUnit.GetId());
                    if (null == npc)
                    {
                        npc = m_NpcMgr.AddNpc(npcUnit);
                        if (null != npc)
                        {
                            npc.SetAIEnable(true);
                            npc.SkillController = new SwordManSkillController(npc, GfxModule.Skill.GfxSkillSystem.Instance);
                            EntityManager.Instance.CreateNpcView(npc.GetId());
                            CustomNpcByUnitId(npc, npcUnit.GetId());
                        }
                    }
                }
            }
            /// scene tips 
            if (IsELiteScene())
            {
                RoleInfo curRole = LobbyClient.Instance.CurrentRole;
                int stars = curRole.GetSceneInfo(m_CurScene.ResId);
                if (stars == 1)
                {
                    GfxSystem.PublishGfxEvent("ge_pve_fightinfo", "ui", 2, m_CurScene.SceneConfig.m_CompletedTime, 0, 0);
                }
                else if (stars == 2)
                {
                    GfxSystem.PublishGfxEvent("ge_pve_fightinfo", "ui", 0, 0, m_CurScene.SceneConfig.m_CompletedHitCount, 0);
                }
            }

            LogSystem.Debug("start game");
        }
        public void ExpeditionStartGame()
        {
            UserInfo user = GetPlayerSelf();
            if (null != user)
            {
                EntityManager.Instance.DestroyUserView(user.GetId());
                DestroyCharacterById(user.GetId());
            }
            user = CreatePlayerSelf(1, NetworkSystem.Instance.HeroId);
            user.SetAIEnable(true);
            user.SetCampId(NetworkSystem.Instance.CampId);
            Data_Unit unit = m_CurScene.StaticData.ExtractData(DataMap_Type.DT_Unit, GlobalVariables.GetUnitIdByCampId(NetworkSystem.Instance.CampId)) as Data_Unit;
            if (null != unit)
            {
                user.GetMovementStateInfo().SetPosition(unit.m_Pos);
                user.GetMovementStateInfo().SetFaceDir(unit.m_RotAngle);
                user.SetHp(Operate_Type.OT_Absolute, user.GetActualProperty().HpMax);
                user.SetEnergy(Operate_Type.OT_Absolute, user.GetActualProperty().EnergyMax);
            }
            EntityManager.Instance.CreatePlayerSelfView(1);
            UserView view = EntityManager.Instance.GetUserViewById(1);
            if (null != view)
            {
                view.Visible = true;
            }

            if (null != LobbyClient.Instance.CurrentRole)
            {
                RoleInfo role_info = LobbyClient.Instance.CurrentRole;
                if (role_info.Nickname.Length > 0)
                {
                    user.SetNickName(role_info.Nickname);
                }
                /// level
                if (role_info.Level > 0)
                {
                    user.SetLevel(role_info.Level);
                }
                /// equips
                if (null != role_info.Equips)
                {
                    for (int i = 0; i < role_info.Equips.Length; i++)
                    {
                        if (null != role_info.Equips[i])
                        {
                            int item_id = role_info.Equips[i].ItemId;
                            if (item_id > 0)
                            {
                                ItemDataInfo info = new ItemDataInfo();
                                info.Level = role_info.Equips[i].Level;
                                info.ItemNum = role_info.Equips[i].ItemNum;
                                info.RandomProperty = role_info.Equips[i].RandomProperty;
                                info.ItemConfig = ItemConfigProvider.Instance.GetDataById(item_id);
                                if (null != info.ItemConfig)
                                {
                                    user.GetEquipmentStateInfo().SetEquipmentData(i, info);
                                }
                            }
                        }
                    }
                }
                /// skills
                RefixSkills(user);
                /// legacys
                if (null != role_info.Legacys)
                {
                    for (int i = 0; i < role_info.Legacys.Length; i++)
                    {
                        if (null != role_info.Legacys[i] && role_info.Legacys[i].IsUnlock)
                        {
                            user.GetLegacyStateInfo().ResetLegacyData(i);
                            int item_id = role_info.Legacys[i].ItemId;
                            if (item_id > 0)
                            {
                                ItemDataInfo info = new ItemDataInfo();
                                info.Level = role_info.Legacys[i].Level;
                                info.ItemNum = role_info.Legacys[i].ItemNum;
                                info.RandomProperty = role_info.Legacys[i].RandomProperty;
                                info.ItemConfig = ItemConfigProvider.Instance.GetDataById(item_id);
                                if (null != info.ItemConfig)
                                {
                                    user.GetLegacyStateInfo().SetLegacyData(i, info);
                                }
                            }
                        }
                    }
                }
                ///
                UserAttrCalculator.Calc(user);
                ///
                ExpeditionPlayerInfo expedition = role_info.GetExpeditionInfo();
                if (null != expedition && null != expedition.Tollgates && expedition.ActiveTollgate >= 0)
                {
                    ExpeditionPlayerInfo.TollgateData cur_tollgate_data = expedition.Tollgates[expedition.ActiveTollgate];
                    if (null != cur_tollgate_data)
                    {
                        if (0 == expedition.Schedule)
                        {
                            user.SetHp(Operate_Type.OT_Absolute, user.GetActualProperty().HpMax);
                            user.SetEnergy(Operate_Type.OT_Absolute, user.GetActualProperty().EnergyMax);
                            user.SetRage(Operate_Type.OT_Absolute, 0);
                        }
                        else
                        {
                            user.SetHp(Operate_Type.OT_Absolute, expedition.Hp);
                            user.SetEnergy(Operate_Type.OT_Absolute, expedition.Mp);
                            user.SetRage(Operate_Type.OT_Absolute, expedition.Rage);
                        }
                        expedition.MonsterDeadTime = -1;
                        expedition.UserDeadTime = -1;
                        if (EnemyType.ET_Monster == cur_tollgate_data.Type)
                        {
                            cur_tollgate_data.FlushNum = 2;
                        }
                        else
                        {
                            cur_tollgate_data.FlushNum = 1;
                        }
                        if (EnemyType.ET_Monster == cur_tollgate_data.Type || EnemyType.ET_Boss == cur_tollgate_data.Type)
                        {
                            if (null != cur_tollgate_data.EnemyList && cur_tollgate_data.EnemyList.Count > 0
                              && null != cur_tollgate_data.EnemyAttrList && cur_tollgate_data.EnemyAttrList.Count > 0
                              && cur_tollgate_data.EnemyList.Count == cur_tollgate_data.EnemyAttrList.Count)
                            {
                                int ct = cur_tollgate_data.EnemyList.Count;
                                //m_NpcMgr.Reset();
                                for (int index = 0; index < ct; index++)
                                {
                                    int enemy_id = cur_tollgate_data.EnemyList[index];
                                    if (enemy_id > 0)
                                    {
                                        foreach (Data_Unit npcUnit in m_CurScene.StaticData.m_UnitMgr.GetData().Values)
                                        {
                                            if (npcUnit.GetId() == enemy_id)
                                            {
                                                NpcInfo npc = m_NpcMgr.AddNpc(npcUnit);
                                                if (null != npc)
                                                {
                                                    ExpeditionTollgateConfig tollgate_config = ExpeditionTollgateConfigProvider.Instance.GetDataById(expedition.ActiveTollgate + 1);
                                                    if (null != tollgate_config && null != tollgate_config.m_Pos)
                                                    {
                                                        int pos_ct = tollgate_config.m_Pos.Count;
                                                        int pos_index = Helper.Random.Next(0, pos_ct);
                                                        Vector3 npc_pos = Converter.ConvertVector3D(tollgate_config.m_Pos[pos_index]);
                                                        if (Vector3.Zero != npc_pos)
                                                        {
                                                            npc.GetMovementStateInfo().SetPosition(npc_pos);
                                                        }
                                                    }
                                                    npc.SetLevel(user.GetLevel());
                                                    npc.SetAIEnable(true);
                                                    npc.SkillController = new SwordManSkillController(npc, GfxModule.Skill.GfxSkillSystem.Instance);
                                                    EntityManager.Instance.CreateNpcView(npc.GetId());
                                                    CustomNpcByUnitId(npc, npcUnit.GetId());
                                                    ///
                                                    int enemy_attr = cur_tollgate_data.EnemyAttrList[index];
                                                    if (enemy_attr > 0)
                                                    {
                                                        AddExpeditionAttr(npc, enemy_attr);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                --cur_tollgate_data.FlushNum;
                            }
                        }
                        else
                        {
                            /// user
                            if (null != cur_tollgate_data.UserImageList && cur_tollgate_data.UserImageList.Count > 0)
                            {
                                int ct = cur_tollgate_data.UserImageList.Count;
                                for (int index = 0; index < ct; index++)
                                {
                                    ExpeditionImageInfo image_info = cur_tollgate_data.UserImageList[index];
                                    if (null != image_info)
                                    {
                                        int image_res_id = image_info.HeroId;
                                        UserInfo image_player = m_UserMgr.AddUser(image_res_id);
                                        if (null != image_player)
                                        {
                                            image_info.Guid = (ulong)image_player.GetId();
                                            image_player.SceneContext = m_SceneContext;
                                            image_player.SetCampId((int)CampIdEnum.Hostile);
                                            Vector3 image_pos = user.GetMovementStateInfo().GetPosition3D();
                                            float image_dir = user.GetMovementStateInfo().GetFaceDir() - (float)Math.PI;
                                            ExpeditionTollgateConfig tollgate_config = ExpeditionTollgateConfigProvider.Instance.GetDataById(expedition.ActiveTollgate);
                                            if (null != tollgate_config && null != tollgate_config.m_Pos)
                                            {
                                                image_player.GetAiStateInfo().AiLogic = tollgate_config.m_ImageAiLogic;
                                                image_player.SetAIEnable(true);
                                                int pos_ct = tollgate_config.m_Pos.Count - 1;
                                                int pos_index = Helper.Random.Next(0, pos_ct);
                                                Vector3 user_image_pos = Converter.ConvertVector3D(tollgate_config.m_Pos[pos_index]);
                                                if (Vector3.Zero != user_image_pos)
                                                {
                                                    image_pos = user_image_pos;
                                                }
                                            }
                                            image_player.GetMovementStateInfo().SetPosition(image_pos.X, image_pos.Y, image_pos.Z);
                                            image_player.GetMovementStateInfo().SetFaceDir(image_dir);
                                            EntityManager.Instance.CreateUserView(image_player.GetId());
                                            image_player.SetLevel(image_info.Level);
                                            image_player.SetNickName(image_info.Nickname);
                                            image_player.SkillController = new SwordManSkillController(image_player, GfxModule.Skill.GfxSkillSystem.Instance);
                                            UserView image_view = EntityManager.Instance.GetUserViewById(image_player.GetId());
                                            if (view != null)
                                            {
                                            }
                                            /// skills
                                            if (null != image_info.Skills)
                                            {
                                                image_player.GetSkillStateInfo().RemoveAllSkill();
                                                int skill_ct = image_info.Skills.Count;
                                                for (int i = 0; i < skill_ct; i++)
                                                {
                                                    SkillInfo info = image_info.Skills[i];
                                                    if (null != info)
                                                    {
                                                        SkillCategory cur_skill_pos = SkillCategory.kNone;
                                                        if (info.Postions.Presets[0] == SlotPosition.SP_A)
                                                        {
                                                            cur_skill_pos = SkillCategory.kSkillA;
                                                        }
                                                        else if (info.Postions.Presets[0] == SlotPosition.SP_B)
                                                        {
                                                            cur_skill_pos = SkillCategory.kSkillB;
                                                        }
                                                        else if (info.Postions.Presets[0] == SlotPosition.SP_C)
                                                        {
                                                            cur_skill_pos = SkillCategory.kSkillC;
                                                        }
                                                        else if (info.Postions.Presets[0] == SlotPosition.SP_D)
                                                        {
                                                            cur_skill_pos = SkillCategory.kSkillD;
                                                        }
                                                        info.ConfigData.Category = cur_skill_pos;
                                                        image_player.GetSkillStateInfo().AddSkill(info);
                                                        WorldSystem.Instance.AddSubSkill(image_player, info.SkillId, cur_skill_pos, info.SkillLevel);
                                                    }
                                                }
                                            }
                                            Data_PlayerConfig playerData = PlayerConfigProvider.Instance.GetPlayerConfigById(image_player.GetLinkId());
                                            if (null != playerData && null != playerData.m_FixedSkillList
                                              && playerData.m_FixedSkillList.Count > 0)
                                            {
                                                foreach (int skill_id in playerData.m_FixedSkillList)
                                                {
                                                    SkillInfo info = new SkillInfo(skill_id, 1);
                                                    image_player.GetSkillStateInfo().AddSkill(info);
                                                }
                                            }
                                            image_player.ResetSkill();
                                            /// equips
                                            if (null != image_info.Equips)
                                            {
                                                int equip_ct = image_info.Equips.Length;
                                                for (int i = 0; i < equip_ct; i++)
                                                {
                                                    ItemDataInfo image_equip = image_info.Equips[i];
                                                    if (null != image_equip)
                                                    {
                                                        image_player.GetEquipmentStateInfo().ResetEquipmentData(i);
                                                        image_equip.ItemConfig = ItemConfigProvider.Instance.GetDataById(image_equip.ItemId);
                                                        if (null != image_equip.ItemConfig)
                                                        {
                                                            image_player.GetEquipmentStateInfo().SetEquipmentData(i, image_equip);
                                                        }
                                                    }
                                                }
                                            }
                                            /// 
                                            if (null != image_info.Legacys)
                                            {
                                                int legacy_ct = image_info.Legacys.Length;
                                                for (int i = 0; i < legacy_ct; i++)
                                                {
                                                    ItemDataInfo image_legacy = image_info.Legacys[i];
                                                    if (null != image_legacy && image_legacy.IsUnlock)
                                                    {
                                                        image_player.GetLegacyStateInfo().ResetLegacyData(i);
                                                        image_legacy.ItemConfig = ItemConfigProvider.Instance.GetDataById(image_legacy.ItemId);
                                                        if (null != image_legacy.ItemConfig)
                                                        {
                                                            image_player.GetLegacyStateInfo().SetLegacyData(i, image_legacy);
                                                        }
                                                    }
                                                }
                                            }
                                            if (Geometry.IsSameFloat(user.HpMaxCoefficient, 1))
                                            {
                                                float hpper = user.Hp / (float)user.GetActualProperty().HpMax;
                                                float mpper = user.Energy / (float)user.GetActualProperty().EnergyMax;
                                                ExpeditionPlayerInfo.CaclCoefficient(user, image_player);
                                                UserAttrCalculator.Calc(user);
                                                user.SetHp(Operate_Type.OT_Absolute, (int)(user.GetActualProperty().HpMax * hpper));
                                                user.SetEnergy(Operate_Type.OT_Absolute, (int)(user.GetActualProperty().EnergyMax * mpper));
                                            }
                                            UserAttrCalculator.Calc(image_player);
                                            image_player.SetHp(Operate_Type.OT_Absolute, image_player.GetActualProperty().HpMax);
                                            image_player.SetRage(Operate_Type.OT_Absolute, 0);
                                            image_player.SetEnergy(Operate_Type.OT_Absolute, image_player.GetActualProperty().EnergyMax);
                                        }
                                    }
                                }
                                --cur_tollgate_data.FlushNum;
                            }
                        }
                    }
                    expedition.StartTime = TimeUtility.CurTimestamp;
                }
            }

            LogSystem.Debug("expedition start game");
        }

        public void SyncGfxUsersInfo()
        {
            List<GfxUserInfo> gfxUsers = new List<GfxUserInfo>();
            LinkedListDictionary<int, UserInfo> users = UserManager.Users;
            for (LinkedListNode<UserInfo> node = users.FirstValue; null != node; node = node.Next)
            {
                UserInfo user = node.Value;
                if (null != user)
                {
                    UserView view = EntityManager.Instance.GetUserViewById(user.GetId());
                    if (null != view)
                    {
                        GfxUserInfo gfxUser = new GfxUserInfo();
                        gfxUser.m_ActorId = view.Actor;
                        gfxUser.m_HeroId = user.GetLinkId();
                        gfxUser.m_Level = user.GetLevel();
                        gfxUser.m_Nick = user.GetNickName();
                        gfxUsers.Add(gfxUser);
                    }
                }
            }
            GfxSystem.PublishGfxEvent("ge_show_name_plates", "ui", gfxUsers);
        }
        public void SyncGfxUserInfo(int objId)
        {
            UserInfo user = UserManager.GetUserInfo(objId);
            if (null != user)
            {
                UserView view = EntityManager.Instance.GetUserViewById(user.GetId());
                if (null != view)
                {
                    GfxUserInfo gfxUser = new GfxUserInfo();
                    gfxUser.m_ActorId = view.Actor;
                    gfxUser.m_HeroId = user.GetLinkId();
                    gfxUser.m_Level = user.GetLevel();
                    gfxUser.m_Nick = user.GetNickName();

                    GfxSystem.PublishGfxEvent("ge_show_name_plate", "ui", gfxUser);
                }
            }
        }

        public void SyncGfxNpcInfo(int objId)
        {
            NpcInfo npc = NpcManager.GetNpcInfo(objId);
            if (null != npc)
            {
                NpcView view = EntityManager.Instance.GetNpcViewById(npc.GetId());
                if (null != view)
                {
                    GfxUserInfo gfxUser = new GfxUserInfo();
                    gfxUser.m_ActorId = view.Actor;
                    gfxUser.m_HeroId = npc.GetLinkId();
                    gfxUser.m_Level = npc.GetLevel();
                    gfxUser.m_Nick = "";

                    GfxSystem.PublishGfxEvent("ge_show_npc_name_plate", "ui", gfxUser);
                }
            }
        }

        private void AddExpeditionAttr(NpcInfo npc, int attr_id)
        {
            if (null == npc || attr_id <= 0)
                return;
            ExpeditionMonsterAttrConfig attr = ExpeditionMonsterAttrConfigProvider.Instance.GetExpeditionMonsterAttrConfigById(attr_id);
            if (null != attr && null != attr.m_AttrData)
            {
                npc.GetBaseProperty().SetHpMax(Operate_Type.OT_Relative, (int)attr.m_AttrData.GetAddHpMax(0, npc.GetLevel()));
                npc.GetBaseProperty().SetEnergyMax(Operate_Type.OT_Relative, (int)attr.m_AttrData.GetAddEpMax(0, npc.GetLevel()));
                npc.GetBaseProperty().SetAttackBase(Operate_Type.OT_Relative, (int)attr.m_AttrData.GetAddAd(0, npc.GetLevel()));
                npc.GetBaseProperty().SetADefenceBase(Operate_Type.OT_Relative, (int)attr.m_AttrData.GetAddADp(0, npc.GetLevel()));
                npc.GetBaseProperty().SetMDefenceBase(Operate_Type.OT_Relative, (int)attr.m_AttrData.GetAddMDp(0, npc.GetLevel()));
                npc.SetHp(Operate_Type.OT_Absolute, (int)attr.m_AttrData.GetAddHpMax(0, npc.GetLevel()));
                npc.SetEnergy(Operate_Type.OT_Absolute, (int)attr.m_AttrData.GetAddEpMax(0, npc.GetLevel()));
            }
        }
        private int GenImageId(int image_id)
        {
            UserInfo info = m_UserMgr.GetUserInfo(image_id);
            if (null == info)
            {
                return image_id;
            }
            else
            {
                int incr_id = image_id * 10000 + Helper.Random.Next(0, 100);
                return GenImageId(incr_id);
            }
        }
        private void CustomNpcByUnitId(NpcInfo npc, int unitId)
        {
            Data_Unit mapUnit = GetCurScene().StaticData.ExtractData(DataMap_Type.DT_Unit, unitId) as Data_Unit;
            if (null != mapUnit)
            {
                NpcView view = EntityManager.Instance.GetCharacterViewById(npc.GetId()) as NpcView;
                if (null != view)
                {
                    view.SetIdleAnim(mapUnit.m_IdleAnims);
                }
            }
        }
        private void DestroyHero()
        {
            if (null != m_PlayerSelf)
            {
                EntityManager.Instance.DestroyUserView(m_PlayerSelfId);
                DestroyCharacterById(m_PlayerSelfId);
            }
        }
        private void CreateSceneLogics()
        {
            MyDictionary<int, object> slogics = m_CurScene.StaticData.m_SceneLogicMgr.GetData();
            foreach (SceneLogicConfig sc in slogics.Values)
            {
                if (null != sc)
                {
                    if (sc.m_IsClient)
                    {
                        m_SceneLogicInfoMgr.AddSceneLogicInfo(sc.GetId(), sc);
                    }
                    else if (sc.m_IsServer)
                    {
                        if (IsPureClientScene() || IsPveScene())
                            m_SceneLogicInfoMgr.AddSceneLogicInfo(sc.GetId(), sc);
                    }
                }
            }
        }

        private void TickInteraction()
        {
            long curTime = TimeUtility.GetServerMilliseconds();
            if (m_LastInteractionCheckTime + c_InteractionCheckInterval < curTime)
            {
                m_LastInteractionCheckTime = curTime;

                if (null != m_PlayerSelf)
                {
                    bool sendMsg = false;
                    int initiator = m_PlayerSelf.GetId();
                    int receiver = 0;

                    List<ISpaceObject> objs = m_SpatialSystem.GetObjectInCircle(m_PlayerSelf.GetMovementStateInfo().GetPosition3D(), 2);
                    foreach (ISpaceObject obj in objs)
                    {
                        if (obj.GetObjType() == SpatialObjType.kNPC)
                        {
                            NpcInfo npc = obj.RealObject as NpcInfo;
                            if (null != npc)
                            {
                                LogSystem.Debug("TickInteraction id:{0} owner:{1} type:{2} self id:{3}", npc.GetId(), npc.OwnerId, npc.NpcType, m_PlayerSelf.GetId());
                                if (npc.OwnerId == m_PlayerSelf.GetId() && npc.NpcType == (int)NpcTypeEnum.InteractiveNpc)
                                {
                                    receiver = (int)obj.GetID();
                                    sendMsg = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (sendMsg)
                    {
                        //todo:通知UI切换普攻图标为对话图标

                    }
                }
            }
        }
        private void TickPve()
        {
            m_AiSystem.Tick();
            m_Profiler.aiSystemTickTime = TimeSnapshot.DoCheckPoint();

            ClientStorySystem.Instance.Tick();
            m_Profiler.storySystemTickTime = TimeSnapshot.DoCheckPoint();
        }
        private void TickRecover()
        {
            float hp_coefficient = 1.0f;
            float mp_coefficient = 1.0f;
            if (null != m_CurScene && null != m_CurScene.SceneConfig)
            {
                Data_SceneConfig scene_data = m_CurScene.SceneConfig;
                hp_coefficient = scene_data.m_RecoverHpCoefficient;
                mp_coefficient = scene_data.m_RecoverMpCoefficient;
            }
            long curTime = TimeUtility.GetServerMilliseconds();
            if (curTime > m_LastTickTimeForTickPerSecond + c_IntervalPerSecond)
            {
                m_LastTickTimeForTickPerSecond = curTime;
                ///
                for (LinkedListNode<UserInfo> linkNode = m_UserMgr.Users.FirstValue; null != linkNode; linkNode = linkNode.Next)
                {
                    UserInfo info = linkNode.Value;
                    if (!info.IsDead())
                    {
                        float hpRecover = info.GetActualProperty().HpRecover * hp_coefficient;
                        float epRecover = info.GetActualProperty().EnergyRecover * mp_coefficient;
                        if (hpRecover > 0.0001)
                        {
                            if (info.Hp + (int)hpRecover >= info.GetActualProperty().HpMax)
                                info.SetHp(Operate_Type.OT_Absolute, (int)info.GetActualProperty().HpMax);
                            else
                                info.SetHp(Operate_Type.OT_Relative, (int)hpRecover);
                        }
                        if (epRecover > 0.0001)
                        {
                            if (info.Energy + (int)epRecover >= info.GetActualProperty().EnergyMax)
                                info.SetEnergy(Operate_Type.OT_Absolute, (int)info.GetActualProperty().EnergyMax);
                            else
                                info.SetEnergy(Operate_Type.OT_Relative, (int)epRecover);
                        }
                    }
                }
                for (LinkedListNode<NpcInfo> linkNode = m_NpcMgr.Npcs.FirstValue; null != linkNode; linkNode = linkNode.Next)
                {
                    NpcInfo info = linkNode.Value;
                    if (!info.IsDead())
                    {
                        float hpRecover = info.GetActualProperty().HpRecover;
                        float npRecover = info.GetActualProperty().EnergyRecover;
                        if (hpRecover > 0.0001)
                        {
                            if (info.Hp + (int)hpRecover >= info.GetActualProperty().HpMax)
                                info.SetHp(Operate_Type.OT_Absolute, (int)info.GetActualProperty().HpMax);
                            else
                                info.SetHp(Operate_Type.OT_Relative, (int)hpRecover);
                        }
                        if (npRecover > 0.0001)
                        {
                            if (info.Energy + (int)npRecover >= info.GetActualProperty().EnergyMax)
                                info.SetEnergy(Operate_Type.OT_Absolute, (int)info.GetActualProperty().EnergyMax);
                            else
                                info.SetEnergy(Operate_Type.OT_Relative, (int)npRecover);
                        }
                    }
                }
            }
        }
        private void TickUsers()
        {
            for (LinkedListNode<UserInfo> linkNode = m_UserMgr.Users.FirstValue; null != linkNode; linkNode = linkNode.Next)
            {
                UserInfo info = linkNode.Value;
                if (info.SkillController != null)
                {
                    info.SkillController.OnTick();
                }
                if (info.LevelChanged || info.GetSkillStateInfo().BuffChanged || info.GetEquipmentStateInfo().EquipmentChanged || info.GetLegacyStateInfo().LegacyChanged)
                {
                    //LogSystem.Debug("UserAttrCalculate LevelChanged:{0} BuffChanged:{1} EquipmentChanged:{2} LegacyChanged:{3}", info.LevelChanged, info.GetSkillStateInfo().BuffChanged, info.GetEquipmentStateInfo().EquipmentChanged, info.GetLegacyStateInfo().LegacyChanged);
                    UserAttrCalculator.Calc(info);
                    info.LevelChanged = false;
                    info.GetSkillStateInfo().BuffChanged = false;
                    info.GetEquipmentStateInfo().EquipmentChanged = false;
                    info.GetLegacyStateInfo().LegacyChanged = false;
                }
                UserView view = EntityManager.Instance.GetUserViewById(info.GetId());
                if (null != view)
                {
                    if (IsPveScene() && !IsExpeditionScene() || IsMultiPveScene())
                    {
                        int battleNpcCount = GetBattleNpcCount();
                        if (battleNpcCount <= 0)
                        {
                            view.SetIndicatorInfo(true, 0, 0);
                        }
                        else if (battleNpcCount <= 3)
                        {
                            float minPowDist = 99999;
                            NpcInfo npc = m_NpcMgr.GetNearest(info.GetMovementStateInfo().GetPosition3D(), ref minPowDist);
                            if (null != npc && minPowDist > info.IndicatorDis * info.IndicatorDis)
                            {
                                float dir = Geometry.GetDirFromVector(npc.GetMovementStateInfo().GetPosition3D() - info.GetMovementStateInfo().GetPosition3D());
                                view.SetIndicatorInfo(true, dir, 1);
                            }
                            else
                            {
                                view.SetIndicatorInfo(false, 0.0f, 1);
                            }
                        }
                        else
                        {
                            view.SetIndicatorInfo(false, 0.0f, 1);
                        }
                    }
                }
            }
            // 连击
            if (null != m_PlayerSelf)
            {
                long curTime = TimeUtility.GetLocalMilliseconds();
                CombatStatisticInfo combatInfo = m_PlayerSelf.GetCombatStatisticInfo();
                if (combatInfo.LastHitTime + 1500 < curTime && combatInfo.MultiHitCount > 1)
                {
                    combatInfo.MultiHitCount = 1;
                    GfxSystem.PublishGfxEvent("ge_hitcount", "ui", 0);
                }
            }
            UserInfo player = WorldSystem.Instance.GetPlayerSelf();
            if (null != player && player.Hp <= 0)
            {
                if (player.DeadTime <= 0)
                {
                    player.GetCombatStatisticInfo().AddDeadCount(1);  //死亡计数+1
                    if (player.SkillController != null)
                    {
                        player.SkillController.ForceInterruptCurSkill();
                    }

                    if (IsPveScene() || IsPureClientScene())
                    {
                        ClientStorySystem.Instance.SendMessage("userkilled", player.GetId());
                        ClientStorySystem.Instance.SendMessage("playerselfkilled", player.GetId());
                    }
                    player.DeadTime = TimeUtility.GetServerMilliseconds();
                    if (WorldSystem.Instance.IsPveScene() && !IsExpeditionScene() || WorldSystem.Instance.IsMultiPveScene())
                    {
                        GfxSystem.PublishGfxEvent("ge_role_dead", "ui");
                    }
                    // 禁止输入
                    PlayerControl.Instance.EnableMoveInput = false;
                    PlayerControl.Instance.EnableRotateInput = false;
                    PlayerControl.Instance.EnableSkillInput = false;
                }
                ///
                ExpeditionUserDeadHandle();
            }
            ///
            ExpeditionImageDeadHandle();
        }
        private void TickNpcs()
        {
            List<NpcInfo> deletes = new List<NpcInfo>();
            for (LinkedListNode<NpcInfo> linkNode = m_NpcMgr.Npcs.FirstValue; null != linkNode; linkNode = linkNode.Next)
            {
                NpcInfo info = linkNode.Value;
                if (info.SkillController != null)
                {
                    info.SkillController.OnTick();
                }
                if (info.LevelChanged || info.GetSkillStateInfo().BuffChanged || info.GetEquipmentStateInfo().EquipmentChanged || info.GetLegacyStateInfo().LegacyChanged)
                {
                    NpcAttrCalculator.Calc(info);
                    info.LevelChanged = false;
                    info.GetSkillStateInfo().BuffChanged = false;
                    info.GetEquipmentStateInfo().EquipmentChanged = false;
                    info.GetLegacyStateInfo().LegacyChanged = false;
                }
                if (info.Hp <= 0 && info.EmptyBloodTime <= 0)
                {
                    CharacterView view = EntityManager.Instance.GetCharacterViewById(info.GetId());
                    if (null != view)
                    {
                        GfxSystem.SendMessage(view.Actor, "OnEventEmptyBlood", null);
                    }
                    info.EmptyBloodTime = TimeUtility.GetServerMilliseconds();
                    OnNpcKilled(info);
                }
                if (info.Hp <= 0 && info.DeadTime <= 0)
                {
                    CharacterView view = EntityManager.Instance.GetCharacterViewById(info.GetId());
                    if (null != view)
                    {
                        if (view.ObjectInfo.IsDead || !info.GetSkillStateInfo().IsImpactControl())
                        {
                            info.DeadTime = TimeUtility.GetServerMilliseconds();
                            if (info.GetSkillStateInfo().IsSkillActivated())
                            {
                                info.SkillController.ForceInterruptCurSkill();
                            }
                            info.GetSkillStateInfo().RemoveAllImpact();
                            //SendDeadImpact(info);
                            if (info.OwnerId == WorldSystem.Instance.PlayerSelfId)
                            {
                                NetworkSystem.Instance.SyncDeleteDeadNpc(info.GetId());
                            }
                        }
                        else
                        {
                            if (info.IsHaveGfxStateFlag(GfxCharacterState_Type.Stiffness))
                            {
                                SendDeadImpact(info);
                            }
                        }
                    }
                }
                if (info.IsBorning && IsNpcBornOver(info))
                {
                    info.IsBorning = false;
                    info.SetAIEnable(true);
                    info.SetStateFlag(Operate_Type.OT_RemoveBit, CharacterState_Type.CST_Invincible);
                }
                if (info.LifeEndTime > 0 && TimeUtility.GetServerMilliseconds() > info.LifeEndTime)
                {
                    OnNpcKilled(info);
                    DestroyNpc(info);
                }
                if (IsPureClientScene() || IsPveScene())
                {
                    // 约定npc的高度低于130时，直接判定npc死亡。
                    if (130.0f > info.GetMovementStateInfo().GetPosition3D().Y)
                    {
                        info.NeedDelete = true;
                        OnNpcKilled(info);
                    }
                    if (info.NeedDelete)
                    {
                        deletes.Add(info);
                    }
                    else if (info.Hp <= 0 && info.DeadTime > 0 && TimeUtility.GetServerMilliseconds() - info.DeadAnimTime * 1000 - info.DeadTime > info.ReleaseTime)
                    {
                        deletes.Add(info);
                    }
                }
            }
            if (IsPureClientScene() || IsPveScene())
            {
                if (deletes.Count > 0)
                {
                    foreach (NpcInfo ni in deletes)
                    {
                        DestroyNpc(ni);
                        return;
                    }
                }
            }
            ///
            ExpeditionNpcDeadHandle();
        }
        private void TickSystemByCharacters()
        {
            for (LinkedListNode<NpcInfo> linkNode = m_NpcMgr.Npcs.FirstValue; null != linkNode; linkNode = linkNode.Next)
            {
                CharacterInfo character = linkNode.Value;
                ImpactSystem.Instance.Tick(character);
            }

            for (LinkedListNode<UserInfo> linkNode = m_UserMgr.Users.FirstValue; null != linkNode; linkNode = linkNode.Next)
            {
                CharacterInfo character = linkNode.Value;
                ImpactSystem.Instance.Tick(character);
            }
        }
        private void TickMoveMeetObstacle()
        {
            UserInfo myself = GetPlayerSelf();
            if (null != myself && myself.GetMovementStateInfo().IsMoving && myself.GetMovementStateInfo().IsMoveMeetObstacle)
            {
                long curTime = TimeUtility.GetLocalMilliseconds();
                if (m_LastNotifyMoveMeetObstacleTime + 100 <= curTime)
                {
                    NotifyMoveMeetObstacle(true);
                    m_LastNotifyMoveMeetObstacleTime = TimeUtility.GetLocalMilliseconds();
                }
            }
        }

        private void DestroyNpc(NpcInfo ni)
        {
            CalcKillIncome(ni);
            ni.DeadTime = 0;
            if (ni.SkillController != null)
            {
                ni.SkillController.ForceInterruptCurSkill();
            }
            CharacterView view = EntityManager.Instance.GetCharacterViewById(ni.GetId());
            if (null != view)
            {
                GfxSystem.SendMessage(view.Actor, "OnEventDead", null);
            }
            EntityManager.Instance.DestroyNpcView(ni.GetId());
            WorldSystem.Instance.DestroyCharacterById(ni.GetId());
        }
        private void SendDeadImpact(NpcInfo npc)
        {
            if (npc.IsCombatNpc())
            {
                CharacterView view = EntityManager.Instance.GetCharacterViewById(npc.GetId());
                if (null != view)
                {
                    GfxSystem.QueueGfxAction(GfxModule.Impact.GfxImpactSystem.Instance.SendDeadImpact, view.Actor);
                }
            }
        }
        private void OnNpcKilled(NpcInfo info)
        {
            if (IsPveScene() || IsPureClientScene())
            {
                ClientStorySystem.Instance.SendMessage(string.Format("npckilled:{0}", info.GetUnitId()));
                TryFireAllNpcKilled(info.GetId());
            }
        }
        private void TryFireAllNpcKilled(int deadNpcId)
        {
            int ct = GetBattleNpcCount();
            LogSystem.Debug("npc {0} killed, left {1}", deadNpcId, ct);
            if (0 == ct)
            {
                ClientStorySystem.Instance.SendMessage("allnpckilled");
            }
        }
        private int GetBattleNpcCount()
        {
            int ct = 0;
            for (LinkedListNode<NpcInfo> linkNode = m_NpcMgr.Npcs.FirstValue; null != linkNode; linkNode = linkNode.Next)
            {
                NpcInfo info = linkNode.Value;
                if (null != info && info.EmptyBloodTime <= 0 && (info.NpcType == (int)NpcTypeEnum.Normal || info.NpcType == (int)NpcTypeEnum.BigBoss || info.NpcType == (int)NpcTypeEnum.LittleBoss))
                {
                    ++ct;
                }
            }
            return ct;
        }
        private void TryFlushExpeditionMonster()
        {
            UserInfo player = WorldSystem.Instance.GetPlayerSelf();
            if (null != player && null != LobbyClient.Instance.CurrentRole)
            {
                RoleInfo role_info = LobbyClient.Instance.CurrentRole;
                if (null != role_info)
                {
                    ExpeditionPlayerInfo expedition = role_info.GetExpeditionInfo();
                    if (null != expedition && expedition.ActiveTollgate >= 0
                      && null != expedition.Tollgates && expedition.ActiveTollgate < expedition.Tollgates.Length)
                    {
                        ExpeditionPlayerInfo.TollgateData tollgate_data = expedition.Tollgates[expedition.ActiveTollgate];
                        if (null != tollgate_data && tollgate_data.FlushNum > 0)
                        {
                            if (EnemyType.ET_Monster == tollgate_data.Type)
                            {
                                if (null != tollgate_data.EnemyList && tollgate_data.EnemyList.Count > 0
                                  && null != tollgate_data.EnemyAttrList && tollgate_data.EnemyAttrList.Count > 0
                                  && tollgate_data.EnemyList.Count == tollgate_data.EnemyAttrList.Count)
                                {
                                    int ct = tollgate_data.EnemyList.Count;
                                    //m_NpcMgr.Reset();
                                    for (int index = 0; index < ct; index++)
                                    {
                                        int enemy_id = tollgate_data.EnemyList[index];
                                        if (enemy_id > 0)
                                        {
                                            foreach (Data_Unit npcUnit in m_CurScene.StaticData.m_UnitMgr.GetData().Values)
                                            {
                                                if (npcUnit.GetId() == enemy_id)
                                                {
                                                    NpcInfo npc = m_NpcMgr.AddNpc(npcUnit);
                                                    if (null != npc)
                                                    {
                                                        ExpeditionTollgateConfig tollgate_config = ExpeditionTollgateConfigProvider.Instance.GetDataById(expedition.ActiveTollgate + 1);
                                                        if (null != tollgate_config && null != tollgate_config.m_Pos)
                                                        {
                                                            int pos_ct = tollgate_config.m_Pos.Count - 1;
                                                            int pos_index = Helper.Random.Next(0, pos_ct);
                                                            Vector3 npc_pos = Converter.ConvertVector3D(tollgate_config.m_Pos[pos_index]);
                                                            if (Vector3.Zero != npc_pos)
                                                            {
                                                                npc.GetMovementStateInfo().SetPosition(npc_pos);
                                                            }
                                                        }
                                                        npc.SetLevel(player.GetLevel());
                                                        npc.SetAIEnable(true);
                                                        npc.SkillController = new SwordManSkillController(npc, GfxModule.Skill.GfxSkillSystem.Instance);
                                                        EntityManager.Instance.CreateNpcView(npc.GetId());
                                                        CustomNpcByUnitId(npc, npcUnit.GetId());
                                                        ///
                                                        int enemy_attr = tollgate_data.EnemyAttrList[index];
                                                        if (enemy_attr > 0)
                                                        {
                                                            AddExpeditionAttr(npc, enemy_attr);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    --tollgate_data.FlushNum;
                                }
                            }
                        }
                        else
                        {
                            double interval_from_start = TimeUtility.CurTimestamp - expedition.StartTime;
                            if (interval_from_start > 1 && !tollgate_data.IsPostResult)
                            {
                                /// expedition finish
                                int scene_id = GetCurSceneId();
                                int tollgate_num = expedition.ActiveTollgate;
                                UserInfo user = GetPlayerSelf();
                                int hp = user.Hp;
                                int mp = user.Energy;
                                int rage = user.Rage;
                                int hp_max = user.GetActualProperty().HpMax;
                                int mp_max = user.GetActualProperty().EnergyMax;
                                if (tollgate_data.Type == EnemyType.ET_OnePlayer || tollgate_data.Type == EnemyType.ET_TwoPlayer)
                                {
                                    hp = (int)(hp / user.HpMaxCoefficient);
                                    mp = (int)(mp / user.EnergyMaxCoefficient);
                                    hp_max = (int)(hp_max / user.HpMaxCoefficient);
                                    mp_max = (int)(mp_max / user.HpMaxCoefficient);
                                    user.HpMaxCoefficient = 1;
                                    user.EnergyMaxCoefficient = 1;
                                }
                                bool isWinner = hp > 0 ? true : false;
                                GfxSystem.EventChannelForLogic.Publish("ge_finish_expedition", "lobby", scene_id, tollgate_num, hp, mp, rage);
                                GfxSystem.PublishGfxEvent("ge_finish_expedition", "expedition", tollgate_num, isWinner, hp, mp, hp_max, mp_max, rage);
                                tollgate_data.IsPostResult = true;
                            }
                        }
                    }
                }
            }
        }
        ///
        private void ExpeditionUserDeadHandle()
        {
            RoleInfo role_info = LobbyClient.Instance.CurrentRole;
            if (IsExpeditionScene() && null != role_info)
            {
                ExpeditionPlayerInfo expedition = role_info.GetExpeditionInfo();
                if (null != expedition && expedition.ActiveTollgate >= 0
                  && null != expedition.Tollgates && expedition.ActiveTollgate < expedition.Tollgates.Length)
                {
                    ExpeditionPlayerInfo.TollgateData tollgate_data = expedition.Tollgates[expedition.ActiveTollgate];
                    if (null != tollgate_data)
                    {
                        if (expedition.UserDeadTime < 0)
                        {
                            expedition.UserDeadTime = TimeUtility.CurTimestamp;
                        }
                        if (expedition.UserDeadTime > 0
                          && TimeUtility.CurTimestamp - expedition.UserDeadTime > ExpeditionPlayerInfo.c_FlushInterval && !tollgate_data.IsPostResult)
                        {
                            expedition.UserDeadTime = -1;
                            expedition.Hp = 0;
                            expedition.Mp = 0;
                            expedition.Rage = 0;
                            int hp = expedition.Hp;
                            int mp = expedition.Mp;
                            int rage = expedition.Rage;
                            UserInfo user = GetPlayerSelf();
                            int hp_max = user.GetActualProperty().HpMax;
                            int mp_max = user.GetActualProperty().EnergyMax;
                            if (tollgate_data.Type == EnemyType.ET_OnePlayer || tollgate_data.Type == EnemyType.ET_TwoPlayer)
                            {
                                hp_max = (int)(hp_max / user.HpMaxCoefficient);
                                mp_max = (int)(mp_max / user.EnergyMaxCoefficient);
                                user.HpMaxCoefficient = 1;
                                user.EnergyMaxCoefficient = 1;
                            }
                            bool isWinner = hp > 0 ? true : false;
                            int tollgate_num = expedition.ActiveTollgate;
                            GfxSystem.PublishGfxEvent("ge_finish_expedition", "expedition", tollgate_num, isWinner, hp, mp, hp_max, mp_max, rage);
                            GfxSystem.EventChannelForLogic.Publish("ge_expedition_failure", "lobby");
                            tollgate_data.IsPostResult = true;
                        }
                    }
                }
            }
        }
        ///
        private bool PlayMixCool()
        {
            bool ret = false;
            UserInfo player = WorldSystem.Instance.GetPlayerSelf();
            if (null != player)
            {
                if (!player.GetSkillStateInfo().IsSkillActivated())
                {
                    CharacterView player_view = EntityManager.Instance.GetCharacterViewById(player.GetId());
                    if (null != player_view)
                    {
                        player_view.PlayBeCoolAnimation();
                        ret = true;
                    }
                }
            }
            return ret;
        }
        ///
        private void ExpeditionImageDeadHandle()
        {
            if (IsExpeditionScene() && null != LobbyClient.Instance.CurrentRole)
            {
                ExpeditionPlayerInfo expedition = LobbyClient.Instance.CurrentRole.GetExpeditionInfo();
                if (null != expedition.Tollgates && expedition.ActiveTollgate >= 0
                  && expedition.ActiveTollgate < expedition.Tollgates.Length)
                {
                    ExpeditionPlayerInfo.TollgateData tollgate_data = expedition.Tollgates[expedition.ActiveTollgate];
                    if (null != tollgate_data)
                    {
                        if (EnemyType.ET_OnePlayer == tollgate_data.Type || EnemyType.ET_TwoPlayer == tollgate_data.Type)
                        {
                            int ct = 0;
                            int mark_id = 0;
                            for (LinkedListNode<UserInfo> linkNode = m_UserMgr.Users.FirstValue; null != linkNode; linkNode = linkNode.Next)
                            {
                                UserInfo info = linkNode.Value;
                                if (null != info && info.Hp > 0)
                                {
                                    mark_id = info.GetId();
                                    ++ct;
                                }
                            }
                            UserInfo player = WorldSystem.Instance.GetPlayerSelf();
                            if (null != player && 1 == ct && null != player && mark_id == player.GetId())
                            {
                                if (expedition.UserDeadTime < 0)
                                {
                                    ClientStorySystem.Instance.SendMessage("expeditioncompleted");
                                    expedition.UserDeadTime = TimeUtility.CurTimestamp;
                                    GfxSystem.PublishGfxEvent("ge_hide_input_ui", "ui");
                                }
                                double interval_time = TimeUtility.CurTimestamp - expedition.UserDeadTime;
                                if (expedition.UserDeadTime > 0
                                && interval_time > ExpeditionPlayerInfo.c_MixCoolTimePoint)
                                {
                                    if (interval_time > ExpeditionPlayerInfo.c_FlushInterval + 1)
                                    {
                                        if (!tollgate_data.IsPlayAnim)
                                        {
                                            if (PlayMixCool())
                                            {
                                                tollgate_data.IsPlayAnim = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!tollgate_data.IsPlayAnim && tollgate_data.FlushNum <= 0)
                                        {
                                            if (PlayMixCool())
                                            {
                                                tollgate_data.IsPlayAnim = true;
                                            }
                                        }
                                    }
                                }
                                if (expedition.UserDeadTime > 0
                                  && TimeUtility.CurTimestamp - expedition.UserDeadTime > ExpeditionPlayerInfo.c_FlushInterval)
                                {
                                    TryFlushExpeditionMonster();
                                    expedition.UserDeadTime = -1;
                                }
                            }
                        }
                    }
                }
            }
        }
        ///
        private void ExpeditionNpcDeadHandle()
        {
            if (IsExpeditionScene())
            {
                int ct = GetBattleNpcCount();
                if (ct > 0)
                    return;
                if (null != LobbyClient.Instance.CurrentRole)
                {
                    ExpeditionPlayerInfo expedition = LobbyClient.Instance.CurrentRole.GetExpeditionInfo();
                    if (null != expedition && null != expedition.Tollgates)
                    {
                        ExpeditionPlayerInfo.TollgateData tollgate_data = expedition.Tollgates[expedition.ActiveTollgate];
                        if (null != tollgate_data
                          && (tollgate_data.Type == EnemyType.ET_Boss || tollgate_data.Type == EnemyType.ET_Monster))
                        {
                            if (expedition.MonsterDeadTime < 0)
                            {
                                expedition.MonsterDeadTime = TimeUtility.CurTimestamp;
                                if (tollgate_data.FlushNum <= 0)
                                {
                                    ClientStorySystem.Instance.SendMessage("expeditioncompleted");
                                    GfxSystem.PublishGfxEvent("ge_hide_input_ui", "ui");
                                }
                            }
                            double interval_time = TimeUtility.CurTimestamp - expedition.MonsterDeadTime;
                            if (expedition.MonsterDeadTime > 0 && interval_time > ExpeditionPlayerInfo.c_MixCoolTimePoint)
                            {
                                if (interval_time > ExpeditionPlayerInfo.c_FlushInterval + 1)
                                {
                                    if (!tollgate_data.IsPlayAnim)
                                    {
                                        if (PlayMixCool())
                                        {
                                            tollgate_data.IsPlayAnim = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!tollgate_data.IsPlayAnim && tollgate_data.FlushNum <= 0)
                                    {
                                        if (PlayMixCool())
                                        {
                                            tollgate_data.IsPlayAnim = true;
                                        }
                                    }
                                }
                            }
                            if (expedition.MonsterDeadTime > 0
                              && TimeUtility.CurTimestamp - expedition.MonsterDeadTime > ExpeditionPlayerInfo.c_FlushInterval)
                            {
                                TryFlushExpeditionMonster();
                                expedition.MonsterDeadTime = -1;
                            }
                        }
                    }
                }
            }
        }
        ///
        private void CalcKillIncome(NpcInfo npc)
        {
            if (this.IsPureClientScene() || IsPveScene())
            {
                if (npc.KillerId == WorldSystem.Instance.PlayerSelfId)
                {
                    UserInfo userKiller = WorldSystem.Instance.GetPlayerSelf();
                    Data_SceneDropOut dropOutInfo = m_CurScene.SceneDropOut;
                    if (null != dropOutInfo)
                    {
                        if (m_CurScene.GetDropMoney(npc.GetUnitId()) > 0)
                        {
                            DropNpc(DropOutType.GOLD, dropOutInfo.m_GoldModel, dropOutInfo.m_GoldParticle, m_CurScene.GetDropMoney(npc.GetUnitId()), npc);
                        }
                        if (m_CurScene.GetDropHp(npc.GetUnitId()) > 0)
                        {
                            DropNpc(DropOutType.HP, dropOutInfo.m_HpModel, dropOutInfo.m_HpParticle, m_CurScene.GetDropHp(npc.GetUnitId()), npc);
                        }
                        if (m_CurScene.GetDropMp(npc.GetUnitId()) > 0)
                        {
                            DropNpc(DropOutType.MP, dropOutInfo.m_MpModel, dropOutInfo.m_MpParticle, m_CurScene.GetDropMp(npc.GetUnitId()), npc);
                        }
                    }
                }
            }
        }

        private void GetPlayerInfoForCache(ref List<int> userList, ref List<int> userSkillList)
        {
            UserInfo playerSelf = GetPlayerSelf();
            if (playerSelf != null)
            {
                userList.Add(playerSelf.GetLinkId());
                List<SkillInfo> allSkills = playerSelf.GetSkillStateInfo().GetAllSkill();
                foreach (SkillInfo skill in allSkills)
                {
                    if (skill.ConfigData.Category != SkillCategory.kNone)
                    {
                        userSkillList.Add(skill.ConfigData.GetId());
                    }
                }
            }

            //List<SkillInfo> userSkills = null;
            //for (LinkedListNode<UserInfo> linkNode = m_UserMgr.Users.FirstValue; null != linkNode; linkNode = linkNode.Next) {
            //  UserInfo info = linkNode.Value;
            //  if (info != null) {
            //    userList.Add(info.GetLinkId());
            //    userSkills = info.GetSkillStateInfo().GetAllSkill();
            //    foreach (SkillInfo skill in userSkills) {
            //      if (skill.ConfigData.Category != SkillCategory.kNone) {
            //        userSkillList.Add(skill.ConfigData.GetId());
            //      }
            //    }
            //  }
            //}
        }
        private void DropNpc(DropOutType dropType, string model, string particle, int value, NpcInfo npc)
        {
            Data_Unit unit = new Data_Unit();
            unit.m_Id = -1;
            unit.m_LinkId = 100001;
            unit.m_AiLogic = (int)AiStateLogicId.DropOut_AutoPick;
            unit.m_RotAngle = 0;
            UserInfo userInfo = WorldSystem.Instance.GetPlayerSelf();
            if (null != userInfo)
            {
                ScriptRuntime.Vector3 pos = npc.GetMovementStateInfo().GetPosition3D();

                NpcInfo npcInfo = NpcManager.AddNpc(unit);
                npcInfo.GetMovementStateInfo().SetPosition(pos);
                npcInfo.GetMovementStateInfo().SetFaceDir(0);
                npcInfo.GetMovementStateInfo().IsMoving = false;
                npcInfo.SetAIEnable(true);
                npcInfo.SetCampId(userInfo.GetCampId());
                npcInfo.OwnerId = userInfo.GetId();
                DropOutInfo dropInfo = new DropOutInfo();
                dropInfo.DropType = dropType;
                dropInfo.Model = model;
                dropInfo.Particle = particle;
                dropInfo.Value = value;
                npcInfo.GetAiStateInfo().AiDatas.AddData<DropOutInfo>(dropInfo);
                npcInfo.SetModel(model);
                EntityManager.Instance.CreateNpcView(npcInfo.GetId());
            }
        }
        private bool IsNpcBornOver(NpcInfo npc)
        {
            if (npc == null)
            {
                return false;
            }
            long cur_time = TimeUtility.GetServerMilliseconds();
            long born_anim_time = npc.BornAnimTimeMs;
            if ((npc.BornTime + born_anim_time) > cur_time)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void ShowObstacle(bool isShow)
        {
            if (!m_IsDebugObstacleCreated)
            {
                if (null != m_SpatialSystem && null != m_PlayerSelf)
                {
                    CellManager cellMgr = m_SpatialSystem.GetCellManager();
                    if (null != cellMgr)
                    {
                        List<float> vertices = new List<float>();
                        List<int> triangles = new List<int>();

                        int maxRows = cellMgr.GetMaxRow();
                        int maxCols = cellMgr.GetMaxCol();
                        float w = cellMgr.GetCellWidth() / 2;

                        uint color = 0xccff0000;

                        for (int i = 0; i < maxRows; ++i)
                        {
                            for (int j = 0; j < maxCols; ++j)
                            {
                                Vector3 pt = cellMgr.GetCellCenter(i, j);
                                byte status = cellMgr.GetCellStatus(i, j);
                                if (BlockType.GetBlockType(status) != BlockType.NOT_BLOCK)
                                {
                                    int stIndex = vertices.Count / 3;
                                    //---------------------
                                    //左上
                                    vertices.Add(pt.X - w);
                                    vertices.Add(0);
                                    vertices.Add(pt.Z - w);
                                    //右上
                                    vertices.Add(pt.X + w);
                                    vertices.Add(0);
                                    vertices.Add(pt.Z - w);
                                    //左下
                                    vertices.Add(pt.X - w);
                                    vertices.Add(0);
                                    vertices.Add(pt.Z + w);
                                    //右下
                                    vertices.Add(pt.X + w);
                                    vertices.Add(0);
                                    vertices.Add(pt.Z + w);
                                    //---------------------
                                    //三角形1
                                    triangles.Add(stIndex);
                                    triangles.Add(stIndex + 2);
                                    triangles.Add(stIndex + 1);
                                    //三角形2
                                    triangles.Add(stIndex + 1);
                                    triangles.Add(stIndex + 2);
                                    triangles.Add(stIndex + 3);

                                    if (stIndex >= 10000)
                                    {
                                        int actor = GameObjectIdManager.Instance.GenNextId();
                                        GfxSystem.CreateGameObjectWithMeshData(actor, vertices, triangles, "Obstacle", true);
                                        //GfxSystem.CreateGameObjectWithMeshData(actor, vertices, triangles, color, "Transparent/Diffuse", true);
                                        m_DebugObstacleActors.Add(actor);
                                        vertices = new List<float>();
                                        triangles = new List<int>();
                                    }
                                }
                            }
                        }

                        if (vertices.Count > 0)
                        {
                            int actor = GameObjectIdManager.Instance.GenNextId();
                            GfxSystem.CreateGameObjectWithMeshData(actor, vertices, triangles, "Obstacle", true);
                            //GfxSystem.CreateGameObjectWithMeshData(actor, vertices, triangles, color, "Transparent/Diffuse", true);
                            m_DebugObstacleActors.Add(actor);
                        }
                        m_IsDebugObstacleCreated = true;
                    }
                }
                return;
            }
            foreach (int actor in m_DebugObstacleActors)
            {
                GfxSystem.SetGameObjectVisible(actor, isShow);
            }
        }
        private void DestroyObstacleObjects()
        {
            foreach (int actor in m_DebugObstacleActors)
            {
                GfxSystem.DestroyGameObject(actor);
            }
            m_IsDebugObstacleCreated = false;
        }

        public CharacterInfo GetCharacterById(int id)
        {
            CharacterInfo obj = null;
            if (null != m_NpcMgr)
                obj = m_NpcMgr.GetNpcInfo(id);
            if (null != m_UserMgr && null == obj)
                obj = m_UserMgr.GetUserInfo(id);
            return obj;
        }
        public CharacterInfo GetCharacterByUnitId(int unitId)
        {
            CharacterInfo obj = null;
            if (null != m_NpcMgr)
                obj = m_NpcMgr.GetNpcInfoByUnitId(unitId);
            return obj;
        }
        public UserInfo GetPlayerSelf()
        {
            return m_PlayerSelf;
        }
        public void DestroyCharacterById(int id)
        {
            if (m_NpcMgr.Npcs.Contains(id))
            {
                m_NpcMgr.RemoveNpc(id);
            }
            if (m_PlayerSelfId == id)
            {
                m_PlayerSelf = null;
            }
            if (m_UserMgr.Users.Contains(id))
            {
                if (null != m_SpatialSystem)
                {
                    CharacterInfo info = m_UserMgr.Users[id];
                    if (null != info)
                    {
                        info.SceneContext = null;
                        m_SpatialSystem.RemoveObj(info.SpaceObject);
                    }
                }
                m_UserMgr.RemoveUser(id);
            }
        }
        public void SetAIEnable(int characterId, bool enable)
        {
            CharacterInfo info = WorldSystem.Instance.GetCharacterById(characterId);
            if (null != info)
            {
                info.SetAIEnable(enable);
                info.GetMovementStateInfo().IsMoving = false;
            }
        }
        public void CreateNpcEntity(int unitId)
        {
            Data_Unit mapUnit = GetCurScene().StaticData.ExtractData(DataMap_Type.DT_Unit, unitId) as Data_Unit;
            if (null != mapUnit)
            {
                NpcInfo npc = m_NpcMgr.AddNpc(mapUnit);
                if (null != npc)
                {
                    npc.SkillController = new SwordManSkillController(npc, GfxModule.Skill.GfxSkillSystem.Instance);
                    npc.SkillController.Init();
                    EntityManager.Instance.CreateNpcView(npc.GetId());
                    CustomNpcByUnitId(npc, unitId);
                }
            }
        }
        public void CreateNpcEntity(int unitId, float rnd)
        {
            Data_Unit mapUnit = GetCurScene().StaticData.ExtractData(DataMap_Type.DT_Unit, unitId) as Data_Unit;
            if (null != mapUnit)
            {
                Vector3 pos;
                if (Geometry.IsSamePoint(mapUnit.m_Pos2, Vector3.Zero))
                {
                    pos = mapUnit.m_Pos;
                }
                else
                {
                    pos = mapUnit.m_Pos * (1.0f - rnd) + mapUnit.m_Pos2 * rnd;
                }
                NpcInfo npc = m_NpcMgr.AddNpc(mapUnit);
                if (null != npc)
                {
                    npc.GetMovementStateInfo().SetPosition(pos);
                    npc.SkillController = new SwordManSkillController(npc, GfxModule.Skill.GfxSkillSystem.Instance);
                    npc.SkillController.Init();
                    EntityManager.Instance.CreateNpcView(npc.GetId());
                    CustomNpcByUnitId(npc, unitId);
                }
            }
        }
        public void CreateNpcEntityWithPos(int unitId, float x, float y, float z, float rotateAngle)
        {
            Data_Unit mapUnit = GetCurScene().StaticData.ExtractData(DataMap_Type.DT_Unit, unitId) as Data_Unit;
            if (null != mapUnit)
            {
                NpcInfo npc = m_NpcMgr.AddNpc(mapUnit);
                if (null != npc)
                {
                    npc.GetMovementStateInfo().SetPosition(x, y, z);
                    npc.GetMovementStateInfo().SetFaceDir(rotateAngle);
                    npc.SkillController = new SwordManSkillController(npc, GfxModule.Skill.GfxSkillSystem.Instance);
                    npc.SkillController.Init();
                    EntityManager.Instance.CreateNpcView(npc.GetId());
                }
            }
        }

        public NpcInfo CreateNpc(int id, int unitId)
        {
            NpcInfo ret = null;
            Data_Unit mapUnit = GetCurScene().StaticData.ExtractData(DataMap_Type.DT_Unit, unitId) as Data_Unit;
            if (null != mapUnit)
            {
                ret = m_NpcMgr.AddNpc(id, mapUnit);
                if (null != ret)
                {
                    ret.SkillController = new SwordManSkillController(ret, GfxModule.Skill.GfxSkillSystem.Instance);
                    ret.SkillController.Init();
                }
            }
            return ret;
        }
        public NpcInfo CreateNpcByLinkId(int id, int linkId)
        {
            Data_Unit mapUnit = new Data_Unit();
            mapUnit.m_Id = -1;
            mapUnit.m_LinkId = linkId;
            NpcInfo npc = m_NpcMgr.AddNpc(id, mapUnit);
            if (null != npc)
            {
                npc.SkillController = new SwordManSkillController(npc, GfxModule.Skill.GfxSkillSystem.Instance);
                npc.SkillController.Init();
            }
            return npc;
        }
        public UserInfo CreateUser(int id, int resId)
        {
            UserInfo info = m_UserMgr.AddUser(id, resId);
            if (null != info)
            {
                info.SceneContext = m_SceneContext;
                info.SkillController = new SwordManSkillController(info, GfxModule.Skill.GfxSkillSystem.Instance);
                info.SkillController.Init();
                if (null != m_SpatialSystem)
                {
                    m_SpatialSystem.AddObj(info.SpaceObject);
                }
            }
            return info;
        }
        public void RelivePlayer()
        {
            UserInfo player = GetPlayerSelf();
            if (null != player)
            {
                player.SetHp(Operate_Type.OT_Absolute, player.GetActualProperty().HpMax);
                player.SetEnergy(Operate_Type.OT_Absolute, player.GetActualProperty().EnergyMax);
                player.DeadTime = 0;
                player.GfxDead = false;
                player.GetSkillStateInfo().RemoveAllImpact();
                player.GfxStateFlag = 0;
                CharacterView view = EntityManager.Instance.GetCharacterViewById(player.GetId());
                if (null != view)
                {
                    view.ObjectInfo.IsDead = false;
                }
                PlayerControl.Instance.EnableMoveInput = true;
                PlayerControl.Instance.EnableRotateInput = true;
                PlayerControl.Instance.EnableSkillInput = true;
            }
        }
        public UserInfo CreatePlayerSelf(int id, int resId)
        {
            m_PlayerSelf = CreateUser(id, resId);
            m_PlayerSelfId = id;
            return m_PlayerSelf;
        }

        public void NotifyMoveMeetObstacle(bool force)
        {
            if (!IsAlreadyNotifyMeetObstacle || force)
            {
                if (null != m_PlayerSelf)
                {
                    MovementStateInfo msi = m_PlayerSelf.GetMovementStateInfo();

                    Msg_CRC_MoveMeetObstacle msg = new Msg_CRC_MoveMeetObstacle();
                    msg.cur_pos_x = msi.PositionX;
                    msg.cur_pos_z = msi.PositionZ;
                    msg.send_time = TimeUtility.GetServerMilliseconds();
                    NetworkSystem.Instance.SendMessage(msg);

                    IsAlreadyNotifyMeetObstacle = true;
                }
            }
        }
        public void UpdateObserverCamera(float x, float y)
        {
            if (!m_IsFollowObserver)
            {
                GetCurScene().UpdateObserverCamera(x, y);
            }
        }
        public void HighlightPrompt(int id, params object[] args)
        {
            var str = Dict.Format(id, args);
        }
        public void RefixSkills(UserInfo user)
        {
            if (null != user)
            {
                if (null != LobbyClient.Instance.CurrentRole && null != LobbyClient.Instance.CurrentRole.SkillInfos)
                {
                    List<SkillInfo> skill_info_list = LobbyClient.Instance.CurrentRole.SkillInfos;
                    SkillInfo[] skill_assit = new SkillInfo[] { new SkillInfo(0), new SkillInfo(0), new SkillInfo(0), new SkillInfo(0) };
                    int cur_preset_index = 0;
                    if (cur_preset_index >= 0)
                    {
                        for (int i = 0; i < skill_assit.Length; i++)
                        {
                            for (int j = 0; j < skill_info_list.Count; j++)
                            {
                                if (skill_info_list[j].Postions.Presets[cur_preset_index] == (SlotPosition)(i + 1))
                                {
                                    skill_assit[i].SkillId = skill_info_list[j].SkillId;
                                    skill_assit[i].SkillLevel = skill_info_list[j].SkillLevel;
                                    break;
                                }
                            }
                        }
                    }
                    ///
                    user.GetSkillStateInfo().RemoveAllSkill();
                    for (int i = 0; i < skill_assit.Length; i++)
                    {
                        if (skill_assit[i].SkillId > 0)
                        {
                            SkillInfo info = new SkillInfo(skill_assit[i].SkillId);
                            info.SkillLevel = skill_assit[i].SkillLevel;
                            info.Postions.SetCurSkillSlotPos(0, (SlotPosition)(i + 1));
                            SkillCategory cur_skill_pos = SkillCategory.kNone;
                            if ((i + 1) == (int)SlotPosition.SP_A)
                            {
                                cur_skill_pos = SkillCategory.kSkillA;
                            }
                            else if ((i + 1) == (int)SlotPosition.SP_B)
                            {
                                cur_skill_pos = SkillCategory.kSkillB;
                            }
                            else if ((i + 1) == (int)SlotPosition.SP_C)
                            {
                                cur_skill_pos = SkillCategory.kSkillC;
                            }
                            else if ((i + 1) == (int)SlotPosition.SP_D)
                            {
                                cur_skill_pos = SkillCategory.kSkillD;
                            }
                            info.ConfigData.Category = cur_skill_pos;
                            user.GetSkillStateInfo().AddSkill(info);
                            ///
                            AddSubSkill(user, info.SkillId, cur_skill_pos, info.SkillLevel);
                        }
                    }
                    Data_PlayerConfig playerData = PlayerConfigProvider.Instance.GetPlayerConfigById(user.GetLinkId());
                    if (null != playerData && null != playerData.m_FixedSkillList
                      && playerData.m_FixedSkillList.Count > 0)
                    {
                        foreach (int skill_id in playerData.m_FixedSkillList)
                        {
                            if (null == user.GetSkillStateInfo().GetSkillInfoById(skill_id))
                            {
                                SkillInfo info = new SkillInfo(skill_id, 1);
                                user.GetSkillStateInfo().AddSkill(info);
                            }
                        }
                    }
                    user.ResetSkill();
                }
            }
        }
        public void PromptExceptionAndGotoMainCity()
        {
            if (WorldSystem.Instance.IsPvpScene() || WorldSystem.Instance.IsMultiPveScene())
            {
                OnRoomServerConnected();
            }
            if (WorldSystem.Instance.IsPveScene() || WorldSystem.Instance.IsMultiPveScene())
            {
                GfxSystem.PublishGfxEvent("ge_show_dialog", "ui", Dict.Get(11), Dict.Get(4), null, null, (MyAction<int>)((int btn) =>
                {
                    LobbyNetworkSystem.Instance.QuitRoom();
                    NetworkSystem.Instance.QuitBattle();
                    QueueAction((MyFunc<int, bool>)this.ChangeScene, LobbyClient.Instance.CurrentRole.CitySceneId);
                }), true);
            }
            else
            {
                if (!NetworkSystem.Instance.IsWaitStart)
                {
                    GfxSystem.PublishGfxEvent("ge_show_dialog", "ui", Dict.Get(10), Dict.Get(4), null, null, (MyAction<int>)((int btn) =>
                    {
                        LobbyNetworkSystem.Instance.QuitRoom();
                        NetworkSystem.Instance.QuitBattle();
                        QueueAction((MyFunc<int, bool>)this.ChangeScene, LobbyClient.Instance.CurrentRole.CitySceneId);
                    }), true);
                }
            }
        }
        public bool AddSubSkill(UserInfo user, int skill_id, SkillCategory pos, int level)
        {
            if (null == user)
                return false;
            SkillLogicData skill_data = SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_SKILL, skill_id) as SkillLogicData;
            if (null != skill_data && skill_data.NextSkillId > 0)
            {
                SkillInfo info = new SkillInfo(skill_data.NextSkillId);
                info.SkillLevel = level;
                info.ConfigData.Category = pos;
                user.GetSkillStateInfo().AddSkill(info);
                AddSubSkill(user, info.SkillId, pos, level);
            }
            return true;
        }

        public void QueueAction(MyAction action)
        {
            m_DelayActionProcessor.QueueAction(action);
        }
        public void QueueAction<T1>(MyAction<T1> action, T1 t1)
        {
            QueueActionWithDelegation(action, t1);
        }
        public void QueueAction<T1, T2>(MyAction<T1, T2> action, T1 t1, T2 t2)
        {
            QueueActionWithDelegation(action, t1, t2);
        }
        public void QueueAction<T1, T2, T3>(MyAction<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            QueueActionWithDelegation(action, t1, t2, t3);
        }
        public void QueueAction<T1, T2, T3, T4>(MyAction<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4);
        }
        public void QueueAction<T1, T2, T3, T4, T5>(MyAction<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6>(MyAction<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7>(MyAction<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
        }
        public void QueueAction<R>(MyFunc<R> action)
        {
            QueueActionWithDelegation(action);
        }
        public void QueueAction<T1, R>(MyFunc<T1, R> action, T1 t1)
        {
            QueueActionWithDelegation(action, t1);
        }
        public void QueueAction<T1, T2, R>(MyFunc<T1, T2, R> action, T1 t1, T2 t2)
        {
            QueueActionWithDelegation(action, t1, t2);
        }
        public void QueueAction<T1, T2, T3, R>(MyFunc<T1, T2, T3, R> action, T1 t1, T2 t2, T3 t3)
        {
            QueueActionWithDelegation(action, t1, t2, t3);
        }
        public void QueueAction<T1, T2, T3, T4, R>(MyFunc<T1, T2, T3, T4, R> action, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4);
        }
        public void QueueAction<T1, T2, T3, T4, T5, R>(MyFunc<T1, T2, T3, T4, T5, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, R>(MyFunc<T1, T2, T3, T4, T5, T6, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
        }
        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16)
        {
            QueueActionWithDelegation(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
        }
        public void QueueActionWithDelegation(Delegate action, params object[] args)
        {
            if (null != m_DelayActionProcessor)
            {
                m_DelayActionProcessor.QueueActionWithDelegation(action, args);
            }
        }
        public int PlayerSelfId
        {
            get { return m_PlayerSelfId; }
            set { m_PlayerSelfId = value; }
        }
        public int HeroId
        {
            get { return NetworkSystem.Instance.HeroId; }
        }
        public int CampId
        {
            get { return NetworkSystem.Instance.CampId; }
        }
        public SceneContextInfo SceneContext
        {
            get { return m_SceneContext; }
        }
        public ISpatialSystem SpatialSystem
        {
            get
            {
                return m_SpatialSystem;
            }
        }
        public SceneLogicInfoManager SceneLogicInfoManager
        {
            get
            {
                return m_SceneLogicInfoMgr;
            }
        }
        public NpcManager NpcManager
        {
            get
            {
                return m_NpcMgr;
            }
        }
        public UserManager UserManager
        {
            get
            {
                return m_UserMgr;
            }
        }

        public long SceneStartTime
        {
            get { return m_SceneStartTime; }
            set
            {
                m_SceneStartTime = value;
                m_SceneContext.StartTime = m_SceneStartTime;
            }
        }
        public bool IsObserver
        {
            get { return m_IsObserver; }
        }
        public bool IsFollowObserver
        {
            get { return m_IsFollowObserver; }
            set { m_IsFollowObserver = value; }
        }
        public int FollowTargetId
        {
            get { return m_FollowTargetId; }
            set { m_FollowTargetId = value; }
        }

        public bool IsAlreadyNotifyMeetObstacle
        {
            get { return m_IsAlreadyNotifyMeetObstacle; }
            set { m_IsAlreadyNotifyMeetObstacle = value; }
        }

        public bool IsWaitMatch
        {
            get { return m_IsWaitMatch; }
            set { m_IsWaitMatch = value; }
        }

        /**
         * @brief 构造函数
         *
         * @return 
         */
        private WorldSystem()
        {
            m_SceneContext.OnHighlightPrompt = (int userId, int dict, object[] args) =>
            {
                WorldSystem.Instance.HighlightPrompt(dict, args);
            };

            m_SceneContext.SpatialSystem = m_SpatialSystem;
            m_SceneContext.SceneLogicInfoManager = m_SceneLogicInfoMgr;
            m_SceneContext.NpcManager = m_NpcMgr;
            m_SceneContext.UserManager = m_UserMgr;
            m_SceneContext.BlackBoard = m_BlackBoard;

            m_NpcMgr.SetSceneContext(m_SceneContext);
            m_SceneLogicInfoMgr.SetSceneContext(m_SceneContext);

            m_AiSystem.SetNpcManager(m_NpcMgr);
            m_AiSystem.SetUserManager(m_UserMgr);
            m_SceneLogicSystem.SetSceneLogicInfoManager(m_SceneLogicInfoMgr);
        }

        private NpcManager m_NpcMgr = new NpcManager(256);
        private UserManager m_UserMgr = new UserManager(16);
        private SceneLogicInfoManager m_SceneLogicInfoMgr = new SceneLogicInfoManager(256);

        private AiSystem m_AiSystem = new AiSystem();
        private SceneLogicSystem m_SceneLogicSystem = new SceneLogicSystem();
        private SpatialSystem m_SpatialSystem = new SpatialSystem();
        private BlackBoard m_BlackBoard = new BlackBoard();

        private UserInfo m_PlayerSelf = null;
        private int m_PlayerSelfId = -1;

        private SceneContextInfo m_SceneContext = new SceneContextInfo();

        private const long c_InteractionCheckInterval = 1000;
        private long m_LastInteractionCheckTime = 0;
        private long m_SceneStartTime = 0;
        private long m_LastLogicTickTime = 0;

        private const long c_ChangeSceneTimeout = 60000;
        private long m_LastTryChangeSceneTime = 0;

        private bool m_IsObserver = false;
        private bool m_IsFollowObserver = false;
        private int m_FollowTargetId = 0;

        private bool m_IsWaitMatch = false;

        private WorldSystemProfiler m_Profiler = new WorldSystemProfiler();
        private SceneResource m_CurScene;

        private bool m_IsAlreadyNotifyMeetObstacle = false;
        private long m_LastNotifyMoveMeetObstacleTime = 0;

        private bool m_IsDebugObstacleCreated = false;
        private List<int> m_DebugObstacleActors = new List<int>();

        private DelayActionProcessor m_DelayActionProcessor = new DelayActionProcessor();

        private const long c_IntervalPerSecond = 5000;
        private long m_LastTickTimeForTickPerSecond = 0;
    }
}
