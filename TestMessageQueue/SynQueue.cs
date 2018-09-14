using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace QuoteServer_FX
{
    public class SynQueue<T>
    {
        #region "Variables"

        private List<T> qlist;
        private AutoResetEvent ars;
        private AutoResetEvent ars2;

        #endregion

        public SynQueue()
        {
            qlist = new List<T>();
            ars = new AutoResetEvent(true);
            ars2 = new AutoResetEvent(false);
        }

        public int Count()
        {
            int ret = 0;
            ars.WaitOne(Timeout.Infinite, true);
            ret = qlist.Count;
            ars.Set();
            return ret;
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="t"></param>
        public void Enqueue(T t)
        {
            if (t == null) return;
            ars.WaitOne(Timeout.Infinite, true);
            qlist.Add(t);
            ars2.Set();
            ars.Set();
        }

        /// <summary>
        /// 出队，如果没内容则阻塞
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            ars.WaitOne(Timeout.Infinite, true);
            while (qlist.Count == 0)
            {
                ars.Set();
                ars2.WaitOne(Timeout.Infinite, true);
                ars.WaitOne(Timeout.Infinite, true);
            }
            T ret = qlist.ElementAt(0);
            qlist.RemoveAt(0);
            ars.Set();
            return ret;
        }

        /// <summary>
        /// 取得队首元素，如果没内容则阻塞
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            ars.WaitOne(Timeout.Infinite, true);
            while (qlist.Count == 0)
            {
                ars.Set();
                ars2.WaitOne(Timeout.Infinite, true);
                ars.WaitOne(Timeout.Infinite, true);
            }
            T ret = qlist.ElementAt(0);
            ars.Set();
            return ret;
        }

        /// <summary>
        /// 全部出队，如果没有则阻塞
        /// </summary>
        /// <returns></returns>
        public List<T> DequeueAll(int timeout = Timeout.Infinite)
        {
            ars.WaitOne(Timeout.Infinite, true);
            while (qlist.Count == 0)
            {
                ars.Set();
                ars2.WaitOne(timeout, true);
                if (qlist.Count == 0 && timeout != Timeout.Infinite)
                {
                    //若为超时
                    return null;
                }
                ars.WaitOne(Timeout.Infinite, true);
            }
            List<T> ret = new List<T>();
            ret.AddRange(qlist);

            qlist.Clear();
            ars.Set();
            return ret;
        }

        /// <summary>
        /// 取得所有元素，如果没内容则阻塞
        /// </summary>
        /// <returns></returns>
        public List<T> PeekAll()
        {
            ars.WaitOne(Timeout.Infinite, true);
            while (qlist.Count == 0)
            {
                ars.Set();
                ars2.WaitOne(Timeout.Infinite, true);
                ars.WaitOne(Timeout.Infinite, true);
            }

            List<T> ret = new List<T>();
            ret.AddRange(qlist);

            ars.Set();
            return ret;
        }

        /// <summary>
        /// 删除元素t
        /// </summary>
        /// <param name="t"></param>
        public void RemoveItem(T t)
        {
            ars.WaitOne(Timeout.Infinite, true);
            qlist.Remove(t);
            ars.Set();
        }
    }
}
