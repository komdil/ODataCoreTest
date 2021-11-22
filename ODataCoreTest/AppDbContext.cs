using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ODataCoreTest
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("data source=DILSHODKPC;integrated security=True;Database=ODataTEst; MultipleActiveResultSets=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var student = modelBuilder.Entity<Student>();
            student.HasKey(s => s.Id);
            student.HasMany(s => s.Backpacks).WithOne(s => s.Student).HasForeignKey(s => s.StudentId);
            student.Ignore(s => s.accessBackpacks).Ignore(s => s.accessName).Ignore(s => s.accessScore);

            var backpack = modelBuilder.Entity<Backpack>();
            backpack.HasKey(s => s.Id);
            backpack.HasOne(s => s.Student).WithMany(s => s.Backpacks).HasForeignKey(s => s.StudentId);
            backpack.Ignore(s => s.accessName).Ignore(s => s.accessStudent);
        }

        public void InitDataBase()
        {
            if (Database.EnsureCreated())
            {
                for (int i = 0; i < 20; i++)
                {
                    var student = new Student(Guid.NewGuid())
                    {
                        Name = $"Name{i}",
                        Score = i,
                    };
                    Add(student);
                    for (int j = 0; j < 10; j++)
                    {
                        var backpack = new Backpack(Guid.NewGuid())
                        {
                            Name = $"Name{i}{j}",
                            Student = student,
                        };
                        Add(backpack);
                    }
                }
                SaveChanges();
            }
        }

        public IQueryable<T> GetEntities<T>() where T : class
        {
            return Set<T>();
        }
    }
}
