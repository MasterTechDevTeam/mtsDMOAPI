using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Repos
{
    public interface ITaskSchedulerRepo
    {
        Task<APICallResponse<bool>> AddUpdateTaskAsync(DMOTaskScheduler taskSchedulerData);
        Task<APICallResponse<List<DMOTaskScheduler>>> GetAllTaskAsync(Guid userId);
        Task<APICallResponse<DMOTaskScheduler>> GetTaskDetailsAsync(Guid taskId);
        Task<APICallResponse<bool>> RemoveTaskAsync(Guid taskId);
    }
}
