using System.Reflection.Emit;
using System.Reflection;

namespace AOPDemo8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var target = new ConsoleILogger();
            var logger = ProxyGenerator.Create<ILogger>(target, new Inteceptor1(), new Inteceptor2());
            logger.Log("ff");
        }
    }

    public interface IInvocation
    {
        void Proceed();
    }
    public class InvocationInteceptor : IInvocation
    {
        private IInteceptor _inteceptor;
        private IInvocation _invocation;
        public InvocationInteceptor(IInteceptor inteceptor, IInvocation invocation)
        {
            _inteceptor = inteceptor;
            _invocation = invocation;
        }

        public void Proceed()
        {
            _inteceptor.Intercept(_invocation);
        }
    }
    public class InvocationTraget : IInvocation
    {
        private object instance;
        private MethodInfo method;
        private object[] arguments;

        public InvocationTraget(object instance, MethodInfo method, object[] arguments)
        {
            this.instance = instance;
            this.method = method;
            this.arguments = arguments;
        }

        public void Proceed()
        {
            method.Invoke(instance, arguments);
        }
    }
    //拦截器
    public interface IInteceptor
    {
        void Intercept(IInvocation invocation);
    }

    //该工具类帮助我们少写emit代码
    public static class InvocationUtilities
    {
        private static IInvocation GetNextInvocation(IInvocation target, IInteceptor[] inteceptors, int index)
        {
            if (index < inteceptors.Length)
            {
                var next = GetNextInvocation(target, inteceptors, index + 1);
                return new InvocationInteceptor(inteceptors[index], next);
            }
            else
            {
                return target;
            }
        }

        public static void Invoke(IInteceptor[] interceptors, object target, MethodInfo? method, object[] arguments)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (interceptors.Any())
            {
                var inteceptor = interceptors[0];
                var invocation = GetNextInvocation(new InvocationTraget(target, method, arguments), interceptors, 1);
                inteceptor.Intercept(invocation);
            }
            else
            {
                method.Invoke(target, arguments);
            }
        }
    }

    //实现拦截器1
    public class Inteceptor1 : IInteceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine("prox1:start");
            invocation.Proceed();
            Console.WriteLine("prox1:end");
        }
    }

    //实现拦截器1
    public class Inteceptor2 : IInteceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine("prox2:start");
            invocation.Proceed();
            Console.WriteLine("prox2:end");
        }
    }

    public interface ILogger
    {
        void Log(string message);
    }

    public class ConsoleILogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine("Console:" + message);
        }
    }

    public class ConsoleILoggerProxy : ILogger
    {
        private readonly ILogger _target;
        private readonly IInteceptor[] _inteceptors;

        public ConsoleILoggerProxy(ILogger target, IInteceptor[] inteceptors)
        {
            _target = target;
            _inteceptors = inteceptors;
        }

        public void Log(string message)
        {
            var method = _target.GetType().GetMethod("Log");
            var args = new object[1];
            args[0] = message;
            InvocationUtilities.Invoke(_inteceptors, _target, method, args);
        }
    }

    public static class ProxyGenerator
    {
        static AssemblyBuilder _assemblyBuilder;
        static ModuleBuilder _moduleBuilder;
        static ProxyGenerator()
        {
            //创建一个程序集
            var assemblyName = new AssemblyName("DynamicProxies");
            _assemblyBuilder = AssemblyBuilder
                .DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            //创建一个模块
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule("Proxies");
        }
        public static TInterface Create<TInterface>(object target, params IInteceptor[] inteceptor)
            where TInterface : class
        {

            #region 定义类型
            //定义一个class，如果这个类型已定义直接返回，缓存
            var typeName = $"{target.GetType().Name}EmitProxy";
            var typeBuilder = _moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Public,
                typeof(object),
                new Type[]
                {
                typeof(TInterface)
                });
            #endregion

            #region 定义字段
            //定义字段
            var targetFieldBuilder = typeBuilder.DefineField("target", typeof(object), FieldAttributes.Private);
            var inteceptorFieldBuilder = typeBuilder.DefineField("inteceptor", typeof(IInteceptor[]), FieldAttributes.Private);
            #endregion

            #region 定义构造器
            //定义构造器
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.ExplicitThis, new Type[]
            {
            typeof(TInterface),
            typeof(IInteceptor[])
            });
            //获取IL编辑器
            var generator = constructorBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);//加载this
            generator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes) ?? throw new InvalidOperationException());
            generator.Emit(OpCodes.Nop);
            // this._target = target;
            generator.Emit(OpCodes.Ldarg_0);//加载this
            generator.Emit(OpCodes.Ldarg_1);//加载target参数
            generator.Emit(OpCodes.Stfld, targetFieldBuilder);//加载target字段
                                                              // this._inteceptor = inteceptor;
            generator.Emit(OpCodes.Ldarg_0);//加载this
            generator.Emit(OpCodes.Ldarg_2);//加载inteceptor参数
            generator.Emit(OpCodes.Stfld, inteceptorFieldBuilder);//加载inteceptor字段
            generator.Emit(OpCodes.Ret);

            #endregion

            #region 实现接口
            var methods = typeof(TInterface).GetMethods();
            foreach (var item in methods)
            {
                var parameterTypes = item.GetParameters().Select(s => s.ParameterType).ToArray();
                var methodBuilder = typeBuilder.DefineMethod(item.Name,
                    MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.HideBySig,
                    CallingConventions.Standard | CallingConventions.HasThis,
                    item.ReturnType,
                    parameterTypes);
                var generator1 = methodBuilder.GetILGenerator();
                //init
                var methodInfoLocal = generator1.DeclareLocal(typeof(MethodInfo));
                var argumentLocal = generator1.DeclareLocal(typeof(object[]));
                generator1.Emit(OpCodes.Nop);//{
                                             // MethodInfo method = this.GetType().GetMethod("Log");
                generator1.Emit(OpCodes.Ldarg_0);
                generator1.Emit(OpCodes.Ldfld, targetFieldBuilder);//this._target
                generator1.Emit(OpCodes.Callvirt, typeof(object).GetMethod(nameof(Type.GetType), Type.EmptyTypes));
                generator1.Emit(OpCodes.Ldstr, item.Name);
                generator1.Emit(OpCodes.Callvirt, typeof(Type).GetMethod(nameof(Type.GetMethod), new Type[] { typeof(string) }));
                generator1.Emit(OpCodes.Stloc, methodInfoLocal);
                // object[] array = new object[0];
                generator1.Emit(OpCodes.Ldc_I4, 1);
                generator1.Emit(OpCodes.Newarr, typeof(object));
                generator1.Emit(OpCodes.Stloc, argumentLocal);
                // array[0] = message;
                generator1.Emit(OpCodes.Ldloc, argumentLocal);
                generator1.Emit(OpCodes.Ldc_I4, 0);
                generator1.Emit(OpCodes.Ldarg_1);
                generator1.Emit(OpCodes.Stelem_Ref);
                // InvocationUtilities.Invoke(_inteceptors, _target, method, array);
                generator1.Emit(OpCodes.Ldarg_0);
                generator1.Emit(OpCodes.Ldfld, inteceptorFieldBuilder);//this._interceptor
                generator1.Emit(OpCodes.Ldarg_0);
                generator1.Emit(OpCodes.Ldfld, targetFieldBuilder);//this._target
                generator1.Emit(OpCodes.Ldloc, methodInfoLocal);
                generator1.Emit(OpCodes.Ldloc, argumentLocal);
                generator1.Emit(OpCodes.Call, typeof(InvocationUtilities).GetMethod(nameof(InvocationUtilities.Invoke)));
                generator1.Emit(OpCodes.Nop);
                generator1.Emit(OpCodes.Ret);
            }
            #endregion
            //创建:这个type可以用一个线程安全的字典缓存起来，第二次需要这个代理类的时候，就不需要在生成一次emit代码了。
            var type = typeBuilder.CreateType() ?? throw new ArgumentException();
            var instance = Activator.CreateInstance(type, target, inteceptor);
            return (TInterface)instance;
        }
    }
}
