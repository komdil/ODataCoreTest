using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ODataCoreTest
{
    public class ODataCustomRoutingConvention : EntitySetRoutingConvention
    {
        public override string SelectAction(RouteContext routeContext, SelectControllerResult controllerResult, IEnumerable<ControllerActionDescriptor> actionDescriptors)
        {
            var context = routeContext.HttpContext;
            var odataPath = context.ODataFeature().Path;

            if (context.Request.Method == "GET")
            {
                var segments = odataPath.Segments;
                if (segments == null || !segments.Any())
                {
                    throw new Exception("Segment is not set");
                }
                string actionName = "GetEntityPropertyWithNaturalKey";
                if (odataPath.Segments.Count > 1 && odataPath.Segments[1] is KeySegment keySegment && keySegment.Keys.First().Key == "Guid")
                {
                    actionName = "GetEntityPropertyWithGuid";
                }

                // Find correct controller

                if (actionDescriptors.Any(a => a.ActionName.Contains(actionName)))
                {
                    if (odataPath.PathTemplate == "~/entityset/key/navigation" || odataPath.PathTemplate == "~/entityset/key/navigation/$value")
                    {
                        var navigationProperty = ((NavigationPropertySegment)segments[2]).NavigationProperty;
                        SetRoutingData(navigationProperty.Name);
                        return actionName;
                    }
                    else if (odataPath.PathTemplate == "~/entityset/key/property" || odataPath.PathTemplate == "~/entityset/key/property/$value")
                    {
                        var property = ((PropertySegment)segments[2]).Property;
                        SetRoutingData(property.Name);
                        return actionName;
                    }
                    void SetRoutingData(string propertyName)
                    {
                        // Add keys to route data, so they will bind to action parameters.
                        var keyValueSegment = segments[1] as KeySegment;
                        routeContext.RouteData.Values[ODataRouteConstants.Key] = keyValueSegment.Keys.First().Value;
                        routeContext.RouteData.Values["PropertyName"] = propertyName;
                    }
                }
            }
            return base.SelectAction(routeContext, controllerResult, actionDescriptors);
        }
    }
}
