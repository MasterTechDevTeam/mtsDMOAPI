using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mtsDMO.Context.Utility;

namespace MasterTechDMO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaskSchedulerController : ControllerBase
    {
        private TaskSchedulerService _taskSchedulerService;

        public TaskSchedulerController(MTDMOContext context, UserManager<DMOUsers> userManager, IConfiguration configuration, IHostingEnvironment environmen)
        {
            _taskSchedulerService = new TaskSchedulerService( userManager, context,configuration,environmen);
        }

        /// <summary>
        /// Return the list of all scheduled task
        /// </summary>
        /// <param name="userId">userId of User</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("getAllTask/{userId}")]
        public async Task<IActionResult> GetAllTaskAsync(Guid userId)
        {
            return Ok(await _taskSchedulerService.GetAllTaskAsync(userId));
        }

        /// <summary>
        /// Return the details of task
        /// </summary>
        /// <param name="taskId">taskId of requested task</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("getTaskDetails/{taskId}")]
        public async Task<IActionResult> GetTaskDetailsAsync(Guid taskId)
        {
            return Ok(await _taskSchedulerService.GetTaskDetailsAsync(taskId));
        }

        /// <summary>
        /// Remove the scheduled task for the user
        /// </summary>
        /// <param name="taskId">taskId of task</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("removeTask/{taskId}")]
        public async Task<IActionResult> RemoveTaskAsync(Guid taskId)
        {
            return Ok(await _taskSchedulerService.RemoveTaskAsync(taskId));
        }

        /// <summary>
        /// Scheduled the task for the user
        /// </summary>
        /// <param name="taskData">Object of SchedulerData Type</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpPost]
        [Route("addTask")]
        public async Task<IActionResult> AddTaskAsync(SchedulerData taskData)
        {
            return Ok(await _taskSchedulerService.AddUpdateTaskAsync(taskData));
        }

        /// <summary>
        /// Return list of all the task in which user is maintion
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("getNotification/{userId}")]
        public async Task<IActionResult> UpdateTemplateAsync(Guid userId)
        {
            return Ok(await _taskSchedulerService.GetNotificationsAsync(userId));
        }
    }
}
