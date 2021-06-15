
/**
 * @file EntityManager.cs
 * @brief 角色管理器
 *
 * @author 
 * @version 0
 * @date 2012-11-14
 */

using System;
using System.Collections.Generic;
using UnityEngine;
//using System.Diagnostics;
//using StarWars.Network;

namespace StarWars
{
    /// <summary>
    /// 角色管理器
    /// 这个类在GameObjects采取数据驱动的方式后，它的职责变为GameObjects的View层，它在每个Tick负责更新各个GameObject的显示
    /// </summary>
    public sealed class EntityManager
    {
        #region Singleton
        private static EntityManager s_instance_ = new EntityManager();
        public static EntityManager Instance
        {
            get { return s_instance_; }
        }
        #endregion

        private EntityManager()
        {
        }

        public void Init()
        {
        }

        public void Release()
        {
        }

        public void Tick()
        {
            Debug.LogError("Entity Managet .Tick() ..." + GfxSystem.SyncLock);
            lock (GfxSystem.SyncLock)
            {
                Debug.LogError("Entity Managet .m_UserViews.count ..." + m_UserViews.Count);
                foreach (UserView view in m_UserViews.Values)
                {
                    Debug.LogError(view.ObjId);
                    view.Update();
                }
            }
            if (m_SpaceInfoViews.Count > 0)
            {
                if (!GlobalVariables.Instance.IsDebug)
                {
                    EntityManager.Instance.MarkSpaceInfoViews();
                    EntityManager.Instance.DestroyUnusedSpaceInfoViews();
                }
            }
        }

        public void CreatePlayerSelfView(int objId)
        {
            CreateUserView(objId);
            UserView view = GetUserViewById(objId);
            if (null != view)
            {
                //GfxSystem.SendMessage("GfxGameRoot", "CameraFollowImmediately", view.Actor);
                GfxSystem.ResetInputState();
            }
        }

        public void CreateUserView(int objId)
        {
            if (!m_UserViews.ContainsKey(objId))
            {

                UserInfo obj = WorldSystem.Instance.UserManager.GetUserInfo(objId);
                if (null != obj)
                {
                    UserView view = new UserView();
                    view.Create(obj);
                    m_UserViews.Add(objId, view);
                }
            }
        }

        public void DestroyUserView(int objId)
        {
            if (m_UserViews.ContainsKey(objId))
            {
                UserView view = m_UserViews[objId];
                if (view != null)
                {
                    view.Destroy();
                }
                m_UserViews.Remove(objId);
            }
        }

        public UserView GetUserViewById(int objId)
        {
            UserView view = null;
            if (m_UserViews.ContainsKey(objId))
                view = m_UserViews[objId];
            return view;
        }

        public NpcView GetNpcViewById(int objId)
        {
            NpcView view = null;
            if (m_NpcViews.ContainsKey(objId))
                view = m_NpcViews[objId];
            return view;
        }

        public CharacterView GetCharacterViewById(int objId)
        {
            CharacterView view = GetUserViewById(objId);
            if (null == view)
                view = GetNpcViewById(objId);
            return view;
        }

        public bool IsVisible(int objId)
        {
            bool ret = false;
            CharacterView view = GetCharacterViewById(objId);
            if (null != view)
            {
                ret = view.Visible;
            }
            return ret;
        }

        public void MarkSpaceInfoViews()
        {
            foreach (SpaceInfoView view in m_SpaceInfoViews.Values)
            {
                view.NeedDestroy = true;
            }
        }

        public void UpdateSpaceInfoView(int objId, bool isPlayer, float x, float y, float z, float dir)
        {
            SpaceInfoView view = GetSpaceInfoViewById(objId);
            if (null == view)
            {
                view = CreateSpaceInfoView(objId, isPlayer);
            }
            if (null != view)
            {
                view.NeedDestroy = false;
                view.Update(x, y, z, dir);
            }
        }

        public void DestroyUnusedSpaceInfoViews()
        {
            List<int> deletes = new List<int>();
            foreach (SpaceInfoView view in m_SpaceInfoViews.Values)
            {
                if (view.NeedDestroy)
                    deletes.Add(view.Id);
            }
            foreach (int id in deletes)
            {
                DestroySpaceInfoView(id);
            }
            deletes.Clear();
        }

        /// <summary>
        /// 检验是否在某位置
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsAtPosition(CharacterInfo entity, Vector2 pos)
        {
            //return Vector2.Distance(entity.GetMovementStateInfo().GetPosition2D(), pos) < ClientConfig.s_PositionRefix;
            return Vector2.Distance(entity.GetMovementStateInfo().GetPosition2D(), pos) < 1;
        }

        /// <summary>
        /// 预估移动时间
        /// </summary>
        /// <param name="character"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public float PredictMoveDuration(CharacterInfo character, Vector2 pos)
        {
            float distance = Vector2.Distance(character.GetMovementStateInfo().GetPosition2D(), pos);
            float duration = distance / character.GetActualProperty().MoveSpeed;
            duration += ClientConfig.s_PredictMoveDurationRefix;

            return duration;
        }

        private SpaceInfoView CreateSpaceInfoView(int objId, bool isPlayer)
        {
            SpaceInfoView view = null;
            if (!m_SpaceInfoViews.ContainsKey(objId))
            {
                view = new SpaceInfoView();
                view.Create(objId, isPlayer);
                m_SpaceInfoViews.Add(objId, view);
            }
            return view;
        }

        private void DestroySpaceInfoView(int objId)
        {
            if (m_SpaceInfoViews.ContainsKey(objId))
            {
                SpaceInfoView view = m_SpaceInfoViews[objId];
                if (view != null)
                {
                    view.Destroy();
                }
                m_SpaceInfoViews.Remove(objId);
            }
        }

        private SpaceInfoView GetSpaceInfoViewById(int objId)
        {
            SpaceInfoView view = null;
            if (m_SpaceInfoViews.ContainsKey(objId))
                view = m_SpaceInfoViews[objId];
            return view;
        }

        private Dictionary<int, UserView> m_UserViews = new Dictionary<int, UserView>();
        private Dictionary<int, NpcView> m_NpcViews = new Dictionary<int, NpcView>();
        private Dictionary<int, SpaceInfoView> m_SpaceInfoViews = new Dictionary<int, SpaceInfoView>();
    }
}
