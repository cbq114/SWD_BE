using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Applications.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<List<RoleDTO>> GetAllRoles()
        {
            var list = await _roleRepository.GetAllRoles();
            return _mapper.Map<List<RoleDTO>>(list);
        }
    }
}
