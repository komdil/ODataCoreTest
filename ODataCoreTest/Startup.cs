using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Reflection;

namespace ODataCoreTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var modelAssembly = Assembly.LoadFrom(@"C:\Users\Owner\source\repos\ODataCoreTest\ClassLibrary2\bin\Debug\netcoreapp3.1\ClassLibrary2.dll");
            services.AddControllers(mvcOptions =>
                 mvcOptions.EnableEndpointRouting = false).PartManager.ApplicationParts.Add(new AssemblyPart(modelAssembly));
            services.AddOData();
            services.AddRouting();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers().Add(a=>a.;
            //});
        }
    }
}
