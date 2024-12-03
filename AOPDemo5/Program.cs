namespace AOPDemo5
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = new WebHost();
            host.AddFilter(new Filter1());
            host.AddFilter(new Filter2());
            var servlet = new HelloServlet();
            await host.ExecuteAsync(new HttpContext("Get"), servlet);
        }
    }
    public class HttpContext
    {
        public string Method { get; set; }
        public HttpContext(string method)
        {
            Method = method;
        }
    }
    // 链路器
    public interface IChain
    {
        Task NextAsync();
    }
    // 用于执行Filter
    public interface IFilter
    {
        Task InvokeAsync(HttpContext context, IChain chain);
    }
    public interface IServlet
    {
        Task InvokeAsync(HttpContext context);
    }
    // 适配Filter链路，用于执行Filter
    public class FilterChain : IChain
    {
        private readonly IFilter _filter;
        private readonly IChain _next;
        private readonly HttpContext _context;

        public FilterChain(IFilter filter, IChain next, HttpContext context)
        {
            _filter = filter;
            _next = next;
            _context = context;
        }

        public async Task NextAsync()
        {
            await _filter.InvokeAsync(_context, _next);
        }
    }
    // 适配Servlet，用于执行Servlet
    public class ServletChain : IChain
    {
        private readonly IServlet _servlet;
        private readonly HttpContext _context;

        public ServletChain(IServlet servlet, HttpContext context)
        {
            _servlet = servlet;
            _context = context;
        }

        public async Task NextAsync()
        {
            await _servlet.InvokeAsync(_context);
        }
    }
    public class Filter1 : IFilter
    {
        public async Task InvokeAsync(HttpContext context, IChain chain)
        {
            Console.WriteLine("身份认证开始");
            await chain.NextAsync();
            Console.WriteLine("身份认证结束");
        }
    }
    public class Filter2 : IFilter
    {
        public async Task InvokeAsync(HttpContext context, IChain chain)
        {
            Console.WriteLine("权限验证开始");
            await chain.NextAsync();
            Console.WriteLine("权限验证结束");
        }
    }
    /// <summary>
    /// 模板方法设计模式
    /// </summary>
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
            Console.WriteLine("Hello World");
            return Task.CompletedTask;
        }

        public override Task PostAsync(HttpContext context)
        {
            Console.WriteLine("Hello World");
            return Task.CompletedTask;
        }
    }

    public class WebHost
    {
        private readonly List<IFilter> _filters = new List<IFilter>();

        public void AddFilter(IFilter filter)
        {
            _filters.Add(filter);
        }

        public async Task ExecuteAsync(HttpContext context, IServlet servlet)
        {
            if (_filters.Count > 0)
            {
                var filter = _filters[0];
                var chain = GetFilterChain(context, servlet, _filters.ToArray(), 1);
                await filter.InvokeAsync(context, chain);
            }
            else
            {
                await servlet.InvokeAsync(context);
            }
        }

        private IChain GetFilterChain(HttpContext context, IServlet servlet, IFilter[] filters, int index)
        {
            if (index < filters.Length)
            {
                var filter = filters[index];
                var next = GetFilterChain(context, servlet, filters, index + 1);
                return new FilterChain(filter, next, context);
            }
            else
            {
                return new ServletChain(servlet, context);
            }
        }
    }
}
