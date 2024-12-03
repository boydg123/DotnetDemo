using Microsoft.Extensions.DependencyInjection;

namespace IOCDemo5
{
    // 工厂模式
    internal class Program
    {
        /*
        工厂模式侧重于对象的管理（创建销毁），一般提供一个Create方法，支持命名创建。
        通过上面的学习我们发现IOC有一个弊端，就是他是通过服务类型的解析服务的。有些情况下我们需要通过命名的方式来解析服务。此时可以使用工厂模式。
        IServiceProvider也是工厂模式
        */
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var services = new ServiceCollection();
            services.AddScoped<SqlServerDbConnection>();
            services.AddScoped<OracleDbConnection>();
            services.AddSingleton(sp =>
            {
                var connections = new Dictionary<string, Type>
                {
                    {"sqlserver", typeof(SqlServerDbConnection)},
                    {"oracle", typeof(OracleDbConnection)}
                };
                return new DbConnectionFactory(connections);
            });
            var serviceProvider = services.BuildServiceProvider();
            var factory = serviceProvider.GetRequiredService<DbConnectionFactory>();
            var sqlserverConnection = factory.CreateConnection(serviceProvider, "sqlserver");
            if (sqlserverConnection != null)
            {
                Console.WriteLine("Connection created successfully!");
            }

            var oracleConnection = factory.CreateConnection(serviceProvider, "oracle");
            if (oracleConnection != null)
            {
                Console.WriteLine("Connection created successfully!");
            }
        }
    }
    public interface IDbConnection { }
    public class SqlServerDbConnection : IDbConnection { }
    public class OracleDbConnection : IDbConnection { }

    public class DbConnectionFactory
    {
        private Dictionary<string, Type> _connections;

        public DbConnectionFactory(Dictionary<string, Type> connections)
        {
            _connections = connections;
        }

        public IDbConnection? CreateConnection(IServiceProvider serviceProvider, string connectionString)
        {
            if (_connections.TryGetValue(connectionString, out Type? connectionType))
            {
                return serviceProvider.GetRequiredService(connectionType) as IDbConnection;
            }
            return default;
        }
    }
}
