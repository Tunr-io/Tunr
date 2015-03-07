using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tunr.Services
{
	interface IEmailService
	{
		void SendEmail(string fromAddress, string fromName, List<string> toAddresses, string subject, string body);
	}
}
