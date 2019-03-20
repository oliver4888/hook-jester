using System;
using System.IO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using HookJester.Models;
using HookJester.Services.Crypto;

namespace HookJester.Controllers
{
    [Route("hook/v1/")]
    [ApiController]
    public class HookV1Controller : ControllerBase
    {
        private ILogger<HookV1Controller> _logger;
        private ICryptoService _cryptoService;

        public HookV1Controller(ILogger<HookV1Controller> logger, ICryptoService cryptoService)
        {
            _logger = logger;
            _cryptoService = cryptoService;
        }

        [HttpPost("[action]/{name}")]
        public IActionResult Simple(string name)
        {
            V1JsonFile output = new V1JsonFile
            {
                Headers = Request.Headers,
                Body = new StreamReader(Request.Body).ReadToEnd()
            };

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

            string outputDir = $"{Environment.CurrentDirectory}/Output/{name}";
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            System.IO.File.WriteAllText($"{Environment.CurrentDirectory}/Output/{name}/v1-{DateTime.Now.ToFileTimeUtc()}-{_cryptoService.GetRandomString(5)}.json", outputJson);

            return NoContent();
        }
    }
}
