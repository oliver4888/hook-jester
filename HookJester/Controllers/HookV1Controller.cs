using System;
using System.IO;
using System.Threading.Tasks;
using IOFile = System.IO.File;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

using Newtonsoft.Json;

using HookJester.Models;
using HookJester.Configs;
using HookJester.Services.Crypto;

namespace HookJester.Controllers
{
    [Route("hook/v1/")]
    [ApiController]
    public class HookV1Controller : ControllerBase
    {
        private ILogger<HookV1Controller> _logger;
        private ICryptoService _cryptoService;
        private IAppSettings _appSettings;

        public HookV1Controller(ILogger<HookV1Controller> logger, ICryptoService cryptoService, IAppSettings appSettings)
        {
            _logger = logger;
            _cryptoService = cryptoService;
            _appSettings = appSettings;
        }

        [HttpPost("[action]/{name}")]
        public async Task<IActionResult> Default(string name)
        {
            V1JsonFile output = new V1JsonFile
            {
                Headers = Request.Headers,
                Body = await new StreamReader(Request.Body).ReadToEndAsync()
            };

            if (Request.Headers.ContainsKey("X-Hub-Signature") && Request.Headers.ContainsKey("Content-Length"))
            {
                if (!_appSettings.Keys.TryGetValue(name, out string key))
                {
                    _logger.LogWarning($"Unable to verify payload from {Request.HttpContext.Connection.RemoteIpAddress} for application \"{name}\"");
                    return NoContent();
                }

                Request.Headers.TryGetValue("X-Hub-Signature", out StringValues hubSignature);
                Request.Headers.TryGetValue("Content-Length", out StringValues contentLength);

                output.PayloadIsVerified = _cryptoService.PayloadIsVerified(
                    long.Parse(contentLength[0]), hubSignature[0], output.Body, key);
            }

            if (output.PayloadIsVerified == false)
            {
                _logger.LogWarning($"Request from {Request.HttpContext.Connection.RemoteIpAddress} failed Hub Signature Verification");
                return NoContent();
            }

            if (Request.QueryString.Value.Length != 0)
            {
                foreach (string match in Request.QueryString.Value.Substring(1).Split("&"))
                {
                    string[] pair = match.Split("=");
                    if (pair.Length == 1)
                        output.QuerySingles.Add(pair[0]);
                    else if (pair.Length == 2)
                        output.QueryPairs.Add(pair[0], pair[1]);
                }
            }

            string outputJson = JsonConvert.SerializeObject(output);

            string outputDir = Path.Combine(Environment.CurrentDirectory, "Output", name);
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            await IOFile.WriteAllTextAsync(
                Path.Combine(
                    outputDir, $"v1-{DateTime.Now.ToFileTimeUtc()}-{_cryptoService.GetRandomString(5)}.json"
                    ), outputJson);

            return Accepted();
        }
    }
}
