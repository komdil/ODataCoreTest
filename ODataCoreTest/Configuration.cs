using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.OData.Edm;
using System;

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
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseODataBatching();
            var edmModel = GetEdmModel(builder);
            app.UseDeveloperExceptionPage();
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
            builder.Action("Get").ReturnsFromEntitySet<Student>("Student").Parameter<string>("Id");
            return builder.GetEdmModel();
        }
    }
}
