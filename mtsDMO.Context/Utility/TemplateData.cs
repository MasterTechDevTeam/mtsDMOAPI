using System;
using System.Collections.Generic;
using System.Text;

namespace mtsDMO.Context.Utility
{
    public class TemplateData
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string HtmlContent { get; set; }
        public string ThumbnailIamgePath { get; set; }
    }
}
