using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ODataCoreTest
{
    public abstract class MyBaseController<TEntity> : ODataController where TEntity : EntityBase
    {
        [HttpGet]
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

        private static Student CreateNewStudent(string name, int score)
        {
            return new Student
            {
                Id = Guid.NewGuid(),
                Name = name,
                Score = score
            };
        }


    }
}
