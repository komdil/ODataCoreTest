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
            StartHost(url);
        }
        internal static IHost StartHost(string url)
        {
            var host = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                webBuilder.UseIISIntegration();
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls(url);
            }).Build();
            host.Start();
            return host;
        }
    }
}
