using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using mtsDMO.Context.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Repos
{
    public interface IFriendListRepo
    {
        Task<APICallResponse<List<DMOUserFriendList>>> GetFriendListAsync(Guid userId);

        Task<APICallResponse<DMOUserFriendList>> GetFriendDataByEmailAsync(Guid userId, string friendEmailId);

        Task<APICallResponse<bool>> AddOrUpdateFriendDataAsync(DMOUserFriendList friendData);

        Task<APICallResponse<bool>> RemoveFriendAsync(Guid userId, string username);
    }
}
