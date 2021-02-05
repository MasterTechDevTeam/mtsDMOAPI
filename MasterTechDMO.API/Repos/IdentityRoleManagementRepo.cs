using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Repos
{
	public class IdentityRoleManagementRepo : IIdentityRoleManagementRepo
    {
        private readonly IServiceProvider _serviceProvider;

        public IdentityRoleManagementRepo(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task CreateRolesAsync(string roleName)
        {
            try
            {
                var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = _serviceProvider.GetRequiredService<UserManager<DMOUsers>>();

                var roleCheck = await roleManager.RoleExistsAsync(roleName);

                if (!roleCheck)
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            catch (Exception Ex)
            {

                throw;
            }
        }

        public async Task<APICallResponse<bool>> AssignRoleToUserAsync(string userId, string roleName)
        {
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = _serviceProvider.GetRequiredService<UserManager<DMOUsers>>();

            bool isRoleExists = await roleManager.RoleExistsAsync(roleName);
            if (!isRoleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            DMOUsers user = await userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var result = await userManager.AddToRoleAsync(user, roleName);

                if (result.Succeeded)
                {
                    return new APICallResponse<bool>()
                    {
                        IsSuccess = true,
                        Message = new List<string>(){ string.Format($"{roleName} assigned to {userId} user") },
                        Respose = true
                    };
                }
                else
                {
                    List<string> iError = result.Errors.Select(x => x.Description).ToList();

                    return new APICallResponse<bool>
                    {
                        IsSuccess = false,
                        Message = iError,
                        Status = "IdentityError",
                        Respose = false
                    };
                }
            }

            return new APICallResponse<bool>
            {
                IsSuccess = false,
                Message = new List<string>() { "Can not assign role.User not found." },
                Status = "Warning",
                Respose = false
            };
        }
    }
}
