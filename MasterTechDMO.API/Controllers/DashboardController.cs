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

        [HttpGet]
        [Route("getFriends/{userId}")]
        public async Task<IActionResult> GetFriendsAsync(Guid userId)
        {
            return Ok(await _dashboardService.GetFriendListAsync(userId));
        }

        [HttpGet]
        [Route("getTask/{userId}")]
        public async Task<IActionResult> GetTaskAsync(Guid userId)
        {
            return Ok(await _dashboardService.GetTaskAsync(userId));
        }

        [HttpGet]
        [Route("getTemplate/{userId}")]
        public async Task<IActionResult> GetTemplateAsync(Guid userId)
        {
            return Ok(await _dashboardService.GetTemplateAsync(userId));
        }


        [HttpGet]
        [Route("getUsers/{userId}")]
        public async Task<IActionResult> GetAllUsersAsync(Guid userId)
        {
            return Ok(await _dashboardService.GetUsersAsync(userId));
        }

        [HttpGet]
        [Route("getRoles/{userId}")]
        public async Task<IActionResult> GetRolesAsync(Guid userId)
        {
            return Ok(await _dashboardService.GetRolesAsync(userId));
        }

        [HttpGet]
        [Route("getDashboardDetails/{userId}")]
        public async Task<IActionResult> GetDashboardDetailsAsync(Guid userId)
        {
            return Ok(await _dashboardService.GetDashboardDetailsAsync(userId));
        }

    }
}
