using System;

namespace ODataCoreTest
{
    public class Student
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

        public Student ErrorProperty { get { throw new Exception(); } set { } }
    }
}
