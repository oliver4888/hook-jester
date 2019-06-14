using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using HookJester.Services.Files;
using Microsoft.Extensions.Primitives;

namespace HookJester.Configs
{
    public class AppSettings : IAppSettings
    {
        private ILogger<AppSettings> _logger;
        private IFileHasher _fileHasher;

        private byte[] _fileHash;

        private IConfiguration _configuration;

        private string _filePath;

        public AppSettings(ILogger<AppSettings> logger, IFileHasher fileHasher)
        {
            _logger = logger;
            _fileHasher = fileHasher;

            _filePath = Path.Combine(Environment.CurrentDirectory, "appsettings.json");

            _logger.LogDebug($"Starting {nameof(AppSettings)}, file path: {_filePath}");

            _configuration = new ConfigurationBuilder()
                .AddJsonFile(_filePath, false, true).Build().GetSection(nameof(AppSettings));

            _fileHash = _fileHasher.ComputeHash(_filePath);

            Rebind();

            ChangeToken.OnChange(() => _configuration.GetReloadToken(), () =>
            {
                byte[] newHash = _fileHasher.ComputeHash(_filePath);

                if (newHash.SequenceEqual(_fileHash)) return;

                _logger.LogDebug("Configuration changed. Reloading...");

                _fileHash = newHash;

                Rebind();
            });
        }

        private void Rebind()
        {
            Keys = new Dictionary<string, string>();
            _configuration.Bind(this);
        }

        public string[] Urls { get; set; }
        public IDictionary<string, string> Keys { get; set; }
    }
}
