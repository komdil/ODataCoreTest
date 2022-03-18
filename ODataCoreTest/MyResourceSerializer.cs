using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData;
using System.Linq;

namespace ODataCoreTest
{
    public class MyResourceSerializer : ODataResourceSerializer
    {
        public MyResourceSerializer(ODataSerializerProvider serializerProvider) : base(serializerProvider) { }

        public override ODataResource CreateResource(SelectExpandNode selectExpandNode, ResourceContext resourceContext)
        {
            var resource = base.CreateResource(selectExpandNode, resourceContext);
            if (selectExpandNode.SelectAllDynamicProperties)
            {
                resource.Properties = resource.Properties.Where(p => p.Name != "Image");
            }
            return resource;
        }
    }
}
