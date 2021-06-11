using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWars
{
    public enum ControllerType : int
    {
        Position = 0,
        MoveDir = 1,
        FaceDir = 2,
        MaxNum
    }
    public sealed class ControllerIdCalculator
    {
        public static int Calc(ControllerType type, int id)
        {
            return ((int)type << 24) + id;
        }
    }
    public interface IController
    {
        int Id { get; }
        bool IsTerminated { get; }
        void Adjust();
        void Recycle();
    }
    public abstract class AbstractController<T> : IController, IPoolAllocatedObject<T> where T : AbstractController<T>, new()
    {
        public int Id
        {
            get { return m_Id; }
        }
        public bool IsTerminated
        {
            get { return m_IsTerminated; }
        }
        public abstract void Adjust();
        public void Recycle()
        {
            m_IsTerminated = false;
            if (null != m_Pool)
            {
                m_Pool.Recycle(this);
            }
        }
        public void InitPool(ObjectPool<T> pool)
        {
            m_DowncastObj = this as T;
            m_Pool = pool;
        }
        public T Downcast()
        {
            return m_DowncastObj;
        }

        protected int m_Id = 0;
        protected bool m_IsTerminated = false;
        private T m_DowncastObj = null;
        private ObjectPool<T> m_Pool = null;
    }
}
