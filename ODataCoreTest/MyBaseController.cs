using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ODataCoreTest
{
    public class MyBaseController<TEntity> : ODataController, IDisposable where TEntity : class
    {
        public const string JsonContentType = "application/json";

        #region CRUD

        #region Get

        /// <summary>
        /// Gets single entity
        /// </summary>
        /// <returns>A queryable collection of entities.</returns>
        [HttpGet("[controller]/{key}")]
        public IActionResult Get(string key)
        {
            return Ok();
        }

        /// <summary>
        /// Gets a collection of entities.
        /// </summary>
        /// <returns>A queryable collection of entities.</returns>
        public IActionResult Get(ODataQueryOptions<TEntity> queryOptions, CancellationToken cancellationToken)
        {
            return Ok();
        }



        #endregion

        #region Post

        /// <summary>
        /// Creates entities and saves to database
        /// </summary>
        /// <param name="entity">Entity to save to database</param>
        /// <returns>Result of creating entity, if saved successfully, returning list of saved, otherwise returning messages</returns>
        public IActionResult Post([FromBody] object jsonContent)
        {
            return Ok();
        }



        #endregion

        #region Put

        /// <summary>
        /// Replacing entity by Guid
        /// </summary>
        [HttpPut("[controller]/{key}")]
        public IActionResult Put([FromRoute] string key, [FromBody] object jsonContent)
        {
            return Ok();
        }



        #endregion

        #region Patch

        /// <summary>
        /// Updating entity by Guid
        /// </summary>
        [AcceptVerbs("PATCH", "MERGE")]
        [HttpPatch("[controller]/{key}")]
        public IActionResult Patch([FromRoute] string key, [FromBody] object jsonContent)
        {
            return Ok();
        }



        #endregion

        #region Delete

        /// <summary>
        /// Deleting entity based on Guid
        /// </summary>
        /// <param name="key">Guid of entity that should be deleted</param>
        [HttpDelete("[controller]/{key}")]
        public IActionResult Delete(string key)
        {
            return Ok();
        }




        #endregion



        #endregion

        #region Error handling




        #endregion

        #region Commands

        /// <summary>
        /// Execute a specific command via an HttpPost call
        /// </summary>
        [HttpPost("[controller]/{key}/Model.Entities.{commandName}")]
        [HttpPost("[controller]({key})/Model.Entities.{commandName}")]
        public IActionResult ExecuteCommand(string commandName, string key, [FromBody] object parameters)
        {
            return Ok();
        }



        #endregion

        #region Batch


        #endregion

        #region Dispose

        /// <summary>
        /// Releases the unmanaged resources that are used by the object and, optionally, releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// True to release both managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        public void Dispose()
        {

        }
        #endregion
    }
}
