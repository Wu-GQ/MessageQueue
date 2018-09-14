# MessageQueue
通过阻塞队列和延时队列，实现在非延时情况以及延时情况下的生产者消费者模式

## UML Class Diagram
![UML Class Diagram](https://github.com/Wu-GQ/MessageQueue/blob/master/MessageQueue.jpg)

## 关键对象说明
* DelayDispatcherQueue<T>
  > 延时队列  
  > 原理：[《采用简易的环形延时队列处理秒级定时任务的解决方案》](https://m.imooc.com/article/21215?block_id=tuijian_wz)  
  > 当检测到数据到期时，取出数据，并使用委托的方式，将数据交给其它模块进行处理  

* MessageQueue<T>
  > 非延时阻塞消息队列

* MessageQueueDelay<T>
  > 延时阻塞消息队列  
  > 将数据保存至延时队列，待数据到期后，将数据放入阻塞队列中，等待处理  
  > TODO: 可以不将取出数据再次放入阻塞队列，直接进行处理吗？  

* MillisecondTimer
  > 计时器对象  
  > 来源：[Timer计时不准确的问题及解决方法](https://www.cnblogs.com/dehai/p/4347061.html)  
  > 延时队列的精确度与计时器对象相关

* SimpleSynQueue<T>
  > 阻塞队列  
  > 主要是靠两个信号量AutoResetEvent对象，实现当队列中无数据时，停住取数据线程，直到有数据被放入再继续运行。  
  
* SynQueue<T>
  > 阻塞队列  
  > 这是SimpleSynQueue<T>的完整版。虽然在本项目中没有用到这个对象，但是它实现了更多的功能。  
  
