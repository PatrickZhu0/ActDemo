
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
    /**
     * @brief 角色管理器
     * @remarks 这个类在GameObjects采取数据驱动的方式后，它的职责变为GameObjects的View层，它在每个Tick负责更新各个GameObject的显示
     */
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
            //lock (GfxSystem.SyncLock)
            //{
            //    foreach (UserView view in m_UserViews.Values)
            //    {
            //        view.Update();
            //    }

            //    foreach (NpcView view in m_NpcViews.Values)
            //    {
            //        view.Update();
            //    }
            //}
            //if (m_SpaceInfoViews.Count > 0)
            //{
            //    if (!GlobalVariables.Instance.IsDebug)
            //    {
            //        EntityManager.Instance.MarkSpaceInfoViews();
            //        EntityManager.Instance.DestroyUnusedSpaceInfoViews();
            //    }
            //}
        }

        public void CreatePlayerSelfView(int objId)
        {
            CreateUserView(objId);
            UserView view = GetUserViewById(objId);
            if (null != view)
            {
                //GfxSystem.SendMessage("GfxGameRoot", "CameraFollowImmediately", view.Actor);
                //GfxSystem.ResetInputState();
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

        public void CreateNpcView(int objId)
        {
            if (!m_NpcViews.ContainsKey(objId))
            {

                //NpcInfo obj = WorldSystem.Instance.NpcManager.GetNpcInfo(objId);
                //if (null != obj)
                //{
                //    NpcView view = new NpcView();
                //    view.Create(obj);
                //    m_NpcViews.Add(objId, view);
                //}
                //WorldSystem.Instance.SyncGfxNpcInfo(objId);
            }
        }

        public void DestroyNpcView(int objId)
        {
            if (m_NpcViews.ContainsKey(objId))
            {
                //NpcView view = m_NpcViews[objId];
                //if (view != null)
                //{
                //    view.Destroy();
                //}
                //m_NpcViews.Remove(objId);
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

        /**
         * @brief 检验是否在某位置
         *
         * @param pos
         *
         * @return 
         */
        public bool IsAtPosition(CharacterInfo entity, Vector2 pos)
        {
            //return Vector2.Distance(entity.GetMovementStateInfo().GetPosition2D(), pos) < ClientConfig.s_PositionRefix;
            return Vector2.Distance(entity.GetMovementStateInfo().GetPosition2D(), pos) < 1;
        }

        /**
         * @brief 预估移动时间
         *
         * @param pos
         *
         * @return 
         */
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
