using ExampleGra.DAO;
using ExampleGra.Models;
using HotChocolate.Subscriptions;
using ExampleGra.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using ExampleGra.Input;

namespace ExampleGra
{
    public class Query
    {
        public List<Employee> AllEmployeeOnly([Service] EmployeeRepository employeeRepository) =>
            employeeRepository.GetEmployees();

        public List<Employee> AllEmployeeWithDepartment([Service] EmployeeRepository employeeRepository) =>
            employeeRepository.GetEmployeesWithDepartment();

        public async Task<Employee> GetEmployeeById([Service] EmployeeRepository employeeRepository,
            [Service] ITopicEventSender eventSender, int id)
        {
            Employee gottenEmployee = employeeRepository.GetEmployeeById(id);
           await eventSender.SendAsync("ReturnedEmployee", gottenEmployee);
            return gottenEmployee;
        }

        public List<Departament> AllDepartmentsOnly([Service] DepartmentRepository departmentRepository) =>
        departmentRepository.GetAllDepartmentOnly();

        public List<Departament> AllDepartmentsWithEmployee([Service] DepartmentRepository departmentRepository) =>
            departmentRepository.GetAllDepartmentsWithEmployee();

        public async Task<List<Departament>> AllReg([Service] DbContextExtension dbContextExtension)
        {
            var departments = await dbContextExtension.GetAllAsync<Departament>();
            return departments.ToList();
        }

        public async Task<List<Employee>> AllEmployeeInclude([Service] DbContextExtension dbContextExtension)
        {
            var employeeWidthDepartament = await dbContextExtension.GetEntitiesWithInclude<Employee>(
                include: q => q.Include(d => d.Departament));

            return employeeWidthDepartament;
        }

        public async Task<List<Employee>> AllEmployeeIncludeFilter([Service] DbContextExtension dbContextExtension,
            EmployeeInput filterModel)
        { 

            Employee filterEmployee = new Employee
            {
                Id = filterModel.Id.HasValue ? filterModel.Id.Value : -999,
                Name = filterModel.Name,
                Age = filterModel.Age.HasValue ? filterModel.Age.Value : -999
            };

            var filterExpression = DbContextExtension.BuildDynamicFilterExpression(filterEmployee);

            var employeeWidthDepartament = await dbContextExtension.GetEntitiesWithInclude<Employee>(
                 filter: filterExpression, include: q => q.Include(d => d.Departament));

            return employeeWidthDepartament;
        }

        //    var filterExpression = QueryExtension.BuildDynamicFilterExpression(filterModel);
          //  Expression<Func<Persona, bool>> filterExpression = p => p.Apellido == "Gomez";
        

    }
}
