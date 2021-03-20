using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using mtsDMO.Context.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Repos
{
    public interface IEditorRepo
    {
        Task<APICallResponse<List<TemplateData>>> GetAllTemplateAsync(Guid userId);
        Task<APICallResponse<TemplateData>> GetTemplateDetailsAsync(Guid templateId);
        Task<APICallResponse<bool>> RemoveTemplateAsync(Guid templateId);
        Task<APICallResponse<bool>> AddOrUpdateTemplateAsync(DMOTemplateData templateData);
    }
}
