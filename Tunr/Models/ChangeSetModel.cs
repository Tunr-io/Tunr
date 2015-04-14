using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tunr.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ChangeSetModel
    {
        [JsonProperty("changeSetId")]
        [Key]
        public Guid ChangeSetId { get; set; }

        public TunrUser Owner { get; set; }

        [JsonProperty("changes")]
        public virtual ICollection<ChangeModel> Changes { get; set; }

        /// <summary>
		/// Time this changeset was last modified.
		/// </summary>
        [JsonProperty("lastModifiedTime")]
        public DateTimeOffset LastModifiedTime { get; set; }

        /// <summary>
        /// Determines whether this changeset is new, and can still be added to.
        /// </summary>
        [JsonProperty("IsFresh")]
        public bool IsFresh { get; set; }
    }
}