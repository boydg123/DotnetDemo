using Castle.DynamicProxy;
using System.Reflection;

namespace AOPDemo7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var target = new Water();
            IWater proxy = new IWaterProxy(target, new Interceptor1(), new Interceptor2());
            proxy.Invoke();
        }
    }

    public interface IWater
    {
        void Invoke();
    }
    public class Water : IWater
    {
        public void Invoke()
        {
            //业务逻辑
            Console.WriteLine("水已经净化了");
        }
    }
    //拦截器1-切面
    public class Interceptor1 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine("开始去除杂质");//系统逻辑
            invocation.Proceed();
            Console.WriteLine("完成去除杂质");//系统逻辑
        }
    }
    //拦截器2-切面
    public class Interceptor2 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine("开始消毒");//系统逻辑
            invocation.Proceed();
            Console.WriteLine("完成消毒");//系统逻辑
        }
    }

    public class IWaterProxy : IWater
    {
        private readonly IWater _target;

        private readonly IInterceptor[] _interceptors;

        public IWaterProxy(IWater target, params IInterceptor[] interceptors)
        {
            _target = target;
            _interceptors = interceptors;
        }

        public void Invoke()
        {
            var method = _target.GetType().GetMethod(nameof(IWater.Invoke));
            InvocationUtilities.Invoke(_interceptors, _target, method, new object[] { });
        }
    }

    public static class InvocationUtilities
    {
        private static IInvocation GetNextInvocation(IInvocation target, IInterceptor[] inteceptors, int index)
        {
            if (index < inteceptors.Length)
            {
                var proxy = inteceptors[index];
                var next = GetNextInvocation(target, inteceptors, index + 1);
                var args = new object[] { next };
                var method = typeof(IInterceptor).GetMethod(nameof(IInterceptor.Intercept));
                return new NextInvocation(proxy, args, method);
            }
            else
            {
                return target;
            }
        }

        public static void Invoke(IInterceptor[] interceptors, object target, MethodInfo? method, object[] arguments)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            var targetInvocation = new NextInvocation(target, arguments, method);
            if (interceptors.Any())
            {
                var inteceptor = interceptors[0];
                var invocation = GetNextInvocation(targetInvocation, interceptors, 1);
                inteceptor.Intercept(invocation);
            }
            else
            {
                targetInvocation.Proceed();
            }
        }
        //因为是反射调用，因此只需要实现一个链路器方案就好了
        class NextInvocation : IInvocation
        {
            public object[] Arguments { get; }
            public object InvocationTarget { get; }

            public MethodInfo Method { get; }

            public void Proceed()
            {
                Method.Invoke(InvocationTarget, Arguments);
            }

            public NextInvocation(object target, object[] arguments, MethodInfo method)
            {
                Arguments = arguments;
                Method = method;
                InvocationTarget = target;
            }

            #region Other
            public Type[] GenericArguments => throw new NotImplementedException();

            public MethodInfo MethodInvocationTarget => throw new NotImplementedException();

            public object Proxy => throw new NotImplementedException();

            public object ReturnValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public Type TargetType => throw new NotImplementedException();

            public IInvocationProceedInfo CaptureProceedInfo()
            {
                throw new NotImplementedException();
            }

            public object GetArgumentValue(int index)
            {
                throw new NotImplementedException();
            }

            public MethodInfo GetConcreteMethod()
            {
                throw new NotImplementedException();
            }

            public MethodInfo GetConcreteMethodInvocationTarget()
            {
                throw new NotImplementedException();
            }

            public void SetArgumentValue(int index, object value)
            {
                throw new NotImplementedException();
            }
            #endregion
        }
    }
}
