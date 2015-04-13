using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tunr.Models
{
    public class SyncBaseModel
    {
        public Guid? LastSyncId { get; set; }
        public IEnumerable<Song> Library { get; set; }
    }
}