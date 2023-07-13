using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ODataCoreTest
{
    public class StudentController : ODataController
    {
        private readonly AppDbContext AppDbContext;

        public StudentController(AppDbContext appDbContext)
        {
            AppDbContext = appDbContext;
            AppDbContext.InitDataBase();
        }

        [HttpGet]
        [EnableQuery()]
        public IActionResult Get(ODataQueryOptions<Student> oDataQueryOptions)
        {
            var result = oDataQueryOptions.ApplyTo(AppDbContext.GetEntities<Student>().Take(100));

            List<Student> entities = new List<Student>();
            foreach (var item in result)
            {
                entities.Add((Student)item);
            }
            return Ok(entities);
        }

        [EnableQuery]
        [HttpPost]
        public IActionResult Run([FromRoute] Guid Id)
        {
            var student = AppDbContext.GetEntities<Student>().Include(s => s.Address).FirstOrDefault();
            return Ok(student);
        }

        [EnableQuery]
        [HttpPost]
        public IActionResult RunWithPrefix([FromRoute] Guid Id)
        {
            return Run(Id);
        }
    }
}
