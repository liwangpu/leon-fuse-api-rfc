using ApiServer.Models;
using ApiServer.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ApiServer.Controllers.Asset
{
    [Route("/[controller]")]
    public class MailController : Controller
    {
        public AppConfig appConfig { get; }

        #region 构造函数
        public MailController(IOptions<AppConfig> settingsOptions)
        {
            appConfig = settingsOptions.Value;
        }
        #endregion

        #region Message 用户留言
        /// <summary>
        /// 用户留言
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Message")]
        [HttpPost]
        public IActionResult Message([FromBody]LeaveMessageModel model)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(appConfig.SMTPSettings.NickName, appConfig.SMTPSettings.Account));
            message.To.Add(new MailboxAddress(appConfig.MessageMail, appConfig.MessageMail));
            message.Subject = model.Subject;

            message.Body = new TextPart("plain")
            {
                Text = model.Content
            };

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(appConfig.SMTPSettings.Hosts, appConfig.SMTPSettings.Port, false);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(appConfig.SMTPSettings.Account, appConfig.SMTPSettings.Password);

                client.Send(message);
                client.Disconnect(true);
                return Ok();
            }
        } 
        #endregion
    }
}