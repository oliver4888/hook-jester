using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

namespace HookJester.Services.Crypto
{
    public class CryptoService : ICryptoService
    {
        private ILogger<CryptoService> _logger;

        public CryptoService(ILogger<CryptoService> logger)
        {
            _logger = logger;
        }

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

        private static Regex HubSignatureRegex = new Regex("^([^=]+)=([^=]+)$");

        public bool PayloadIsVerified(long contentLength, string hubSignature, string payload, string secret)
        {
            if (payload.Length != contentLength)
                return false;

            Match match = HubSignatureRegex.Match(hubSignature);

            if (!match.Success)
                return false;

            string algorithm = match.Groups[1].Value;

            byte[] key = Encoding.ASCII.GetBytes(secret);
            byte[] input = Encoding.ASCII.GetBytes(payload);

            byte[] hash;

            switch (algorithm)
            {
                case "sha1":
                    HMACSHA1 sha1 = new HMACSHA1(key);
                    hash = sha1.ComputeHash(input);
                    break;
                case "sha256":
                    HMACSHA256 sha256 = new HMACSHA256(key);
                    hash = sha256.ComputeHash(input);
                    break;
                case "sha384":
                    HMACSHA384 sha384 = new HMACSHA384(key);
                    hash = sha384.ComputeHash(input);
                    break;
                case "sha512":
                    HMACSHA512 sha512 = new HMACSHA512(key);
                    hash = sha512.ComputeHash(input);
                    break;
                default:
                    _logger.LogWarning($"Unknown algorithm \"{algorithm}\"");
                    return false;
            }

            StringBuilder builder = new StringBuilder(hash.Length * 2);

            foreach (byte b in hash)
                builder.AppendFormat("{0:x2}", b);

            return match.Groups[2].Value == builder.ToString();
        }
    }
}
