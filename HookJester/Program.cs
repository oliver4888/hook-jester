using System;
using System.IO;
using System.Linq;
using System.Diagnostics;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            IWebHostBuilder webHostBuilder = CreateWebHostBuilder(args);

            IWebHost webHost = webHostBuilder.Build();

            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(webHost.Services.GetRequiredService<IConfiguration>())
                .CreateLogger();

            try
            {
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

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseUrls("http://localhost:9090")
                .UseStartup<Startup>();
    }
}
