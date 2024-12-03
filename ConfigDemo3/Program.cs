using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ConfigDemo3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationManager();
            //json配置源
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
                //需要设置reloadOnChange为true
                .AddJsonFile(path: "appsettings.json", optional: true, reloadOnChange: true);
            //构建
            IConfiguration configuration = configurationBuilder;
            //绑定生产者和消费者
            ChangeToken.OnChange(configuration.GetReloadToken, () =>
            {
                Console.WriteLine("配置更新了：Port = " + configuration["MvcOptions:Port"]);
            });
            //阻塞
            new TaskCompletionSource().Task.Wait();

            Console.WriteLine("Hello, World!");
        }
    }
}
