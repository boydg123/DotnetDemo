using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OptionDemo2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationManager();
            configuration.SetBasePath(Directory.GetCurrentDirectory());
            configuration.AddJsonFile("config.json", false, true);

            var services = new ServiceCollection();
            services.Configure<MvcOptions>(configuration.GetSection("MvcOptions"));
            var container = services.BuildServiceProvider();

            while (true)
            {
                Thread.Sleep(2000);
                using (var scope = container.CreateScope())
                {
                    //测试更改监听
                    var o1 = scope.ServiceProvider.GetRequiredService<IOptions<MvcOptions>>();
                    var o2 = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<MvcOptions>>();
                    var o3 = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<MvcOptions>>();
                    Console.WriteLine("==================");
                    Console.WriteLine($"IOptions:{o1.Value.Url}");
                    Console.WriteLine($"IOptionsSnapshot:{o2.Value.Url}");
                    Console.WriteLine($"IOptionsMonitor:{o3.CurrentValue.Url}");
                    o3.OnChange(o =>
                    {
                        Console.WriteLine("选项发生更改了");
                    });
                }
            }
        }
    }

    public class MvcOptions
    {
        public string? Url { get; set; }
    }
}
