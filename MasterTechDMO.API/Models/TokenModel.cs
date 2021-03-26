using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Models
{
    public class TokenModel
    {
        public string EmailId { get; set; }
        public string Type { get; set; }
        public DateTime Time { get; set; }
    }
}
