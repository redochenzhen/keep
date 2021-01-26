using System.Collections.Generic;

namespace Keep.Common.Framework
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
