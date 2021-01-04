using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace ODataCoreTest
{
    public class Student : EntityBase
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IBackpack Backpack
        {
            get
            {
                return new Backpack() { Id = Guid.NewGuid() };
            }
            set
            {

            }
        }

      
        public List<IBackpack> Backpacks { get; set; } = new List<IBackpack>();
    }

    public class Backpack : IBackpack
    {
        public Guid Id { get; set; }

        public ODataBaseInterface MyProperty { get; set; } = new ODataBaseInterface();

        public Address Address { get; set; }
        public string Name { get; set; }
    }

    public class Address
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int PlaceNumber { get; set; }
    }

    public interface IEntity
    {
        public Guid Id { get; set; }
    }

    public class ODataBaseInterface
    {
        public string Name { get; set; } = "Test";
    }

    public class ODataInterface<T> : ODataBaseInterface where T : class
    {
        public Guid Id { get; set; }
        public T Entity { get; set; }
        public static ODataInterface<T> Getinstanse(T entity, Guid guid)
        {
            return new ODataInterface<T> { Entity = entity, Id = guid };
        }
    }



    public interface IBackpack : IEntity
    {
        ODataBaseInterface MyProperty { get; set; }
    }
}
