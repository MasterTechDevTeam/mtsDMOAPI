using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using MasterTechDMO.API.Repos;
using mtsDMO.Context.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Services
{
    public class TaskSchedulerService
    {
        private ITaskSchedulerRepo _taskSchedulerRepo;

        public TaskSchedulerService(MTDMOContext context)
        {
            _taskSchedulerRepo = new TaskSchedulerRepo(context);
        }

        public async Task<APICallResponse<List<SchedulerData>>> GetAllTaskAsync(Guid userId)
        {
            var lstTaskResponse = await _taskSchedulerRepo.GetAllTaskAsync(userId);
            if (lstTaskResponse.Respose != null)
            {
                List<SchedulerData> lstSchedulers = new List<SchedulerData>();

                foreach (var task in lstTaskResponse.Respose)
                {
                    var schedulerData = new SchedulerData();
                    schedulerData.Description = task.Description;
                    schedulerData.EndDate = task.EndDate;
                    schedulerData.EventID = task.Id;
                    schedulerData.IsFullDay = task.IsFullDay;
                    schedulerData.StartDate = task.StartDate;
                    schedulerData.Subject = task.Subject;
                    schedulerData.ThemeColor = task.ThemeColor;

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

                var schedulerData = new SchedulerData();
                schedulerData.Description = task.Description;
                schedulerData.EndDate = task.EndDate;
                schedulerData.EventID = task.Id;
                schedulerData.IsFullDay = task.IsFullDay;
                schedulerData.StartDate = task.StartDate;
                schedulerData.Subject = task.Subject;
                schedulerData.ThemeColor = task.ThemeColor;


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
                Attendee = string.Empty,
                InsUser = schedulerData.UserId,
                IsFullDay = schedulerData.IsFullDay,
                StartDate = schedulerData.StartDate,
                Subject = schedulerData.Subject,
                ThemeColor = schedulerData.ThemeColor,
            };
            return await _taskSchedulerRepo.AddUpdateTaskAsync(data);

        }
    }
}
