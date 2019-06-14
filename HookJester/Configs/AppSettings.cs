using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.Configuration;

namespace HookJester.Configs
{
    public class AppSettings : IAppSettings
    {
        public AppSettings()
        {
            //Configuration. change token, move this to method, reload on change token change
            Keys = Configuration.GetSection("AppSettings:Keys").GetChildren()
                .Select(item => new KeyValuePair<string, string>(item.Key, item.Value))
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private static IConfiguration _configuration;
        private IConfiguration Configuration = _configuration ?? (_configuration = GetConfiguration());

        private static IConfiguration GetConfiguration() =>
            new ConfigurationBuilder().AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"), false, true).Build();

        private string GetSetting([CallerMemberName]string setting = "") => Configuration.GetSection($"AppSettings:{setting}").Value;

        public string[] Urls => GetSetting().Split(",");
        public IDictionary<string, string> Keys { get; set; }
    }
}
