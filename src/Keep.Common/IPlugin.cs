using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Keep.Common
{
    public interface IPlugin
    {
        void Register(IServiceCollection services);
    }
}
