using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OptionDemo4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            //注册di服务，只能是单实例
            services.AddSingleton<MvcOptionsDep>();
            //注册选项
            services.AddOptions<MvcOptions>()
                //第一个配置
                .Configure(a => a.Url = "123")
                //后续配置
                .PostConfigure<MvcOptionsDep>((options, dep) =>
                {
                    //调用MvcOptionsDep中的方法来进行配置        
                    dep.Configure(options);
                });
            var container = services.BuildServiceProvider();
            var options = container.GetRequiredService<IOptions<MvcOptions>>();
        }
    }

    //你也可以实现IConfigureOptions但是IConfigureOptions的执行顺序优先级比较低（要学会举一反三）
    internal class MvcOptionsPostConfigureOptions : IPostConfigureOptions<MvcOptions>
    {
        public void PostConfigure(string name, MvcOptions options)
        {
            options.Url = "789";
            //可以编写验证逻辑
            //if (options.Url == "123")
            //{
            //    throw new InvalidDataException("Url不能等于123");
            //}
        }
    }

    internal class MvcOptionsDep
    {
        public void Configure(MvcOptions options)
        {
            options.Url = "6666";
        }
    }

    public class MvcOptions
    {
        public string? Url { get; set; }
    }
}
