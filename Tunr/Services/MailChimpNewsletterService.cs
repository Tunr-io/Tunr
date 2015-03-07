using MailChimp;
using MailChimp.Helper;
using MailChimp.Lists;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunr.Models;

namespace Tunr.Services
{
	class MailChimpNewsletterService : INewsletterService
	{
		public void AddUser(TunrUser user)
		{
			var mc = new MailChimpManager(ConfigurationManager.AppSettings["MailChimpApiKey"]);
			EmailParameter email = new EmailParameter()
			{
				Email = user.Email
			};
			MergeVar myMergeVars = new MergeVar();
			myMergeVars.Add("DNAME", user.DisplayName);
			myMergeVars.Add("USERID", user.Id);
			EmailParameter results = mc.Subscribe(ConfigurationManager.AppSettings["MailChimpListId"],
				email,
				myMergeVars,
				doubleOptIn: false,
				updateExisting: true);
		}
	}
}
