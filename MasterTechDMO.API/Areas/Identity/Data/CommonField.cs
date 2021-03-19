using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Areas.Identity.Data
{
    public class CommonField
    {
        public Guid InsUser { get; set; }
        public Guid UpdUser { get; set; }
        public DateTime InsDT { get; set; }
        public DateTime UpdDT { get; set; }
    }
}
