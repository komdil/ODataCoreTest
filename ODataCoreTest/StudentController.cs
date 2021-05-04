using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ODataCoreTest
{
    public class StudentController : MyBaseController<Student>
    {
        [HttpDelete("Student/{key}")]
        public IActionResult Delete(string key)
        {
            return Ok($"Orders {key} from OData");
        }

        [HttpGet("Student/{key}")]
        public IActionResult Get2(string key)
        {
            return Ok($"Orders {key} from OData");
        }

        [HttpGet("Student({propName}={propValue})")]
        public IActionResult Get(string propName, string propValue)
        {
            return Ok($"Orders {propName} from OData" + propValue);
        }
    }
}
