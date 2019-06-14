using System;
using System.IO;
using System.Threading;
using System.Security.Cryptography;

namespace HookJester.Services.Files
{
    public class FileHasher : IFileHasher
    {
        public byte[] ComputeHash(string filePath)
        {
            var runCount = 1;

            while (runCount < 4)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        using (var fs = File.OpenRead(filePath))
                        {
                            return SHA1.Create().ComputeHash(fs);
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException();
                    }
                }
                catch (IOException ex)
                {
                    if (runCount == 3 || ex.HResult != -2147024864)
                    {
                        throw;
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(Math.Pow(2, runCount)));
                        runCount++;
                    }
                }
            }

            return new byte[20];
        }
    }
}
