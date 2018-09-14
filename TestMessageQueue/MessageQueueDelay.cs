namespace TestMessageQueue
{
    /**
     * 若通过接口实现，则只能调用Enqueue(T t)函数，将加入的所有元素的定时时间设为一致
     * 如果要对每个元素进行单独的定时设置，可以不实现接口，直接实现MessageQueueDelay<T>对象，调用Enqueue(T t, int delayTime)函数即可
     */
    //延时阻塞队列
    public class MessageQueueDelay<T>: IMessageQueue<T>
    {
        private int _count;
        private readonly int _delayTime;
        private readonly SimpleSynQueue<T> _synQueue;
        private readonly DelayDispatcherQueue<T> _delayDispatcherQueue;
        private readonly object _lockObj;   //保证_count的多线程同步

        /// <summary>
        /// 创建一个延时的阻塞队列
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="delayTime">队列中每个元素的延时时间</param>
        public MessageQueueDelay(int capacity, int delayTime)
        {
            _count = 0;
            _delayTime = delayTime;

            /**
             * 先将元素保存于延时队列中
             * 若过期，则从延时队列抛出
             * 将延时队列抛出的元素保存至缓冲队列中
             * 若缓冲队列中存在元素，则元素可被取出；否则进入阻塞状态
             */
            _synQueue = new SimpleSynQueue<T>();
            _delayDispatcherQueue = new DelayDispatcherQueue<T>(capacity);            
            _delayDispatcherQueue.GetEvent += delegate(T t) { _synQueue.Enqueue(t); };

            _lockObj = new object();
        }

        public int Count()
        {
            return _count;
        }
        
        //所有元素，按照构造函数中的定时事假加入延时队列
        public void Enqueue(T t)
        {
            _delayDispatcherQueue.Enqueue(t, _delayTime);
            lock (_lockObj)
            {
                _count++;
            }
        }

        //未在接口中注册该函数，可以实现设置每个加入延时队列的元素的定时时间
        public void Enqueue(T t, int delayTime)
        {
            _delayDispatcherQueue.Enqueue(t, delayTime);
            lock (_lockObj)
            {
                _count++;
            }
        }

        public T Dequeue()
        {
            T t = _synQueue.Dequeue();
            lock (_lockObj)
            {
                _count--;
            }
            return t;
        }
    }
}