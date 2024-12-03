using System.Collections;

namespace AOPDemo1
{
    internal class Program
    {
        /*
         * 基础知识
         * 假设这是一个U型管道，污水水从一端流入，另一端流出。
         * 现在我们要对污水进行过滤，A负责过滤B负责消毒。显然有四个处理点，他们的顺序分别是：1->2->3->4。
         * 其中1，4是A过滤器的处理点，2，3是B过滤器的处理点。显然3个过滤器就有6个处理点。我们可以随意调整A，B过滤器的顺序，可随意插拔。这就是AOP的思想。
         * 执行顺序：先进后出（栈）
         * 执行点数：过滤器数 * 2
         * AOP是对OOP的一种补充，即面向切面编程，一种编程思想。我们管A，B为切面。1~4为切入点。AOP的优势是面向切面编程，每个切面负责独立的系统逻辑，
         * 降低代码的复杂度，提高代码的复用率。可以随意调整顺序，随意插拔。用于对业务逻辑进行增强。面向切面编程可以使得系统逻辑和业务逻辑进行分离。
         * 系统逻辑：比如身份认证，异常处理，参数校验
         * 业务逻辑：就是我们真正关心不得不写的业务逻辑。
         */
        static void Main(string[] args)
        {
            var target = new List<object>();
            //可以看到MyCollection对target进行了代理，加强了GetEnumerator函数（可以打印消息）
            var collection = new MyCollection(target);
            //此时GetEnumerator就会被加强，返回target的迭代器。
            var it = collection.GetEnumerator();

            Console.WriteLine("Hello, World!");
        }
    }

    /*
     * 假设我们需要实现一个IList接口，我们知道IList接口有很多方法，实现成本非常高。我们可以通过代理模式来实现
     * 代理模式可以降低实现的成本，还可以对目标对象进行加强。代理者不需要实现具体的业务逻辑，只需要编写加强逻辑即可。
     */
    public class MyCollection : IEnumerable<object>
    {
        private IEnumerable<object> _target;
        public MyCollection(IEnumerable<object> target)
        {
            _target = target;
        }
        public IEnumerator<object> GetEnumerator()
        {
            //编写加强逻辑比如打印
            Console.WriteLine("调用迭代器了");
            //通过target来实现，代理类之关系加强逻辑，不关心接口实现
            return _target.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_target).GetEnumerator();
        }
    }
}
