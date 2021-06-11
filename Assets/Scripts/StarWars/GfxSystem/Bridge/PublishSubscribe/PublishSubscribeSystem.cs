using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWars
{
    public sealed class ProxyPublishData
    {
        public string m_EventName;
        public string m_Group;
        public object[] m_Args;
    }
    public sealed class PublishSubscribeSystem
    {
        private class ReceiptInfo
        {
            public string name_;
            public Delegate delegate_;
            public ReceiptInfo() { }
            public ReceiptInfo(string n, Delegate d)
            {
                name_ = n;
                delegate_ = d;
            }
        }

        public bool RunInLogicThread
        {
            get { return run_in_logic_thread_; }
            set { run_in_logic_thread_ = value; }
        }

        public object Subscribe(string ev_name, string group, MyAction subscriber) { return AddSubscriber(ev_name, group, subscriber); }
        public object Subscribe<T0>(string ev_name, string group, MyAction<T0> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
        public object Subscribe<T0, T1>(string ev_name, string group, MyAction<T0, T1> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
        public object Subscribe<T0, T1, T2>(string ev_name, string group, MyAction<T0, T1, T2> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
        public object Subscribe<T0, T1, T2, T3>(string ev_name, string group, MyAction<T0, T1, T2, T3> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
        public object Subscribe<T0, T1, T2, T3, T4>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
        public object Subscribe<T0, T1, T2, T3, T4, T5>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
        public object Subscribe<T0, T1, T2, T3, T4, T5, T6>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5, T6> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
        public object Subscribe<T0, T1, T2, T3, T4, T5, T6, T7>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5, T6, T7> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
        public object Subscribe<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
        public object Subscribe<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
        public object Subscribe<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
        public object Subscribe<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> subscriber) { return AddSubscriber(ev_name, group, subscriber); }

        public void Unsubscribe(object receipt)
        {
            ReceiptInfo r = receipt as ReceiptInfo;
            Delegate d;
            if (null != r && subscribers_.TryGetValue(r.name_, out d))
            {
                subscribers_[r.name_] = Delegate.Remove(d, r.delegate_);
            }

        }

        public void ProxyPublish(string ev_name, string group, params object[] parameters)
        {
            ProxyPublishData arg = new ProxyPublishData();
            arg.m_EventName = ev_name;
            arg.m_Group = group;
            arg.m_Args = parameters;

            if (RunInLogicThread)
                LogSystem.Info("ProxyPublish {0} {1}", ev_name, group);
            else
                LogicSystem.LogicLog("ProxyPublish {0} {1}", ev_name, group);

            Publish(c_proxy_ev_name, c_proxy_group, arg);
        }

        public void ProxySubscribe(MyAction<ProxyPublishData> subscriber)
        {
            AddSubscriber(c_proxy_ev_name, c_proxy_group, subscriber);
        }

        public void Publish(string ev_name, string group, params object[] parameters)
        {
            try
            {
                if (RunInLogicThread)
                    LogSystem.Info("Publish {0} {1}", ev_name, group);
                else
                    LogicSystem.LogicLog("Publish {0} {1}", ev_name, group);

                Delegate d;
                string key = group + '#' + ev_name;
                if (subscribers_.TryGetValue(key, out d))
                {
                    if (null == d)
                    {
                        if (RunInLogicThread)
                            LogSystem.Error("Publish {0} {1}, Subscriber is null, Remove it", ev_name, group);
                        else
                            LogicSystem.LogicErrorLog("Publish {0} {1}, Subscriber is null, Remove it", ev_name, group);
                        subscribers_.Remove(key);
                    }
                    else
                    {
                        d.DynamicInvoke(parameters);
                    }
                }
            }
            catch (Exception ex)
            {
                if (RunInLogicThread)
                    LogSystem.Error("PublishSubscribe.Publish({0},{1}) exception:{2}\n{3}", ev_name, group, ex.Message, ex.StackTrace);
                else
                    LogicSystem.LogicErrorLog("PublishSubscribe.Publish({0},{1}) exception:{2}\n{3}", ev_name, group, ex.Message, ex.StackTrace);
            }
        }

        private object AddSubscriber(string ev_name, string group, Delegate d)
        {
            Delegate source;
            string key = group + '#' + ev_name;
            if (subscribers_.TryGetValue(key, out source))
            {
                if (null != source)
                    source = Delegate.Combine(source, d);
                else
                    source = d;
            }
            else
            {
                source = d;
            }
            subscribers_[key] = source;
            return new ReceiptInfo(key, d);
        }

        private Dictionary<string, Delegate> subscribers_ = new Dictionary<string, Delegate>();
        private bool run_in_logic_thread_ = true;

        private const string c_proxy_ev_name = "sys_proxy_publish";
        private const string c_proxy_group = "sys";
    }
}
