using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace TestMessageQueue
{
    /// <summary>
    /// 定时器
    /// </summary>
    // 保证定时器的准确程度是延时队列的关键问题，如果能找到更加完美的定时器可以对该定时器进行替换。
    // 该定时器来源：https://www.cnblogs.com/dehai/p/4347061.html
    public sealed class MillisecondTimer : IComponent
    {
        #region 私有字段

        private static TimerCaps _caps;
        private int _interval;
        private bool _isRunning;
        private readonly int _resolution;
        private readonly TimerCallback _timerCallback;
        private int _timerId;

        #endregion

        #region 属性

        /// <summary>
        /// 
        /// </summary>
        public int Interval
        {
            get
            {
                return _interval;
            }
            set
            {
                if (value < _caps.periodMin || value > _caps.periodMax)
                {
                    throw new Exception("超出计时范围！");
                }
                _interval = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ISite Site { set; get; }

        #endregion

        #region 事件

        public event EventHandler Disposed;  // 这个事件实现了IComponet接口
        public event EventHandler Tick;

        #endregion

        #region 构造函数和析构函数

        static MillisecondTimer()
        {
            timeGetDevCaps(ref _caps, Marshal.SizeOf(_caps));
        }

        public MillisecondTimer()
        {
            _interval = _caps.periodMin; // 
            _resolution = _caps.periodMin; //

            _isRunning = false;
            _timerCallback = TimerEventCallback;
        }

        public MillisecondTimer(IContainer container)
            : this()
        {
            container.Add(this);
        }

        ~MillisecondTimer()
        {
            timeKillEvent(_timerId);
        }

        #endregion

        #region 公有方法

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            if (!_isRunning)
            {
                _timerId = timeSetEvent(_interval, _resolution, _timerCallback, 0, 1); // 间隔性地运行

                if (_timerId == 0)
                {
                    throw new Exception("无法启动计时器");
                }
                _isRunning = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            if (_isRunning)
            {
                timeKillEvent(_timerId);
                _isRunning = false;
            }
        }

        /// <summary>
        /// 实现IDisposable接口
        /// </summary>
        public void Dispose()
        {
            timeKillEvent(_timerId);
            GC.SuppressFinalize(this);
            EventHandler disposed = Disposed;
            if (disposed != null)
            {
                disposed(this, EventArgs.Empty);
            }
        }

            #endregion

        #region 内部函数

        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, TimerCallback callback, int user, int mode);


        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int id);


        [DllImport("winmm.dll")]
        private static extern int timeGetDevCaps(ref TimerCaps caps, int sizeOfTimerCaps);
        //  The timeGetDevCaps function queries the timer device to determine its resolution. 

        private void TimerEventCallback(int id, int msg, int user, int param1, int param2)
        {
            if (Tick != null)
            {
                new Task(() => Tick(null, null)).Start(); // 引发事件
            }
        }

        #endregion

        #region 内部类型

        private delegate void TimerCallback(int id, int msg, int user, int param1, int param2); // timeSetEvent所对应的回调函数的签名

        /// <summary>
        /// 定时器的分辨率（resolution）。单位是ms，毫秒？
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct TimerCaps
        {
            public readonly int periodMin;
            public readonly int periodMax;
        }

        #endregion
    }
}