using Microsoft.Extensions.Primitives;

namespace ConfigDemo2
{
    internal class Program
    {
        /*
            依赖项
            Microsoft.Extensions.Primitives：提供配置更改的核心接口和实现
            核心接口
            IChangeToken：用于注册回调
            ChangeToken：用于绑定生产者和消费者，注册一个回调，如果更改发生就获取使用生成者生产一个新的IChangeToken，并执行消费者，并在次注册回调
            CancellationChangeToken：IChangeToken的实现一种实现，
            CancellationTokenSource：用于产生取消令牌，执行取消令牌，
            CancellationToken：取消令牌，可以注册取消之后的回调。负责在CancellationChangeToken和CancellationTokenSource直接传递消息 
        */
        static void Main(string[] args)
        {
            var provider = new FileConfigurationProvider();
            //绑定
            provider.Watch();
            new TaskCompletionSource().Task.Wait();
        }

        /// <summary>
        /// 文件配置程序超类
        /// </summary>
        public class FileConfigurationProvider
        {
            private CancellationTokenSource? tokenSource;

            public void Load()
            {
                Console.WriteLine($"[{DateTime.Now}]文件已加载...");
            }

            public void Watch()
            {
                Load();
                //将changeToken生产者和changeToken消费者进行绑定(订阅)
                ChangeToken.OnChange(GetReloadToken, Load);
                //触发Change事件，通知更新
                var t = new Thread(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(3000);
                        var t = tokenSource;
                        tokenSource = null;//取消之前一定要设置成null                
                        t!.Cancel();//执行回调，发布取消事件。
                    }
                });
                t.Start();
            }

            /// <summary>
            /// 更新令牌，通过该令牌可以注册回调，用于执行更新通知。
            /// </summary>
            /// <returns></returns>
            public IChangeToken GetReloadToken()
            {
                lock (this)
                {
                    //如果被消费就创建一个新的
                    if (tokenSource == null)
                    {
                        tokenSource = new CancellationTokenSource();
                    }
                    return new CancellationChangeToken(tokenSource.Token);
                }
            }
        }
    }
}
