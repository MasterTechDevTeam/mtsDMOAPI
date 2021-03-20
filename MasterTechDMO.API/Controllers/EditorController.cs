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
using Microsoft.Extensions.Configuration;
using mtsDMO.Context.Utility;

namespace MasterTechDMO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EditorController : ControllerBase
    {
        private EditorServices _editorService;
        private IConfiguration _configuration;

        public EditorController(MTDMOContext context, IConfiguration configuration)
        {
            _editorService = new EditorServices(context);
            _configuration = configuration;
        }

        [HttpGet]
        [Route("getAllTemplate/{userId}")]
        public async Task<IActionResult> GetAllTemplateAsync(Guid userId)
        {
            return Ok(await _editorService.GetAllTemplateAsync(userId));

        }

        [HttpGet]
        [Route("getTemplateDetails/{templateId}")]
        public async Task<IActionResult> GetTemplateDetailsAsync(Guid templateId)
        {
            return Ok(await _editorService.GetTemplateDetailsAsync(templateId));

        
        }

        [HttpGet]
        [Route("removeTemplate/{templateId}")]
        public async Task<IActionResult> RemoveTemplateAsync(Guid templateId)
        {
            return Ok(await _editorService.RemoveTemplateAsync(templateId));


        }

        [HttpPost]
        [Route("addTemplate")]
        public async Task<IActionResult> AddTemplateAsync(TemplateData templateData)
        {
            return Ok(await _editorService.AddOrUpdateTemplateAsync(templateData));
        }

        [HttpPost]
        [Route("updateTemplate")]
        public async Task<IActionResult> UpdateTemplateAsync(TemplateData templateData)
        {
            return Ok(await _editorService.AddOrUpdateTemplateAsync(templateData));
        }

    }
}
