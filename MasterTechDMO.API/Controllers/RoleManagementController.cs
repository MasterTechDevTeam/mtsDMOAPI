using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mtsDMO.Context.RoleManagement;

namespace MasterTechDMO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RoleManagementController : ControllerBase
    {

        private IdentityRoleService _identityRoleService;
        private IConfiguration _confifuraton;
        public RoleManagementController(
            IServiceProvider serviceProvider,
             IConfiguration confifuraton,
             MTDMOContext context)
        {
            _identityRoleService = new IdentityRoleService(serviceProvider, context);
        }

        [HttpPost]
        [Route("createRole")]
        public async Task<IActionResult> CreateRoleAsync(CreateRoleDetails roleDetails)
        {
            return Ok(await _identityRoleService.CreateRolesAsync(roleDetails));
        }

        [HttpGet]
        [Route("getRole/{userId}")]
        public async Task<IActionResult> GetRoles(Guid userId)
        {

            return Ok(await _identityRoleService.GetRolesAsync(userId));
        }

        [HttpGet]
        [Route("findRoleById/{roleId}")]
        public async Task<IActionResult> FindRoleById(string roleId)
        {
            return Ok(await _identityRoleService.FindRoleByIdAsync(Guid.Parse(roleId)));
        }

        [HttpPost]
        [Route("updateRoleById/{roleId}")]
        public async Task<IActionResult> UpdateRoleById(DMOOrgRoles orgRoleData)
        {
            return Ok(await _identityRoleService.UpdateRoleByIdAsync(orgRoleData));
        }
    }
}
