using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MasterTechDMO.API.Controllers
{
    /// <summary>
    /// APIs for Dashboard
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DashboardController : ControllerBase
    {
        private DashboardService _dashboardService;

        public DashboardController(MTDMOContext context, IServiceProvider serviceProvider)
        {
            _dashboardService = new DashboardService(context, serviceProvider);
        }

        /// <summary>
        /// Return the list of top 5 friend for dashboard
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("getFriends/{userId}")]
        public async Task<IActionResult> GetFriendsAsync(Guid userId)
        {
            return Ok(await _dashboardService.GetFriendListAsync(userId));
        }


        /// <summary>
        /// Return list of top 5 task
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("getTask/{userId}")]
        public async Task<IActionResult> GetTaskAsync(Guid userId)
        {
            return Ok(await _dashboardService.GetTaskAsync(userId));
        }



        /// <summary>
        /// Return list of top 5 template
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("getTemplate/{userId}")]
        public async Task<IActionResult> GetTemplateAsync(Guid userId)
        {
            return Ok(await _dashboardService.GetTemplateAsync(userId));
        }


        /// <summary>
        /// Return list of top 5 user if requested user is either admin or organization
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("getUsers/{userId}")]
        public async Task<IActionResult> GetAllUsersAsync(Guid userId)
        {
            return Ok(await _dashboardService.GetUsersAsync(userId));
        }



        /// <summary>
        /// Return list of top 5 roles if requested user is either admin or organization
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("getRoles/{userId}")]
        public async Task<IActionResult> GetRolesAsync(Guid userId)
        {
            return Ok(await _dashboardService.GetRolesAsync(userId));
        }


        /// <summary>
        /// Return list of top 5 Roles,User,Task and Template for user in single API call
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("getDashboardDetails/{userId}")]
        public async Task<IActionResult> GetDashboardDetailsAsync(Guid userId)
        {
            return Ok(await _dashboardService.GetDashboardDetailsAsync(userId));
        }

    }
}
