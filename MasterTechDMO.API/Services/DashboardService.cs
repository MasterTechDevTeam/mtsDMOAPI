using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using MasterTechDMO.API.Repos;
using mtsDMO.Context.UserManagement;
using mtsDMO.Context.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Services
{
    public class DashboardService
    {
        private IDashboardRepo _dashboardRepo;
        private IIdentityRoleManagementRepo _identityRoleManagementRepo;
        public DashboardService(MTDMOContext context, IServiceProvider serviceProvider)
        {
            _dashboardRepo = new DashboardRepo(context);
            _identityRoleManagementRepo = new IdentityRoleManagementRepo(serviceProvider, context);
        }

        public async Task<APICallResponse<List<UserFriendData>>> GetFriendListAsync(Guid userId)
        {
            var callResponse = new APICallResponse<List<UserFriendData>>();
            var dbFriendListCallResponse = await _dashboardRepo.GetFriendListAsync(userId);

            if (dbFriendListCallResponse.Respose != null)
            {
                callResponse.Respose = new List<UserFriendData>();
                foreach (var dbFriend in dbFriendListCallResponse.Respose)
                {
                    callResponse.Respose.Add(new UserFriendData
                    {
                        EmailId = dbFriend.EmailId,
                        Id = dbFriend.Id,
                        Name = dbFriend.Name,
                        PhoneNumber = dbFriend.PhoneNumber,
                        UserId = dbFriend.UserId
                    });
                }
            }
            callResponse.IsSuccess = dbFriendListCallResponse.IsSuccess;
            callResponse.Message = dbFriendListCallResponse.Message;
            callResponse.Status = dbFriendListCallResponse.Status;
            return callResponse;
        }

        public async Task<APICallResponse<List<SchedulerData>>> GetTaskAsync(Guid userId)
        {
            var lstTaskResponse = await _dashboardRepo.GetTaskAsync(userId);
            if (lstTaskResponse.Respose != null)
            {
                List<SchedulerData> lstSchedulers = new List<SchedulerData>();

                foreach (var task in lstTaskResponse.Respose)
                {
                    var lstAttendee = new List<Guid>();
                    if (task.Attendee != "")
                    {

                        foreach (var contact in task.Attendee.Split(',').ToList())
                        {
                            lstAttendee.Add(Guid.Parse(contact));
                        }
                    }

                    var schedulerData = new SchedulerData();
                    schedulerData.Description = task.Description;
                    schedulerData.EndDate = task.EndDate;
                    schedulerData.EventID = task.Id;
                    schedulerData.IsFullDay = task.IsFullDay;
                    schedulerData.StartDate = task.StartDate;
                    schedulerData.Subject = task.Subject;
                    schedulerData.ThemeColor = task.ThemeColor;
                    schedulerData.Attendee = lstAttendee;

                    lstSchedulers.Add(schedulerData);
                }

                return new APICallResponse<List<SchedulerData>>
                {
                    IsSuccess = lstTaskResponse.IsSuccess,
                    Message = lstTaskResponse.Message,
                    Respose = lstSchedulers,
                    Status = lstTaskResponse.Status
                };
            }

            return new APICallResponse<List<SchedulerData>>
            {
                IsSuccess = lstTaskResponse.IsSuccess,
                Message = lstTaskResponse.Message,
                Respose = null,
                Status = lstTaskResponse.Status
            };
        }

        public async Task<APICallResponse<List<TemplateData>>> GetTemplateAsync(Guid userId)
        {
            return await _dashboardRepo.GetTemplateAsync(userId);
        }

        public async Task<APICallResponse<List<UserProfile>>> GetUsersAsync(Guid userId)
        {
            var callReponse = new APICallResponse<List<UserProfile>>();

            var lstOrgUsers = await _dashboardRepo.GetUsersAsync(userId);

            if (lstOrgUsers.Respose != null)
            {
                List<UserProfile> lstUsers = new List<UserProfile>();
                foreach (var user in lstOrgUsers.Respose)
                {
                    string assignedRole = string.Empty;
                    string userType = string.Empty;

                    var result = await _identityRoleManagementRepo.GetAssignedRole(Guid.Parse(user.Id));

                    if (result.IsSuccess && result.Status == "Success")
                    {
                        assignedRole = result.Respose;
                    }

                    if (user.IsOrg)
                        userType = Constants.BaseRole.Org;
                    else if (user.OrgId != null)
                        userType = Constants.BaseRole.OrgUser;
                    else
                        userType = Constants.BaseRole.Indevidual;

                    lstUsers.Add(
                        new UserProfile
                        {
                            Address = user.Address,
                            City = user.City,
                            ContactNo = user.ContactNo,
                            Country = user.Country,
                            DateofBirth = user.DateofBirth,
                            EmailId = user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            State = user.State,
                            UserId = Guid.Parse(user.Id),
                            Zipcode = user.Zipcode,
                            AssignedRole = assignedRole,
                            UserType = userType,
                            IsVerified = user.EmailConfirmed
                        });
                }
                callReponse.Respose = lstUsers;
                callReponse.IsSuccess = true;
                callReponse.Message = lstOrgUsers.Message;
                callReponse.Status = "Success";
                return callReponse;
            }
            callReponse.Respose = null;
            callReponse.IsSuccess = lstOrgUsers.IsSuccess;
            callReponse.Message = lstOrgUsers.Message;
            callReponse.Status = lstOrgUsers.Status;
            return callReponse;

        }

        public async Task<APICallResponse<List<OrgAssignedRoles>>> GetRolesAsync(Guid userId)
        {
            var callResponse = await _dashboardRepo.GetRolesAsync(userId);
            if (callResponse != null)
            {
                var orgRoles = new List<OrgAssignedRoles>();
                if (callResponse.IsSuccess && callResponse.Respose != null)
                {
                    foreach (var role in callResponse.Respose)
                    {
                        orgRoles.Add(new OrgAssignedRoles
                        {
                            DisplayName = role.DisplayName,
                            Id = role.Id,
                            OrgId = role.OrgId,
                            RoleId = role.RoleId
                        }); ;
                    }
                }

                return new APICallResponse<List<OrgAssignedRoles>>()
                {
                    IsSuccess = callResponse.IsSuccess,
                    Message = callResponse.Message,
                    Respose = orgRoles,
                    Status = callResponse.Status
                };
            }
            else
            {

                return new APICallResponse<List<OrgAssignedRoles>>()
                {
                    IsSuccess = false,
                    Message = new List<string>() { "Oops! something went wrong" },
                    Respose = null,
                    Status = "Error"
                };
            }
        }

        public async Task<APICallResponse<DashboardData>> GetDashboardDetailsAsync(Guid userId)
        {
            var dashboardData = new DashboardData();
            var friendCallResponse = await GetFriendListAsync(userId);
            var roleCallResponse = await GetRolesAsync(userId);
            var userCallResponse = await GetUsersAsync(userId);
            var taskCallResponse = await GetTaskAsync(userId);
            var templateCallResponse = await GetTemplateAsync(userId);

            dashboardData.Friends = friendCallResponse?.Respose;
            dashboardData.Roles = roleCallResponse?.Respose;
            dashboardData.Users = userCallResponse?.Respose;
            dashboardData.Tasks = taskCallResponse?.Respose;
            dashboardData.Templates = templateCallResponse?.Respose;



            if (friendCallResponse.IsSuccess && roleCallResponse.IsSuccess && userCallResponse.IsSuccess
                && taskCallResponse.IsSuccess && templateCallResponse.IsSuccess)
            {

                return new APICallResponse<DashboardData>
                {

                    IsSuccess = true,
                    Message = new List<string> { "Data found" },
                    Respose = dashboardData,
                    Status = "Success"
                };
            }
            else
            {
                List<string> errMessages = new List<string>();

                if (!friendCallResponse.IsSuccess)
                    errMessages = friendCallResponse.Message;
                else if (!roleCallResponse.IsSuccess)
                    errMessages = roleCallResponse.Message;
                else if (!userCallResponse.IsSuccess)
                    errMessages = userCallResponse.Message;
                else if (!taskCallResponse.IsSuccess)
                    errMessages = taskCallResponse.Message;
                else
                    errMessages = templateCallResponse.Message;

                return new APICallResponse<DashboardData>
                {
                    IsSuccess = false,
                    Message = errMessages,
                    Respose = dashboardData,
                    Status = "Error"
                };
            }

        }
    }
}
