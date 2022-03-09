using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System.Collections.Generic;

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
            services.AddControllers(mvcOptions => mvcOptions.EnableEndpointRouting = false);
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
            var builder = new ODataConventionModelBuilder(app.ApplicationServices) { ContainerName = "DefaultContainer" };
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseODataBatching();
            var edmModel = GetEdmModel();
            app.UseDeveloperExceptionPage();
            app.UseMvc(routeBuilder =>
            {
                routeBuilder.EnableDependencyInjection();
                routeBuilder.Select().Filter().Expand();
                routeBuilder.MapODataServiceRoute("OData", "odata", b =>
                {
                    b.AddService(Microsoft.OData.ServiceLifetime.Singleton, sp => edmModel);
                    var conventions = ODataRoutingConventions.CreateDefault();
                    //Workaround for https://github.com/OData/WebApi/issues/1622
                    conventions.Insert(0, new AttributeRoutingConvention("OData", app.ApplicationServices, new DefaultODataPathHandler()));
                    //Custom Convention
                    b.AddService<IEnumerable<IODataRoutingConvention>>(Microsoft.OData.ServiceLifetime.Singleton, a => conventions);
                });
            });
        }

        static IEdmModel GetEdmModel()
        {
            var odataBuilder = new ODataConventionModelBuilder();

            var student = odataBuilder.EntityType<Student>();
            student.HasKey(s => s.Id);
            odataBuilder.EntitySet<Student>("Student");

            return odataBuilder.GetEdmModel();
        }
    }
}
