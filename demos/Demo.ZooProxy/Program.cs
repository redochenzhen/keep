
using Keep.ZooProxy;
using org.apache.zookeeper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.ZooProxy
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var props = new Dictionary<string, string>
            {
                { "name", "hello" },
                { "ip", "10.1.1.1" }
            };
            for (int i = 0; i < 10; i++)
            {
                props.Add(i.ToString(), i.ToString());
            }
            //var RCA = Permission.Read | Permission.Create | Permission.Admin;
            var perm = Permission.All;

            string connStr = "127.0.0.1:2181/test";
            var zkClient = new ZooKeeperClient(connStr, int.MaxValue);
            await zkClient.OpenAsync();
            //INode node1 = await zkClient.ProxyNodeAsync("/services");
            //await node1.CreateAsync(perm);
            //INode node2 = await node1.ProxySubNodeAsync("/app1");
            //await node2.CreateAsync(perm);
            //INode node3 = await node2.ProxySubNodeAsync("/order");
            //await node3.CreateAsync(perm);

            var flag = await zkClient.ProxyNodeAsync("/flag");
            await flag.CreateAsync(perm, Mode.Ephemeral, ignoreExists: true);

            if (true)
            {

                var n1 = await zkClient.ProxyNodeAsync("n1");

                n1.NodeCreated += (s, e) => Console.WriteLine(e.Path);
                n1.NodeDeleted += (s, e) => Console.WriteLine(e.Path);
                //n1.ChildrenChanged += async (s, e) =>
                // {
                //     Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                //     //if (e.Children.Count == 3)
                //     //    await Task.Delay(3000);
                //     await Task.Delay(1000 * e.Children.Count);
                //     if (e.Children.Count > 0)
                //         Console.WriteLine(string.Join(',', e.Children.ToArray()));
                //     else
                //         Console.WriteLine("0");
                // };
                n1.ChildrenChanged += (s, e) =>
                {
                    Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                    //if (e.Children.Count == 3)
                    //    await Task.Delay(3000);
                    //Task.Delay(1000 * e.Children.Count).Wait();
                    if (e.Children.Count > 0)
                        Console.WriteLine(string.Join(',', e.Children.ToArray()));
                    else
                        Console.WriteLine("--0--");
                };
                await n1.CreateAsync(perm, ignoreExists: true);

                var d1 = await n1.ProxyJsonNodeAsync<Service>("/d1");
                var s0 = new Service
                {
                    Name = "order",
                    Ip = "10.1.1.1",
                    Port = 80
                };
                await d1.CreateAsync(s0, perm, ignoreExists: true);

                var d2 = await n1.ProxyValueNodeAsync<int>("/x");
                d2.DataChanged += (s, e) =>
                  {
                      Console.WriteLine($"change: {e.Data}");
                  };
                await d2.CreateAsync(12, perm, Mode.Persistent, true);
                var v = await d2.GetDataAsync();
                Console.WriteLine($"value: {v}");

                var s = await d1.GetDataAsync();
                Console.WriteLine(s.Name);
                Console.WriteLine(s.Ip);
                Console.WriteLine(s.Port);

                d1.DataChanged += (s, e) =>
                  {
                      Console.WriteLine(e.Path);
                      if (e.Data != null)
                          Console.WriteLine($"[{e.Data.Name},{e.Data.Ip},{e.Data.Port}]");
                      else
                          Console.WriteLine("null");
                  };
                d1.ChildrenChanged += (s, e) =>
                {
                    if (e.Children.Count > 0)
                        Console.WriteLine(string.Join(',', e.Children.ToArray()));
                    else
                        Console.WriteLine("--0--");
                };
            }

            if (false)
            {

                var p1 = await zkClient.ProxyPropertyNodeAsync("/p1");
                p1.NodeCreated += (s, e) =>
                  {
                      Console.WriteLine("========================");
                      Console.WriteLine(e.Path);
                  };
                p1.ChildrenChanged += (s, e) =>
                  {
                      //Console.WriteLine(e.Children.Count);
                      if (e.Children.Count > 0)
                          Console.WriteLine(string.Join(',', e.Children.ToArray()));
                      else
                          Console.WriteLine("--0--"); Console.WriteLine();
                  };
                p1.PropertyChanged += (s, e) =>
                 {
                     Console.WriteLine($"({e.Key},{e.Value})");
                 };
                await p1.CreateAsync(props, perm, ignoreExists: true);
            }

            if (false)
            {
                var n3 = await zkClient.ProxyNodeAsync("/n3");
                var n4 = await n3.ProxyNodeAsync("/n4");
                await n3.CreateAsync(perm, ignoreExists: true);
                await n4.CreateAsync(perm, ignoreExists: true);
                var n5 = await n4.ProxyValueNodeAsync<int>("/n5");
                await n5.CreateAsync(100, perm, ignoreExists: true);
                n5.DataChanged += (s, e) =>
                  {
                      Console.WriteLine($"path: {e.Path} data: {e.Data}");
                  };

                var n6 = await n4.ProxyPropertyNodeAsync("/n6");
                n6.NodeCreated += (s, e) => Console.WriteLine("c");
                //await n6.CreateAsync(perm);
                //await n6.CreateAsync(props, perm, ignoreExists: true);

            }

            for (; ; )
            {
                try
                {
                    Console.ReadLine();
                    var x = await zkClient.ProxyNodeAsync("/y");
                    await x.CreateAsync(perm, Mode.EphemeralSequential, ignoreExists: true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Console.WriteLine("!!!");
            Console.ReadLine();
        }

        class Service
        {
            public string Name { get; set; }
            public string Ip { get; set; }
            public int Port { get; set; }
        }
    }
}
