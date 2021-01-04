using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace ODataCoreTest
{
    [ExtendedEnableQuery]
    public class BaseController<TEntity> : ODataController where TEntity : class
    {
        [HttpGet]
        public IActionResult Get(ODataQueryOptions<TEntity> queryOptions, CancellationToken cancellationToken)
        {

           // queryOptions.Filter.Validate(new ODataValidationSettings());
            var list = new List<EntityBase>
            {
                CreateNewStudent(),
            }.AsQueryable();

            return Ok(list);
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
