using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OptionDemo1
{
    internal class Program
    {
        /*
        在aspnetcore里，已经第三方框架中经常见到已Options结尾的选项。我们需要了解Options的原理，以及配置和执行流程。选项是基于容器实现。
        依赖项
        Microsoft.Extensions.Options：选项的核心包，扩展IServiceCollection接口，只支持内存配置。
        Microsoft.Extensions.Options.ConfigurationExtensions：配置文件的扩展，支持IConfiguration进行配置。
        Microsoft.Extensions.DependencyInjection：选项必须配合容器使用
        Microsoft.Extensions.Options.DataAnnotations：支持数据注解验证
        核心接口
        IOptions：单实列，不支持命名。
        IOptionsSnapshot：每次实列化时更新选项，scope级别，支持命名配置
        IOptionsMonitor：监听配置更改并通知，Singleton级别，支持命名配置
        IOptionsFactory：管理Options实列，包括创建、配置、验证，支持命名配置
        IConfigureOptions：保存配置选项时的委托，一个选项可以添加多个委托进行配置
        IPostConfigureOptions：保存配置选项的委托，在IConfigureOptions委托执行之后执行
        OptionsChangeTokenSource：用于监听IConfiguration的更改通知
        IValidateOptions：配置选项之后的验证
        */
        static void Main(string[] args)
        {
            #region 基本使用
            //var services = new ServiceCollection();
            //services.Configure<MvcOptions>(config =>
            //{
            //    config.Url1 = "Configure1";
            //});
            //services.PostConfigure<MvcOptions>(config =>
            //{
            //    config.Url2 = "Configure2";
            //});
            //var sp = services.BuildServiceProvider();
            //var options = sp.GetRequiredService<IOptions<MvcOptions>>();
            #endregion 

            #region  核心方法
            ////1.通过委托来配置选项，同一个选项可以按顺序执行多个Configure。
            //services.Configure<MvcOptions>(a =>
            //{
            //    a.Url1 = "132";
            //});
            ////2.在所有的Configure执行之后配置选项。同一个选项可以按顺序执行多个PostConfigure。
            //services.PostConfigure<MvcOptions>(a =>
            //{
            //    a.Url1 = "132";
            //});
            ////AddOptions返回一个OptionsBuilder，可以连续配置选项，本质还是执行Configure和PostConfigure
            //services.AddOptions<MvcOptions>()
            //    .Configure(a => a.Url1 = "123")
            //    .PostConfigure(a => a.Url1 = "155");
            #endregion

            #region 解析选项
            var services = new ServiceCollection();
            services.Configure<MvcOptions>(a =>
            {
                a.Url1 = "132";
            });
            var sp = services.BuildServiceProvider();
            var optionsFactory = sp.GetRequiredService<IOptionsFactory<MvcOptions>>();
            var options = sp.GetRequiredService<IOptions<MvcOptions>>();
            var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<MvcOptions>>();
            var optionsSnapshot = sp.GetRequiredService<IOptionsSnapshot<MvcOptions>>();
            #endregion 
        }

        /*
         * 1.调用Configure会向容器注册一个IConfigureOptions类型的服务，并且记录我们编写的委托
         * 2.调用PostConfigure会向容器注册一个IPostConfigureOptions类型的服务，并且记录我们编写的委托
         * 3.Configure或者PostConfigure都会执行AddOptions();
         * 4.AddOptions();会尝试向容器注册IOptions、IOptionsSnapshot、IOptionsMonitor、IOptionsFactory、IOptionsMonitorCache
         * 5.IOptionsFactory内部实现逻辑是，首先加载所有注册的IConfigureOptions、IPostConfigureOptions、IValidateOptions服务实列Create方法首先通过反射
         * 调用选项的无参数构造器来实列化选项。然后通过选项实列来执行所有IConfigureOptions中的委托，然后在执行所有IPostConfigureOptions中委托，
         * 然后在执行IValidateOptions中的验证逻辑。
         * 6.IOptions实列依赖IOptionsFactory创建一个名为Options.DefaultName的选项，由于是单实列的，只能执行一次委托来计算选项的值。
         * 7.IOptionsSnapshot依赖IOptionsFactory创建一个名为Options.DefaultName的选项Value，并支持命名Get方法获取选项（内置缓存机制）
         * 因为是Scoped所以每次获取新的选项实列都会执行委托来重新计算选项值。
         * 8.IOptionsMonitor依赖IOptionsFactory提供支持命名的Get方法获取选项，虽然IOptionsMonitor是单实列的并且会监听配置更改，
         * 可以通过注册OnChange来执行更改之后的逻辑来更新选项。
         */
    }

    public class MvcOptions
    {
        public string? Url1 { get; set; }
        public string? Url2 { get; set; }
    }
}
