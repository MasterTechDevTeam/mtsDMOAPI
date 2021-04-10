using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mtsDMO.Context.Utility;

namespace MasterTechDMO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaskSchedulerController : ControllerBase
    {
        private TaskSchedulerService _taskSchedulerService;

        public TaskSchedulerController(MTDMOContext context, UserManager<DMOUsers> userManager)
        {
            _taskSchedulerService = new TaskSchedulerService( userManager, context);
        }

        [HttpGet]
        [Route("getAllTask/{userId}")]
        public async Task<IActionResult> GetAllTaskAsync(Guid userId)
        {
            return Ok(await _taskSchedulerService.GetAllTaskAsync(userId));
        }

        [HttpGet]
        [Route("getTaskDetails/{taskId}")]
        public async Task<IActionResult> GetTaskDetailsAsync(Guid taskId)
        {
            return Ok(await _taskSchedulerService.GetTaskDetailsAsync(taskId));
        }

        [HttpGet]
        [Route("removeTask/{taskId}")]
        public async Task<IActionResult> RemoveTaskAsync(Guid taskId)
        {
            return Ok(await _taskSchedulerService.RemoveTaskAsync(taskId));
        }

        [HttpPost]
        [Route("addTask")]
        public async Task<IActionResult> AddTaskAsync(SchedulerData taskData)
        {
            return Ok(await _taskSchedulerService.AddUpdateTaskAsync(taskData));
        }

        [HttpGet]
        [Route("getNotification/{userId}")]
        public async Task<IActionResult> UpdateTemplateAsync(Guid userId)
        {
            return Ok(await _taskSchedulerService.GetNotificationsAsync(userId));
        }
    }
}
