namespace AOPDemo3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var buider = new ApplicationBuilder();
            buider.Use(next => context =>
            {
                Console.WriteLine("开始去污");
                next(context);
                Console.WriteLine("完成去污");
            });

            buider.Use(next => context =>
            {
                Console.WriteLine("开始消毒");
                next(context);
                Console.WriteLine("完成消毒");
            });

            buider.Use(next => context =>
            {
                Console.WriteLine("Hello World!");
            });

            var app = buider.Build();
            var context = new HttpContext();
            app.Invoke(context);
        }
    }

    public class HttpContext
    {
        public string? Request { get; set; }
        public string? Response { get; set; }
    }

    public delegate void RequestDelegate(HttpContext context);

    public class ApplicationBuilder
    {
        private readonly List<Func<RequestDelegate, RequestDelegate>> _components = new List<Func<RequestDelegate, RequestDelegate>>();

        public void Use(Func<RequestDelegate, RequestDelegate> component)
        {
            _components.Add(component);
        }

        public RequestDelegate Build()
        {
            //负责兜底
            RequestDelegate app = c =>
            {
                throw new InvalidOperationException("无效的管道");
            };
            for (int i = _components.Count - 1; i > -1; i--)
            {
                app = _components[i](app);//完成嵌套
            }
            return app;
        }
    }
}
