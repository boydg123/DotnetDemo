using Microsoft.Extensions.DependencyInjection;

namespace IOCDemo2
{
    // 生命周期：Singleton、Scoped、Transient
    internal class Program
    {
        // 容器除了会帮我们创建对象，还负责对象的销毁，特别对于托管资源
        // 不要试图通过根容器来解析Scoped或者Transient生命周期的实列
        // 单实例的对象不能依赖一个Scoped或者Transient生命周期的实列
        // 在Debug模式下可以看到容器是否是根容器，以及容器解析的实列，容器会记录由它解析的所有实列，为释放做准备。
        static void Main(string[] args)
        {
            #region 
            var services = new ServiceCollection();
            services.AddTransient<A>();
            //var provider = services.BuildServiceProvider();

            //var a1 = provider.GetService<A>();
            //var a2 = provider.GetService<A>();

            //using (var scope = provider.CreateScope())
            //{
            //    var a3 = scope.ServiceProvider.GetService<A>();
            //    var a4 = scope.ServiceProvider.GetService<A>();
            //    Console.WriteLine(a1.ID);
            //    Console.WriteLine(a2.ID);
            //    Console.WriteLine(a3.ID);
            //    Console.WriteLine(a4.ID);
            //}
            #endregion

            Console.WriteLine("Hello, World!");

            #region 
            IServiceProvider rootContainer = services.BuildServiceProvider();
            var scope = rootContainer.CreateScope();
            var a1 = rootContainer.GetRequiredService<A>();
            Thread.Sleep(5 * 1000);
            scope.Dispose();
            #endregion
        }
        // 通过修改A服务注册的生命周期我们可以得到一下结论。
        // 测试Singleton发现：a1,a2,a3,a4的Id都相同
        // 测试Scope发现：a1和a2的Id相同，a3和a4的Id相同，a1和a3的Id不相同
        // 测试Transient发现：a1,a2,a3,a4的Id都不同

        //Singleton：无论通过根容器还是子容器，获取的都是同一实列，而且不会执行释放（除非释放根容器）。

        //Scoped：同一scope获取的都是同一实列，不同的scope获取的实列不同。scope释放会释放由它解析出来的所有实列（除了单实例以外），如果并执行Dispose方法（前提实现了IDisposable）。

        //Transient：无论是否同一scope获取的实列都不同，每次获取都是一个新的实列，scope释放会释放所有的实列。

        // 注意：ServiceProvider会记录由它创建的所有实列，如果释放IServiceScope的实列，则会释放(ServiceProvider)和所有（单实列除外）由它创建的实列。

        // Scope范围：scope的范围有多大取决于你何时创建何时释放。从创建到释放就是他的生命周期。

        public class A : IDisposable
        {
            public string ID { get; }

            public A()
            {
                ID = Guid.NewGuid().ToString();
            }

            public void Dispose()
            {
                Console.WriteLine(ID + ":已释放...");
            }
        }
    }
}
