using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.UriParser;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ODataCoreTest
{
    public class BaseController<TEntity> : ODataController where TEntity : class
    {
        AppDbContext AppDbContext = new AppDbContext();

        public BaseController()
        {
            AppDbContext.InitDataBase();
        }

        [HttpGet]
        [EnableQuery()]
        public IActionResult Get(ODataQueryOptions<TEntity> oDataQueryOptions)
        {
            var result = oDataQueryOptions.ApplyTo(AppDbContext.GetEntities<TEntity>());

            List<TEntity> entities = new List<TEntity>();
            foreach (var item in result)
            {
                entities.Add((TEntity)item);
            }
            return Ok(entities);
        }
    }
}
