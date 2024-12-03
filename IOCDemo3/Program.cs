using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IOCDemo3
{
    // 组件扫描
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddInjection<Program>();
            var provider = services.BuildServiceProvider();
            var dbConnection = provider.GetService<IDbConnection>();
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class InjectionAttribute : Attribute
    {
        public Type? ServiceType { get; set; }
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
    }

    public static class InjectionIServiceCollectionExtensions
    {
        public static IServiceCollection AddInjection<T>(this IServiceCollection services)
        {
            var serviceTypes = typeof(T).Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsDefined(typeof(InjectionAttribute), false));
            foreach (var type in serviceTypes)
            {
                if (type.IsClass && !type.IsAbstract && type.IsDefined(typeof(InjectionAttribute), false))
                {
                    var attribute = type.GetCustomAttribute<InjectionAttribute>();
                    services.Add(new ServiceDescriptor(attribute!.ServiceType ?? type, type, attribute.Lifetime));
                }
            }
            return services;
        }
    }

    public interface IDbConnection
    { }

    [Injection(ServiceType = typeof(IDbConnection), Lifetime = ServiceLifetime.Singleton)]
    public class DbConnection : IDbConnection
    { }
}
