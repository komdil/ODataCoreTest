using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Text;

namespace ODataCoreTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var controllers = ControllerGenerator();
            //File.WriteAllText("controllers.txt", controllers);

            //var edmModel = EdmModelGenerator();
            //File.WriteAllText("edmModel.txt", edmModel);


            //var model = ModelGenerator();
            //File.WriteAllText("model.txt", model);
            var url = "https://localhost:44383";
            StartHost(url);
        }
        internal static IHost StartHost(string url)
        {
            var host = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                webBuilder.UseIISIntegration();
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls(url);
            }).Build();
            host.Start();
            return host;
        }

        public static string ControllerGenerator()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("namespace ODataCoreTest");
            builder.AppendLine("{");
            for (int i = 0; i < 200; i++)
            {
                builder.AppendLine($"public class Student{i}Controller : MyBaseController<Student{i}>");
                builder.AppendLine("{");
                builder.AppendLine("}");
                builder.AppendLine();
            }
            builder.AppendLine("}");
            return builder.ToString();
        }

        public static string EdmModelGenerator()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("namespace ODataCoreTest");
            builder.AppendLine("{");

            for (int i = 0; i < 200; i++)
            {
                builder.AppendLine($"odataBuilder.EntitySet<Student{i}>(\"Student{i}\");");
                builder.AppendLine($"odataBuilder.EntityType<Student{i}>().HasKey(s => s.Id);");
            }
            builder.AppendLine("}");
            return builder.ToString();
        }

        public static string ModelGenerator()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("namespace ODataCoreTest");
            builder.AppendLine("{");
            for (int i = 0; i < 200; i++)
            {
                builder.AppendLine($"public class Student{i} : EntityBase");
                builder.AppendLine("{");
                builder.AppendLine("public Guid Id { get; set; }");
                builder.AppendLine("public string Name { get; set; }");
                builder.AppendLine("public int Score { get; set; }");
                builder.AppendLine("}");
                builder.AppendLine();
            }
            builder.AppendLine("}");
            return builder.ToString();
        }
    }
}
