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

            var mvcBuilder = services.AddMvcCore();
            var edmModel = GetEdmModel();

            mvcBuilder.AddOData(opt =>
            {
                opt.AddModel("", edmModel, configureAction =>
                {
                    configureAction.AddService(Microsoft.OData.ServiceLifetime.Singleton, typeof(ODataUriResolver), s => new AlternateKeyPrefixFreeEnumODataUriResolver(edmModel));
                    configureAction.AddService(Microsoft.OData.ServiceLifetime.Singleton, typeof(ODataBatchHandler), s => new EagleODataBatchHandler());
                    configureAction.AddService(Microsoft.OData.ServiceLifetime.Singleton, typeof(ODataSerializerProvider), sp => new EagleODataSerializerProvider(sp));
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

            var baseEntity = odataBuilder.EntityType<EntityBase>();
            baseEntity.Abstract();
            baseEntity.Ignore(s => s.Test);
            baseEntity.Ignore(s => s.Test2);

            return odataBuilder.GetEdmModel();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseODataBatching();
            app.UseEndpoints(s => s.MapControllerRoute("OData", "[controller]/[action]"));
        }
    }
}
