namespace TestMessageQueue
{
    //非延时阻塞队列
    public class MessageQueue<T> : IMessageQueue<T>
    {
        private readonly SimpleSynQueue<T> _synQueue;

        /// <summary>
        /// 创建一个非延时的阻塞队列
        /// </summary>
        public MessageQueue()
        {
            _synQueue = new SimpleSynQueue<T>();
        }

        public int Count()
        {
            return _synQueue.Count();
        }

        public void Enqueue(T t)
        {
            _synQueue.Enqueue(t);
        }

        public T Dequeue()
        {
            T t = _synQueue.Dequeue();
            return t;
        }
    }
}