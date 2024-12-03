namespace GenericInOut;

internal class Program
{
    static void Main(string[] args)
    {
        #region 协变
        //ITest1<string> implementStr = new ImplementStr();
        //object result1 = implementStr.Foo();
        //Console.WriteLine(result1);

        //ITest1<object> implementObj = implementStr;
        //object result2 = implementObj.Foo();
        //Console.WriteLine(result2);
        #endregion

        #region 逆变
        ITest2<object> implementObj = new ImplementObj();
        implementObj.Bar("bar1");

        ITest2<string> implementStr = implementObj;
        implementStr.Bar("bar2");

        #endregion
    }
}
#region 协变
public interface ITest1<out T>
{
    T Foo();
}

public partial class ImplementStr : ITest1<string>
{
    public string Foo()
    {
        return " foo";
    }
}

public partial class ImplementObj : ITest1<object>
{
    public object Foo()
    {
        return 1;
    }
}
#endregion

#region 逆变
public interface ITest2<in T>
{
    void Bar(T parameter);
}

public partial class ImplementStr : ITest2<string>
{
    public void Bar(string parameter)
    {
        Console.WriteLine($" string parameter：{parameter}");
    }
}

public partial class ImplementObj : ITest2<object>
{
    public void Bar(object parameter)
    {
        Console.WriteLine($" object parameter：{parameter}");
    }
}
#endregion
