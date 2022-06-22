using AutoMapper;
using GrpcDotNetDemo.Server.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApiRestContracts = GrpcDotNetDemo.Contracts.Rest;

namespace GrpcDotNetDemo.Server.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IMapper mapper, IEmployeeService employeeService)
        {
            _mapper = mapper;
            _employeeService = employeeService;
        }

        [HttpGet]
        [Route("")]
        public ActionResult<List<ApiRestContracts.Employee>> Get()
        {
            var employees = _employeeService.GetAllEmployees();

            var employeesResponse = _mapper.Map<List<ApiRestContracts.Employee>>(employees);

            return Ok(employeesResponse);
        }

        [HttpGet]
        [Route("{employeeId}")]
        public ActionResult<ApiRestContracts.Employee> Get([FromRoute][Required]Guid employeeId)
        {
            var employee = _employeeService.GetEmployeeById(employeeId);
            if (employee == null)
            {
                return NotFound();
            }

            var employeeResponse = _mapper.Map<ApiRestContracts.Employee>(employee);

            return Ok(employeeResponse);
        }

        [HttpPost]
        [Route("")]
        public ActionResult Post([FromBody]ApiRestContracts.Employee employeeRequest)
        {
            var employeeModel = _mapper.Map<Models.Employee>(employeeRequest);
            var result = _employeeService.CreateEmployee(employeeModel);

            if (result.ActionResult == Models.ActionResultType.ValidationFailure)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { EmployeeId = result.Value });
        }

        [HttpPut]
        [Route("{employeeId}")]
        public ActionResult Put([FromRoute][Required]Guid employeeId, [FromBody]ApiRestContracts.Employee employeeRequest)
        {
            var employeeModel = _mapper.Map<Models.Employee>(employeeRequest);
            var result = _employeeService.UpdateEmployee(employeeId, employeeModel);

            if (result.ActionResult == Models.ActionResultType.ValidationFailure)
            {
                return BadRequest(result.Errors);
            }
            else if (result.ActionResult == Models.ActionResultType.EntityNotFound)
            {
                return NotFound(result.Errors);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("{employeeId}")]
        public ActionResult Delete(Guid employeeId)
        {
            var result = _employeeService.DeleteEmployee(employeeId);
            if (result.ActionResult == Models.ActionResultType.EntityNotFound)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [Route("bulk")]
        public ActionResult BulkPost(ApiRestContracts.EmployeeBulkRequest employeeBulkRequest)
        {
            var employees = _mapper.Map<List<Models.Employee>>(employeeBulkRequest.Employees);
            var result = _employeeService.CreateEmployees(employees);

            if (result.ActionResult == Models.ActionResultType.ValidationFailure)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { EmployeeIds = result.Value });
        }
    }
}