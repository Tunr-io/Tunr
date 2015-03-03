using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Hosting;

public class StreamHandler : IHttpHandler
{
	/// <summary>
	/// You will need to configure this handler in the Web.config file of your 
	/// web and register it with IIS before being able to use it. For more information
	/// see the following link: http://go.microsoft.com/?linkid=8101007
	/// </summary>
	#region IHttpHandler Members

	public bool IsReusable
	{
		// Return false in case your Managed Handler cannot be reused for another request.
		// Usually this would be false in case you have some state information preserved per request.
		get { return false; }
	}

	void ffmpeg_ErrorDataReceived(object sender, DataReceivedEventArgs e)
	{
		System.Diagnostics.Debug.WriteLine(e.Data);
	}

	public void ProcessRequest(HttpContext context)
	{
		HttpRequest request = context.Request;
		HttpResponse response = context.Response;

		string songid = request.Path.Substring(request.Path.LastIndexOf("/")+1);
		System.Diagnostics.Debug.WriteLine("Song stream id: " + songid);

		Process ffmpeg = new Process();
		ProcessStartInfo startinfo = new ProcessStartInfo(HostingEnvironment.MapPath("~/App_Data/executables/ffmpeg.exe"), "-i - -vn -ar 44100 -ac 2 -ab 192k -f mp3 - ");
		startinfo.RedirectStandardError = true;
		startinfo.RedirectStandardOutput = true;
		startinfo.RedirectStandardInput = true;
		startinfo.UseShellExecute = false;
		startinfo.CreateNoWindow = true;
		ffmpeg.StartInfo = startinfo;
		ffmpeg.ErrorDataReceived += ffmpeg_ErrorDataReceived;

		//ffmpeg.BeginOutputReadLine();

		// Our response is a stream
		response.StatusCode = 200;

		//response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "track.mp3" };

		// Retrieve storage account from connection string.
		CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
			CloudConfigurationManager.GetSetting("StorageConnectionString"));

		// Create the blob client.
		CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

		// Retrieve reference to a previously created container.
		CloudBlobContainer container = blobClient.GetContainerReference("uploads");

		// Retrieve reference to a blob
		CloudBlockBlob blockBlob = container.GetBlockBlobReference(songid.ToString());

		ffmpeg.Start();
#if DEBUG
		ffmpeg.BeginErrorReadLine();
#endif

		// Buffer the streams (don't cross them HUR HURR)
		var ffmpegBufferedIn = new BufferedStream(ffmpeg.StandardInput.BaseStream);
		var ffmpegBufferedOut = new BufferedStream(ffmpeg.StandardOutput.BaseStream);

		System.Diagnostics.Debug.WriteLine("Opening Blob Stream.");
		blockBlob.DownloadToStreamAsync(ffmpegBufferedIn, null, new BlobRequestOptions() { ServerTimeout = TimeSpan.FromMinutes(10), MaximumExecutionTime = TimeSpan.FromMinutes(10) }, null).ContinueWith((t) =>
		{
			ffmpegBufferedIn.Flush();
			ffmpegBufferedIn.Close();
		});

		
		response.AddHeader("Connection", "close");
		response.AddHeader("Content-Type", "audio/mpeg");
		//response.AddHeader("Content-Length", "*");
		response.AddHeader("Content-Transfer-Encoding", "binary");

		response.BufferOutput = false;

		byte[] buf = new byte[1024*32];
		int read;
		do
		{
			try
			{
				read = ffmpegBufferedOut.Read(buf, 0, buf.Length);
				response.OutputStream.Write(buf, 0, read);
				response.OutputStream.Flush();
				if (!response.IsClientConnected)
				{
					System.Diagnostics.Debug.WriteLine("Client is no longer connected.");
					ffmpeg.Kill();
					break;
				}
			}
			catch (Exception)
			{
				System.Diagnostics.Debug.WriteLine("Stream terminated early.");
				ffmpeg.Kill();
				break;
			}
		} while (read > 0);

		response.Close();
		ffmpeg.Close();
	}

	#endregion
}