using Microsoft.AspNetCore.OData.Formatter.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ODataCoreTest
{
    /// <summary>
    /// Provides custom serializers
    /// </summary>
    public class MyODataSerializerProvider : DefaultODataSerializerProvider
    {
        public MyODataSerializerProvider(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }
    }
}
