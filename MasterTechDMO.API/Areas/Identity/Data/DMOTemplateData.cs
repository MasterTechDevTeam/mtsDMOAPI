using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Areas.Identity.Data
{
    public class DMOTemplateData : CommonField
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string HtmlContent { get; set; }
        public string ThumbnailIamgePath { get; set; }
        public bool IsRemoved { get; set; }
    }
}
