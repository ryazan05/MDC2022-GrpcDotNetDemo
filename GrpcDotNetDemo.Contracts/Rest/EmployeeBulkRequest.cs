using System.Collections.Generic;

namespace GrpcDotNetDemo.Contracts.Rest
{
    public class EmployeeBulkRequest
    {
        public List<Employee> Employees { get; set; }
    }
}
