using Microsoft.EntityFrameworkCore;
using System;

namespace ODataCoreTest
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("data source=localhost;integrated security=True;Database=ODataTEst2; MultipleActiveResultSets=true");
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
                    Add(student);
                }
                SaveChanges();
            }
        }
    }
}
