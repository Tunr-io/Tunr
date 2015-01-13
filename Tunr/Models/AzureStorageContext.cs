using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tunr.Models
{
	public class AzureStorageContext
	{
		private CloudStorageAccount _StorageAccount;
		private CloudStorageAccount StorageAccount 
		{ 
			get {
				if (_StorageAccount == null) {
					_StorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
				}
				return _StorageAccount;
			}
		}
		
		private CloudTableClient _TableClient;
		private CloudTableClient  TableClient
		{
			get {
				if (_TableClient == null) {
					_TableClient = StorageAccount.CreateCloudTableClient();
				}
				return _TableClient;
			}
		}

		private CloudTable _SongTable;
		public CloudTable SongTable 
		{
			get {
				if (_SongTable == null) {
					_SongTable = TableClient.GetTableReference("Song");
					_SongTable.CreateIfNotExists();
				}
				return _SongTable;
			}
		}

		private CloudTable _ChangeSetTable;
		public CloudTable ChangeSetTable
		{
			get
			{
				if (_ChangeSetTable == null)
				{
					_ChangeSetTable = TableClient.GetTableReference("ChangeSet");
					_ChangeSetTable.CreateIfNotExists();
				}
				return _ChangeSetTable;
			}
		}

		private CloudBlobClient _BlobClient;
		private CloudBlobClient BlobClient
		{
			get
			{
				if (_BlobClient == null)
				{
					_BlobClient = StorageAccount.CreateCloudBlobClient();
				}
				return _BlobClient;
			}
		}

		private CloudBlobContainer _UploadsContainer;
		public CloudBlobContainer UploadsContainer
		{
			get {
				if (_UploadsContainer == null) {
					_UploadsContainer = BlobClient.GetContainerReference("uploads");
					_UploadsContainer.CreateIfNotExists();
				}
				return _UploadsContainer;
			}
		}
	}
}