using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWars
{
    public sealed class ControlSystem
    {
        public void Reset()
        {
            int ct = m_Controllers.Count;
            for (int ix = ct - 1; ix >= 0; --ix)
            {
                IController ctrl = m_Controllers[ix];
                ctrl.Recycle();
            }
            m_Controllers.Clear();
        }
        public bool ExistController(int id)
        {
            return null != GetController(id);
        }
        public IController GetController(int id)
        {
            int index;
            return GetController(id, out index);
        }
        public void AddController(IController ctrl)
        {
            if (null != ctrl)
            {
                int index;
                IController oldCtrl = GetController(ctrl.Id, out index);
                if (null != oldCtrl)
                {
                    m_Controllers[index] = ctrl;
                    oldCtrl.Recycle();
                }
                else
                {
                    m_Controllers.Add(ctrl);
                }
            }
        }
        public void Tick()
        {
            try
            {
                int ct = m_Controllers.Count;
                for (int ix = ct - 1; ix >= 0; --ix)
                {
                    IController ctrl = m_Controllers[ix];
                    ctrl.Adjust();
                    if (ctrl.IsTerminated)
                    {
                        ctrl.Recycle();
                        m_Controllers.RemoveAt(ix);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug("ControlSystem Exception:%s\n%s", ex.Message, ex.StackTrace);
            }
        }

        private IController GetController(int id, out int index)
        {
            IController ctrl = null;
            index = -1;
            int ct = m_Controllers.Count;
            for (int ix = 0; ix < ct; ++ix)
            {
                IController t = m_Controllers[ix];
                if (t.Id == id)
                {
                    index = ix;
                    ctrl = t;
                    break;
                }
            }
            return ctrl;
        }

        private List<IController> m_Controllers = new List<IController>();
    }
}
