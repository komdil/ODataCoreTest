using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OData.UriParser;
using Newtonsoft.Json.Serialization;
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
            var edmModel = GetEdmModel();
            services.AddControllers(op => op.AllowEmptyInputInBodyModelBinding = true);

            var mvcBuilder = services.AddMvc(options => { options.EnableEndpointRouting = false; }).AddNewtonsoftJson(op => op.SerializerSettings.ContractResolver = new DefaultContractResolver());
            mvcBuilder.AddOData(opt =>
            {
                opt.EnableContinueOnErrorHeader = true;
                opt.AddModel("", edmModel, configureAction =>
                {
                    configureAction.AddService(Microsoft.OData.ServiceLifetime.Singleton, typeof(ODataBatchHandler), s => new MyODataBatchHandler());
                    configureAction.AddService(Microsoft.OData.ServiceLifetime.Singleton, typeof(ODataSerializerProvider), sp => new MyODataSerializerProvider(sp));
                    configureAction.AddService(Microsoft.OData.ServiceLifetime.Singleton, typeof(IEdmModel), s => edmModel);
                    configureAction.AddService(Microsoft.OData.ServiceLifetime.Singleton, typeof(ODataUriResolver), s => new AlternateKeyPrefixFreeEnumODataUriResolver(edmModel));
                });
                opt.Filter().Select().Expand().SetMaxTop(null).Count().OrderBy();
            });
        }

        static IEdmModel GetEdmModel()
        {
            var odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.EntitySet<Student>("Student");
            var entity = odataBuilder.EntityType<Student>();
            entity.DerivesFrom<CoolEntityBase>();

            entity.HasKey(s => s.Id);


            var baseEntity = odataBuilder.EntityType<EntityBase>();
            baseEntity.Abstract();

            var cool = odataBuilder.EntityType<CoolEntityBase>();
            cool.Abstract();

            var student = odataBuilder.EntityType<Student>().HasKey(s => s.Id);
            student.DerivesFrom<CoolEntityBase>();


            var model = odataBuilder.GetEdmModel();
            AddAlternateKey(model as EdmModel, "Student", "Name");
            return model;
        }

        /// <summary>
        /// Adding alternate key, so that we are able to get entities by NaturalKey: ~OData/[EntityType](NaturalKey='CokeOrAnythingElse')
        /// </summary>
        static void AddAlternateKey(EdmModel edmModel, string entityName, string propertyName)
        {
            var edmEntityType = edmModel.FindDeclaredEntitySet(entityName).EntityType();
            var naturalKeyProperty = edmEntityType.FindProperty(propertyName);
            edmModel.AddAlternateKeyAnnotation(edmEntityType, new Dictionary<string, IEdmProperty> {
            {
                propertyName, naturalKeyProperty
            }});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder builder, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                builder.UseDeveloperExceptionPage();
            }
            builder.UseODataBatching();
            builder.UseHttpsRedirection();
            builder.UseRouting();
            builder.UseAuthentication();
            builder.UseAuthorization();
            builder.UseMvc(routeBuilder =>
            {
                routeBuilder.MapRoute("OData", "[controller]/[action]");
            });
        }
    }
}
