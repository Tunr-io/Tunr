using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Tunr.Models
{
	public class AlphaToken
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid AlphaTokenId { get; set; }
		public DateTimeOffset CreatedTime { get; set; }
		public DateTimeOffset? UsedTime { get; set; }
		public TunrUser User { get; set; }
	}
}