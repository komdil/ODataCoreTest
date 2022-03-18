using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData.Edm;
using System;

namespace ODataCoreTest
{
    public class MyODataSerializerProvider : DefaultODataSerializerProvider
    {
        MyResourceSerializer myResourceSerializer;
        public MyODataSerializerProvider(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            myResourceSerializer = new MyResourceSerializer(this);
        }

        public override ODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
        {
            if (edmType.IsEntity())
            {
                return myResourceSerializer;
            }
            return base.GetEdmTypeSerializer(edmType);
        }
    }
}
