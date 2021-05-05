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
            services.AddControllers(op => op.AllowEmptyInputInBodyModelBinding = true);

            var mvcBuilder = services.AddMvc(options => { options.EnableEndpointRouting = false; }).AddNewtonsoftJson(op => op.SerializerSettings.ContractResolver = new DefaultContractResolver());
            var edmModel = GetEdmModel();

            mvcBuilder.AddOData(opt =>
            {
                opt.AddModel("", edmModel, configureAction =>
                {
<<<<<<< Updated upstream
                    configureAction.AddService(Microsoft.OData.ServiceLifetime.Singleton, typeof(ODataBatchHandler), s => new EagleODataBatchHandler());
                    configureAction.AddService(Microsoft.OData.ServiceLifetime.Singleton, typeof(ODataSerializerProvider), sp => new EagleODataSerializerProvider(sp));
=======
                    configureAction.AddService(Microsoft.OData.ServiceLifetime.Singleton, typeof(IEdmModel), s => edmModel);
                    configureAction.AddService(Microsoft.OData.ServiceLifetime.Singleton, typeof(ODataUriResolver), s => new AlternateKeyPrefixFreeEnumODataUriResolver(edmModel));
                    configureAction.AddService(Microsoft.OData.ServiceLifetime.Singleton, typeof(ODataBatchHandler), s => new MyODataBatchHandler());
                    configureAction.AddService(Microsoft.OData.ServiceLifetime.Singleton, typeof(ODataSerializerProvider), sp => new MyODataSerializerProvider(sp));
>>>>>>> Stashed changes
                });
                opt.Filter().Select().Expand().SetMaxTop(null).Count().OrderBy();
            });
        }

        static IEdmModel GetEdmModel()
        {
            var odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.EntitySet<Student>("Student");
            var entity = odataBuilder.EntityType<Student>();
            entity.DerivesFrom<EntityBase>();
            entity.Ignore(s => s.Test);
            entity.Ignore(s => s.Test2);
            entity.HasKey(s => s.Id);

<<<<<<< Updated upstream
            var baseEntity = odataBuilder.EntityType<EntityBase>();
            baseEntity.Abstract();
            baseEntity.Ignore(s => s.Test);
            baseEntity.Ignore(s => s.Test2);
=======
            var cool = odataBuilder.EntityType<CoolEntityBase>();
            cool.Abstract();
            cool.Ignore(a => a.Test);
            cool.Ignore(s => s.Test2);
            var student = odataBuilder.EntityType<Student>().HasKey(s => s.Id);
            student.DerivesFrom<CoolEntityBase>();
            student.Ignore(a => a.Test);
            cool.Ignore(s => s.Test2);

            odataBuilder.EntityType<Student1>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student2>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student3>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student4>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student5>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student6>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student7>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student8>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student9>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student10>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student11>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student12>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student13>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student14>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student15>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student16>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student17>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student18>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student19>().HasKey(s => s.Id);
            odataBuilder.EntityType<Student20>().HasKey(s => s.Id);
>>>>>>> Stashed changes


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
<<<<<<< Updated upstream
                routeBuilder.MapControllerRoute("OData", "[controller]/[action]");
=======
                routeBuilder.MapRoute("OData", "[controller]/[action]");
>>>>>>> Stashed changes
            });
        }
    }
}
