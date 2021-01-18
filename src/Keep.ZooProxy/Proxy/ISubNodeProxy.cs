using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keep.ZooProxy
{
    public interface ISubNodeProxy
    {
        Task<INode> ProxyNodeAsync(string name, bool watch = false);

        Task<IDataNode<T>> ProxyValueNodeAsync<T>(string name, bool watch = false);

        Task<IDataNode<T>> ProxyJsonNodeAsync<T>(string name, bool watch = false) where T : class;

        Task<IPropertyNode> ProxyPropertyNodeAsync(string name, bool watch = false);
    }

    public interface INodeProxy
    {
        Task<INode> ProxyNodeAsync(string name, bool watch = false, string parentPath = default);

        Task<IDataNode<T>> ProxyValueNodeAsync<T>(string name, bool watch = false, string parentPath = default);

        Task<IDataNode<T>> ProxyJsonNodeAsync<T>(string name, bool watch = false, string parentPath = default) where T : class;

        Task<IPropertyNode> ProxyPropertyNodeAsync(string name, bool watch = false, string parentPath = default);
    }
}
