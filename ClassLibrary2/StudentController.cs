﻿using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using ODataCoreTest;
using System;

namespace ClassLibrary2
{
    [Controller]
    public class StudentController : BaseController<Student>
    {
        [ODataRoute("Student(Id={key})")]
        public override IActionResult Get([FromODataUri] string key)
        {
            return Get("Id", key);
        }
    }
}
