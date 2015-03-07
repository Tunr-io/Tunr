using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tunr.Models;

namespace Tunr.Services
{
	interface INewsletterService
	{
		void AddUser(TunrUser user);
	}
}
