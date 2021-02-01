using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;

namespace ODataCoreTest
{
    [EnableQuery]
    public class BaseController<TEntity> : ODataController where TEntity : class
    {
        List<Type> modelClasses;
        public List<Type> ModelClasses
        {
            get
            {
                if (modelClasses == null)
                {
                    modelClasses = GetClasses("ODataCoreTest");
                }
                return modelClasses;
            }
        }

        static List<Type> GetClasses(string nameSpace)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            List<Type> namespacelist = new List<Type>();
            List<Type> classlist = new List<Type>();

            foreach (Type type in asm.GetTypes())
            {
                if (type.Namespace == nameSpace)
                    namespacelist.Add(type);
            }

            foreach (Type classType in namespacelist)
                classlist.Add(classType);

            return classlist;
        }

        [HttpGet]
        public IActionResult Get(ODataQueryOptions<TEntity> queryOptions, CancellationToken cancellationToken)
        {
            if (queryOptions.Filter?.FilterClause != null)
            {
                ValidateOpenType(queryOptions.Filter.FilterClause.Expression);
            }
            var res = new List<Student>() { CreateNewStudent() };

            return Ok(res);
        }

        public static string SerializeODataClass(object oDataClass)
        {
            return JsonConvert.SerializeObject(oDataClass, Formatting.Indented);
        }

        void ValidateOpenType(QueryNode expression)
        {
            if (expression is BinaryOperatorNode operatorNode)
            {
                ValidateOpenType(operatorNode.Left);
                ValidateOpenType(operatorNode.Right);
            }
            else if (expression is ConvertNode convertNode)
            {
                ValidateOpenType(convertNode.Source);
            }
            else if (expression is SingleValueFunctionCallNode functionCallNode)
            {
                foreach (var queryNode in functionCallNode.Parameters)
                {
                    ValidateOpenType(queryNode);
                }
            }
            else if (expression is SingleValuePropertyAccessNode singleValueProperty && !string.IsNullOrEmpty(singleValueProperty.Property?.Name))
            {
                if (singleValueProperty.Source is SingleNavigationNode navigationNode)
                {
                    ThrowExceptionWhenPropertyDoesNotExists(singleValueProperty.Property.Name, navigationNode.NavigationSource.Name);
                }
                else
                {
                    ThrowExceptionWhenPropertyDoesNotExists(singleValueProperty.Property.Name, typeof(TEntity).Name);
                }
            }
            else if (expression is SingleValueOpenPropertyAccessNode valueOpenPropertyAccessNode)
            {
                if (valueOpenPropertyAccessNode.Source is SingleNavigationNode navigationNode)
                {
                    ThrowExceptionWhenPropertyDoesNotExists(valueOpenPropertyAccessNode.Name, navigationNode.NavigationSource.Name);
                }
                else
                {
                    ThrowExceptionWhenPropertyDoesNotExists(valueOpenPropertyAccessNode.Name, typeof(TEntity).Name);
                }
            }
        }

        void ThrowExceptionWhenPropertyDoesNotExists(string propertyName, string entityName)
        {
            if (ModelClasses.Any(t => t.Name == entityName && t.GetProperty(propertyName) == null))
            {
                throw new InvalidOperationException($"Could not find a property named '{propertyName}' on '{entityName}' entity");
            }
        }

        static Student CreateNewStudent()
        {
            return new Student
            {
                Id = Guid.NewGuid(),
                Backpacks = new List<IBackpack>
                {
                    new Backpack{Name="Backpack",Address=new Address(){ PlaceNumber=342} },
                    new Backpack{Name="Backpack",Address=new Address(){ PlaceNumber=123} },
                    new Backpack{Name="Backpack",Address=new Address(){ PlaceNumber=987} },
                }
            };
        }
    }
}
