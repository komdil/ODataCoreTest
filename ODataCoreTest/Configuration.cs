using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;

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
            var config = new RouteBuilder(app);
            var builder = new ODataConventionModelBuilder(config.ApplicationBuilder.ApplicationServices) { Namespace = "Model.Entities", ContainerName = "DefaultContainer" };
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
                    var customRoutingConvention = new ODataCustomRoutingConvention();
                    var conventions = ODataRoutingConventions.CreateDefault();
                    //Workaround for https://github.com/OData/WebApi/issues/1622
                    conventions.Insert(0, new AttributeRoutingConvention("OData", config.ServiceProvider, new DefaultODataPathHandler()));
                    //Custom Convention
                    conventions.Insert(0, customRoutingConvention);
                    b.AddService<IEnumerable<IODataRoutingConvention>>(Microsoft.OData.ServiceLifetime.Singleton, a => conventions);
                });
            });
        }

        static IEdmModel GetEdmModel()
        {
            var odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.EntitySet<Student>("Student");

            return odataBuilder.GetEdmModel();
        }
    }
}
