using System;
using System.Linq;
using Google.Protobuf.WellKnownTypes;

namespace GrpcDotNetDemo.Server.Mapping
{
    public class GrpcMapper : IGrpcMapper
    {
        public Contracts.Grpc.Employee MapEmployeeModelToContract(Models.Employee employee)
        {
            if (employee == null)
            {
                return null;
            }

            var grpcEmployee = new Contracts.Grpc.Employee();

            grpcEmployee.EmployeeId = employee.Id.ToString();
            grpcEmployee.FirstName = employee.FirstName;
            grpcEmployee.LastName = employee.LastName;
            grpcEmployee.Title = employee.Title;
            grpcEmployee.Department = employee.Department;
            grpcEmployee.StartDate = Timestamp.FromDateTime(employee.StartDate);
            grpcEmployee.EndDate = employee.EndDate.HasValue ? Timestamp.FromDateTime(employee.EndDate.Value) : new Timestamp();
            grpcEmployee.Type = ToGrpcEmployeeType(employee.Type);
            grpcEmployee.IsActive = employee.IsActive;

            if (employee.Skills != null)
            {
                grpcEmployee.Skills.AddRange(employee.Skills);
            }

            grpcEmployee.HourlyPay = employee.HourlyPay;

            return grpcEmployee;
        }

        public Contracts.Grpc.CommandResult MapActionResponseModelToContract(Models.ActionResponse actionResponse)
        {
            if (actionResponse == null)
            {
                return null;
            }

            var grpcCommandResult = new Contracts.Grpc.CommandResult();

            grpcCommandResult.ActionResult = ToGrpcActionResultType(actionResponse.ActionResult);

            if (actionResponse.Errors != null)
            {
                grpcCommandResult.Errors.AddRange(actionResponse.Errors);
            }

            return grpcCommandResult;
        }

        public Models.Employee MapContractToEmployeeModel(Contracts.Grpc.Employee employee)
        {
            if (employee == null)
            {
                return null;
            }

            var employeeModel = new Models.Employee
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Title = employee.Title,
                Department = employee.Department,
                StartDate = employee.StartDate.ToDateTime(),
                EndDate = employee.EndDate?.ToDateTime(),
                Type = ToModelEmployeeType(employee.Type),
                IsActive = employee.IsActive,
                Skills = employee.Skills.ToList(),
                HourlyPay = employee.HourlyPay
            };

            return employeeModel;
        }

        private static Contracts.Grpc.EmployeeType ToGrpcEmployeeType(Models.EmployeeType employeeType) => employeeType switch
        {
            Models.EmployeeType.FullTime => Contracts.Grpc.EmployeeType.FullTime,
            Models.EmployeeType.PartTime => Contracts.Grpc.EmployeeType.PartTime,
            Models.EmployeeType.Contractor => Contracts.Grpc.EmployeeType.Contractor,
            Models.EmployeeType.Intern => Contracts.Grpc.EmployeeType.Intern,
            _ => throw new ArgumentOutOfRangeException()
        };

        private static Models.EmployeeType ToModelEmployeeType(Contracts.Grpc.EmployeeType employeeType) => employeeType switch
        {
            Contracts.Grpc.EmployeeType.FullTime => Models.EmployeeType.FullTime,
            Contracts.Grpc.EmployeeType.PartTime => Models.EmployeeType.PartTime,
            Contracts.Grpc.EmployeeType.Contractor => Models.EmployeeType.Contractor,
            Contracts.Grpc.EmployeeType.Intern => Models.EmployeeType.Intern,
            _ => throw new ArgumentOutOfRangeException()
        };

        private static Contracts.Grpc.ActionResultType ToGrpcActionResultType(Models.ActionResultType actionResultType) => actionResultType switch
        {
            Models.ActionResultType.Success => Contracts.Grpc.ActionResultType.Success,
            Models.ActionResultType.ValidationFailure => Contracts.Grpc.ActionResultType.ValidationFailure,
            Models.ActionResultType.EntityNotFound => Contracts.Grpc.ActionResultType.EntityNotFound,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
