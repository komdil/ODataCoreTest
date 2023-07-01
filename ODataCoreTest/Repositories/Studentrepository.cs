using Microsoft.EntityFrameworkCore;

namespace ODataCoreTest.Repositories
{
    public class Studentrepository : IStudentRepository
    {
        readonly AppDbContext _context;
        public Studentrepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public DbSet<Student> GetStudents()
        {
            return _context.Set<Student>();
        }
    }
}
