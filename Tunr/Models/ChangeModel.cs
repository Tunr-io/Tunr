using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tunr.Models
{
    public class ChangeModel
    {
        public enum ChangeType
        {
            Create = 0,
            Update = 1,
            Delete = 2
        }

        [Key]
        public Guid ChangeId { get; set; }

        public Guid SongId { get; set; }

        public ChangeType Type { get; set; }
    }
}