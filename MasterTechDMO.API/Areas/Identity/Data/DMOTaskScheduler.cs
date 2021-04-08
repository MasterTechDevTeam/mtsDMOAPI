using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Areas.Identity.Data
{
    public class DMOTaskScheduler : CommonField
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ThemeColor { get; set; }
        public bool IsFullDay { get; set; }

        // List of Selected Friend
        public string Attendee { get; set; }

    }
}
