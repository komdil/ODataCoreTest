using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ODataCoreTest
{
    public class BlogController : ODataController
    {
        [HttpGet]
        public IActionResult Get()
        {
            var list = new List<Blog>() { new Blog() { Code = Guid.NewGuid().ToString(), Name = "Test", }, new Blog() { Code = Guid.NewGuid().ToString(), Name = "Test", } };
            return Ok(list);
        }
    }
}
