using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using mtsDMO.Context.RoleManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterTechDMO.API.Helpers;

namespace MasterTechDMO.API.Repos
{
    public class IdentityRoleManagementRepo : IIdentityRoleManagementRepo
    {
        private readonly IServiceProvider _serviceProvider;
        private MTDMOContext _context;

        public IdentityRoleManagementRepo(IServiceProvider serviceProvider, MTDMOContext context)
        {
            _serviceProvider = serviceProvider;
            _context = context;
        }

        public async Task<KeyValuePair<bool, string>> CreateRolesAsync(Guid orgId, string roleName)
        {
            try
            {
                var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                //var userManager = _serviceProvider.GetRequiredService<UserManager<DMOUsers>>();

                var roleCheck = await roleManager.RoleExistsAsync(roleName);

                if (!roleCheck)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (MapOrgWithRole(orgId, roleName))
                    {
                        return new KeyValuePair<bool, string>(true, $"{roleName} created sucessfully");
                    }
                    else
                        return new KeyValuePair<bool, string>(false, $"Oops! Something went wrong while assigning {roleName} role to organization");
                }
                return new KeyValuePair<bool, string>(true, $"{roleName} role already exists.");

            }
            catch (Exception Ex)
            {
                return new KeyValuePair<bool, string>(false, Ex.Message);
            }
        }

        public async Task<APICallResponse<bool>> AssignRoleToUserAsync(string userId, string roleName, string roleId = null)
        {
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = _serviceProvider.GetRequiredService<UserManager<DMOUsers>>();

            if (!string.IsNullOrEmpty(roleId))
            {
                var roleData = await roleManager.FindByIdAsync(roleId.ToString());
                roleName = roleData.Name;
            }

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
                        Message = new List<string>() { string.Format($"{roleName} assigned to {userId} user") },
                        Respose = true,
                        Status = "Success"
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

        public async Task<APICallResponse<List<DMOOrgRoles>>> GetRolesAsync(Guid orgId)
        {
            try
            {

                var lstOrgRoles = new APICallResponse<List<DMOOrgRoles>>();
                if (orgId != null && RepoHelpers.IsOrgUser(orgId, _context))
                {
                    lstOrgRoles.Respose = _context.DMOOrgRoles.Where(x => x.OrgId == orgId).ToList();
                    lstOrgRoles.Message = new List<string> { $"{lstOrgRoles.Respose.Count} roles founds." };
                    lstOrgRoles.Status = "Success";
                }
                else if (orgId != null && RepoHelpers.IsMTAdmin(orgId, _context))
                {
                    lstOrgRoles.Respose = _context.DMOOrgRoles.ToList();
                    lstOrgRoles.Message = new List<string> { $"{lstOrgRoles.Respose.Count} roles founds." };
                    lstOrgRoles.Status = "Success";
                }
                else
                {
                    lstOrgRoles.Respose = null;
                    lstOrgRoles.Message = new List<string> { "Either user not found or user is missing permission." };
                    lstOrgRoles.Status = "Warning";
                }

                lstOrgRoles.IsSuccess = true;
                return lstOrgRoles;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<List<DMOOrgRoles>>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = null
                };
            }
        }

