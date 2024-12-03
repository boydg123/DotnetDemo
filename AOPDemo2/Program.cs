namespace AOPDemo2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 此时是先去除杂质后在消毒
            // p1的target是Water，p2的target是p1
            var target = new Water();
            var proxy = new WaterProxy(target);
            var proxy2 = new WaterProxy2(proxy);
            proxy2.Invoke();

            // 此时是先消毒后在去除杂质
            // p2的target是Water，p1的target是p2
            target = new Water();
            proxy2 = new WaterProxy2(target);
            proxy = new WaterProxy(proxy2);
            proxy.Invoke();

            /*
             * 可以看到系统逻辑和业务逻辑进行了分离，系统逻辑写到了不同的切面。切面之间何以随意组合，增减。
             * 这就是AOP思想的一种呈现方式。代码服用度很高，可以代理所有的IWater的实现。
             * （假设Mercury也实现了IWater接口，那么WaterProxy1和WaterProxy2也能对他进行增强）
             * 
             * 静态代理的本质是子类继承父类，或者实现接口，对目标对象进行增强。
             * 
             * 静态代理的弊端是只能实现一个接口（标准），无法代理其他类型的实列。他的切面的可复用率有限，限定在它实现的接口。
             */
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

    public class WaterProxy : IWater
    {
        private IWater _water;
        public WaterProxy(IWater water)
        {
            _water = water;
        }
        public void Invoke()
        {
            //前置通知
            Console.WriteLine("开始消毒杀菌");
            //业务逻辑
            _water.Invoke();
            //后置通知
            Console.WriteLine("完成消毒杀菌");
        }
    }

    public class WaterProxy2 : IWater
    {
        private IWater _water;
        public WaterProxy2(IWater water)
        {
            _water = water;
        }
        public void Invoke()
        {
            //前置通知
            Console.WriteLine("开始去除杂质");
            //业务逻辑
            _water.Invoke();
            //后置通知
            Console.WriteLine("完成去除杂质");
        }
    }
}
