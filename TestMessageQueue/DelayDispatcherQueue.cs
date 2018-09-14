using System;
using System.Collections.Generic;

namespace TestMessageQueue
{
    /// <summary>
    /// 延时队列
    /// </summary>
    public class DelayDispatcherQueue<T>
    {
        public delegate void Get(T t);
        public event Get GetEvent;

        private readonly LinkedList<Item<T>>[] _list;
        private readonly int _length;
        private int _pointer;   //当前时间所指向的元素
        private int _count;     //该延时队列中包含的元素总数
        private readonly object _lockObj;
        private readonly MillisecondTimer _timer;

        /// <summary>
        /// 创建一个延时发送对象
        /// </summary>        
        // 逻辑上的队列，代码层面上表现为容量为[capacity]的链表指针数组，一个单位各自对应一个链表
        public DelayDispatcherQueue(int capacity)
        {
            _list = new LinkedList<Item<T>>[capacity];
            _length = capacity;
            _pointer = 0;
            _count = 0;
            _lockObj = new object();

            //创建一个定时器，每个1000毫秒触发一次
            _timer = new MillisecondTimer {Interval = 1000};
            _timer.Tick += TimerOnTick;
            _timer.Start();
        }

        public int Count()
        {
            return _count;
        }

        /// <summary>
        /// 往延时发送对象中加入一个新的项目
        /// </summary>
        /// <param name="t">加入的项目</param>
        /// <param name="delayTime">项目延时发送的时间间隔，单位为秒</param>
        public void Enqueue(T t, int delayTime = 840)
        {
            int localPoint = _pointer;
            int cycleNum = delayTime / _length;     //加入的元素需要被循环的次数
            Item<T> item = new Item<T>(t, cycleNum);

            int index = (delayTime + localPoint) % _length;
            if (_list[index] == null)
                _list[index] = new LinkedList<Item<T>>();
            _list[index].AddLast(item);             //将新的元素放到链表的表尾
            _count++;
        }

        //每隔1s，扫描下一个数组单位
        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            lock (_lockObj)
            {
                _pointer++;
                if (_pointer == _length)
                    _pointer = 0;
            }
            var localPoint = _pointer;

            if (_list[localPoint] == null)
                return;

            while (_list[localPoint].Count > 0 && _list[localPoint].First.Value.CycleNum <= 0)
            {
                if (GetEvent != null)
                    GetEvent(_list[localPoint].First.Value.Object); //将过期元素从延时队列中抛出
                _list[localPoint].RemoveFirst();
                _count--;
            }
        }

        private class Item<T>
        {
            public T Object { get; private set; }
            public int CycleNum { get; private set; }

            public Item(T o, int cycleNum)
            {
                Object = o;
                CycleNum = cycleNum;
            }
        }
    }
}