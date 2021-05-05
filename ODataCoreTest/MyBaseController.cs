using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ODataCoreTest
{
    [EnableQuery(MaxExpansionDepth = 0, EnsureStableOrdering = false)]
    public abstract class MyBaseController<TEntity> : ODataController where TEntity : Student
    {
        [HttpGet("/{contextToken}/[controller]")]
        public IEnumerable<Student> Get(ODataQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var list = new List<Student>
            {
                CreateNewStudent("Cody Allen", 130),
                CreateNewStudent("Todd Ostermeier", 160),
                CreateNewStudent("Viral Pandya", 140)
            };
            return list;
        }

        public IActionResult Get(Guid key)
        {
            var student = CreateNewStudentWithGuid("Cody Allen", 130);
            return Ok(student);
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

        private static Student CreateNewStudentWithGuid(string name, int score)
        {
            return new Student
            {
                Id = new Guid("9DCC39C7-754C-4002-8627-BD719AA13E73"),
                Name = name,
                Score = score
            };
        }

        [AcceptVerbs("PATCH", "MERGE")]
        public IActionResult Patch()
        {
            return Ok(CreateNewStudent("Hello", 12));
        }

        [HttpDelete("[controller]({propName}={propValue})")]
        [HttpDelete("[controller]({key})")]
        [HttpDelete("[controller]/{key}")]
        public IActionResult Delete(Guid key, string propName, string propValue)
        {
            return Ok($"Orders {key} from OData");
        }
    }
}
