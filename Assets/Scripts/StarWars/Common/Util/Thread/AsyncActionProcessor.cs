using System;
using System.Collections.Generic;

namespace StarWars
{
    public sealed class AsyncActionProcessor : IActionQueue
    {
        public int CurActionNum
        {
            get
            {
                return m_ActionQueue.CurActionNum;
            }
        }

        public void QueueActionWithDelegation(Delegate action, params object[] args)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueActionWithDelegation(action, args);
            }
        }

        public void QueueAction(MyAction action)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action);
            }
        }

        public void QueueAction<T1>(MyAction<T1> action, T1 t1)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1);
            }
        }

        public void QueueAction<T1, T2>(MyAction<T1, T2> action, T1 t1, T2 t2)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2);
            }
        }

        public void QueueAction<T1, T2, T3>(MyAction<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3);
            }
        }

        public void QueueAction<T1, T2, T3, T4>(MyAction<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5>(MyAction<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6>(MyAction<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7>(MyAction<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
            }
        }

        public void QueueAction<R>(MyFunc<R> action)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action);
            }
        }

        public void QueueAction<T1, R>(MyFunc<T1, R> action, T1 t1)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1);
            }
        }

        public void QueueAction<T1, T2, R>(MyFunc<T1, T2, R> action, T1 t1, T2 t2)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2);
            }
        }

        public void QueueAction<T1, T2, T3, R>(MyFunc<T1, T2, T3, R> action, T1 t1, T2 t2, T3 t3)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3);
            }
        }

        public void QueueAction<T1, T2, T3, T4, R>(MyFunc<T1, T2, T3, T4, R> action, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, R>(MyFunc<T1, T2, T3, T4, T5, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, R>(MyFunc<T1, T2, T3, T4, T5, T6, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16)
        {
            lock (m_Lock)
            {
                m_ActionQueue.QueueAction(action, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
            }
        }

        public MyAction DequeueAction()
        {
            return m_ActionQueue.DequeueAction();
        }

        public void HandleActions(int maxCount)
        {
            m_ActionQueue.HandleActions(maxCount);
        }

        public void Reset()
        {
            lock (m_Lock)
            {
                m_ActionQueue.Reset();
            }
        }

        public void DebugPoolCount(MyAction<string> output)
        {
            lock (m_Lock)
            {
                m_ActionQueue.DebugPoolCount(output);
            }
        }

        public AsyncActionProcessor()
        {
            m_ActionQueue = new DelayActionProcessor(m_Lock);
        }

        private object m_Lock = new object();
        private DelayActionProcessor m_ActionQueue = null;
    }
}
