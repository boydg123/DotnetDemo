using static IOCDemo4.ServiceDescriptor;

namespace IOCDemo4
{
    // 构造模式
    internal class Program
    {
        // 构造器的目的和构造函数一样，但是构造器可以提供丰富的api来简化对象的构造
        // 构造模式用于简化被构造对象的创建，通过提供一大堆的api来丰富简化构造过程，增加调用者的体验。
        // 构造者需要提供一个Build方法用于构建和返回将要构造的对象实列。
        // 在容器中一般需要提供一个公开的IServiceCollection类型的属性，用于注册服务。
        // IServiceCollection是构造者模式
        static void Main(string[] args)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.AddTransient<DbContext>();
            var container = containerBuilder.Build();
            Console.WriteLine("Hello World!");
        }
    }

    public class DbContext
    {
       
    }

    public enum ServiceLifetime
    {
        Transient,
        Scoped
    }
    public class ServiceDescriptor
    {
        public Type ServiceType { get; }
        public ServiceLifetime Lifetime { get; set; }
        public ServiceDescriptor(Type serviceType, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
        }
    }
    // 目标对象
    public interface IContainer
    { }

    // 直接创建成本高,体验差
    public class Container : IContainer
    {
        private readonly List<ServiceDescriptor> _services = new List<ServiceDescriptor>();
        public Container(List<ServiceDescriptor> services)
        {
            _services = services;
        }
    }
    // 目标对象的构造者
    public interface IContainerBuilder
    {
        // 接口只提供一个通用方法，降低实现成本
        void Add(ServiceDescriptor descriptor);
        IContainer Build();
    }
    // 实现构造者
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly List<ServiceDescriptor> _services = new List<ServiceDescriptor>();
        public void Add(ServiceDescriptor descriptor)
        {
            _services.Add(descriptor);
        }
        public IContainer Build()
        {
            return new Container(_services);
        }
    }
    public static class IContainerBuidlerExtensions
    {
        public static void AddTransient<TService>(this IContainerBuilder builder)
        {
            builder.Add(new ServiceDescriptor(typeof(TService), ServiceLifetime.Transient));
        }
        public static void AddScoped<TService>(this IContainerBuilder builder)
        {
            builder.Add(new ServiceDescriptor(typeof(TService), ServiceLifetime.Scoped));
        }
    }
}
