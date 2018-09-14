using System.Collections.Generic;
using System.Threading;

namespace TestMessageQueue
{
    /// <summary>
    /// 简易版的阻塞队列
    /// </summary>
    public class SimpleSynQueue<T>
    {
        /**
         * 简单地说，AutoResetEvent分为T和F两种状态
         * 调用Set函数，使F变成T状态
         * 调用WaitOne函数，使T变成F状态（若设置超时，则超时后由F变成T状态）
         * T状态能让程序继续运行，F状态则会让程序卡住
         */

        private readonly Queue<T> _queue;
        private readonly AutoResetEvent _ars;
        private readonly AutoResetEvent _ars2;
        private int _count;        

        public SimpleSynQueue()
        {
            _queue = new Queue<T>();
            _ars = new AutoResetEvent(true);        //初始状态为终止状态，即自动调用一次Set()，允许一个或多个等待线程继续运行
            _ars2 = new AutoResetEvent(false);      //初始状态为非终止状态，即需要手动调用Set()，阻止当前线程
            _count = 0;
        }

        public int Count()
        {
            return _count;
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="t"></param>
        public void Enqueue(T t)
        {
            if (t == null) return;
            _ars.WaitOne(Timeout.Infinite, true);
            _queue.Enqueue(t);
            _count++;
            _ars2.Set();
            _ars.Set();
        }

        /// <summary>
        /// 出队，如果没内容则阻塞
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            _ars.WaitOne(Timeout.Infinite, true);
            while (_queue.Count == 0)
            {
                _ars.Set();
                _ars2.WaitOne(Timeout.Infinite, true);
                _ars.WaitOne(Timeout.Infinite, true);
            }
            T ret = _queue.Dequeue();
            _count--;
            _ars.Set();            
            return ret;
        }
    }
}