using System;
using System.Collections.Generic;

namespace ODataCoreTest
{
    public class EntityBase
    {
        public IBackpack Backpack2
        {
            get
            {
                return new Backpack() { Id = Guid.NewGuid() };
            }
            set
            {

            }
        }


        ODataInterfacesDictionary odataInteraces;

        public virtual ODataInterfacesDictionary ODataInterfaces
        {
            get
            {
                if (odataInteraces == null)
                {
                    var interfaces = new Dictionary<string, object>();
                    interfaces.Add("Backpack2", Backpack2);
                    interfaces.Add("Backpack3", null);
                    odataInteraces = new ODataInterfacesDictionary(interfaces);
                }
                return odataInteraces;
            }
        }

    }
}