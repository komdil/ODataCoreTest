using System;
using System.Linq;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;


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
                webBuilder.ConfigureServices(services =>
                {
                    services.AddControllers(op => op.AllowEmptyInputInBodyModelBinding = true).AddNewtonsoftJson(op => op.SerializerSettings.ContractResolver = new DefaultContractResolver());
                    services.AddOData();
                    services.AddMvc(options =>
                    {
                        //options.OutputFormatters.Insert(0, new VcardOutputFormatter(options.OutputFormatters.OfType<ODataOutputFormatter>().Last()));
                        options.EnableEndpointRouting = false;
                    }).AddNewtonsoftJson(op => op.SerializerSettings.ContractResolver = new DefaultContractResolver());
                });
                webBuilder.UseHttpSys(op =>
                {
                    op.UrlPrefixes.Add(url);
                    op.AllowSynchronousIO = true;
                });
                webBuilder.Configure(action);
            }).Build();
            host.Start();
            return host;
        }
    }
}
