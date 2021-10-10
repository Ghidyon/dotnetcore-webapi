using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;
using Contracts;
using AutoMapper;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.JsonPatch;
using CompanyEmployees.ActionFilters;

namespace CompanyEmployees.Controllers
{

    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : Controller
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public EmployeesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);

            if (company == null)
            {
                _logger.LogInfo($"Company with Id: {companyId} doesn't exist in our database");
                return NotFound();
            }

            var employees = await _repository.Employee.GetEmployeesAsync(companyId, trackChanges: false);
            var employeeDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return Ok(employeeDto);
        }

        [HttpGet("{id}", Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);

            if (company == null)
            {
                _logger.LogInfo($"Company with Id: {companyId} doesn't exist in our database");
                return NotFound();
            }

            var employee = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: false);

            if (employee == null)
            {
                _logger.LogInfo($"Employee with Id: {id} doesn't exist in our database");
                return NotFound();
            }

            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            return Ok(employeeDto);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployee(Guid companyId, [FromBody]EmployeeForCreationDto employee)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employeeEntity = _mapper.Map<Employee>(employee);

            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            await _repository.SaveAsync();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

            return CreatedAtRoute("GetEmployeeForCompany", 
                new {
                    companyId,
                    id = employeeToReturn.Id
                },
                employeeToReturn);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company is null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employeeforCompany = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: false);

            if (employeeforCompany is null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            _repository.Employee.DeleteEmployee(employeeforCompany);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody]EmployeeForUpdateDto employee)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company is null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database."); 
                return NotFound();
            }

            var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: true);
            if (employeeEntity is null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            _mapper.Map(employee, employeeEntity);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id,
            [FromBody]JsonPatchDocument<EmployeeForUpdateDto> patchDocument)
        {
            if (patchDocument == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }


            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: true);
            if (employeeEntity == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);

            patchDocument.ApplyTo(employeeToPatch, ModelState);
            
            // Re-validate already patched model to further check for error
            TryValidateModel(employeeToPatch);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(employeeToPatch, employeeEntity);

            await _repository.SaveAsync();
            return NoContent();
        }
    }
}
