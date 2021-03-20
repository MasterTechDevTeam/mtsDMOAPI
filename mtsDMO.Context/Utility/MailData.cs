using System;
using System.Collections.Generic;
using System.Text;

namespace mtsDMO.Context.Utility
{
    public class MailData
    {
        public MailData()
        {
            Bcc = new List<string>();
            To = new List<string>();
            Cc = new List<string>();
            AttachmentItemPath = new List<string>();
        }
        public string From { get; set; }
        public List<string> Bcc { get; set; }
        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> AttachmentItemPath { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
