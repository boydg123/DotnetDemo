using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace LogDemo2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region 编码方式
            //var services = new ServiceCollection();
            //services.AddLogging(config =>
            //{
            //    config.AddConsole();
            //    config.AddDebug();
            //    config.AddFilter((provider, category, logLevel) =>
            //    {
            //        return category!.StartsWith("A.B") && logLevel >= LogLevel.Information;
            //    });
            //    //过滤指定的提供程序
            //    //config.AddFilter<DebugLoggerProvider>((category, logLevel) =>
            //    //{
            //    //    return category!.StartsWith("A.B") && logLevel >= LogLevel.Information;
            //    //});
            //});
            //var sp = services.BuildServiceProvider();
            //var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            //var logger1 = loggerFactory.CreateLogger("A");
            //var logger2 = loggerFactory.CreateLogger("A.B");
            //var logger3 = loggerFactory.CreateLogger("A.B.C");
            //logger1.LogInformation("Hello from logger1");
            //logger2.LogInformation("Hello from logger2");
            //logger3.LogInformation("Hello from logger3");
            #endregion

            #region 配置方式
            // 需要安装Microsoft.Extensions.Logging.Configuration和Microsoft.Extensions.Configuration.Json支持

            var services = new ServiceCollection();
            var configuration = new ConfigurationManager();
            configuration.SetBasePath(Directory.GetCurrentDirectory());
            configuration.AddJsonFile("appsettings.json");
            services.AddLogging(configure =>
            {
                configure.AddDebug();
                configure.AddConsole();
                configure.AddConfiguration(configuration.GetSection("Logging"));
            });
            var sp = services.BuildServiceProvider();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var logger1 = loggerFactory.CreateLogger("A");
            var logger2 = loggerFactory.CreateLogger("A.B");
            var logger3 = loggerFactory.CreateLogger("A.B.C");
            logger1.LogInformation("666");
            logger2.LogInformation("666");
            logger3.LogInformation("666");
            #endregion 
        }
    }
}
