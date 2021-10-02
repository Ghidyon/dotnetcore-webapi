using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface IEmployeeRepository
    {
        IEnumerable<Employee> GetEmployees(Guid CompanyId, bool trackChanges);

        Employee GetEmployee(Guid CompanyId, Guid EmployeeId, bool trackChaanges);
    }
}
