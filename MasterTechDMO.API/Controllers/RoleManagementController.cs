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
        public RoleManagementController(
          IServiceProvider serviceProvider,
             MTDMOContext context)
        {
            _identityRoleService = new IdentityRoleService(serviceProvider, context);
        }

        /// <summary>
        /// Create role for organization
        /// </summary>
        /// <param name="roleDetails">Object of CreateRoleDetails Type</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpPost]
        [Route("createRole")]
        public async Task<IActionResult> CreateRoleAsync(CreateRoleDetails roleDetails)
        {
            return Ok(await _identityRoleService.CreateRolesAsync(roleDetails));
        }

        /// <summary>
        /// Return list of roles details requested by organization
        /// </summary>
        /// <param name="userId">OrganizationId of organization</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("getRole/{userId}")]
        public async Task<IActionResult> GetRoles(Guid userId)
        {

            return Ok(await _identityRoleService.GetRolesAsync(userId));
        }

        /// <summary>
        /// Return role data
        /// </summary>
        /// <param name="roleId">roleId of Role</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("findRoleById/{roleId}")]
        public async Task<IActionResult> FindRoleById(string roleId)
        {
            return Ok(await _identityRoleService.FindRoleByIdAsync(Guid.Parse(roleId)));
        }

        /// <summary>
        /// Update role
        /// </summary>
        /// <param name="orgRoleData">Object of DMOOrgRoles Type</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpPost]
        [Route("updateRoleById/{roleId}")]
        public async Task<IActionResult> UpdateRoleById(DMOOrgRoles orgRoleData)
        {
            return Ok(await _identityRoleService.UpdateRoleByIdAsync(orgRoleData));
        }

        /// <summary>
        /// Remove role for requested organization
        /// </summary>
        /// <param name="orgId">orgId of Organization</param>
        /// <param name="roleName">name of Role</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("removeRole/{orgId}/{roleName}")]
        public async Task<IActionResult> UpdateRoleById(Guid orgId, string roleName)
        {
            return Ok(await _identityRoleService.RemoveRoleAsync(orgId, roleName));
        }
    }
}
