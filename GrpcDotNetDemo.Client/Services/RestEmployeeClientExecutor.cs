using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Bogus;
using GrpcDotNetDemo.Contracts.Rest;

namespace GrpcDotNetDemo.Client.Services
{
    public class RestEmployeeClientExecutor : IEmployeeClientExecutor
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RestEmployeeClientExecutor(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> GetAll()
        {
            return await SendRequest(HttpMethod.Get, "api/employees");
        }

        public async Task<bool> Get(Guid employeeId)
        {
            return await SendRequest(HttpMethod.Get, $"api/employees/{employeeId}");
        }

        public async Task<bool> Create()
        {
            var employee = GenerateEmployeeRecord();

            return await SendRequest(HttpMethod.Post, "api/employees", employee);
        }

        public async Task<bool> Update(Guid employeeId)
        {
            var employee = GenerateEmployeeRecord();

            return await SendRequest(HttpMethod.Put, $"api/employees/{employeeId}", employee);
        }

        public async Task<bool> Delete(Guid employeeId)
        {
            return await SendRequest(HttpMethod.Delete, $"api/employees/{employeeId}");
        }

        private async Task<bool> SendRequest(HttpMethod httpMethod, string endpoint, Employee employee = null)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage(httpMethod, $"https://localhost:7209/{endpoint}")
            {
                Version = new Version(2, 0)
            };

            if (employee != null)
            {
                httpRequestMessage.Content = JsonContent.Create(employee);
            }

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"REST call failed. Status Code: {httpResponseMessage.StatusCode}");
                return false;
            }
        }

        private Employee GenerateEmployeeRecord()
        {
            var employeeFaker = new Faker<Employee>()
                .RuleFor(e => e.FirstName, f => f.Name.FirstName())
                .RuleFor(e => e.LastName, f => f.Name.LastName())
                .RuleFor(e => e.Title, f => f.Name.JobTitle())
                .RuleFor(e => e.Department, f => f.Commerce.Department())
                .RuleFor(e => e.StartDate, f => f.Date.Between(new DateTime(2000, 1, 1), new DateTime(2017, 12, 31)))
                .RuleFor(e => e.EndDate, f => f.Date.Between(new DateTime(2018, 1, 1), DateTime.UtcNow.Date).OrNull(f, .1f))
                .RuleFor(e => e.Type, f => f.PickRandom<EmployeeType>())
                .RuleFor(e => e.IsActive, f => f.PickRandom(new List<bool> { false, true }))
                .RuleFor(e => e.Skills, f => f.Make(f.Random.Number(0, 10), () => f.Lorem.Word()))
                .RuleFor(e => e.HourlyPay, f => f.Finance.Random.Double(50, 100));

            return employeeFaker.Generate();
        }

        public Task<bool> GetAllStreaming()
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkCreateClientStreaming(int num)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkCreateBidirectionalStreaming(int num)
        {
            throw new NotImplementedException();
        }

        public Task GetToken()
        {
            throw new NotImplementedException();
        }
    }
}
