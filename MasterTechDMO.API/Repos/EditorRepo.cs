using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using mtsDMO.Context.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Repos
{
    public class EditorRepo : IEditorRepo
    {
        private MTDMOContext _context;
        public EditorRepo(MTDMOContext context)
        {
            _context = context;
        }

        public async Task<APICallResponse<bool>> AddOrUpdateTemplateAsync(DMOTemplateData templateData)
        {
            try
            {
                var callResponse = new APICallResponse<bool>();
                var dbTemplate = _context.DMOTemplateData.Where(x => x.Id == templateData.Id).FirstOrDefault();
                if (dbTemplate != null)
                {
                    dbTemplate.HtmlContent = templateData.HtmlContent;
                    dbTemplate.ThumbnailIamgePath = templateData.ThumbnailIamgePath;
                    dbTemplate.UserId = templateData.UserId;
                    dbTemplate.UpdUser = templateData.UserId;
                    dbTemplate.UpdDT = DateTime.Now;

                    callResponse.Message = new List<string> { "Template Updated" };
                    callResponse.Respose = true;
                    callResponse.Status = "Success";
                }
                else
                {
                    templateData.InsDT = DateTime.Now;
                    templateData.InsUser = templateData.UserId;

                    _context.DMOTemplateData.Add(templateData);

                    callResponse.Message = new List<string> { "Template Addded" };
                    callResponse.Respose = true;
                    callResponse.Status = "Success";
                }
                _context.SaveChanges();
                callResponse.IsSuccess = true;
                return callResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<bool>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = false
                };
            }
        }

        public async Task<APICallResponse<List<TemplateData>>> GetAllTemplateAsync(Guid userId)
        {
            try
            {
                var callResponse = new APICallResponse<List<TemplateData>>();
                var dbTemplateList = _context.DMOTemplateData.Where(x => x.UserId == userId && x.IsRemoved == false).ToList();
                if (dbTemplateList != null)
                {
                    var lstTemplateData = new List<TemplateData>();
                    foreach (var template in dbTemplateList)
                    {
                        lstTemplateData.Add(new TemplateData
                        {
                            HtmlContent = template.HtmlContent,
                            Id = template.Id,
                            ThumbnailIamgePath = template.ThumbnailIamgePath,
                            UserId = template.UserId
                        });
                    }

                    callResponse.Message = new List<string> { $"{lstTemplateData.Count} template(s) found" };
                    callResponse.Respose = lstTemplateData;
                    callResponse.Status = "Success";
                }
                else
                {
                    callResponse.Message = new List<string> { "No template found" };
                    callResponse.Respose = null;
                    callResponse.Status = "Warning";
                }
                callResponse.IsSuccess = true;
                return callResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<List<TemplateData>>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = null
                };
            }
        }

        public async Task<APICallResponse<TemplateData>> GetTemplateDetailsAsync(Guid templateId)
        {
            try
            {
                var callResponse = new APICallResponse<TemplateData>();
                var dbTemplateList = _context.DMOTemplateData.Where(x => x.Id == templateId && x.IsRemoved == false).FirstOrDefault();
                if (dbTemplateList != null)
                {
                    var templateData = new TemplateData
                    {
                        HtmlContent = dbTemplateList.HtmlContent,
                        Id = dbTemplateList.Id,
                        ThumbnailIamgePath = dbTemplateList.ThumbnailIamgePath,
                        UserId = dbTemplateList.UserId
                    };

                    callResponse.Message = new List<string> { $"Template found" };
                    callResponse.Respose = templateData;
                    callResponse.Status = "Success";
                }
                else
                {
                    callResponse.Message = new List<string> { "No template found" };
                    callResponse.Respose = null;
                    callResponse.Status = "Warning";
                }
                callResponse.IsSuccess = true;
                return callResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<TemplateData>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = null
                };
            }
        }

        public async Task<APICallResponse<bool>> RemoveTemplateAsync(Guid templateId)
        {
            try
            {
                var callResponse = new APICallResponse<bool>();
                var dbTemplate = _context.DMOTemplateData.Where(x => x.Id == templateId).FirstOrDefault();
                if (dbTemplate != null)
                {
                    dbTemplate.IsRemoved = true;
                    _context.SaveChanges();
                    callResponse.Message = new List<string> { "Template Removed" };
                    callResponse.Respose = true;
                    callResponse.Status = "Success";
                }
                else
                {
                    callResponse.Message = new List<string> { "Template not found" };
                    callResponse.Respose = true;
                    callResponse.Status = "Warning";
                }
                callResponse.IsSuccess = true;
                return callResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<bool>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = false
                };
            }
        }
    }
}
