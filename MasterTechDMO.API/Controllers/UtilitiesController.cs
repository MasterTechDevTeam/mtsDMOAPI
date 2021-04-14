using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MasterTechDMO.API.Models;
using MasterTechDMO.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mtsDMO.Context.Utility;

namespace MasterTechDMO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UtilitiesController : ControllerBase
    {
        private UtilityServices _utilityService;
        public UtilitiesController(IConfiguration configuration)
        {
            _utilityService = new UtilityServices(configuration);
        }

        [HttpPost]
        [Route("SendMail")]
        public async Task<IActionResult> SendMailAsync(MailData mailData)
        {
            return Ok(await _utilityService.SendMailAsync(mailData));

            //var callResponse = new APICallResponse<bool>();
            //try
            //{
            //    var smtpClient = GetSMTPSettings();

            //    string strMailBody = string.Empty;
            //    strMailBody = mailData.Message;

            //    var mailMessage = new MailMessage
            //    {
            //        From = new MailAddress("sandboxmail@mastertechsolution.com",mailData.From),
            //        Subject = mailData.Subject,
            //        Body = strMailBody,
            //        IsBodyHtml = true,
            //    };

            //    foreach (var to in mailData.To)
            //    {
            //        mailMessage.To.Add(to);
            //    }

            //    foreach (var cc in mailData.Cc)
            //    {
            //        mailMessage.To.Add(cc);
            //    }

            //    foreach (var bcc in mailData.Bcc)
            //    {
            //        mailMessage.To.Add(bcc);
            //    }

            //    Attachment mailAttaachment;
            //    foreach (var attachment in mailData.AttachmentItemPath)
            //    {
            //        mailAttaachment = new Attachment(attachment);
            //        mailMessage.Attachments.Add(mailAttaachment);
            //    }

            //    callResponse.IsSuccess = true;
            //    callResponse.Message = new List<string> { "Mail send." };
            //    callResponse.Respose = true;
            //    callResponse.Status = "Success";

            //    smtpClient.Send(mailMessage);
            //}
            //catch (Exception Ex)
            //{
            //    callResponse.IsSuccess = false;
            //    callResponse.Message = new List<string> { Ex.Message };
            //    callResponse.Respose = false;
            //    callResponse.Status = "Success";
            //}

            //return Ok(callResponse);
        }

        //private SmtpClient GetSMTPSettings()
        //{
        //    SMTPSettings smtpSettings = new SMTPSettings();

        //    _configuration.GetSection("SMTPSettings").Bind(smtpSettings);

        //    return new SmtpClient(smtpSettings.SMTP)
        //    {
        //        Port = smtpSettings.Port,
        //        Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password),
        //        EnableSsl = smtpSettings.EnableSsl,
        //    };
        //}
    }
}
