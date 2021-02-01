using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
                    Func<IServiceProvider, IEnumerable<IODataRoutingConvention>> magicFunc = ((IServiceProvider sp) => ODataRoutingConventions.CreateDefaultWithAttributeRouting("OData", routeBuilder.ServiceProvider));

                    // configureAction.AddService<ODataQueryOptionParser>(ServiceLifetime.Singleton, s => new CustomODataUriResolver(edmModel));
                    configureAction.AddService(ServiceLifetime.Singleton, sp => edmModel);
                    configureAction.AddService(ServiceLifetime.Singleton, magicFunc);
                    // var customRoutingConvention = new ODataCustomRoutingConvention();
                    var conventions = ODataRoutingConventions.CreateDefault();
                    //Workaround for https://github.com/OData/WebApi/issues/1622
                    // conventions.Insert(0, new AttributeRoutingConvention("OData", app.ApplicationServices, new MyClass2()));
                    //Custom Convention
                    // conventions.Insert(0, customRoutingConvention);
                    //  conventions.Insert(0, new CustomPropertyRoutingConvention2());
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
            //builder.EntityType<ODataCommandResponse>();
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
            builder.EntitySet<Backpack>("Backpack");
            var model = builder.GetEdmModel();
            return model;
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

        private EagleODataDecimalSerializer presuoruceSerializer;
        public CustomODataSerializerProvider(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            resuoruceSerializer = new CustomODataResourceSerializer(this);
            presuoruceSerializer = new EagleODataDecimalSerializer();
        }

        public override ODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
        {
            if (edmType.IsPrimitive())
            {
                return presuoruceSerializer;
            }

            return base.GetEdmTypeSerializer(edmType);
        }
    }

    public class CustomODataResourceSerializer : ODataResourceSerializer
    {
        public CustomODataResourceSerializer(ODataSerializerProvider serializerProvider) : base(serializerProvider)
        {

        }

        public override ODataResource CreateResource(SelectExpandNode selectExpandNode, ResourceContext resourceContext)
        {
            var resource = base.CreateResource(selectExpandNode, resourceContext);

            if (selectExpandNode.SelectedDynamicProperties?.Any() == true)
            {
                foreach (var dynamicProperty in selectExpandNode.SelectedDynamicProperties)
                {
                    if (!resource.Properties.Any(s => s.Name == dynamicProperty) && resourceContext.DynamicComplexProperties?.Any(s => s.Key == dynamicProperty) != true)
                    {
                        throw new InvalidOperationException($"Cannot find property '{dynamicProperty}' on '{resource.TypeName}' entity");
                    }
                }
            }

            return resource;
        }

        public override void AppendDynamicProperties(ODataResource resource, SelectExpandNode selectExpandNode, ResourceContext resourceContext)
        {
            if (selectExpandNode.SelectedDynamicProperties?.Any() == true)
            {
                base.AppendDynamicProperties(resource, selectExpandNode, resourceContext);
            }
        }
    }

    public class ODataInterfacesDictionary : IDictionary<string, object>
    {
        public Dictionary<string, object> SourceDictionary { get; }

        public int Count => SourceDictionary.Count;

        ICollection<string> IDictionary<string, object>.Keys => SourceDictionary.Keys;

        ICollection<object> IDictionary<string, object>.Values => SourceDictionary.Values;

        public bool IsReadOnly => false;

        object IDictionary<string, object>.this[string key] { get => SourceDictionary[key]; set => SourceDictionary[key] = value; }

        public object this[string key] => SourceDictionary[key];

        public ODataInterfacesDictionary(Dictionary<string, object> source)
        {
            SourceDictionary = source;
        }

        public bool ContainsKey(string key)
        {
            return SourceDictionary.ContainsKey(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
        {
            return SourceDictionary.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var item in SourceDictionary)
            {
                if (item.Value == null)
                {
                    yield return new KeyValuePair<string, object>(item.Key, string.Empty);
                }
                else
                {
                    yield return item;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string key, object value)
        {
            SourceDictionary.Add(key, value);
        }

        public bool Remove(string key)
        {
            return SourceDictionary.Remove(key);
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

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return SourceDictionary.Remove(item.Key);
        }
    }

    public class EagleODataDecimalSerializer : ODataPrimitiveSerializer
    {
        /// <summary>
        /// Rewriting decimal value
        /// </summary>
        public override ODataPrimitiveValue CreateODataPrimitiveValue(object graph, IEdmPrimitiveTypeReference primitiveType, ODataSerializerContext writeContext)
        {
            if (graph is decimal decimalValue && primitiveType is EdmDecimalTypeReference decimalTypeReference && decimalTypeReference.Scale != null)
            {
                graph = CalculateDecimalPrecision(decimalValue, decimalTypeReference.Scale.Value);
            }
            else if (graph is string)
            {

            }
            return base.CreateODataPrimitiveValue(graph, primitiveType, writeContext);
        }

        /// <summary>
        /// Calculates precision of decimal value based on VisualDecimals
        /// </summary>
        decimal CalculateDecimalPrecision(decimal value, int precision)
        {
            if (ShouldCalculatePrecision(value, precision, out bool shouldRound))
            {
                if (shouldRound)
                {
                    value = Math.Round(value, precision);
                }
                else
                {
                    var calculatedPrecision = decimal.Divide(1, Convert.ToDecimal(Math.Pow(10, precision)));
                    var zeroAfterPoint = calculatedPrecision - calculatedPrecision;
                    value += zeroAfterPoint;
                }
            }
            return value;
        }

        /// <summary>
        /// Returns true if decimal value should be calculated based on VisualDecimals
        /// </summary>
        /// <param name="shouldRound">It will be true when value precision higher then VisualDecimals</param>
        bool ShouldCalculatePrecision(decimal value, int precision, out bool shouldRound)
        {
            shouldRound = false;
            var splitedDecimalWithPoints = value.ToString().Split(".");
            if (splitedDecimalWithPoints.Count() < 1 && precision > 1)
            {
                return true;
            }
            else if (splitedDecimalWithPoints.Last().Length < precision)
            {
                return true;
            }
            else if (splitedDecimalWithPoints.Last().Length > precision)
            {
                shouldRound = true;
                return true;
            }
            return false;
        }
    }

    public class MyClass : ODataEdmTypeSerializer
    {
        public MyClass(ODataPayloadKind payloadKind) : base(payloadKind)
        {
        }

        public override ODataValue CreateODataValue(object graph, IEdmTypeReference expectedType, ODataSerializerContext writeContext)
        {
            return base.CreateODataValue(graph, expectedType, writeContext);
        }
    }
}
