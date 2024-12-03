using System.Text;

namespace IOCDemo7
{
    internal class Program
    {
        /*
          装饰者模式侧重于添加装饰（方法），装饰者模式在Stream里面使用非常频繁，我们说流本质都是二进制。但是实际操作起来，有的是字符串。
          于是就有了TextStream、StreamReader把他们装饰成文本流，并提供新的api，我们看一个案例。
        */
        static void Main(string[] args)
        {
            IOStream stream1 = new FileStream();
            //当作代理来使用，此时我们只能调用到IOStream中的api
            IOStream streamProxy = new TextStream(stream1);
            //使用装饰者特征，因为现在这个流被装饰成文本了
            TextStream textStream = new TextStream(stream1);
            var text = textStream.ReadToEnd();
        }
    }

    public interface IOStream
    {
        byte[] ReadAll();
        void Write(byte[] buffer);
        void Close();
    }

    public class FileStream : IOStream
    {
        private List<byte> _buffer = new List<byte>();
        public void Write(byte[] buffer)
        {
            _buffer.AddRange(buffer);
        }

        public byte[] ReadAll()
        {
            return _buffer.ToArray();
        }

        public void Close()
        {
            Console.WriteLine("文件已关闭");
        }
    }
    // TextStream既表现出代理特征，也表现出装饰特征，但是侧重装饰，因为它并没有加强目标对象的函数（没有不代表不可以）
    // 一个类可以使用很多设计模式，并没有谁规定只能使用一个，我们要分析侧重那个点，是侧重代理还是侧重装饰
    public class TextStream : IOStream
    {
        private IOStream _stream;

        public TextStream(IOStream stream)
        {
            _stream = stream;
        }
        //表现代理特征，因为我不关系具体实现，并且他是我要实现的标准
        public void Write(byte[] buffer)
        {
            //实打实的加强了
            Console.WriteLine("要开始写入了");
            _stream.Write(buffer);
        }
        //表现代理特征，因为我不关系具体实现，并且他是我要实现的标准
        public byte[] ReadAll()
        {
            //必须调用目标对象的函数才算代理
            return _stream.ReadAll();
        }
        //表现重写特征，因为我想自己写
        public void Close()
        {
            Console.WriteLine("释放了");
        }
        //表现装饰特征，因为这是多出来的装饰，不是标准要求的，额外的
        public string ReadToEnd()
        {
            return Encoding.UTF8.GetString(ReadAll());
        }
    }
}
