using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using ODataCoreTest;
using System;

namespace ODataCoreTest
{
    public class StudentController : BaseController<Student>
    {
        [HttpGet]
        [ODataRoute("Test_GuidedChild(Id={id})")]
        public override IActionResult Get([FromODataUri] string id)
        {
            return Get("Id", id);
        }
    }
}
