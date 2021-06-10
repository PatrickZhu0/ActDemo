using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWars
{
    public sealed class UserManager
    {
        public DamageDelegation OnDamage;
        public GainMoneyDelegation OnGainMoney;

        public UserManager(int poolSize)
        {
            m_UserPoolSize = poolSize;
        }

        public LinkedListDictionary<int, UserInfo> Users
        {
            get { return m_Users; }
        }

        public UserInfo GetUserInfo(int id)
        {
            UserInfo info;
            Users.TryGetValue(id, out info);
            return info;
        }

        public UserInfo AddUser(int resId)
        {
            UserInfo user = NewUserInfo();
            user.LoadData(resId);
            m_Users.AddLast(user.GetId(), user);
            return user;
        }

        public UserInfo AddUser(int id, int resId)
        {
            UserInfo user = NewUserInfo(id);
            user.LoadData(resId);
            m_Users.AddLast(user.GetId(), user);
            return user;
        }

        public void RemoveUser(int id)
        {
            UserInfo user = GetUserInfo(id);
            if (null != user)
            {
                m_Users.Remove(id);
                user.SceneContext = null;
                RecycleUserInfo(user);
            }
        }

        public void Reset()
        {
            m_Users.Clear();
            m_UnusedUsers.Clear();
            m_NextInfoId = c_StartId;
        }

        public void FireDamageEvent(int receiver, int sender, bool isShootDamage, bool isCritical, int hpDamage, int npDamage)
        {
            if (OnDamage != null)
            {
                OnDamage(receiver, sender, isShootDamage, isCritical, hpDamage, npDamage);
            }
        }
        public void FireGainMoneyEvent(int receiver, int money)
        {
            if (OnGainMoney != null)
            {
                OnGainMoney(receiver, money);
            }
        }

        private UserInfo NewUserInfo()
        {
            UserInfo user = null;
            int id = GenNextId();
            if (m_UnusedUsers.Count > 0)
            {
                user = m_UnusedUsers.Dequeue();
                user.Reset();
                user.InitId(id);
            }
            else
            {
                user = new UserInfo(id);
            }
            return user;
        }

        private UserInfo NewUserInfo(int id)
        {
            UserInfo user = null;
            if (m_UnusedUsers.Count > 0)
            {
                user = m_UnusedUsers.Dequeue();
                user.Reset();
                user.InitId(id);
            }
            else
            {
                user = new UserInfo(id);
            }
            return user;
        }

        private void RecycleUserInfo(UserInfo userInfo)
        {
            if (null != userInfo && m_UnusedUsers.Count < m_UserPoolSize)
            {
                userInfo.Reset();
                m_UnusedUsers.Enqueue(userInfo);
            }
        }

        private int GenNextId()
        {
            int id = 0;
            for (int i = 0; i < c_MaxIdNum; ++i)
            {
                id = (m_NextInfoId + i - c_StartId) % c_MaxIdNum + c_StartId;
                if (!m_Users.Contains(id))
                    break;
            }
            if (id > 0)
            {
                m_NextInfoId = (id + 1 - c_StartId) % c_MaxIdNum + c_StartId;
            }
            return id;
        }

        private LinkedListDictionary<int, UserInfo> m_Users = new LinkedListDictionary<int, UserInfo>();
        private Queue<UserInfo> m_UnusedUsers = new Queue<UserInfo>();
        private int m_UserPoolSize = 1024;

        private const int c_StartId = 1;
        private const int c_MaxIdNum = 19999;
        private int m_NextInfoId = c_StartId;
    }
}
