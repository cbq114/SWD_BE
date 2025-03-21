using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Entities;

namespace Tutor.Infratructures.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<Roles>> GetAllRoles();
        Task<Roles> GetRolesById(int id);
    }
}
