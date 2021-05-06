using Microsoft.AspNetCore.Authentication;
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

            services.AddAuthentication("EagleODataAuthentication").AddScheme<AuthenticationSchemeOptions, EagleODataAuthenticationHandler>("EagleODataAuthentication", null);
            services.AddScoped<EagleODataAuthenticationService>();

            var mvcBuilder = services.AddMvc(options => { options.EnableEndpointRouting = false; }).AddNewtonsoftJson(op => op.SerializerSettings.ContractResolver = new DefaultContractResolver());
            var edmModel = GetEdmModel();

            mvcBuilder.AddOData(opt =>
            {
                opt.AddModel("{contextToken}", edmModel, configureAction =>
                {
                    configureAction.AddService(Microsoft.OData.ServiceLifetime.Singleton, typeof(ODataBatchHandler), s => new MyODataBatchHandler());
                    configureAction.AddService(Microsoft.OData.ServiceLifetime.Singleton, typeof(ODataSerializerProvider), sp => new MyODataSerializerProvider(sp));
                });
                opt.Filter().Select().Expand().SetMaxTop(null).Count().OrderBy();
            });
        }

        public static IEdmModel GetEdmModel()
        {
            var odataBuilder = new ODataConventionModelBuilder() { Namespace = "Model.Entities", ContainerName = "DefaultContainer" };
            odataBuilder.EntitySet<Student>("Student");
            odataBuilder.EntitySet<Student1>("Student1");
            odataBuilder.EntitySet<Student2>("Student2");
            odataBuilder.EntitySet<Student3>("Student3");
            odataBuilder.EntitySet<Student4>("Student4");
            odataBuilder.EntitySet<Student5>("Student5");
            odataBuilder.EntitySet<Student6>("Student6");
            odataBuilder.EntitySet<Student7>("Student7");
            odataBuilder.EntitySet<Student8>("Student8");
            odataBuilder.EntitySet<Student9>("Student9");
            odataBuilder.EntitySet<Student10>("Student10");
            odataBuilder.EntitySet<Student11>("Student11");
            odataBuilder.EntitySet<Student12>("Student12");
            odataBuilder.EntitySet<Student13>("Student13");
            odataBuilder.EntitySet<Student14>("Student14");
            odataBuilder.EntitySet<Student15>("Student15");
            odataBuilder.EntitySet<Student16>("Student16");
            odataBuilder.EntitySet<Student17>("Student17");
            odataBuilder.EntitySet<Student18>("Student18");
            odataBuilder.EntitySet<Student19>("Student19");
            odataBuilder.EntitySet<Student20>("Student20");

            var student = odataBuilder.EntityType<Student>().HasKey(s => s.Id);
            student.Property(s => s.Name);
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

            return odataBuilder.GetEdmModel();
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
            builder.UseEndpoints(routeBuilder =>
            {
                routeBuilder.MapControllers();
            });
        }
    }
}
