using System;
using System.Collections.Generic;

namespace ODataCoreTest
{
    public class Student
    {
        public Student(Guid guid)
        {
            Id = guid;
        }
        protected Student()
        {

        }

        public Guid Id { get; set; }

        public bool accessName
        {
            get
            {
                return true;
            }
        }
        public string Name { get; set; }

        public bool accessScore
        {
            get
            {
                return true;
            }
        }

        public int Score { get; set; }

        public bool accessBackpacks
        {
            get
            {
                return true;
            }
        }
        public virtual List<Backpack> Backpacks { get; set; } = new List<Backpack>();
    }

    public class Backpack
    {
        protected Backpack()
        {

        }

        public Backpack(Guid guid)
        {
            Id = guid;
        }

        public Guid Id { get; set; }

        public bool accessName
        {
            get
            {
                return true;
            }
        }

        public string Name { get; set; }

        public bool accessStudent
        {
            get
            {
                return true;
            }
        }

        public virtual Student Student { get; set; }

        public Guid StudentId { get; set; }
    }
}
