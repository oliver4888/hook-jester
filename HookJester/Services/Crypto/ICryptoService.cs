using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HookJester.Services.Crypto
{
    public interface ICryptoService
    {
        string GetCryptoRandomString(int length = 64);
        string GetRandomString(int length = 64);
    }
}
