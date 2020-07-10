using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ODataCoreTest
{
    public class BaseController<TEntity> : ODataController where TEntity : class
    {
        [HttpGet]
        [EnableQuery()]
        public IActionResult Get(ODataQueryOptions<TEntity> queryOptions, CancellationToken cancellationToken)
        {
            var list = new List<Student>
            {
                CreateNewStudent("Cody Allen", 130),
                CreateNewStudent("Todd Ostermeier", 160),
                CreateNewStudent("Viral Pandya", 140)
            };
            return Ok(list);
        }

        private static Student CreateNewStudent(string name, int score)
        {
            return new Student
            {
                Id = Guid.NewGuid(),
                Name = name,
                Score = score
            };
        }

        protected IActionResult Get(string propertyName, string key)
        {
            var list = new List<Student>
            {
                CreateNewStudent("Cody Allen", 130),
                CreateNewStudent("Todd Ostermeier", 160),
                CreateNewStudent("Viral Pandya", 140)
            };
            return Ok(list);
        }

        public virtual IActionResult Get(string key) => GetEntity(key);

        IActionResult GetEntity(string key)
        {
            var list = new List<Student>
            {
                CreateNewStudent("Todd Ostermeier", 160),
                CreateNewStudent("Viral Pandya", 140)
            };
            return Ok(list);
        }
    }
}
