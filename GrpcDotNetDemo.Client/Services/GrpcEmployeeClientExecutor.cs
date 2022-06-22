using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Grpc.Core;
using GrpcDotNetDemo.Contracts.Grpc;

namespace GrpcDotNetDemo.Client.Services
{
    public class GrpcEmployeeClientExecutor : IEmployeeClientExecutor
    {
        private readonly IGrpcChannelAccessor _grpcChannelAccessor;
        private static Metadata _headers;

        public GrpcEmployeeClientExecutor(IGrpcChannelAccessor grpcChannelAccessor)
        {
            _grpcChannelAccessor = grpcChannelAccessor;
        }

        public async Task<bool> GetAll()
        {
            try
            {
                var client = GetClient();

                var result = await client.GetEmployeesAsync(new GetEmployeesRequest(), headers: _headers);

                return true;
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC call failed. Status Code: {ex.StatusCode}. Message: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Get(Guid employeeId)
        {
            try
            {
                var client = GetClient();

                var result = await client.GetEmployeeAsync(new GetEmployeeRequest { EmployeeId = employeeId.ToString() }, headers: _headers);

                return true;
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC call failed. Status Code: {ex.StatusCode}. Message: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Create()
        {
            try
            {
                var client = GetClient();

                var employee = GenerateEmployeeRecord();
                var result = await client.CreateEmployeeAsync(new CreateEmployeeRequest { Employee = employee }, headers: _headers);

                if (result.Result.ActionResult == ActionResultType.Success)
                {
                    return true;
                }

                return false;
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC call failed. Status Code: {ex.StatusCode}. Message: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Update(Guid employeeId)
        {
            try
            {
                var client = GetClient();

                var employee = GenerateEmployeeRecord();
                var result = await client.UpdateEmployeeAsync(new UpdateEmployeeRequest { EmployeeId = employeeId.ToString(), Employee = employee }, headers: _headers);

                if (result.Result.ActionResult == ActionResultType.Success)
                {
                    return true;
                }

                return false;
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC call failed. Status Code: {ex.StatusCode}. Message: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Delete(Guid employeeId)
        {
            try
            {
                var client = GetClient();
                
                var result = await client.DeleteEmployeeAsync(new DeleteEmployeeRequest { EmployeeId = employeeId.ToString() }, headers: _headers);

                if (result.Result.ActionResult == ActionResultType.Success)
                {
                    return true;
                }

                return false;
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC call failed. Status Code: {ex.StatusCode}. Message: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> GetAllStreaming()
        {
            Console.WriteLine("Employees server streaming started");

            try
            {
                var client = GetClient();

                using (var call = client.GetEmployeesStreaming(new GetEmployeesRequest(), headers: _headers))
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        Console.WriteLine($"Returned Employee with Id: {call.ResponseStream.Current.Employee.EmployeeId}");
                    }
                }

                return true;
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC call failed. Status Code: {ex.StatusCode}. Message: {ex.Message}");
                return false;
            }
            finally
            {
                Console.WriteLine("Employees server streaming completed");
            }
        }

        public async Task<bool> BulkCreateClientStreaming(int num)
        {
            Console.WriteLine($"Employees client streaming started. Creating {num} employees.");

            try
            {
                var client = GetClient();

                using (var call = client.BulkCreateEmployeesClientStreaming(headers: _headers))
                {
                    for (int i = 0; i < num; i++)
                    {
                        Console.WriteLine($"Creating employee number: {i+1}");
                        var employee = GenerateEmployeeRecord();
                        await call.RequestStream.WriteAsync(new CreateEmployeeRequest { Employee = employee });
                    }
                    await call.RequestStream.CompleteAsync();

                    var result = await call.ResponseAsync;
                    Console.WriteLine($"Created {result.CreateEmployeeResult.Count} employees.");
                    foreach (var createEmployeeResult in result.CreateEmployeeResult)
                    {
                        Console.WriteLine($"Created employee with Id: {createEmployeeResult.EmployeeId}");
                    }
                }

                return true;
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC call failed. Status Code: {ex.StatusCode}. Message: {ex.Message}");
                return false;
            }
            finally
            {
                Console.WriteLine("Employees client streaming completed");
            }
        }

        public async Task<bool> BulkCreateBidirectionalStreaming(int num)
        {
            Console.WriteLine($"Employees bidirectional streaming started. Creating {num} employees.");

            try
            {
                var client = GetClient();

                using (var call = client.BulkCreateEmployeesBidirectionalStreaming(headers: _headers))
                {
                    var responseTask = Task.Run(async () =>
                    {
                        while (await call.ResponseStream.MoveNext())
                        {
                            Console.WriteLine($"Created Employee with Id: {call.ResponseStream.Current.EmployeeId}");
                        }
                    });

                    for (int i = 0; i < num; i++)
                    {
                        Console.WriteLine($"Creating employee number: {i + 1}");
                        var employee = GenerateEmployeeRecord();
                        await call.RequestStream.WriteAsync(new CreateEmployeeRequest { Employee = employee });
                    }

                    await call.RequestStream.CompleteAsync();
                    await responseTask;
                }

                return true;
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC call failed. Status Code: {ex.StatusCode}. Message: {ex.Message}");
                return false;
            }
            finally
            {
                Console.WriteLine("Employees bidirectional streaming completed");
            }
        }

        public async Task GetToken()
        {
            var client = GetClient();

            var result = await client.GetTokenAsync(new GetTokenRequest { Username = "user1", Password = "password123" });

            _headers = new Metadata();
            _headers.Add("Authorization", $"Bearer {result.Token}");
        }

        private EmployeeManagementService.EmployeeManagementServiceClient GetClient()
        {
            return new EmployeeManagementService.EmployeeManagementServiceClient(_grpcChannelAccessor.CallInvoker);
            //return new EmployeeManagementService.EmployeeManagementServiceClient(_grpcChannelAccessor.Channel);
        }

        private Employee GenerateEmployeeRecord()
        {
            var employeeFaker = new Faker<Employee>()
                .RuleFor(e => e.FirstName, f => f.Name.FirstName())
                .RuleFor(e => e.LastName, f => f.Name.LastName())
                .RuleFor(e => e.Title, f => f.Name.JobTitle())
                .RuleFor(e => e.Department, f => f.Commerce.Department())
                .RuleFor(e => e.StartDate, f =>
                {
                    var startDateTime = f.Date.Between(new DateTime(2000, 1, 1), new DateTime(2017, 12, 31));
                    startDateTime = startDateTime.ToUniversalTime();

                    return Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(startDateTime);
                })
                .RuleFor(e => e.EndDate, f =>
                {
                    var endDateTime = f.Date.Between(new DateTime(2018, 1, 1), DateTime.UtcNow.Date).OrNull(f, .1f);
                    endDateTime = endDateTime.HasValue ? endDateTime.Value.ToUniversalTime() : null;

                    return endDateTime.HasValue ? Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(endDateTime.Value) : new Google.Protobuf.WellKnownTypes.Timestamp();
                })
                .RuleFor(e => e.Type, f => f.PickRandom<EmployeeType>())
                .RuleFor(e => e.IsActive, f => f.PickRandom(new List<bool> { false, true }))
                .RuleFor(e => e.HourlyPay, f => f.Finance.Random.Double(50, 100));

            var employee = employeeFaker.Generate();
                        
            var faker = new Faker();
            var skills = faker.Make<string>(faker.Random.Number(0, 10), () => faker.Lorem.Word());

            employee.Skills.AddRange(skills);

            return employee;
        }
    }
}
