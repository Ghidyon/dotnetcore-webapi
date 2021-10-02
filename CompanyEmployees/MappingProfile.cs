using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;
using Entities.DataTransferObjects;

namespace CompanyEmployees
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(c => c.FullAddress, option => option.MapFrom(x => string.Join(" ", x.Address, x.Country)));

            CreateMap<Employee, EmployeeDto>();

            CreateMap<CompanyForCreationDto, Company>();
        }
    }
}
