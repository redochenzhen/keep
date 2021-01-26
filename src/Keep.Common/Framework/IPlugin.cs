using Microsoft.Extensions.DependencyInjection;

namespace Keep.Common.Framework
{
    public interface IPlugin
    {
        void Register(IServiceCollection services);
    }
}
