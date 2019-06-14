using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HookJester.Configs
{
    public interface IAppSettings
    {
        string[] Urls { get; }
        IDictionary<string, string> Keys { get; }
    }
}
