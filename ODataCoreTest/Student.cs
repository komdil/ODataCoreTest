using System;

namespace ODataCoreTest
{
    public class Student
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }
    }

    public class Address
    {
        public Guid Id { get; set; }

        public string Street { get; set; }

        public string Country { get; set; }

        public string City { get; set; }
    }
}
