using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Repos
{
    public class TaskSchedulerRepo : ITaskSchedulerRepo
    {
        private MTDMOContext _context;
        public TaskSchedulerRepo(MTDMOContext context)
        {
            _context = context;
        }

        public async Task<APICallResponse<bool>> AddUpdateTaskAsync(DMOTaskScheduler taskSchedulerData)
        {
            try
            {
                var callResponse = new APICallResponse<bool>();
                var dbTask = _context.DMOTaskScheduler.Where(x => x.Id == taskSchedulerData.Id).FirstOrDefault();
                if (dbTask != null)
                {
                    dbTask.Attendee = taskSchedulerData.Attendee;
                    dbTask.Description = taskSchedulerData.Description;
                    dbTask.EndDate = taskSchedulerData.EndDate;
                    dbTask.IsFullDay = taskSchedulerData.IsFullDay;
                    dbTask.StartDate = taskSchedulerData.StartDate;
                    dbTask.Subject = taskSchedulerData.Subject;
                    dbTask.ThemeColor = taskSchedulerData.ThemeColor;
                    dbTask.UpdUser = taskSchedulerData.InsUser;
                    dbTask.UpdDT = DateTime.Now;

                    callResponse.Message = new List<string> { "Task Updated" };
                    callResponse.Respose = true;
                    callResponse.Status = "Success";
                }
                else
                {
                    _context.DMOTaskScheduler.Add(taskSchedulerData);

                    callResponse.Message = new List<string> { "Task Addded" };
                    callResponse.Respose = true;
                    callResponse.Status = "Success";
                }
                _context.SaveChanges();
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

        public async Task<APICallResponse<List<DMOTaskScheduler>>> GetAllTaskAsync(Guid userId)
        {
            try
            {
                var callResponse = new APICallResponse<List<DMOTaskScheduler>>();
                var dbTaskList = _context.DMOTaskScheduler.Where(x => x.UserId == userId || x.Attendee.Contains(userId.ToString())).ToList();
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

        public async Task<APICallResponse<DMOTaskScheduler>> GetTaskDetailsAsync(Guid taskId)
        {
            try
            {
                var callResponse = new APICallResponse<DMOTaskScheduler>();
                var dbTask = _context.DMOTaskScheduler.Where(x => x.Id == taskId).FirstOrDefault();
                if (dbTask != null)
                {
                    callResponse.Message = new List<string> { $"Task found" };
                    callResponse.Respose = dbTask;
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
                return new APICallResponse<DMOTaskScheduler>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = null
                };
            }
        }

        public async Task<APICallResponse<bool>> RemoveTaskAsync(Guid taskId)
        {
            try
            {
                var callResponse = new APICallResponse<bool>();
                var dbTask = _context.DMOTaskScheduler.Where(x => x.Id == taskId).FirstOrDefault();
                if (dbTask != null)
                {
                    _context.DMOTaskScheduler.Remove(dbTask);
                    _context.SaveChanges();
                    callResponse.Message = new List<string> { "Task Removed" };
                    callResponse.Respose = true;
                    callResponse.Status = "Success";
                }
                else
                {
                    callResponse.Message = new List<string> { "Task not found" };
                    callResponse.Respose = true;
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
    }
}
