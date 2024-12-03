using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data;

namespace IocDemo
{
    //我们如何理解IOC？我们可以通过一个现实世界的模型来进行解释。比如有一本菜谱这个菜谱就是我们的IServiceCollection，里面记录了菜（Service）的描述信息（ServiceDescriptor）菜名（ServiceDescriptor.ServiceType）以及菜具体制作方法（ServiceDescriptor.ImplementationType），通过菜名（ServiceType）告诉厨师（IServiceProvider）制作（实列化、解析）出来我们要吃的菜。这就是IOC技术。
    //依赖项
    //Microsoft.Extensions.DependencyInjection.Abstractions：抽象包，用于扩展容器
    //Microsoft.Extensions.DependencyInjection：实现包，实现IOC的基本功能

    //核心接口
    //Service：就是我们需要的服务实列（菜）
    //ServiceDescriptor：用于描述服务的信息。比如服务名（ServiceType）、实现类(ImplementationType)、生命周期(Lifetime)。（某道菜的制作描述信息）
    //IServiceCollection：是一个List集合，用于保存服务描述信息。（菜谱，记录了很多菜的描述信息）
    //IServiceProvider：用于解析服务实列，根容器和子容器实现类不同（厨师）实现类里面有字段用于标记是否是根容器，以及记录所有解析的实列，为将来释放做准备。
    //ActivatorUtilities：用于解析一个容器中不存在，但是依赖了容器中的服务的实列。

    //关键字
    //依赖：如果一个类A的构造器中有一个类B的参数，我们说A依赖B
    //注入：如果A依赖B，要想实列化A，就必须先实列化B，然后把B载入A的构造器的过程
    //依赖注入：IOC容器根据反射得到一个类的依赖关系，自动帮你载入依赖项的过程（注意循环依赖问题）

    //需要安装：Microsoft.Extensions.DependencyInjection
    //创建IServiceCollection实列
    //IServiceCollection services = new ServiceCollection();
    //由于IServiceCollection实现了IList<ServiceDescriptor>接口
    //因此下面是一个万能公式，其它的都是扩展方法，本质调用的还是这个万能公式，包括委托的方式（他的实现类型是一个委托）
    //services.Add(new ServiceDescriptor(typeof(IConnection),typeof(SqlDbConnection), ServiceLifetime.Singleton));
    internal class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            #region 1.泛型接口注册
            //注册服务
            //services.AddSingleton<IDbConnection, SqlDbConnection>();
            #endregion

            #region 2.反射接口注册
            // services.AddSingleton(typeof(IDbConnection), typeof(SqlDbConnection));
            #endregion 

            #region 3.委托注册  

            //当我们构建的对象需要编写逻辑时，委托方式十分有用
            services.AddSingleton<IDbConnection, SqlDbConnection>();
            //假设DbContext依赖IDbConnection，并且需要一个name
            //sp是一个IServiceProvider的实例
            //sp到底是根容器还是子容器，由解析时的IServiceProvider决定

            // 低级用法
            services.AddSingleton(sp =>
            {
                //委托的方式在注册的同时还能进行预解析，即在解析服务之前，先解析依赖项，然后把依赖项注入到委托中，最后再解析委托。
                var connection = sp.GetRequiredService<IDbConnection>();
                return new DbContext(connection, "N1");
            });

            //高级用法
            //services.AddSingleton(sp =>
            //{
            //    return ActivatorUtilities.CreateInstance<DbContext>(sp, "N2");
            //});
            #endregion

            #region 4.泛型注册
            services.AddSingleton(typeof(ILogger<>), typeof(ConsoleLogger<>));
            #endregion

            #region 5.尝试注册
            //如果IDbConnection已注册则后续的services.TryAddSingleton(typeof(IDbConnection), typeof(SqlDbConnection));不会注册新的实现
            services.TryAddSingleton(typeof(IDbConnection), typeof(MySqlDbConnection));
            #endregion

            //构建服务提供者
            //IServiceProvider serviceProvider = services.BuildServiceProvider();

            IServiceProvider serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true, //构建时检查是否有依赖没有注册的服务
                ValidateScopes = true //在解析服务时检查是否通过根容器来解析Scoped类型的实列
            });

            //解析服务
            IDbConnection dbConnection = serviceProvider.GetRequiredService<IDbConnection>();
            dbConnection.ConnectDB();

            var dbContext = serviceProvider.GetRequiredService<DbContext>();

            var logger = serviceProvider.GetService<ILogger<SqlDbConnection>>();
            logger?.Log("SqlDbConnection log message");

            Console.WriteLine("Hello, World!");
        }
    }

    public interface IDbConnection
    {
        void ConnectDB();
    }

    public class SqlDbConnection : IDbConnection
    {
        public void ConnectDB()
        {
            Console.WriteLine("SqlDbConnection connect success!");
        }
    }

    public class MySqlDbConnection : IDbConnection
    {
        public void ConnectDB()
        {
            Console.WriteLine("MySqlDbConnection connect success!");
        }
    }

    public class DbContext : IDisposable
    {
        private IDbConnection _dbConnection;
        private string _connectionName;
        public DbContext(IDbConnection dbConnection, string connectionName)
        {
            _dbConnection = dbConnection;
            _connectionName = connectionName;
            Console.WriteLine("DbContext create success!");
        }
        public void Dispose()
        {
            this.Dispose();
        }
    }

    public interface ILogger<T>
    {
        void Log(string message);
    }

    public class ConsoleLogger<T> : ILogger<T>
    {
        public void Log(string message)
        {
            Console.WriteLine($"[{typeof(T).Name}] {message}");
        }
    }
}
