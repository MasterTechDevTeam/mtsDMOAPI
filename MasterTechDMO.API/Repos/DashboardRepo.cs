using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Helpers;
using MasterTechDMO.API.Models;
using mtsDMO.Context.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Repos
{
    public class DashboardRepo : IDashboardRepo
    {
        private MTDMOContext _context;
        public DashboardRepo(MTDMOContext context)
        {
            _context = context;
        }

        public async Task<APICallResponse<List<DMOUserFriendList>>> GetFriendListAsync(Guid userId)
        {
            try
            {
                var callResponse = new APICallResponse<List<DMOUserFriendList>>();
                var dbFriendList = _context.DMOUserFriendList.Where(x => x.UserId == userId).Take(5).ToList();
                if (dbFriendList != null)
                {
                    callResponse.Message = new List<string>() { $"{dbFriendList.Count} frined found." };
                    callResponse.Respose = dbFriendList;
                    callResponse.Status = "Success";
                }
                else
                {
                    callResponse.Message = new List<string>() { "Friend List is empty." };
                    callResponse.Status = "Warning";
                    callResponse.Respose = null;
                }
                callResponse.IsSuccess = true;
                return callResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<List<DMOUserFriendList>>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = null
                };
            }
        }

        public async Task<APICallResponse<List<DMOTaskScheduler>>> GetTaskAsync(Guid userId)
        {
            try
            {
                var callResponse = new APICallResponse<List<DMOTaskScheduler>>();
                var dbTaskList = _context.DMOTaskScheduler.Where(x => x.UserId == userId || x.Attendee.Contains(userId.ToString())).Take(5).ToList();
                if (dbTaskList != null)
                {
                    callResponse.Message = new List<string> { $"{dbTaskList.Count} task(s) found" };
                    callResponse.Respose = dbTaskList;
                    callResponse.Status = "Success";
                }
                else
                {
                    callResponse.Message = new List<string> { "No Task found" };
                    callResponse.Respose = null;
                    callResponse.Status = "Warning";
                }
                callResponse.IsSuccess = true;
                return callResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<List<DMOTaskScheduler>>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = null
                };
            }
        }

        public async Task<APICallResponse<List<TemplateData>>> GetTemplateAsync(Guid userId)
        {
            try
            {
                var callResponse = new APICallResponse<List<TemplateData>>();
                var dbTemplateList = _context.DMOTemplateData.Where(x => x.UserId == userId && x.IsRemoved == false).Take(5).ToList();
                if (dbTemplateList != null)
                {
                    var lstTemplateData = new List<TemplateData>();
                    foreach (var template in dbTemplateList)
                    {
                        lstTemplateData.Add(new TemplateData
                        {
                            HtmlContent = template.HtmlContent,
                            Id = template.Id,
                            ThumbnailIamgePath = template.ThumbnailIamgePath,
                            UserId = template.UserId
                        });
                    }

                    callResponse.Message = new List<string> { $"{lstTemplateData.Count} template(s) found" };
                    callResponse.Respose = lstTemplateData;
                    callResponse.Status = "Success";
                }
                else
                {
                    callResponse.Message = new List<string> { "No template found" };
                    callResponse.Respose = null;
                    callResponse.Status = "Warning";
                }
                callResponse.IsSuccess = true;
                return callResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<List<TemplateData>>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = null
                };
            }
        }

        public async Task<APICallResponse<List<DMOUsers>>> GetUsersAsync(Guid userId)
        {
            try
            {
                var lstOrgUsers = new APICallResponse<List<DMOUsers>>();
                if (userId != null && RepoHelpers.IsOrgUser(userId, _context))
                {
                    lstOrgUsers.Respose = _context.Users.Where(x => x.OrgId == userId && x.IsDeactive == false).Take(5).ToList();
                    lstOrgUsers.Message = new List<string> { $"{lstOrgUsers.Respose.Count} users founds." };
                    lstOrgUsers.Status = "Success";
                }
                else if (userId != null && RepoHelpers.IsMTAdmin(userId, _context))
                {
                    lstOrgUsers.Respose = _context.Users.Take(5).ToList();
                    lstOrgUsers.Message = new List<string> { $"{lstOrgUsers.Respose.Count} users founds." };
                    lstOrgUsers.Status = "Success";
                }
                else
                {
                    lstOrgUsers.Respose = null;
                    lstOrgUsers.Message = new List<string> { "Either user not found or user is missing permission." };
                    lstOrgUsers.Status = "Warning";
                }

                lstOrgUsers.IsSuccess = true;
                return lstOrgUsers;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<List<DMOUsers>>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = null
                };
            }
        }

        public async Task<APICallResponse<List<DMOOrgRoles>>> GetRolesAsync(Guid orgId)
        {
            try
            {

                var lstOrgRoles = new APICallResponse<List<DMOOrgRoles>>();
                if (orgId != null && RepoHelpers.IsOrgUser(orgId, _context))
                {
                    lstOrgRoles.Respose = _context.DMOOrgRoles.Where(x => x.OrgId == orgId && x.IsDeactive == false).ToList();
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
        
    }
}
