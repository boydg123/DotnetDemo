using Microsoft.Extensions.Configuration;

namespace ConfigDemo1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationManager();
            //内存配置源
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>()
            {
                { "m1","45"},
                { "m2","45"}
            });
            //json配置源
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            //命令行配置源
            configurationBuilder.AddCommandLine(args);
            //环境变量配置源：可以指定前缀
            configurationBuilder.AddEnvironmentVariables("ASPNETCORE_");

            //由于ConfigurationManager实现了IConfiguration接口，并且没有build方法
            //因此构建很简单，或者你根本不需要构建，但是建议把配置和读取分开
            IConfiguration configuration = configurationBuilder;

            //通过索引读取配置
            string m1 = configuration["m1"];
            //获取子配置
            IConfigurationSection options = configuration.GetSection("MvcOptions");
            var host = options["Host"];
            //获取数组
            IEnumerable<IConfigurationSection> sections = configuration.GetSection("MvcOptions:Urls").GetChildren();
            foreach (IConfigurationSection item in sections)
            {
                var url = item.Value;
            }
            //内置的一个获取链接字符串的api
            var defaultConnectionString = configuration.GetConnectionString("default");

            // 扩展接口，需要安装Binder支持

            //将配置绑定到基本类型上，底层调用Convert.ToXXX();
            var host1 = configuration.GetValue<string>("MvcOptions:Host");
            var port1 = configuration.GetValue<int>("MvcOptions:Port");
            //将配置绑定到实列上，底层调用GetValue
            var options1 = new MvcOptions();
            configuration.Bind("MvcOptions", options);
            //将配置绑定到实列上，并返回这个实列，底层调用Bind
            var options2 = configuration.GetSection("MvcOptions").Get<MvcOptions>();
        }
    }

    public class MvcOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
