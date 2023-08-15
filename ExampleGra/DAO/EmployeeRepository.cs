using ExampleGra.Datos;
using ExampleGra.Models;
using Microsoft.EntityFrameworkCore;

namespace ExampleGra.DAO
{
    public class EmployeeRepository
    {
        private readonly ExampleDBContext _sampleAppDbContext;

        public EmployeeRepository(ExampleDBContext sampleAppDbContext)
        {
            _sampleAppDbContext = sampleAppDbContext;
        }

        public List<Employee> GetEmployees()
        {
            return _sampleAppDbContext.Employee.ToList();
        }

        public Employee GetEmployeeById(int id)
        {
            var employee = _sampleAppDbContext.Employee
                .Include(e => e.Departament)
                .Where(e => e.Id == id)
                .FirstOrDefault();

            if (employee != null)
                return employee;

            return null;
        }

        public List<Employee> GetEmployeesWithDepartment()
        {
            return _sampleAppDbContext.Employee
                .Include(e => e.Departament)
                .ToList();
        }

        public async Task<Employee> CreateEmployee(Employee employee)
        {
            await _sampleAppDbContext.Employee.AddAsync(employee);
            await _sampleAppDbContext.SaveChangesAsync();
            return employee;
        }
    }
}
