using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogDemo1
{
    internal class Program
    {
        // aspnetcore日志是基于IOC实现的。微软提供了大量的日志提供程序，以及第三方日志也相继支持。我们尽量面向微软的日志接口进行编程。
        /*
         * 日志接口 ILogger
         * 日志级别：Debug、Information、Warning、Error、Critical
         * 日志输出：Console、文件、数据库、网络、邮件、其他
         * 日志格式：文本、Json、Xml
         * 日志过滤：按日志级别、按日志内容、按日志来源
         * 日志聚合：按时间、按日志级别、按日志来源
         * 日志采样：按百分比、按日志条数
         * 日志跟踪：记录方法调用链
         * 日志监控：监控日志输出、日志处理时间、日志堆积
         * 日志管理：日志配置、日志查看、日志分析
         * 依赖项
         * Microsoft.Extensions.Logging.Abstractions：抽象包
         * Microsoft.Extensions.Logging：核心包
         * Microsoft.Extensions.Logging.Console：控制台输出
         * Microsoft.Extensions.Logging.Debug：调试输出
         * Microsoft.Extensions.Logging.EventLog：Windows事件日志输出
         * Microsoft.Extensions.Logging.EventSource：Windows事件源输出
         * Microsoft.Extensions.Logging.TraceSource：.NET Framework 跟踪源输出
         * Microsoft.Extensions.Logging.Configuration：配置文件配置
         * Microsoft.Extensions.Logging.File：文件输出
         * Microsoft.Extensions.Logging.AzureAppServices：Azure应用服务输出
         * Microsoft.Extensions.Logging.NLog：NLog输出
         * Microsoft.Extensions.Logging.Log4Net：Log4Net输出
         * Microsoft.Extensions.Logging.Serilog：Serilog输出
         * 核心接口
         * ILogger：日志记录器
         * ILogger<T> 泛型日志记录器，基础自ILogger
         * ILoggerFactory：日志工厂
         * ILoggingBuilder：用于配置日志服务到容器里，并简化ILoggerFactory的创建，ILoggingBuilder拥有丰富的api，用以支持扩展
         */
        static void Main(string[] args)
        {
            //这里用到了工厂模式和构造模式
            //思考为什么微软设计成委托的方式？
            //因为它想简化ILoggingBuilder的创建

            #region 内置容器
            var factory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            var logger1 = factory.CreateLogger<Program>();
            var logger2 = factory.CreateLogger("logger2");

            logger1.LogInformation("logger1 information");
            logger2.LogInformation("logger2 information");
            #endregion

            #region 现有容器
            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            var provider = services.BuildServiceProvider();
            var logger3 = provider.GetRequiredService<ILogger<Program>>();
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var logger4 = loggerFactory.CreateLogger("logger4");

            logger3.LogInformation("logger3 information");
            logger4.LogInformation("logger4 information");
            #endregion 
            Console.WriteLine("Hello, World!");
        }
    }
}