        public async Task<APICallResponse<bool>> CheckUserInRole(Guid Id)
        {
            try
            {
                var callResponse = new APICallResponse<bool>();

                var userInRole = _context.UserRoles.Where(x => x.UserId == Id.ToString()).FirstOrDefault();
                if (userInRole != null)
                {
                    var roleDetails = _context.Roles.Where(x => x.Id == userInRole.RoleId).FirstOrDefault();
                    if (roleDetails.Name == Constants.BaseRole.MTAdmin || roleDetails.Name == Constants.BaseRole.Org)
                    {
                        callResponse.Respose = true;
                        callResponse.Message = new List<string> { $"user {Id} found with {roleDetails.Name} role" };
                        callResponse.Status = "Success";
                    }
                }
                else
                {
                    callResponse.Respose = false;
                    callResponse.Message = new List<string> { "User not found." };
                    callResponse.Status = "Warning";
                }
                callResponse.IsSuccess = true;
                return callResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<bool>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = false
                };
            }
        }

        public async Task<KeyValuePair<bool, string>> CreateBaseRoleAsync(string roleName)
        {
            try
            {
                var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var roleCheck = await roleManager.RoleExistsAsync(roleName);

                if (!roleCheck)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                    return new KeyValuePair<bool, string>(true, $"{roleName} created sucessfully");
                }
                return new KeyValuePair<bool, string>(true, $"{roleName} role already exists.");

            }
            catch (Exception Ex)
            {
                return new KeyValuePair<bool, string>(false, Ex.Message);
            }
        }

        public async Task<APICallResponse<string>> GetAssignedRole(Guid userId)
        {
            try
            {
                var callResponse = new APICallResponse<string>();

                var userInRole = _context.UserRoles.Where(x => x.UserId == userId.ToString()).FirstOrDefault();
                if (userInRole != null)
                {
                    var roleDetails = _context.Roles.Where(x => x.Id == userInRole.RoleId).FirstOrDefault();
                    callResponse.Respose = roleDetails.Name;
                    callResponse.Message = new List<string> { $"user {userId} found with {roleDetails.Name} role" };
                    callResponse.Status = "Success";
                }
                else
                {
                    callResponse.Respose = string.Empty;
                    callResponse.Message = new List<string> { "User not found." };
                    callResponse.Status = "Warning";
                }
                callResponse.IsSuccess = true;
                return callResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<string>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = string.Empty
                };
            }
        }

        public async Task<APICallResponse<bool>> RemoveRoleFromUserAsync(Guid userId)
        {
            try
            {
                var callResponse = new APICallResponse<bool>();

                var userInRoles = _context.UserRoles.Where(x => x.UserId == userId.ToString()).ToList();
                if (userInRoles != null)
                {
                    _context.UserRoles.RemoveRange(userInRoles);
                    await _context.SaveChangesAsync();
                    callResponse.Respose = true;
                }
                else
                {
                    callResponse.Respose = false;
                    callResponse.Message = new List<string> { "User not found." };
                    callResponse.Status = "Warning";
                }
                callResponse.IsSuccess = true;
                return callResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<bool>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = false
                };
            }
        }

        public async Task<APICallResponse<DMOOrgRoles>> FindRoleByIdAsync(Guid roleId)
        {
            try
            {
                var dbOrgRole = _context.DMOOrgRoles.Where(x => x.RoleId == roleId).FirstOrDefault();
                if (dbOrgRole != null)
                {
                    return new APICallResponse<DMOOrgRoles>
                    {
                        IsSuccess = true,
                        Message = new List<string> { "Role Found." },
                        Respose = dbOrgRole,
                        Status = "Success"
                    };
                }
                else
                {
                    return new APICallResponse<DMOOrgRoles>
                    {
                        IsSuccess = true,
                        Message = new List<string> { "Role not found." },
                        Respose = null,
                        Status = "Warning"
                    };
                }
            }
            catch (Exception Ex)
            {
                return new APICallResponse<DMOOrgRoles>
                {
                    IsSuccess = false,
                    Message = new List<string> { Ex.Message },
                    Respose = null,
                    Status = "Error"
                };
            }

        }


        public async Task<APICallResponse<bool>> UpdateRoleByIdAsync(DMOOrgRoles orgRole)
        {
            try
            {
                var dbRole = _context.DMOOrgRoles.Where(x => x.RoleId == orgRole.RoleId).FirstOrDefault();
                if (dbRole != null)
                {
                    dbRole.DisplayName = orgRole.DisplayName;
                    _context.SaveChanges();

                    return new APICallResponse<bool>
                    {
                        IsSuccess = true,
                        Message = new List<string> { "Role update successfully." },
                        Respose = true,
                        Status = "Success"
                    };
                }
                else
                {
                    return new APICallResponse<bool>
                    {
                        IsSuccess = true,
                        Message = new List<string> { "Role not found." },
                        Respose = false,
                        Status = "Warning"
                    };
                }
            }
            catch (Exception Ex)
            {
                return new APICallResponse<bool>
                {
                    IsSuccess = false,
                    Message = new List<string> { Ex.Message },
                    Respose = false,
                    Status = "Error"
                };
            }
           
        }

        #region Helper Methods
        private bool MapOrgWithRole(Guid orgId, string roleName)
        {
            try
            {
                var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var roleId = roleManager.FindByNameAsync(roleName).Result.Id;

                _context.DMOOrgRoles.Add(new DMOOrgRoles
                {
                    DisplayName = roleName,
                    Id = Guid.NewGuid(),
                    OrgId = orgId,
                    RoleId = Guid.Parse(roleId)
                });

                _context.SaveChanges();
                return true;
            }
            catch (Exception Ex)
            {
                return false;
            }
        }
        #endregion
    }
}
