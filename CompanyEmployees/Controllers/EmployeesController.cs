using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;
using Contracts;
using AutoMapper;
using Entities.DataTransferObjects;

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
        public IActionResult GetEmployeesForCompany(Guid companyId)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);

            if (company == null)
            {
                _logger.LogInfo($"Company with Id: {companyId} doesn't exist in our database");
                return NotFound();
            }

            var employees = _repository.Employee.GetEmployees(companyId, trackChanges: false);
            var employeeDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return Ok(employeeDto);
        }
    }
}
