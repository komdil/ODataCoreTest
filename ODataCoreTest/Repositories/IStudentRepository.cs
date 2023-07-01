using Microsoft.EntityFrameworkCore;

namespace ODataCoreTest.Repositories
{
    public interface IStudentRepository
    {
        DbSet<Student> GetStudents();
    }
}
