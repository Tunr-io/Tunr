using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tunr.Models
{
	/// <summary>
	/// This class keeps track of changes to the user's library.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class TableChangeSet : TableEntity
	{
		public enum ChangeType
		{
			Create = 0,
			Update = 1,
			Delete = 2
		}

		/// <summary>
		/// I set the rowkey
		/// </summary>
		[JsonProperty]
		[IgnoreProperty]
		public Guid ChangeSetId
		{
			get
			{
				return Guid.Parse(this.RowKey);
			}
			set
			{
				this.RowKey = value.ToString();
			}
		}

		/// <summary>
		/// I set the PartitionKey
		/// </summary>
		[JsonProperty]
		[IgnoreProperty]
		public Guid OwnerId
		{
			get
			{
				return Guid.Parse(this.PartitionKey);
			}
			set
			{
				this.PartitionKey = value.ToString();
			}
		}

		/// <summary>
		/// A list of changes that occur in this particular changeset.
		/// NOTE: each field is limited to 64KB.
		/// It is recommended that you not store more than 1000 changes here.
		/// </summary>
		[JsonProperty]
		[IgnoreProperty]
		public List<KeyValuePair<ChangeType, Guid>> Changes { get; set; }

		/// <summary>
		/// Time this changeset was last modified.
		/// </summary>
		[JsonProperty]
		[IgnoreProperty]
		public DateTimeOffset LastModifiedTime { get; set; }

		/// <summary>
		/// Determines whether this changeset is new, and can still be added to.
		/// </summary>
		[JsonProperty]
		public bool IsFresh { get; set; }

		public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
		{
			var results = base.WriteEntity(operationContext);
			// Serialize unsupported values to JSON
			results.Add("Changes", new EntityProperty(JsonConvert.SerializeObject(this.Changes)));
			results.Add("LastModifiedTime", new EntityProperty(JsonConvert.SerializeObject(this.LastModifiedTime)));
			return results;
		}

		public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
		{
			base.ReadEntity(properties, operationContext);
			// Deserialize values from JSON
			this.Changes = JsonConvert.DeserializeObject<List<KeyValuePair<ChangeType,Guid>>>(properties["Changes"].StringValue);
			this.LastModifiedTime = JsonConvert.DeserializeObject<DateTimeOffset>(properties["LastModifiedTime"].StringValue);
		}
	}
}