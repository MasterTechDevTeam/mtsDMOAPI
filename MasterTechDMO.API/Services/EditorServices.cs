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
    public class EditorServices
    {
        private IEditorRepo _editorRepo;
        public EditorServices(MTDMOContext context)
        {
            _editorRepo = new EditorRepo(context);
        }

        public async Task<APICallResponse<List<TemplateData>>> GetAllTemplateAsync(Guid userId)
        {
            return await _editorRepo.GetAllTemplateAsync(userId);
        }

        public async Task<APICallResponse<TemplateData>> GetTemplateDetailsAsync(Guid templateId)
        {
            return await _editorRepo.GetTemplateDetailsAsync(templateId);

        }

        public async Task<APICallResponse<bool>> RemoveTemplateAsync(Guid templateId)
        {
            return await _editorRepo.RemoveTemplateAsync(templateId);
        }

        public async Task<APICallResponse<bool>> AddOrUpdateTemplateAsync(TemplateData templateData)
        {
            var dmoTemplateData = new DMOTemplateData { 
                Id = templateData.Id,
                HtmlContent = templateData.HtmlContent,
                ThumbnailIamgePath= templateData.ThumbnailIamgePath,
                UserId = templateData.UserId
            };
            return await _editorRepo.AddOrUpdateTemplateAsync(dmoTemplateData);

        }
    }
}
