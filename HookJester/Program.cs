using System;
using System.IO;
using System.Linq;
using System.Diagnostics;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using Serilog;

using HookJester.Extensions;

namespace HookJester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            bool isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                IWebHostBuilder webHostBuilder = WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseUrls(configuration.GetSection("AppSettings:Urls").Value.Split(","))
                .UseStartup<Startup>();

                IWebHost webHost = webHostBuilder.Build();

                Log.Information("Starting web host" + (isService ? " as a service" : ""));
                if (isService)
                    webHost.RunAsCustomService();
                else
                    webHost.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
