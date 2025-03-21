using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Applications.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleDTO>> GetAllRoles();
    }
}
