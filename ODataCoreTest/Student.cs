using System;
using System.Collections.Generic;

namespace ODataCoreTest
{
    public class Student : EntityBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
    }

    public abstract class EntityBase
    {
        public Dictionary<string, object> Test { get; set; }
        public Dictionary<string, object> Test2 { get; set; }
    }
}
