using System;
using System.Collections.Generic;
using System.Text;

namespace Keep.Common
{
    public class OptionsBase
    {
        public List<IPlugin> Plugins { get; } = new List<IPlugin>();

        public void RegisterPlugin(IPlugin plugin)
        {
            Plugins.Add(plugin);
        }
    }
}
