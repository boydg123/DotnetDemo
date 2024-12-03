using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OptionDemo3
{
    internal class Program
    {
        static void Main(string[] args)
        { 
            var services = new ServiceCollection();
            //name=Options.DefaultName
            services.Configure<MvcOptions>(c => c.Url = "123");
            //name="o1"
            services.Configure<MvcOptions>("o1", c => c.Url = "456");
            //name="o2"
            services.Configure<MvcOptions>("o2", c => c.Url = "789");

            var container = services.BuildServiceProvider();
            var o1 = container.GetRequiredService<IOptionsSnapshot<MvcOptions>>();
            var o2 = container.GetRequiredService<IOptionsMonitor<MvcOptions>>();
            //name="o1"
            Console.WriteLine("IOptionsSnapshot:Named:" + o1.Get("o1").Url);
            //name=Options.DefaultName
            Console.WriteLine("IOptionsSnapshot:Value:" + o1.Value.Url);
            //name="o2"
            Console.WriteLine("IOptionsMonitor:Named:" + o2.Get("o2").Url);
            //name=Options.DefaultName
            Console.WriteLine("IOptionsMonitor:Value:" + o2.CurrentValue.Url);
            var optionsFactory = container.GetRequiredService<IOptionsFactory<MvcOptions>>();
            var options = optionsFactory.Create(Options.DefaultName);
        }
    }

    public class MvcOptions
    {
        public string? Url { get; set; }
    }
}
