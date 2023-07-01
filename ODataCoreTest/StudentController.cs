using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ODataCoreTest.Repositories;
using System.Collections.Generic;

namespace ODataCoreTest
{
    [EnableQuery]
    public class StudentController : ODataController
    {
        IStudentRepository _studentRepository;

        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [HttpGet]
        public IActionResult Get(ODataQueryOptions<Student> oDataQueryOptions)
        {
            var result = oDataQueryOptions.ApplyTo(_studentRepository.GetStudents());

            List<Student> entities = new List<Student>();
            foreach (var item in result)
            {
                entities.Add((Student)item);
            }
            return Ok(entities);
        }
    }
}
