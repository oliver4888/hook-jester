using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using HookJester.Hosting;
using HookJester.Services.Crypto;


namespace HookJester.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LocalController : ControllerBase
    {
        private ILogger<HookV1Controller> _logger;
        private ICryptoService _cryptoService;

        public LocalController(ILogger<HookV1Controller> logger, ICryptoService cryptoService)
        {
            _logger = logger;
            _cryptoService = cryptoService;
        }

        [HttpGet("[action]/")]
        [LocalActionFilter]
        public IActionResult GetCryptoRandomStrings()
        {
            string[] strings = new string[5];
            for (int i = 0; i < 5; i++)
                strings[i] =_cryptoService.GetCryptoRandomString();
            return Ok(strings);
        }
    }
}
