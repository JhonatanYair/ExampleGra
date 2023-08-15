using ExampleGra.Datos;
using ExampleGra.Models;
using Microsoft.EntityFrameworkCore;

namespace ExampleGra.DAO
{
    public class DepartmentRepository
    {
        private readonly ExampleDBContext _sampleAppDbContext;

        public DepartmentRepository(ExampleDBContext sampleAppDbContext)
        {
            _sampleAppDbContext = sampleAppDbContext;
        }

        public List<Departament> GetAllDepartmentOnly()
        {
            return _sampleAppDbContext.Departament.ToList();
        }

        public List<Departament> GetAllDepartmentsWithEmployee()
        {
            return _sampleAppDbContext.Departament
                .Include(d => d.Employee)
            .ToList();
        }

        public async Task<Departament> CreateDepartment(Departament department)
        {
            await _sampleAppDbContext.Departament.AddAsync(department);
            await _sampleAppDbContext.SaveChangesAsync();
            return department;
        }
    }
}
