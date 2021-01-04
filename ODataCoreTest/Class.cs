using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ODataCoreTest
{
    public class MyODataRoutingConvention : EntitySetRoutingConvention
    {
        public override string SelectAction(RouteContext routeContext, SelectControllerResult controllerResult, IEnumerable<ControllerActionDescriptor> actionDescriptors)
        {
            var context = routeContext.HttpContext;
            var odataPath = context.ODataFeature().Path;
            string actionName = "";
            var method = context.Request.Method;

            var segments = odataPath.Segments;
            if (odataPath.PathTemplate == "~/entityset/key/navigation" || odataPath.PathTemplate == "~/entityset/key/navigation/$value")
            {
                actionName = ExpandNavigatioinPropertyByKey(segments, routeContext);
            }
            else if (odataPath.PathTemplate == "~/entityset/key/property" || odataPath.PathTemplate == "~/entityset/key/property/$value")
            {
                actionName = SelectValuePropertyByKey(segments, routeContext);
            }
            else if ((odataPath.PathTemplate == "~/entityset/key" || odataPath.PathTemplate == "~/entityset/key/$value"))
            {
                actionName = SelectEntityByKey(segments, routeContext, out bool isGuid);

                //Handling OData CRUD operators
                if ((method == "PATCH" || method == "MERGE") && !isGuid)
                {
                    return "PatchByNaturalKey";
                }
                else if (method == "DELETE" && !isGuid)
                {
                    return "DeleteByNaturalKey";
                }
                else if (method == "PUT" && !isGuid)
                {
                    return "PutByNaturalKey";
                }
            }
            else if (odataPath.PathTemplate == "~/entityset/key/action" && method == "POST")
            {
                SelectEntityByKey(segments, routeContext);
            }

            if (method == "GET" && actionName != "")
            {
                return actionName;
            }
            return base.SelectAction(routeContext, controllerResult, actionDescriptors);
        }

        void SelectEntityByKey(IEnumerable<ODataPathSegment> segments, RouteContext routeContext)
        {
            if (TryGetKeySegment(segments, out KeyValuePair<string, object> keyValue))
            {
                if (keyValue.Key != "Guid")
                {
                    routeContext.RouteData.Values["naturalKeyName"] = keyValue.Key;
                    routeContext.RouteData.Values["naturalKeyValue"] = keyValue.Value;
                }
            }
        }

        string SelectEntityByKey(IEnumerable<ODataPathSegment> segments, RouteContext routeContext, out bool isGuidKey)
        {
            isGuidKey = false;
            if (TryGetKeySegment(segments, out KeyValuePair<string, object> keyValue))
            {
                string action;
                if (keyValue.Key == "Guid")
                {
                    isGuidKey = true;
                    action = "GetEntityFromGuid";
                    routeContext.RouteData.Values[ODataRouteConstants.Key] = keyValue.Value;
                }
                else
                {
                    action = "GetEntityFromNaturalKeyValue";
                    routeContext.RouteData.Values[ODataRouteConstants.Key] = keyValue.Key;
                    routeContext.RouteData.Values["naturalKeyValue"] = keyValue.Value;
                }
                return action;
            }
            return "";
        }

        string SelectValuePropertyByKey(IEnumerable<ODataPathSegment> segments, RouteContext routeContext)
        {
            string action = "";
            if (TryGetKeySegment(segments, out KeyValuePair<string, object> keyValuePair))
            {

                if (TryGetPropertySegment(segments, out string propertyName))
                {
                    if (keyValuePair.Key == "Guid")
                    {
                        action = "GetEntityPropertyWithGuid";
                    }
                    else
                    {
                        action = "GetEntityPropertyWithNaturalKey";
                    }
                    routeContext.RouteData.Values[ODataRouteConstants.Key] = keyValuePair.Value;
                    routeContext.RouteData.Values["PropertyName"] = propertyName;
                }
            }
            return action;
        }

        string ExpandNavigatioinPropertyByKey(IEnumerable<ODataPathSegment> segments, RouteContext routeContext)
        {
            string action = "";
            if (TryGetKeySegment(segments, out KeyValuePair<string, object> keyValuePair))
            {
                if (TryGetNavigationPropertySegment(segments, out string propertyName))
                {
                    if (keyValuePair.Key == "Guid")
                    {
                        action = "GetEntityPropertyWithGuid";
                    }
                    else
                    {
                        action = "GetEntityPropertyWithNaturalKey";
                    }
                    routeContext.RouteData.Values[ODataRouteConstants.Key] = keyValuePair.Value;
                    routeContext.RouteData.Values["PropertyName"] = propertyName;
                }
            }
            return action;
        }

        bool TryGetKeySegment(IEnumerable<ODataPathSegment> segments, out KeyValuePair<string, object> keyValuePair)
        {
            keyValuePair = new KeyValuePair<string, object>("", null);
            foreach (var segment in segments)
            {
                if (segment is KeySegment keySegment)
                {
                    keyValuePair = keySegment.Keys.First();
                    return true;
                }
            }
            return false;
        }

        bool TryGetPropertySegment(IEnumerable<ODataPathSegment> segments, out string propertyName)
        {
            propertyName = "";
            foreach (var segment in segments)
            {
                if (segment is PropertySegment keySegment)
                {
                    propertyName = keySegment.Property.Name;
                    return true;
                }
            }
            return false;
        }

        bool TryGetNavigationPropertySegment(IEnumerable<ODataPathSegment> segments, out string propertyName)
        {
            propertyName = "";
            foreach (var segment in segments)
            {
                if (segment is NavigationPropertySegment keySegment)
                {
                    propertyName = keySegment.NavigationProperty.Name;
                    return true;
                }
            }
            return false;
        }
    }
}
