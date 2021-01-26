using org.apache.zookeeper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keep.ZooProxy
{
    internal class PropertyNodeProxy : NodeProxy, IPropertyNodeProxy
    {
        public event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        public PropertyNodeProxy(ZooKeeperClient zkClient, string name, string path) : base(zkClient, name, path) { }

        protected void OnPropertyChanged(object sender, string path, string key, string value)
        {
            if (PropertyChanged == null) return;
            var args = new PropertyChangedEventArgs
            {
                Path = path,
                Key = key,
                Value = value
            };
            PropertyChanged(sender, args);
        }

        public async Task CreateAsync(IDictionary<string, string> properties, Permission permission, Mode mode, bool ignoreExists)
        {
            await CreateAsync(permission, mode, ignoreExists);
            //理论上，当前NodeProxy的ChildrenChanged事件可能触发多次（且Children参数不稳定），
            //但是，在这些事件中，一定存在Children的最终状态
            var tasks = (properties ?? new Dictionary<string, string>())
                .Select(async kv =>
                {
                    var keyNode = await ProxyValueNodeAsync<string>(kv.Key, IsWatched);
                    await keyNode.CreateAsync(kv.Value, permission, mode, ignoreExists).ConfigureAwait(false);
                    keyNode.DataChanged += (sender, args) =>
                      {
                          OnPropertyChanged(sender, args.Path, kv.Key, args.Data);
                      };
                });
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        //public override async Task WatchedByAsync(Watcher watcher)
        //{
        //    await base.WatchedByAsync(watcher);
        //    ChildrenChanged += async (_, args) =>
        //     {
        //         var tasks = args.Children
        //          .Select(async child =>
        //          {
        //              var path = $"{args.Path}{child}";
        //              await _connection.existsAsync(path, watcher).ConfigureAwait(false);
        //          });
        //         await Task.WhenAll(tasks).ConfigureAwait(false);
        //     };
        //}

        public Task<string> GetValue(string key)
        {
            throw new NotImplementedException();
        }

        public Task SetValue(string key, string value)
        {
            throw new NotImplementedException();
        }
    }
}
