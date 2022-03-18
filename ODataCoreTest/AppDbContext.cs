using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ODataCoreTest
{
    //public class AppDbContext : DbContext
    //{
    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    {
    //        optionsBuilder.UseSqlServer("data source=DILSHODKPC;integrated security=True;Database=ODataTEst; MultipleActiveResultSets=true");
    //    }

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        var student = modelBuilder.Entity<Student>();
    //        student.HasKey(s => s.Id);
    //    }

    //    public void InitDataBase()
    //    {
    //        if (Database.EnsureCreated())
    //        {
    //            for (int i = 0; i < 20; i++)
    //            {
    //                var student = new Student()
    //                {
    //                    Id = Guid.NewGuid(),
    //                    Name = $"Name{i}",
    //                };
    //                Add(student);
    //            }
    //            SaveChanges();
    //        }
    //    }

    //    public IQueryable<T> GetEntities<T>() where T : class
    //    {
    //        return Set<T>();
    //    }
    //}
}
