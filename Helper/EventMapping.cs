using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Wpf.MVVM.Foundation.Behaviors
{
  ///<summary>
  ///功能描述：1.执行事件与命令的链接与移除链接功能
  ///          2.继承至Freezable，主要利用其与WPF的传递DataContext功能
  ///创建人：*** 
  ///编写日期：2013-08-01
  ///</summary>

  public class CommandEvent : Freezable
  {

    #region 依赖属性定义
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandEvent), new UIPropertyMetadata(null));
    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandEvent), new UIPropertyMetadata(null));
    public static readonly DependencyProperty EventProperty = DependencyProperty.Register("Event", typeof(String), typeof(CommandEvent), new UIPropertyMetadata(string.Empty));

    #endregion 

    #region 属性/变量定义
    public string Event
    {
      get { return (string)GetValue(EventProperty); }
      set { SetValue(EventProperty, value); }
    }

    public ICommand Command
    {
      get { return (ICommand)GetValue(CommandProperty); }
      set { SetValue(CommandProperty, value); }
    }
    public object CommandParameter
    {
      get { return GetValue(CommandParameterProperty); }
      set { SetValue(CommandParameterProperty, value); }
    }

    #endregion



    #region 私有函数

    /// <summary>
    /// 将事件和命令进行链接
    /// </summary>
    /// <param name="target">所附加的窗体对象</param>
    internal void Subscribe(object target)
    {
      if (target == null)//若附加对象为null，直接退出
        return;

      string eventName = Event;
      EventInfo ei = target.GetType().GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance);//在附加窗体对象中查找事件类型
      if (ei != null)
      {
        ei.RemoveEventHandler(target, GetEventMethod(ei));//处于安全目的，首先移除事件处理委托
        ei.AddEventHandler(target, GetEventMethod(ei));//添加事件处理委托
        return;
      }


      //移除事件名称字符串中可能存在的命令空间字符串
      int dotPos = eventName.IndexOf(':');
      if (dotPos > 0)
        eventName = eventName.Substring(dotPos + 1);

      dotPos = eventName.IndexOf('.');   // 查找依赖对象的附加路由事件
      if (dotPos > 0 && eventName.Length > dotPos)
      {
        var attachedEvent = EventManager.GetRoutedEvents().Where(evt => evt.ToString() == eventName).SingleOrDefault();//在事件管理中，查找对应的路由事件
        if (attachedEvent == null)//未查找到，直接返回
          return;

        FrameworkElement fe = target as FrameworkElement;
        if (fe == null)
          return;

        fe.RemoveHandler(attachedEvent, GetRoutedEventMethod());//处于安全目的，首先移除路由事件处理委托
        fe.AddHandler(attachedEvent, GetRoutedEventMethod());//添加路由事件处理委托
      }
    }



    /// <summary>
    /// 解除事件和命令的链接
    /// </summary>
    /// <param name="target">所附加的窗体对象</param>
    internal void Unsubscribe(object target)
    {
      if (target == null)//若附加对象为null，直接退出
        return;

      EventInfo ei = target.GetType().GetEvent(Event, BindingFlags.Public | BindingFlags.Instance);
      if (ei != null)//常规事件，直接移除事件委托
      {
        ei.RemoveEventHandler(target, GetEventMethod(ei));
        return;
      }

      string eventName = Event;
      //移除事件名称字符串中可能存在的命令空间字符串
      int dotPos = eventName.IndexOf(':');
      if (dotPos > 0)
        eventName = eventName.Substring(dotPos + 1);

      dotPos = eventName.IndexOf('.'); // 查找依赖对象的附加路由事件
      if (dotPos > 0 && eventName.Length > dotPos)
      {
        //在事件管理中，查找对应的路由事件
        var attachedEvent = EventManager.GetRoutedEvents().Where(evt => evt.Name == eventName).SingleOrDefault();
        if (attachedEvent != null)
        {
          FrameworkElement fe = target as FrameworkElement;
          if (fe != null)//移除路由事件处理委托
            fe.RemoveHandler(attachedEvent, GetRoutedEventMethod());
        }
      }
    }

    /// <summary>
    ///  获取事件的执行委托
    /// </summary>
    /// <param name="ei">事件信息</param>
    /// <returns>返回事件的执行委托，失败，返回null</returns>
    private Delegate GetEventMethod(EventInfo ei)
    {
      if (ei == null || ei.EventHandlerType == null)
        return null;

      return Delegate.CreateDelegate(ei.EventHandlerType, this, GetType().GetMethod("OnEventRaised", BindingFlags.NonPublic | BindingFlags.Instance));
    }

    /// <summary>

    /// </summary>
    /// <returns>返回路由事件的执行委托，失败，返回null</returns>
    private Delegate GetRoutedEventMethod()
    {
      return Delegate.CreateDelegate(typeof(RoutedEventHandler), this, GetType().GetMethod("OnEventRaised", BindingFlags.NonPublic | BindingFlags.Instance));
    }


    /// <summary>
    /// 该方法被事件调用，其调用命令处理函数
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">参数</param>
    private void OnEventRaised(object sender, EventArgs e)
    {
      if (Command == null)
        return;

      ICommand c = Command;
      if (Command.CanExecute(CommandParameter))
        Command.Execute(CommandParameter);
    }

    #endregion //私有函数

    #region Freezable 抽象成员实现
    /// <summary>
    /// 未实现，返回null
    /// </summary>
    /// <returns>返回null</returns>
    protected override Freezable CreateInstanceCore()
    {
      return null;
    }

    #endregion Freezable 抽象成员实现
  }

  ///<summary>
  ///功能描述：1.事件与命令映射的集合
  ///          2.继承至FreezableCollection<T>,主要为在WFP Visual Tree 上传递DataContext对象
  ///创建人：*** 
  ///编写日期：2013-08-01
  ///</summary>
  public class CommandEventCollection : FreezableCollection<CommandEvent>
  {
    #region 属性/变量
    private object _target;//附加对象
    #endregion //属性/变量

    #region 构造函数
    /// <summary>
    /// 构造函数
    /// </summary>
    public CommandEventCollection()
    {
      ((INotifyCollectionChanged)this).CollectionChanged += OnCollectionChanged;//添加集合变更事件处理函数
    }
    #endregion //构造函数

    #region 私有函数
    /// <summary>
    /// 集合中的命令和事件进行链接
    /// </summary>
    internal void Subscribe(object target)
    {
      _target = target;//初始化附加对象

      foreach (var item in this)
        item.Subscribe(target);
    }

    /// <summary>
    /// 集合中的命令和事件解除链接
    /// </summary>
    /// <param name="target">附加对象</param>
    internal void Unsubscribe(object target)
    {
      foreach (var item in this)
        item.Unsubscribe(target);

      _target = null;
    }

    /// <summary>
    /// 事件与命令的集合变更函数
    /// </summary>
    /// <param name="sender">事件触发者</param>
    /// <param name="e">参数</param>
    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)//集合变更类型
      {
        case NotifyCollectionChangedAction.Add://添加新元素
          foreach (var item in e.NewItems)
            OnItemAdded((CommandEvent)item);
          break;

        case NotifyCollectionChangedAction.Remove://移除元素
          foreach (var item in e.OldItems)
            OnItemRemoved((CommandEvent)item);
          break;

        case NotifyCollectionChangedAction.Replace://更新元素
          foreach (var item in e.OldItems)
            OnItemRemoved((CommandEvent)item);

          foreach (var item in e.NewItems)
            OnItemAdded((CommandEvent)item);
          break;

        case NotifyCollectionChangedAction.Move:
          break;

        case NotifyCollectionChangedAction.Reset://集合重置
          foreach (var item in this)
            OnItemRemoved(item);

          foreach (var item in this)
            OnItemAdded(item);
          break;

        default:
          return;
      }
    }

    /// <summary>
    /// 添加新元素
    /// </summary>
    /// <param name="item">新元素</param>
    private void OnItemAdded(CommandEvent item)
    {
      if (item != null && _target != null)
      {
        item.Subscribe(_target);
      }
    }

    /// <summary>
    /// 移除元素
    /// </summary>
    /// <param name="item">元素</param>
    private void OnItemRemoved(CommandEvent item)
    {
      if (item != null && _target != null)
      {
        item.Unsubscribe(_target);
      }
    }
    #endregion //私有函数
  }

  ///<summary>
  ///功能描述：1.管理事件与命令链接集合
  ///          2.继承至FreezableCollection<T>,主要为在WFP Visual Tree 上传递DataContext对象
  ///          3.注意，若为生命周期事件（加载、激活、关闭、已关闭等），请使用生命周期事件行为代替
  ///创建人：*** 
  ///编写日期：2013-08-01
  ///</summary>
  /// <example>
  /// <![CDATA[
  /// 
  /// <Behaviors:EventCommander.Mappings>
  ///    <Behaviors:CommandEvent Command="{Binding MouseEnterCommand}" Event="MouseEnter" />
  ///    <Behaviors:CommandEvent Command="{Binding MouseLeaveCommand}" Event="MouseLeave" />
  /// </Behaviors:EventCommander.Mappings>
  /// 
  /// ]]>
  /// </example>
  public static class EventCommander
  {
    #region 依赖属性定义
    /// 事件与命令映射集合附加依赖属性定义
    /// </summary>
    internal static readonly DependencyProperty MappingsProperty = DependencyProperty.RegisterAttached(
                       "InternalMappings",
                        typeof(CommandEventCollection),
                        typeof(EventCommander),
                        new UIPropertyMetadata(null, OnMappingsChanged)
                        );

    /// <summary>
    /// Mappings附加依赖属性读取方法
    /// </summary>
    /// <param name="obj">附加对象</param>
    /// <returns>返回附加依赖属性值</returns>
    public static CommandEventCollection GetMappings(DependencyObject obj)
    {
      return InternalGetMappingCollection(obj);
    }



    /// <summary>
    /// Mappings附加依赖属性设置方法
    /// </summary>
    /// <param name="obj">附加对象</param>
    /// <param name="value">设置值</param>
    public static void SetMappings(DependencyObject obj, CommandEventCollection value)
    {
      obj.SetValue(MappingsProperty, value);
    }

    #endregion //依赖属性定义



    #region 私有函数

    /// <summary>
    /// 获取Mapping依赖属性值，若为null，则初始化为空集合
    /// </summary>
    /// <param name="obj">附加对象</param>
    /// <returns>返回Mapping依赖属性值</returns>
    internal static CommandEventCollection InternalGetMappingCollection(DependencyObject obj)
    {
      var map = obj.GetValue(MappingsProperty) as CommandEventCollection;
      if (map == null)//属性值为null，初始化新集合
      {
        map = new CommandEventCollection();
        SetMappings(obj, map);
      }
      return map;
    }

    /// <summary>
    /// Mapping附加依赖属性值变更处理函数
    /// </summary>
    /// <param name="target">附加对象</param>
    /// <param name="e">信息参数</param>
    private static void OnMappingsChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      if (e.OldValue != null)
      {
        CommandEventCollection cec = e.OldValue as CommandEventCollection;
        if (cec != null)//解除命令与事件链接
          cec.Unsubscribe(target);
      }

      if (e.NewValue != null)
      {
        CommandEventCollection cec = e.NewValue as CommandEventCollection;
        if (cec != null)//进行命令与事件链接
          cec.Subscribe(target);
      }
    }

    #endregion //私有函数
  }
}