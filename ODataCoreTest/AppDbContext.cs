using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ODataCoreTest
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var student = modelBuilder.Entity<Student>();
            student.HasKey(s => s.Id);
            student.HasOne(s => s.Address);

            var address = modelBuilder.Entity<Address>();
            address.HasKey(s => s.Id);
        }

        public void InitDataBase()
        {
            if (Database.EnsureCreated())
            {
                for (int i = 0; i < 100000; i++)
                {
                    var student = new Student()
                    {
                        Id = Guid.NewGuid(),
                        Name = $"Name{i}",
                    };
                    var address = new Address()
                    {
                        Id = Guid.NewGuid(),
                        City = $"City{i}",
                        Country = $"Country{i}",
                        Street = $"Street{i}"
                    };
                    student.Address = address;

                    Add(student);
                }
                SaveChanges();
            }
        }

        public DbSet<T> GetEntities<T>() where T : class
        {
            return Set<T>();
        }
    }
}
