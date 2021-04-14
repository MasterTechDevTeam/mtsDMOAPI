using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Helpers;
using MasterTechDMO.API.Models;
using MasterTechDMO.API.Repos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using mtsDMO.Context.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Services
{
    public class TaskSchedulerService
    {
        private ITaskSchedulerRepo _taskSchedulerRepo;
        private UserManager<DMOUsers> _userManager;
        private UtilityServices _utilityServices;
        private IHostingEnvironment _environment;

        public TaskSchedulerService(UserManager<DMOUsers> userManager, MTDMOContext context, IConfiguration configuration, IHostingEnvironment environment)
        {
            _taskSchedulerRepo = new TaskSchedulerRepo(context);
            _userManager = userManager;
            _environment = environment;
            _utilityServices = new UtilityServices(configuration);
        }

        public async Task<APICallResponse<List<SchedulerData>>> GetAllTaskAsync(Guid userId)
        {
            var lstTaskResponse = await _taskSchedulerRepo.GetAllTaskAsync(userId);
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

        public async Task<APICallResponse<SchedulerData>> GetTaskDetailsAsync(Guid taskId)
        {
            var taskResponse = await _taskSchedulerRepo.GetTaskDetailsAsync(taskId);
            if (taskResponse.Respose != null)
            {
                var task = taskResponse.Respose;

                var lstAttendee = new List<Guid>();
                foreach (var contact in task.Attendee.Split(',').ToList())
                {
                    lstAttendee.Add(Guid.Parse(contact));
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

                return new APICallResponse<SchedulerData>
                {
                    IsSuccess = taskResponse.IsSuccess,
                    Message = taskResponse.Message,
                    Respose = schedulerData,
                    Status = taskResponse.Status
                };
            }

            return new APICallResponse<SchedulerData>
            {
                IsSuccess = taskResponse.IsSuccess,
                Message = taskResponse.Message,
                Respose = null,
                Status = taskResponse.Status
            };

        }

        public async Task<APICallResponse<bool>> RemoveTaskAsync(Guid taskId)
        {
            return await _taskSchedulerRepo.RemoveTaskAsync(taskId);
        }

        public async Task<APICallResponse<bool>> AddUpdateTaskAsync(SchedulerData schedulerData)
        {
            var data = new DMOTaskScheduler
            {
                Id = schedulerData.EventID,
                Description = schedulerData.Description,
                EndDate = schedulerData.EndDate,
                Attendee = string.Join(',', schedulerData.Attendee),
                InsUser = schedulerData.UserId,
                IsFullDay = schedulerData.IsFullDay,
                StartDate = schedulerData.StartDate,
                Subject = schedulerData.Subject,
                ThemeColor = schedulerData.ThemeColor,
                UserId = schedulerData.UserId
            };


            var res = await _taskSchedulerRepo.AddUpdateTaskAsync(data);

            if (res.IsSuccess)
            {
                var mailData = new MailData();

                mailData.From = _userManager.FindByIdAsync(schedulerData.UserId.ToString()).Result?.Email;

                mailData.Cc.Add(mailData.From);

                foreach (var attendeeId in schedulerData.Attendee)
                {
                    mailData.To.Add(_userManager.FindByIdAsync(attendeeId.ToString()).Result?.Email);
                }

                mailData.Subject = schedulerData.Subject;

                string htmlTemplate = File.ReadAllText(Path.Combine(_environment.WebRootPath, "Templates", "SchedulerTemplate.html"));

                htmlTemplate = htmlTemplate.Replace("<% CREATOR_NAME %>", mailData.From);
                htmlTemplate = htmlTemplate.Replace("<% Name %>", schedulerData.Subject);
                htmlTemplate = htmlTemplate.Replace("<% FROM %>", schedulerData.StartDate.ToString("MM/dd/yyyy hh:mm tt"));
                htmlTemplate = htmlTemplate.Replace("<% Details %>", schedulerData.Description);
                if (schedulerData.IsFullDay)
                    htmlTemplate = htmlTemplate.Replace("<% TO %>", schedulerData.StartDate.ToString("MM/dd/yyyy hh:mm tt"));
                else
                    htmlTemplate = htmlTemplate.Replace("<% TO %>", schedulerData.EndDate.Value.ToString("MM/dd/yyyy hh:mm tt"));

                mailData.Message = htmlTemplate;

                _utilityServices.SendMailAsync(mailData);
            }
            return res;
        }

        public async Task<APICallResponse<List<NotificationDetails>>> GetNotificationsAsync(Guid userId)
        {
            try
            {
                var callResponse = await GetAllTaskAsync(userId);
                if (callResponse.Respose != null)
                {
                    var currentDT = DateTime.Now;
                    var data = callResponse.Respose.Where(x => (x.EndDate.HasValue && x.EndDate.Value >= RepoHelpers.ConvertDateTime(currentDT)) || x.IsFullDay == true).ToList();
                    var lstNotifications = new List<NotificationDetails>();

                    foreach (var task in data)
                    {
                        var notification = new NotificationDetails();
                        var userInfo = _userManager.FindByIdAsync(userId.ToString()).Result;
                        notification.Creator = $"{userInfo.FirstName} {userInfo.LastName}";
                        notification.Description = task.Description;
                        if (task.EndDate != null)
                            notification.EndTime = task.EndDate.Value.ToString("MM/dd/yyyy hh:mm tt");
                        notification.StartTime = task.StartDate.ToString("MM/dd/yyyy hh:mm tt");
                        notification.Title = task.Subject;
                        lstNotifications.Add(notification);
                    }

                    return new APICallResponse<List<NotificationDetails>>
                    {
                        IsSuccess = true,
                        Message = new List<string>() { "Notification Found" },
                        Respose = lstNotifications,
                        Status = "Success"
                    };
                }
                else
                {
                    return new APICallResponse<List<NotificationDetails>>
                    {
                        IsSuccess = callResponse.IsSuccess,
                        Message = callResponse.Message,
                        Respose = null,
                        Status = callResponse.Status
                    };
                }
            }
            catch (Exception Ex)
            {
                return new APICallResponse<List<NotificationDetails>>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Respose = null,
                    Status = "Error"
                };
            }
        }
    }
}
