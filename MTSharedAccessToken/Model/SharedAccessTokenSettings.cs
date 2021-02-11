using System;
using System.Collections.Generic;
using System.Text;

namespace MTSharedAccessToken.Model
{
    public class SharedAccessTokenSettings
    {
        public string SharedName { get; set; }
        public int ExpTimeInMinute { get; set; }
        public string IssuerName { get; set; }
        public string AudienceName { get; set;}
        public string SharedKey { get; set; }
    }
}
