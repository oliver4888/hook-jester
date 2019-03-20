using System;
using System.Linq;
using System.Security.Cryptography;

namespace HookJester.Services.Crypto
{
    public class CryptoService : ICryptoService
    {
        private static Random random = new Random();
        // Not crypto but meh
        public string GetRandomString(int length = 64)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string GetCryptoRandomString(int length = 64)
        {
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[length];
                rng.GetBytes(tokenData);

                return Convert.ToBase64String(tokenData);
            }
        }
    }
}
