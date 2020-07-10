using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace ODataCoreTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var url = "https://localhost:44383";
            var actionBuilder = Configuration.GetBuilder();
            StartHost(url, actionBuilder);
        }
        internal static IHost StartHost(string url, Action<IApplicationBuilder> action)
        {
            var host = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                webBuilder.UseIISIntegration();
                webBuilder.UseStartup<Startup>();
                webBuilder.UseHttpSys(op => op.UrlPrefixes.Add(url));
                webBuilder.Configure(action);
            }).Build();
            host.Start();
            return host;
        }
    }
}
