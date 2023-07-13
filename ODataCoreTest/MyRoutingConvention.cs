using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using Microsoft.AspNet.OData.Extensions;
using System.Linq;
using Microsoft.OData.UriParser;

namespace ODataCoreTest
{
    public class MyRoutingConvention : EntitySetRoutingConvention
    {
        public override string SelectAction(RouteContext routeContext, SelectControllerResult controllerResult, IEnumerable<ControllerActionDescriptor> actionDescriptors)
        {
            var context = routeContext.HttpContext;
            var odataFeature = context.ODataFeature();
            var odataPath = odataFeature.Path;

            var keySegment = odataPath.Segments.OfType<KeySegment>().FirstOrDefault();
            if (odataPath.PathTemplate == "~/entityset/key/unresolved" && keySegment != null)
            {
                routeContext.RouteData.Values["Id"] = keySegment.Keys.First().Value;

                return nameof(StudentController.RunWithPrefix);
            }
            return base.SelectAction(routeContext, controllerResult, actionDescriptors);
        }
    }
}
