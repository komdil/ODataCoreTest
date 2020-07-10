using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ODataCoreTest
{
    public class EagleODataBatchHandler : DefaultODataBatchHandler
    {
        /// <summary />
        public EagleODataBatchHandler()
            : base()
        {
        }

        /// <summary>
        /// Executes the batch request and associates a instance with all the requests of 
        /// a single changeset and wraps the execution of the whole changeset within a transaction.
        /// </summary>
        /// <param name="requests">The <see cref="ODataBatchRequestItem"/> instances of this batch request.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> associated with the request.</param>
        /// <returns>The list of responses associated with the batch request.</returns>
        public override async Task<IList<ODataBatchResponseItem>> ExecuteRequestMessagesAsync(IEnumerable<ODataBatchRequestItem> requests, RequestDelegate handler)
        {
            if (requests == null)
                throw new ArgumentNullException("There is no requests in batch");

            IList<ODataBatchResponseItem> responses = new List<ODataBatchResponseItem>();
            try
            {
                foreach (ODataBatchRequestItem request in requests)
                {
                    responses.Add(await request.SendRequestAsync(handler));
                }
            }
            catch
            {
                throw;
            }
            return responses;
        }
    }
}
