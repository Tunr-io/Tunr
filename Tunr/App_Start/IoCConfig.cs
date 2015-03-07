using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyIoC;
using Tunr.Services;

namespace Tunr.App_Start
{
	public static class IoCConfig
	{
		public static void Register()
		{
			var container = TinyIoCContainer.Current;

			container.Register<IEmailService, SendGridEmailService>();
			container.Register<INewsletterService, MailChimpNewsletterService>();
		}
	}
}
