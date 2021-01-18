using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Keep.Common
{
    public abstract class PluginBase<TOptions> : IPlugin where TOptions : class
    {
        private readonly Action<TOptions> _configure;

        protected PluginBase() { }

        protected PluginBase(Action<TOptions> configure)
        {
            _configure = configure;
        }

        protected abstract void Configure(IServiceCollection services, IConfiguration config);

        public virtual void Register(IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            var config = sp.GetRequiredService<IConfiguration>();
            Configure(services, config);
            if (_configure != null)
            {
                services.Configure(_configure);
            }
        }
    }
}
