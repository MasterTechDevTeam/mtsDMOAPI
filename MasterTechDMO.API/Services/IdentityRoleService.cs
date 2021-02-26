using MasterTechDMO.API.Models;
using MasterTechDMO.API.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Services
{
	public class IdentityRoleService
	{
		private IIdentityRoleManagementRepo _identityRoleManagementRepo;
		public IdentityRoleService(IServiceProvider serviceProvider)
		{
			_identityRoleManagementRepo = new IdentityRoleManagementRepo(serviceProvider);
		}

		public async Task<APICallResponse<bool>> CreateRolesAsync(string[] arrIdentityRoles)
		{
			var callResponse = new APICallResponse<bool>();
			callResponse.Message = new List<string>();

			if (arrIdentityRoles.Length > 0)
			{
				foreach (string roleName in arrIdentityRoles)
				{
					var resp = await _identityRoleManagementRepo.CreateRolesAsync(roleName);
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
	}
}
