using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWars
{
    public class SpaceInfoView
    {
        public int Id
        {
            get
            {
                return m_ObjId;
            }
        }
        public bool NeedDestroy
        {
            get { return m_NeedDestroy; }
            set { m_NeedDestroy = value; }
        }
        public SharedGameObjectInfo ObjectInfo
        {
            get { return m_ObjectInfo; }
        }

        public void Create(int objId, bool isPlayer)
        {
            m_ObjId = objId;
            m_Actor = GameObjectIdManager.Instance.GenNextId();
            //if (isPlayer)
            //    GfxSystem.CreateGameObject(m_Actor, "BlueCylinder", m_ObjectInfo);
            //else
            //    GfxSystem.CreateGameObject(m_Actor, "RedCylinder", m_ObjectInfo);
        }

        public void Destroy()
        {
            //GfxSystem.DestroyGameObject(m_Actor);
        }

        public void Update(float x, float y, float z, float dir)
        {
            m_ObjectInfo.x = x;
            m_ObjectInfo.y = 0;
            m_ObjectInfo.z = z;
            m_ObjectInfo.FaceDir = dir;
            m_ObjectInfo.DataChangedByLogic = true;
        }

        private int m_Actor = 0;
        private int m_ObjId = 0;
        private bool m_NeedDestroy = false;
        private SharedGameObjectInfo m_ObjectInfo = new SharedGameObjectInfo();
    }
}
