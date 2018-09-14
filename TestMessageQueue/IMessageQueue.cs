namespace TestMessageQueue
{
    //阻塞队列接口
    public interface IMessageQueue<T>
    {
        /// <summary>
        /// 返回队列内元素数量
        /// </summary>
        int Count();

        /// <summary>
        /// 将元素加入队列
        /// </summary>
        void Enqueue(T t);

        /// <summary>
        /// 从队列中取出元素
        /// </summary>
        T Dequeue();
    }
}