using GrpcDotNetDemo.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GrpcDotNetDemo.Server.Services
{
    public class EmployeeService : IEmployeeService
    {
        private static readonly Dictionary<Guid, Employee> _employees = new Dictionary<Guid, Employee>();

        public List<Employee> GetAllEmployees()
        {
            return _employees.Values.ToList();
        }

        public Employee GetEmployeeById(Guid employeeId)
        {
            if (_employees.TryGetValue(employeeId, out Employee employee))
            {
                return employee;
            }

            return null;
        }

        public ActionResponse CreateEmployee(Employee employee)
        {
            var employeeId = Guid.NewGuid();
            employee.Id = employeeId;

            _employees.Add(employeeId, employee);

            return new ActionResponse(ActionResultType.Success, employeeId);
        }

        public ActionResponse UpdateEmployee(Guid employeeId, Employee employee)
        {
            if (!_employees.ContainsKey(employeeId))
            {
                var actionResponse = new ActionResponse(ActionResultType.EntityNotFound);
                actionResponse.Errors.Add($"Employee with Id {employeeId} does not exist.");

                return actionResponse;
            }

            employee.Id = employeeId;
            _employees[employeeId] = employee;

            return new ActionResponse(ActionResultType.Success, employeeId);
        }

        public ActionResponse DeleteEmployee(Guid employeeId)
        {
            if (!_employees.ContainsKey(employeeId))
            {
                var actionResponse = new ActionResponse(ActionResultType.EntityNotFound);
                actionResponse.Errors.Add($"Employee with Id {employeeId} does not exist.");

                return actionResponse;
            }

            _employees.Remove(employeeId);

            return new ActionResponse(ActionResultType.Success, employeeId);
        }

        public ActionResponse CreateEmployees(List<Employee> employees)
        {
            foreach (var employee in employees)
            {
                var employeeId = Guid.NewGuid();
                employee.Id = employeeId;

                _employees.Add(employeeId, employee);
            }

            var employeeIds = _employees.Keys.ToList();

            return new ActionResponse(ActionResultType.Success, employeeIds);
        }
    }
}
