using System;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcDotNetDemo.Contracts.Grpc;
using GrpcDotNetDemo.Server.Mapping;
using GrpcDotNetDemo.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace GrpcDotNetDemo.Server.gRPC
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EmployeeGrpcService : EmployeeManagementService.EmployeeManagementServiceBase
    {
        private readonly IGrpcMapper _mapper;
        private readonly IEmployeeService _employeeService;
        private readonly IJwtTokenAuthService _jwtTokenAuthService;

        public EmployeeGrpcService(IGrpcMapper mapper, IEmployeeService employeeService, IJwtTokenAuthService jwtTokenAuthService)
        {
            _mapper = mapper;
            _employeeService = employeeService;
            _jwtTokenAuthService = jwtTokenAuthService;
        }

        [AllowAnonymous]
        public override Task<GetTokenResponse> GetToken(GetTokenRequest request, ServerCallContext context)
        {
            var getTokenResponse = new GetTokenResponse();

            getTokenResponse.Token = _jwtTokenAuthService.GenerateToken(request.Username, request.Password);

            return Task.FromResult(getTokenResponse);
        }

        public override Task<GetEmployeesResponse> GetEmployees(GetEmployeesRequest request, ServerCallContext context)
        {
            var getEmployeesResponse = new GetEmployeesResponse();

            var employees = _employeeService.GetAllEmployees();
            foreach (var employeeModel in employees)
            {
                var grpcEmployee = _mapper.MapEmployeeModelToContract(employeeModel);
                getEmployeesResponse.Employees.Add(grpcEmployee);
            }

            return Task.FromResult(getEmployeesResponse);
        }

        public override Task<GetEmployeeResponse> GetEmployee(GetEmployeeRequest request, ServerCallContext context)
        {
            var getEmployeeResponse = new GetEmployeeResponse();

            if (Guid.TryParse(request.EmployeeId, out Guid employeeId))
            {
                var employeeModel = _employeeService.GetEmployeeById(employeeId);

                getEmployeeResponse.Employee = _mapper.MapEmployeeModelToContract(employeeModel);
            }

            return Task.FromResult(getEmployeeResponse);
        }

        public override Task<CreateEmployeeResponse> CreateEmployee(CreateEmployeeRequest request, ServerCallContext context)
        {
            var createEmployeeResponse = new CreateEmployeeResponse();

            var employeeModel = _mapper.MapContractToEmployeeModel(request.Employee);
            var result = _employeeService.CreateEmployee(employeeModel);
                        
            createEmployeeResponse.Result = _mapper.MapActionResponseModelToContract(result);
                
            if (result.ActionResult == Models.ActionResultType.Success)
            {
                createEmployeeResponse.EmployeeId = result.Value.ToString();
            }

            return Task.FromResult(createEmployeeResponse);
        }

        public override Task<UpdateEmployeeResponse> UpdateEmployee(UpdateEmployeeRequest request, ServerCallContext context)
        {
            var updateEmployeeResponse = new UpdateEmployeeResponse();

            if (Guid.TryParse(request.EmployeeId, out Guid employeeId))
            {
                var employeeModel = _mapper.MapContractToEmployeeModel(request.Employee);
                var result = _employeeService.UpdateEmployee(employeeId, employeeModel);

                updateEmployeeResponse.Result = _mapper.MapActionResponseModelToContract(result);
            }
            else
            {
                updateEmployeeResponse.Result = new CommandResult();
                updateEmployeeResponse.Result.ActionResult = ActionResultType.ValidationFailure;
                updateEmployeeResponse.Result.Errors.Add("Invalid EmployeeId");
            }

            return Task.FromResult(updateEmployeeResponse);
        }

        public override Task<DeleteEmployeeResponse> DeleteEmployee(DeleteEmployeeRequest request, ServerCallContext context)
        {
            var deleteEmployeeResponse = new DeleteEmployeeResponse();

            if (Guid.TryParse(request.EmployeeId, out Guid employeeId))
            {
                var result = _employeeService.DeleteEmployee(employeeId);

                deleteEmployeeResponse.Result = _mapper.MapActionResponseModelToContract(result);
            }
            else
            {
                deleteEmployeeResponse.Result = new CommandResult();
                deleteEmployeeResponse.Result.ActionResult = ActionResultType.ValidationFailure;
                deleteEmployeeResponse.Result.Errors.Add("Invalid EmployeeId");
            }

            return Task.FromResult(deleteEmployeeResponse);
        }

        // --- Streaming ---
        // Server
        public override async Task GetEmployeesStreaming(GetEmployeesRequest request, IServerStreamWriter<GetEmployeeResponse> responseStream, ServerCallContext context)
        {
            var employees = _employeeService.GetAllEmployees();
            foreach (var employeeModel in employees)
            {
                var grpcEmployee = _mapper.MapEmployeeModelToContract(employeeModel);

                await responseStream.WriteAsync(new GetEmployeeResponse
                {
                    Employee = grpcEmployee
                });
            }
        }

        // --- Streaming ---
        // Client
        public override async Task<BulkCreateEmployeeResponse> BulkCreateEmployeesClientStreaming(IAsyncStreamReader<CreateEmployeeRequest> requestStream, ServerCallContext context)
        {
            var bulkCreateEmployeeResponse = new BulkCreateEmployeeResponse();

            var createAllTask = Task.Run(async () =>
            {
                while (await requestStream.MoveNext())
                {
                    var employeeModel = _mapper.MapContractToEmployeeModel(requestStream.Current.Employee);
                    var result = _employeeService.CreateEmployee(employeeModel);

                    var createResult = _mapper.MapActionResponseModelToContract(result);
                    var createEmployeeResponse = new CreateEmployeeResponse
                    {
                        Result = createResult
                    };                 

                    if (result.ActionResult == Models.ActionResultType.Success)
                    {
                        createEmployeeResponse.EmployeeId = result.Value.ToString();
                    }

                    bulkCreateEmployeeResponse.CreateEmployeeResult.Add(createEmployeeResponse);
                }
            });

            await createAllTask;

            return bulkCreateEmployeeResponse;            
        }

        // --- Streaming ---
        // Bi-directional
        public override async Task BulkCreateEmployeesBidirectionalStreaming(IAsyncStreamReader<CreateEmployeeRequest> requestStream, IServerStreamWriter<CreateEmployeeResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var employeeModel = _mapper.MapContractToEmployeeModel(requestStream.Current.Employee);
                var result = _employeeService.CreateEmployee(employeeModel);

                var createResult = _mapper.MapActionResponseModelToContract(result);
                var createEmployeeResponse = new CreateEmployeeResponse
                {
                    Result = createResult
                };

                if (result.ActionResult == Models.ActionResultType.Success)
                {
                    createEmployeeResponse.EmployeeId = result.Value.ToString();
                }

                await responseStream.WriteAsync(createEmployeeResponse);
            }
        }
    }
}
