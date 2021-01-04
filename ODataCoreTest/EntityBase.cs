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


        ODataDictionary odataInteraces;

        public virtual ODataDictionary ODataInterfaces
        {
            get
            {
                if (odataInteraces == null)
                {
                    var interfaces = new Dictionary<string, object>();
                    interfaces.Add("Backpack2", Backpack2);
                    odataInteraces = new ODataDictionary(interfaces);
                }
                return odataInteraces;
            }
        }

    }
}