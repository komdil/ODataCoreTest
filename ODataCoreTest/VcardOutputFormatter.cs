using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataCoreTest
{
    public class VcardOutputFormatter : ODataOutputFormatter
    {
        public VcardOutputFormatter(ODataOutputFormatter oDataOutputFormatter) : base(new List<ODataPayloadKind> { ODataPayloadKind.Resource })
        {
            foreach (var item in oDataOutputFormatter.SupportedMediaTypes)
            {
                SupportedMediaTypes.Add(item);
            }
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }
        protected override bool CanWriteType(Type type)
        {
            return true;
        }

        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            var res = base.CanWriteResult(context);
            return res;
        }

        public override Task WriteAsync(OutputFormatterWriteContext context)
        {
            return base.WriteAsync(context);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            return base.WriteResponseBodyAsync(context, selectedEncoding);
        }
    }
}
