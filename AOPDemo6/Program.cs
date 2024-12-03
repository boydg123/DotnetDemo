using Castle.DynamicProxy;

namespace AOPDemo6
{
    /*
     *动态代理可以通过Castle.Core来实现。我们说静态代理和动态代理的区别是，静态代理在代码编译之前就已经确立的代理关系。
     *而动态代理的原理是，在编译之后，运行时通过Emit来动态创建目标对象的子类，或者实现目标对象的接口。把拦截器织入到动态生成的类中，
     *这里的拦截器可以织入到任意的实现类中。（Emit技术可以在运行时生成一个class，大家可以通过打印castle.core返回的实列的类名来进行验证）
     *动态代理和静态代理的本质都是继承或者实现，但是静态代理是需要手动编写代理类，而动态代理由框架动态生成代理类。动态代理性能更差，对异步支持不友好。
     *注意：如果是通过实现类的方式，那么无论静态代理还是动态代理，都只能代理父类中的虚函数(virtual)，因为子类只能重写父类中的虚函数。所以建议使用接口的方式。
     */
    internal class Program
    {
        static void Main(string[] args)
        {
            var generator = new ProxyGenerator();
            var target = new Water();
            //通过框架生成实现IWater接口的代理类
            var proxy = generator.CreateInterfaceProxyWithTarget<IWater>(target, new Interceptor1(), new Interceptor2());
            Console.WriteLine(proxy.GetType().FullName);//可以看到这个类并不是我们生成的
            proxy.Invoke();
            //如果通过继承方式会怎么样？
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
}
