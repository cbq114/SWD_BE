using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Persistence;

namespace Tutor.Infratructures.Repositories
{
    public class RoleRepository : Repository<Roles>, IRoleRepository
    {
        private readonly TutorDBContext _dbContext;

        public RoleRepository(TutorDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Roles>> GetAllRoles()
        {
            return await Entities
                .Where(r => r.RoleName == "Manager" || r.RoleName == "Student")
                .ToListAsync();
        }

        public async Task<Roles> GetRolesById(int id)
        {
            return await Entities.FindAsync(id);
        }
    }
}
