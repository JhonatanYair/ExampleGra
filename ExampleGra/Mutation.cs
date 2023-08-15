using ExampleGra.DAO;
using ExampleGra.Models;
using HotChocolate.Subscriptions;
using HotChocolate;
using ExampleGra.Repository;
using ExampleGra.Input;

namespace ExampleGra
{
    public class Mutation
    {
        public async Task<Departament> CreateDepartment([Service] DepartmentRepository departmentRepository,
            [Service] ITopicEventSender eventSender, string departmentName)
        {
            var newDepartment = new Departament
            {
                Name = departmentName                
            };
            var createdDepartment = await departmentRepository.CreateDepartment(newDepartment);

            await eventSender.SendAsync("DepartmentCreated", createdDepartment);
            return createdDepartment;
        }

        public async Task<Employee> CreateEmployeeWithDepartmentId([Service] EmployeeRepository employeeRepository,
            string name, int age, string email, int departmentId)
        {
            Employee newEmployee = new Employee
            {
                Name = name,
                Age = age,
                Email = email,
                DepartamentId = departmentId
            };

            var createdEmployee = await employeeRepository.CreateEmployee(newEmployee);
            return createdEmployee;
        }

        public async Task<Employee> CreateEmployeeWithDepartment([Service] EmployeeRepository employeeRepository,
            string name, int age, string email, string departmentName)
        {
            Employee newEmployee = new Employee
            {
                Name = name,
                Age = age,
                Email = email,
                Departament = new Departament { Name = departmentName }
            };

            var createdEmployee = await employeeRepository.CreateEmployee(newEmployee);
            return createdEmployee;
        }

        public async Task<Departament> CreateDepartamentV2([Service] DbContextExtension dbContextExtension,
            DepartamentSaveInput departament)
        {

            Departament newDepartament = new Departament
            {
                Name = departament.Name,
            };

            var createdDepartament = await dbContextExtension.AddAsync(newDepartament);
            return createdDepartament;
          
        }

        public async Task<Departament> UpdateDepartamentV2([Service] DbContextExtension dbContextExtension,
    DepartamentUpdateInput departament)
        {

            Departament newDepartament = new Departament
            {
                Id = departament.Id,
                Name = departament.Name,
            };

            var createdDepartament = await dbContextExtension.UpdateAsync(newDepartament);
            return createdDepartament;

        }

        public async Task<string> DeleteDepartament([Service] DbContextExtension dbContextExtension, int Id)
        {
            
            var result = await dbContextExtension.DeleteAsync<Departament>(Id);
            return result ? "Eliminado" : "No eliminado";

        }

    }
}
