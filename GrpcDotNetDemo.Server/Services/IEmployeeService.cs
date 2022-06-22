using GrpcDotNetDemo.Server.Models;
using System;
using System.Collections.Generic;

namespace GrpcDotNetDemo.Server.Services
{
    public interface IEmployeeService
    {
        List<Employee> GetAllEmployees();
        Employee GetEmployeeById(Guid employeeId);

        ActionResponse CreateEmployee(Employee employee);
        ActionResponse UpdateEmployee(Guid employeeId, Employee employee);
        ActionResponse DeleteEmployee(Guid employeeId);

        ActionResponse CreateEmployees(List<Employee> employees);
    }
}
