using SendGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tunr.Services
{
	class SendGridEmailService : IEmailService
	{
		public void SendEmail(string fromAddress, string fromName, List<string> toAddresses, string subject, string body)
		{
			var myMessage = new SendGridMessage();
			myMessage.From = new System.Net.Mail.MailAddress("support@tunr.io", "Tunr Support");
			myMessage.AddTo(toAddresses);
			myMessage.Subject = subject;
			myMessage.Text = body;

			var credentials = new NetworkCredential(ConfigurationManager.AppSettings["SendGridUsername"],
				ConfigurationManager.AppSettings["SendGridPassword"]);
			var transportWeb = new Web(credentials);
			transportWeb.Deliver(myMessage);
		}
	}
}
