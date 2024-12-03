using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace EventBusDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            //注册服务
            //构建服务提供者
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            // 在下单后发布事件
            var eventBus = new LocalEventBusManager<OrderPlacedEvent>(serviceProvider);
            eventBus.Publish(new OrderPlacedEvent { OrderId = 1, ProductId = 101, Quantity = 2 });
            Console.WriteLine("Hello, World!");
            Console.ReadLine();
        }
    }

    public interface IEvent
    { }

    public interface IAsyncEventHandler<in TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event);
        void HandleException(TEvent @event, Exception exception);
    }

    public interface IEventBus
    {    // 同步发布事件
        void Publish<TEvent>(TEvent @event) where TEvent : IEvent;
        // 异步发布事件
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;
        // 订阅事件
        void OnSubscribe<TEvent>() where TEvent : IEvent;
    }

    public interface ILocalEventBusManager<in TEvent> where TEvent : IEvent
    {
        void Publish(TEvent @event);
        Task PublishAsync(TEvent @event);
        void AutoHandle();
    }

    public class LocalEventBusManager<TEvent> : ILocalEventBusManager<TEvent> where TEvent : IEvent
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Channel<TEvent> _eventChannel = Channel.CreateUnbounded<TEvent>();
        public LocalEventBusManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void Publish(TEvent @event)
        {
            _eventChannel.Writer.WriteAsync(@event).GetAwaiter().GetResult();
        }
        public async Task PublishAsync(TEvent @event)
        {
            await _eventChannel.Writer.WriteAsync(@event);
        }
        public async void AutoHandle()
        {
            var reader = _eventChannel.Reader; while (reader.TryRead(out var eventItem))
            {
                var handler = _serviceProvider.GetService<IAsyncEventHandler<TEvent>>();
                if (handler != null)
                {
                    try
                    {
                        await handler.HandleAsync(eventItem);
                    }
                    catch (Exception ex) { handler.HandleException(eventItem, ex); }
                }
            }
        }
    }

    //假设我们有一个电子商务平台，需要在用户下单后通知库存服务更新库存。我们可以通过事件总线来实现这一流程。
    public class OrderPlacedEvent : IEvent
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    public class InventoryServiceEventHandler : IAsyncEventHandler<OrderPlacedEvent>
    {
        public Task HandleAsync(OrderPlacedEvent @event)
        {        // 更新库存逻辑
            Console.WriteLine($"Updating inventory for product {@event.ProductId} by {@event.Quantity}");
            return Task.CompletedTask;
        }
        public void HandleException(OrderPlacedEvent @event, Exception ex)
        {        // 异常处理逻辑
            Console.WriteLine($"Error handling event: {@event.OrderId}");
        }
    }
}
