using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using ODataCoreTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ODataCoreTest
{
    [ODataRoutePrefix("ABC")]
    [Route("odata/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class BackpackController : ODataController
    {
        [HttpGet]
        public IActionResult Get(ODataQueryOptions<Student> queryOptions, CancellationToken cancellationToken)
        {
            var list = new List<Student>
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

        //[HttpGet]
        //[ODataRoute("Test")]
        //public IActionResult MyTest(ODataQueryOptions<Student> queryOptions, CancellationToken cancellationToken)
        //{
        //    var list = new List<Student>
        //    {
        //        CreateNewStudent(),
        //    }.AsQueryable();

        //    return Ok(list);
        //}
    }
}
