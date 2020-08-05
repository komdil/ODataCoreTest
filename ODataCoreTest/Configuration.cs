using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.OData.Edm;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ODataCoreTest
{
    public static class Configuration
    {
        public static Action<IApplicationBuilder> ConvertToAppBuilder(Action<object> myActionT)
        {
            if (myActionT == null) return null;
            else return new Action<object>(o => myActionT(o));
        }


        public static Action<IApplicationBuilder> GetBuilder()
        {
            return ConvertToAppBuilder(Configure);
        }

        public static void Configure(object appBuilder)
        {
            var app = appBuilder as IApplicationBuilder;
            var builder = new ODataConventionModelBuilder(app.ApplicationServices);
            app.UseRouting();
            app.UseAuthorization();

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(ODataExceptionHandler.HandleException());
            });

            var edmModel = GetEdmModel(builder);
            app.UseEndpoints(endpoints =>
            {
                var pathHandler = new DefaultODataPathHandler();
                var customRoutingConvention = new ODataCustomRoutingConvention();
                var conventions = ODataRoutingConventions.CreateDefault();
                //Workaround for https://github.com/OData/WebApi/issues/1622
                conventions.Insert(0, new AttributeRoutingConvention("OData", app.ApplicationServices, pathHandler));
                //Custom Convention
                conventions.Insert(0, customRoutingConvention);

                endpoints.EnableDependencyInjection();
                endpoints.Select().Filter().Expand();
                endpoints.MapODataRoute("OData", "OData", edmModel, pathHandler, conventions, new EagleODataBatchHandler());
            });
        }

        static IEdmModel GetEdmModel(ODataConventionModelBuilder builder)
        {
            builder.EntitySet<Student>("Student");
            builder.EntityType<Student>().HasKey(a => a.Id);

            return builder.GetEdmModel();
        }
    }

    public class ODataExceptionHandler
    {
        public static RequestDelegate HandleException()
        {
            return async context =>
            {
                await Handle(context);
            };
        }

        public static Task Handle(HttpContext context)
        {
            return Task.Run(new Action(() =>
            {
                context.Response.ContentType = "application/problem+json";
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var content = JsonConvert.SerializeObject(exceptionHandlerPathFeature);
                byte[] byteArray = Encoding.ASCII.GetBytes(content);
                context.Response.Body.Write(byteArray);
            }));
        }
    }
}
