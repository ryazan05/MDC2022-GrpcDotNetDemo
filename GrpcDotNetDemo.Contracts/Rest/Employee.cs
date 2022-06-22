using System;
using System.Collections.Generic;

namespace GrpcDotNetDemo.Contracts.Rest
{
    public class Employee
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public EmployeeType Type { get; set; }
        public bool IsActive { get; set; }
        public List<string> Skills { get; set; }
        public double HourlyPay { get; set; }
    }
}
