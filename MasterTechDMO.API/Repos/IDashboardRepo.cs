using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using mtsDMO.Context.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Repos
{
    interface IDashboardRepo
    {
        Task<APICallResponse<List<DMOUserFriendList>>> GetFriendListAsync(Guid userId);
        Task<APICallResponse<List<DMOTaskScheduler>>> GetTaskAsync(Guid userId);
        Task<APICallResponse<List<TemplateData>>> GetTemplateAsync(Guid userId);
        Task<APICallResponse<List<DMOUsers>>> GetUsersAsync(Guid userId);
        Task<APICallResponse<List<DMOOrgRoles>>> GetRolesAsync(Guid orgId);
    }
}
