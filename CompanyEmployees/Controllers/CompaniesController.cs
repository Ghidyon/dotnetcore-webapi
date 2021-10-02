using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoggerService;
using Contracts;
using Entities.DataTransferObjects;
using AutoMapper;

namespace CompanyEmployees.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public CompaniesController(ILoggerManager logger, IRepositoryManager repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetCompanies()
        {
            
            var companies = _repository.Company.GetAllCompanies(trackChanges: false);

            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

            return Ok(companiesDto);
            
        }

        [HttpGet("{id}")]
        public IActionResult GetCompany(Guid id)
        {
            var company = _repository.Company.GetCompany(id, trackChanges: false);
            var companyDto = _mapper.Map<CompanyDto>(company);
            return Ok(companyDto);
            //if (company == null)
            //{
            //    _logger.LogInfo($"Comany with id: {id} doesn't exist in the database");
            //    return NotFound();
            //} else
            //{
                
            //}
        }
    }
}
