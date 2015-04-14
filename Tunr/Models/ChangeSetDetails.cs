using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tunr.Models
{
    /// <summary>
	/// This object is returned to clients who request a list of changes
	/// from a certain changeset forward.
	/// </summary>
	public class ChangeSetDetails
    {
        public Guid ChangeSetId { get; set; }

        public List<KeyValuePair<TableChangeSet.ChangeType, TableSong>> Changes { get; set; }

        public DateTimeOffset LastModifiedTime { get; set; }
    }
}