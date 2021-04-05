using System;
using System.Collections.Generic;
using System.Text;

namespace mtsDMO.Context.Utility
{
    public class SchedulerData
    {
        public Guid EventID { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ThemeColor { get; set; }
        public bool IsFullDay { get; set; }
    }
}
