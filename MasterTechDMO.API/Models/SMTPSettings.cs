using System;
using System.Collections.Generic;
using System.Text;

namespace MasterTechDMO.API.Models
{
    public class SMTPSettings
    {
        public string SMTP { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string DisplayName { get; set; }
    }
}
