namespace IOCDemo8
{
    internal class Program
    {
        /* 
            实现容器有三个重要的对象，通过IContainerBuilder来构建Container实列。Container负责根据服务描述来找到服务实现，
            通过服务实现的依赖来进行注入。下面我们写一个简化版本的容器。

            ServiceDescriptor：负责描述服务信息
            IContainerBuilder：负责构建容器
            IContainer：负责根据服务描述信息解析服务
        */

        static void Main(string[] args)
        {
            IContainerBuilder builder = new ContainerBuilder();
            builder.Add(c => new DbConnection());
            builder.Add<DbContext>();
            var container = builder.Build();
            var context = container.GetService(typeof(DbContext));
        }
    }

    public class DbConnection
    {
    }

    public class DbContext
    {
        public DbConnection Connection { get; }

        public DbContext(DbConnection connection)
        {
            Connection = connection;
        }
    }

    public class ServiceDescriptor
    {
        public Type ServiceType { get; }
        public Type ImplementionType { get; }
        public object? Instance { get; }

        public ServiceDescriptor(Type serviceType, Type implementionType, object? instance = null)
        {
            ServiceType = serviceType;
            ImplementionType = implementionType;
            Instance = instance;
        }
    }

    public interface IContainer
    {
        object? GetService(Type serviceType);
    }

    public interface IContainerBuilder
    {
        void Add(ServiceDescriptor descriptor);
        IContainer Build();
    }

    public class Container : IContainer
    {
        private IEnumerable<ServiceDescriptor> _services;

        public Container(IEnumerable<ServiceDescriptor> services)
        {
            _services = services;
        }

        public object? GetService(Type serviceType)
        {
            var descriptor = _services.FirstOrDefault(a => a.ServiceType == serviceType);
            if (descriptor == null)
            {
                throw new InvalidOperationException("服务未注册");
            }
            //判断是否是委托(涉及到了协变)
            var invokerType = typeof(Func<IContainer, object>);
            if (descriptor.Instance != null && typeof(Func<IContainer, object>).IsInstanceOfType(descriptor.Instance))
            {
                var func = descriptor.Instance as Func<IContainer, object> ?? throw new ArgumentNullException();
                return func(this);
            }
            var constructor = serviceType.GetConstructors()
                .OrderByDescending(a => a.GetParameters().Length)
                .FirstOrDefault() ?? throw new ArgumentNullException();
            //递归解析依赖
            var parameters = constructor.GetParameters()
                //递归
                .Select(s => GetService(s.ParameterType));
            //反射
            return Activator.CreateInstance(descriptor.ImplementionType, parameters.ToArray());
        }
    }

    public class ContainerBuilder : IContainerBuilder
    {
        private List<ServiceDescriptor> _services = new();

        public void Add(ServiceDescriptor descriptor)
        {
            _services.Add(descriptor);
        }

        public IContainer Build()
        {
            return new Container(_services);
        }
    }

    public static class IContainerBuilderExtensions
    {
        public static void Add<TService>(this IContainerBuilder builder)
            where TService : class
        {
            builder.Add(new ServiceDescriptor(typeof(TService), typeof(TService)));
        }

        public static void Add<TService, TImplement>(this IContainerBuilder builder)
        {
            builder.Add(new ServiceDescriptor(typeof(TService), typeof(TImplement)));
        }

        public static void Add<TService>(this IContainerBuilder builder, Func<IContainer, TService> func)
        {
            builder.Add(new ServiceDescriptor(typeof(TService), typeof(Action<IContainer, TService>), func));
        }
    }
}
