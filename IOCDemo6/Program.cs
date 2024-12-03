using System.Diagnostics;

namespace IOCDemo6
{
    // 提供模式
    internal class Program
    {
        /*
        如果看到提供者模式，说明我们可以提供多个方案，支持多实现
        一般通过工厂来管理提供者，用以支持命名实列
        */
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }

    public interface ILogger
    {
        void Log(string message);
    }

    public interface ILoggerProvider
    {
        ILogger CreateLogger(string name);
    }
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string name)
        {
            return new ConsoleLogger(name);
        }

        public class ConsoleLogger : ILogger
        {
            private string _name;

            public ConsoleLogger(string name)
            {
                _name = name;
            }
            public void Log(string message)
            {
                Console.WriteLine($"{_name}:{message}");
            }
        }
    }

    public class DebugLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string name)
        {
            return new DebugLogger(name);
        }

        public class DebugLogger : ILogger
        {
            private string _name;
            public DebugLogger(string name)
            {
                _name = name;
            }
            public void Log(string message)
            {
                Debug.WriteLine($"{_name}:{message}");
            }
        }
    }

    public class LoggerFactoryBuilder
    {
        private List<ILoggerProvider> _providers = new List<ILoggerProvider>();

        public void AddProvider(ILoggerProvider provider)
        {
            _providers.Add(provider);
        }

        public LoggerFactory Build()
        {
            return new LoggerFactory(_providers);
        }
    }
    //这里用到了：代理模式，工厂模式，构造模式，提供模式
    public class LoggerFactory
    {
        private List<ILoggerProvider> _providers;

        public LoggerFactory(List<ILoggerProvider> providers)
        {
            _providers = providers;
        }

        public static LoggerFactory Create(Action<LoggerFactoryBuilder> configure)
        {
            var buider = new LoggerFactoryBuilder();
            configure(buider);
            return buider.Build();
        }

        public void AddProvider(ILoggerProvider provider)
        {
            _providers.Add(provider);
        }

        public ILogger Create(string name)
        {
            var loggers = _providers.Select(p => p.CreateLogger(name)).Where(l => l != null);
            return new LoggerCollection(loggers);
        }
        //代理模式
        /*
            代理模式侧重于对目标对象进行加强，通过实现目标对象的接口具备目标对象的能力。
            一般通过实现和目标对象相同的接口来获得目标对象的能力
            代理可以通过目标对象来简化实现成本，代理只负责编写加强逻辑
            一般代理器只代理单个目标对象，我们把下面这个模式也可以归纳到代理模式，因为它能满足代理的许多特点比如加强、拥有目标对象的能力
            思考我们需要一个LoggerCollection，需要实现ICollection接口，如何降低实现成本？
        */
        class LoggerCollection : ILogger
        {
            private IEnumerable<ILogger> _loggers;
            public LoggerCollection(IEnumerable<ILogger> loggers)
            {
                _loggers = loggers;
            }

            public void Log(string message)
            {
                foreach (var logger in _loggers)
                {
                    logger.Log(message);
                }
            }
        }
    }
}
