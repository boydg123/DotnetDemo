namespace AOPDemo4
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var helloServlet = new HelloServlet();
            await helloServlet.InvokeAsync(new HttpContext { Method = "Get" });
        }
    }
    public class HttpContext
    {
        public string? Method { get; set; }
    }

    public interface IServlet
    {
        Task InvokeAsync(HttpContext context);
    }
    //如果IServlet是一个抽象类，InvokeAsync是一个虚函数，则必须使用sealed来保护模板方法
    public abstract class HttpServlet : IServlet
    {
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Method == "Get")
            {
                await GetAsync(context);
            }
            else if (context.Method == "Post")
            {
                await PostAsync(context);
            }
        }

        public abstract Task GetAsync(HttpContext context);
        public abstract Task PostAsync(HttpContext context);
    }

    public class HelloServlet : HttpServlet
    {
        public override Task GetAsync(HttpContext context)
        {
            Console.WriteLine("Get Hello World!");
            return Task.CompletedTask;
        }

        public override Task PostAsync(HttpContext context)
        {
            Console.WriteLine("Post Hello World!");
            return Task.CompletedTask;
        }
    }
}
