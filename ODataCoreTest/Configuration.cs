using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNet.OData.Routing.Template;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            app.UseDeveloperExceptionPage();
            var builder = new ODataConventionModelBuilder(app.ApplicationServices);

            app.UseRouting();
            app.UseAuthorization();
            var edmModel = GetEdmModel(builder);
            app.UseMvc(routeBuilder =>
            {
                var pathHandler = new DefaultODataPathHandler();
                var conventions = ODataRoutingConventions.CreateDefault();
                //Workaround for https://github.com/OData/WebApi/issues/1622
                conventions.Insert(0, new AttributeRoutingConvention("OData", app.ApplicationServices, pathHandler));
                //Custom Convention
                routeBuilder.EnableDependencyInjection();
                routeBuilder.Select().OrderBy().Filter().Expand().Count().OrderBy();

                routeBuilder.MapODataServiceRoute("OData", "OData", configureAction =>
                {
                    configureAction.AddService<ODataQueryOptionParser>(ServiceLifetime.Singleton, s => new CustomODataUriResolver(edmModel));
                    configureAction.AddService(ServiceLifetime.Singleton, sp => edmModel);
                    // var customRoutingConvention = new ODataCustomRoutingConvention();
                    var conventions = ODataRoutingConventions.CreateDefault();
                    //Workaround for https://github.com/OData/WebApi/issues/1622
                    conventions.Insert(0, new AttributeRoutingConvention("OData", app.ApplicationServices, new MyClass2()));
                    //Custom Convention
                    // conventions.Insert(0, customRoutingConvention);
                    conventions.Insert(0, new CustomPropertyRoutingConvention2());
                    configureAction.AddService<IEnumerable<IODataRoutingConvention>>(ServiceLifetime.Singleton, sp => conventions);

                    configureAction.AddService<ODataActionPayloadDeserializer>(ServiceLifetime.Singleton);
                    configureAction.AddService<ODataResourceDeserializer>(ServiceLifetime.Singleton);

                    configureAction.AddService(ServiceLifetime.Singleton, typeof(ODataSerializerProvider), sp => new CustomODataSerializerProvider(sp));
                });
            });
        }

        static IEdmModel GetEdmModel(ODataConventionModelBuilder builder)
        {
            builder.EntityType<IEntity>().Abstract().HasKey(s => s.Id);
            builder.EntityType<EntityBase>().Ignore(s => s.Backpack2);
            var entity = builder.EntityType<Student>().DerivesFrom<EntityBase>();
            //entity.Ignore(d => d.ODataInterfaces);
            entity.HasDynamicProperties(d => d.ODataInterfaces);
            entity.Ignore(s => s.Backpacks);
            entity.Ignore(s => s.Backpack);
            entity.Ignore(s => s.Backpack2);
            builder.EntityType<Backpack>().DerivesFrom<IEntity>();
            // builder.EntityType<IBackpack>();
            // builder.ComplexType<IBackpack>();
            builder.EntityType<ODataBaseInterface>();
            builder.EntitySet<Student>("Student");
            var model = builder.GetEdmModel();
            return model;
        }
    }

    public class CustomPropertyRoutingConvention2 : NavigationSourceRoutingConvention
    {
        public override string SelectAction(RouteContext routeContext, SelectControllerResult controllerResult, IEnumerable<ControllerActionDescriptor> actionDescriptors)
        {
            return "Get";
        }
    }

    public class MyClass2 : DefaultODataPathHandler
    {
        public MyClass2()
        {

        }

        public override string Link(Microsoft.AspNet.OData.Routing.ODataPath path)
        {
            return base.Link(path);
        }

        public override Microsoft.AspNet.OData.Routing.ODataPath Parse(string serviceRoot, string odataPath, IServiceProvider requestContainer)
        {
            return base.Parse(serviceRoot, odataPath, requestContainer);
        }

        public override ODataPathTemplate ParseTemplate(string odataPathTemplate, IServiceProvider requestContainer)
        {
            return base.ParseTemplate(odataPathTemplate, requestContainer);
        }
    }

    public class CustomODataUriResolver : ODataQueryOptionParser
    {
        public CustomODataUriResolver(IEdmModel edmModel) : base(edmModel, null, null)
        {

        }
    }

    public class EagleODataRoutingConvention : ActionRoutingConvention
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
                return "PutByNaturalKey";
            }
            return base.SelectAction(routeContext, controllerResult, actionDescriptors);
        }
    }

    public class CustomODataSerializerProvider : DefaultODataSerializerProvider
    {
        private CustomODataResourceSerializer resuoruceSerializer;


        public CustomODataSerializerProvider(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            resuoruceSerializer = new CustomODataResourceSerializer(this);
        }

        public override ODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
        {
            if (edmType.IsEntity())
            {
                return resuoruceSerializer;
            }
            return base.GetEdmTypeSerializer(edmType);
        }
    }
    public class ExpandedNavigationSelectedItemTranslator : SelectItemTranslator<IEnumerable<string>>
    {
        public override IEnumerable<string> Translate(ExpandedNavigationSelectItem item)
        {
            var expandedNavigationProperies = new List<string>();
            expandedNavigationProperies.Add(item.PathToNavigationProperty.Cast<NavigationPropertySegment>().Single().NavigationProperty.Name);
            if (item.SelectAndExpand.SelectedItems.Any())
            {
                expandedNavigationProperies.AddRange(TranslateSelectExpandClause(item.SelectAndExpand).Select(p => item.PathToNavigationProperty.Cast<NavigationPropertySegment>().Single().NavigationProperty.Name + "." + p));
            }
            return expandedNavigationProperies;
        }

        public IEnumerable<string> TranslateSelectExpandClause(SelectExpandClause selectExpandClause)
        {
            return selectExpandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>().SelectMany(Translate);
        }
    }

    public class CustomODataResourceSerializer : ODataResourceSerializer
    {
        public CustomODataResourceSerializer(ODataSerializerProvider serializerProvider) : base(serializerProvider)
        {

        }

        /// <summary>
        /// Creating resource for entity with selected interface type property
        /// </summary>
        public override ODataResource CreateResource(SelectExpandNode selectExpandNode, ResourceContext resourceContext)
        {
            var resource = base.CreateResource(selectExpandNode, resourceContext);

            //if (selectExpandNode.SelectedDynamicProperties?.Any() == true)
            //{
            //    foreach (var dynamicProperty in selectExpandNode.SelectedDynamicProperties)
            //    {
            //        if (!resource.Properties.Any(s => s.Name == dynamicProperty))
            //        {
            //            throw new InvalidOperationException($"Cannot find property '{dynamicProperty}' on '{resource.TypeName}' entity");
            //        }
            //    }
            //}

            return resource;
        }

        public override ODataAction CreateODataAction(IEdmAction action, ResourceContext resourceContext)
        {
            return base.CreateODataAction(action, resourceContext);
        }

        public override SelectExpandNode CreateSelectExpandNode(ResourceContext resourceContext)
        {
            var expand = base.CreateSelectExpandNode(resourceContext);
            return expand;
        }

        public override void AppendDynamicProperties(ODataResource resource, SelectExpandNode selectExpandNode, ResourceContext resourceContext)
        {
            if (selectExpandNode.SelectedDynamicProperties?.Any() == true)
            {
                base.AppendDynamicProperties(resource, selectExpandNode, resourceContext);
            }
        }
    }

    public class MyReference : EdmEntityReferenceType, IEdmTypeReference
    {
        public MyReference(IEdmEntityType edmEntityType) : base(edmEntityType)
        {

        }

        public bool IsNullable => true;

        public IEdmType Definition => throw new NotImplementedException();
    }

    public class AnnotatingEntitySerializer : ODataPrimitiveSerializer
    {
        public override ODataPrimitiveValue CreateODataPrimitiveValue(object graph, IEdmPrimitiveTypeReference primitiveType, ODataSerializerContext writeContext)
        {
            return base.CreateODataPrimitiveValue(graph, primitiveType, writeContext);
        }
    }

    public class Property : IEdmProperty
    {
        public Property(IEdmProperty originalProperty, MyClass edmTypeReference)
        {
            PropertyKind = originalProperty.PropertyKind;
            DeclaringType = originalProperty.DeclaringType;
            Name = originalProperty.Name;
            Type = edmTypeReference;
        }

        public EdmPropertyKind PropertyKind { get; set; }
        public IEdmStructuredType DeclaringType { get; set; }
        public string Name { get; set; }
        public IEdmTypeReference Type { get; set; }
    }

    class Writer : ODataWriter
    {
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override Task FlushAsync()
        {
            throw new NotImplementedException();
        }

        public override void WriteEnd()
        {
            throw new NotImplementedException();
        }

        public override Task WriteEndAsync()
        {
            throw new NotImplementedException();
        }

        public override void WriteEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink)
        {
            throw new NotImplementedException();
        }

        public override Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink entityReferenceLink)
        {
            throw new NotImplementedException();
        }

        public override void WriteStart(ODataResourceSet resourceSet)
        {
            throw new NotImplementedException();
        }

        public override void WriteStart(ODataResource resource)
        {
            throw new NotImplementedException();
        }

        public override void WriteStart(ODataNestedResourceInfo nestedResourceInfo)
        {
            throw new NotImplementedException();
        }

        public override Task WriteStartAsync(ODataResourceSet resourceSet)
        {
            throw new NotImplementedException();
        }

        public override Task WriteStartAsync(ODataResource resource)
        {
            throw new NotImplementedException();
        }

        public override Task WriteStartAsync(ODataNestedResourceInfo nestedResourceInfo)
        {
            throw new NotImplementedException();
        }
    }

    public class MyClass : EdmTypeReference
    {
        public MyClass(IEdmType edmType, bool isNullable) : base(edmType, isNullable)
        {

        }
    }
    public class ODataExceptionHandler
    {
        public const string UnhundledExceptionId = "UnhandledException";

        public static RequestDelegate HandleException() => async context => await HandleAsync(context);

        public static async Task HandleAsync(HttpContext context)
        {
            await HandleErrorResponseAsync(context.Response, context.Features.Get<IExceptionHandlerPathFeature>()?.Error);
        }

        public static async Task HandleErrorResponseAsync(HttpResponse response, Exception ex)
        {
            await HandleErrorResponseAsync(response, ex.Message);
        }

        public static async Task HandleErrorResponseAsync(HttpResponse response, string errors)
        {
            response.ContentType = "application/problem+json";
            byte[] byteArray = Encoding.ASCII.GetBytes(errors);
            await response.Body.WriteAsync(byteArray);
        }
    }

    public class ExtendedEnableQueryAttribute : EnableQueryAttribute
    {
        public override void ValidateQuery(HttpRequest request, ODataQueryOptions queryOptions)
        {
            base.ValidateQuery(request, queryOptions);
        }

        public override IQueryable ApplyQuery(IQueryable queryable, ODataQueryOptions queryOptions)
        {
            //return queryable;
            return base.ApplyQuery(queryable, queryOptions);
        }
        public override object ApplyQuery(object entity, ODataQueryOptions queryOptions)
        {
            return base.ApplyQuery(entity, queryOptions);
        }


        public override bool Match(object obj)
        {
            return base.Match(obj);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }
    }

    public class ODataDictionary : IDictionary<string, object>
    {
        public Dictionary<string, object> SourceDictionary { get; set; }

        public ODataDictionary(Dictionary<string, object> keyValuePairs)
        {
            SourceDictionary = keyValuePairs;
        }

        public object this[string key]
        {
            get
            {
                var value = SourceDictionary[key];
                return value;
            }
            set
            {
                SourceDictionary[key] = value;
            }
        }

        public ICollection<string> Keys => SourceDictionary.Keys;

        public ICollection<object> Values => SourceDictionary.Values;

        public int Count => SourceDictionary.Count;

        public bool IsReadOnly => false;

        public void Add(string key, object value)
        {
            SourceDictionary.Add(key, value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            SourceDictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            SourceDictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return SourceDictionary.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return SourceDictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {

        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            var enumerator = SourceDictionary.GetEnumerator();
            return enumerator;
        }

        public bool Remove(string key)
        {
            return SourceDictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return SourceDictionary.Remove(item.Key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
        {
            return SourceDictionary.TryGetValue(key, out value);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return SourceDictionary.GetEnumerator();
        }
    }


}
