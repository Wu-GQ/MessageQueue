using System;
using System.Threading;

namespace TestMessageQueue
{
    // 缺陷：MillisecondTimer定时器的准确度
    class Program
    {
        private static IMessageQueue<long> _messageQueue;

        // 测试函数
        static void Main()
        {
            _messageQueue = new MessageQueueDelay<long>(900, 10);

            new Thread(Thread1).Start();

            new Thread(Thread2).Start();
        }

        // 将元素加入队列的时间转化为long类型后，加入队列
        static void Thread1()
        {
            while (true)
            {
                DateTime time = DateTime.Now;
                _messageQueue.Enqueue(DateTime2Utc(time));
                Console.WriteLine("Enqueue: [{0:hh:mm:ss.fff}]", time);
                Thread.Sleep(1000);
            }
        }

        // 从队列中取出元素并转化为DateTime类型显示
        static void Thread2()
        {
            while (true)
            {
                Console.WriteLine("\t\t\t\tDequeue: [{0:hh:mm:ss.fff}]", Utc2DateTime(_messageQueue.Dequeue()));
            }
        }
        
        #region DateTime-Unix时间戳转换

        static DateTime Utc2DateTime(long utc, bool isLocal = false)
        {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0);
            if (isLocal)
                time = time.AddHours(8);
            return time.AddMilliseconds(utc);
        }

        static long DateTime2Utc(DateTime time)
        {
            if (time.Kind == DateTimeKind.Local)
                return (long)time.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)).TotalMilliseconds;
            return (long)time.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
        }

        #endregion
    }
}
