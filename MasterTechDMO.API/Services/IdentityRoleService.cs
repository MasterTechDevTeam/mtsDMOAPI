using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using MasterTechDMO.API.Repos;
using mtsDMO.Context.RoleManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Services
{
    public class IdentityRoleService
    {
        private IIdentityRoleManagementRepo _identityRoleManagementRepo;
        public IdentityRoleService(IServiceProvider serviceProvider, MTDMOContext context)
        {
            _identityRoleManagementRepo = new IdentityRoleManagementRepo(serviceProvider, context);
        }

        public async Task<APICallResponse<bool>> CreateRolesAsync(CreateRoleDetails roleDetails)
        {
            var callResponse = new APICallResponse<bool>();
            callResponse.Message = new List<string>();
            var arrIdentityRoles = roleDetails.RoleName.Split(',').ToArray();
            if (arrIdentityRoles.Length > 0)
            {
                foreach (string roleName in arrIdentityRoles)
                {
                    var resp = await _identityRoleManagementRepo.CreateRolesAsync(roleDetails.UserId, roleName);
                    callResponse.Message.Add(resp.Value);
                    callResponse.IsSuccess = resp.Key;
                    callResponse.Respose = resp.Key;
                }
                callResponse.Status = "Success";
            }

            return callResponse;
        }

        public async Task<APICallResponse<bool>> CreateBaseRolesAsync(string[] roles)
        {
            var callResponse = new APICallResponse<bool>();
            callResponse.Message = new List<string>();
            if (roles.Length > 0)
            {
                foreach (string roleName in roles)
                {
                    var resp = await _identityRoleManagementRepo.CreateBaseRoleAsync(roleName);
                    callResponse.Message.Add(resp.Value);
                    callResponse.IsSuccess = resp.Key;
                    callResponse.Respose = resp.Key;
                }
            }

            return callResponse;
        }

        public async Task<APICallResponse<bool>> AssignRoleToUserAsync(string userId, string roleType)
        {
            return await _identityRoleManagementRepo.AssignRoleToUserAsync(userId, roleType);
        }

        public async Task<APICallResponse<List<DMOOrgRoles>>> GetRolesAsync(Guid userId)
        {
            var userInRoleResponse = await _identityRoleManagementRepo.CheckUserInRole(userId);
            if (userInRoleResponse.IsSuccess && userInRoleResponse.Status == "Success")
            {
                return await _identityRoleManagementRepo.GetRolesAsync(userId);
            }
            else
            {
                return new APICallResponse<List<DMOOrgRoles>> { 
                    IsSuccess = userInRoleResponse.IsSuccess,
                    Message = userInRoleResponse.Message,
                    Respose = null,
                    Status = userInRoleResponse.Status
                };
            }
        }

        public async Task<APICallResponse<DMOOrgRoles>> FindRoleByIdAsync(Guid roleId)
        {
            return await _identityRoleManagementRepo.FindRoleByIdAsync(roleId);
        }

        public async Task<APICallResponse<bool>> UpdateRoleByIdAsync(DMOOrgRoles orgRole)
        {
            return await _identityRoleManagementRepo.UpdateRoleByIdAsync(orgRole);
        }

        public async Task<APICallResponse<bool>> RemoveRoleAsync(Guid orgId,string roleName)
        {
            return await _identityRoleManagementRepo.RemoveRoleAsync(roleName, orgId);
        }
    }
}
