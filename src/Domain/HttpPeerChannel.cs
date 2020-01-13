using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using Domain.Converters;
using Domain.Elections;
using Microsoft.Extensions.Logging;

namespace Domain
{
	public class HttpPeerChannel : IPeerChannel
	{
		private readonly ILogger<HttpPeerChannel> logger;

		public HttpPeerChannel(ILogger<HttpPeerChannel> logger)
		{
			this.logger = logger;
		}

		private const int HttpDefaultTimeout = 2000;

		public void Connect(string myPublicUrl, string targetPublicUrl)
		{
			// TODO: POST a $"{publicUrl}/api/peers/" => {myPublicUrl, targetPublicUrl}
			throw new System.NotImplementedException();
		}

		public IList<PeerInfo> ListPeers(string publicUrl)
		{
			try
			{
				// TODO: GET a $"{publicUrl}/api/peers/"
				using (var client = new HttpClient())
				{
					var url = new Uri(new Uri(publicUrl), "api/peers");
					var task = client.GetAsync(url);
					task.Wait(HttpDefaultTimeout);
					if (task.IsCompleted)
					{
						var result = task.Result;
						result.EnsureSuccessStatusCode();

						var bodyTask = result.Content.ReadAsByteArrayAsync();
						bodyTask.Wait();

						var peers = JsonSerializer.Deserialize<PeerInfo[]>(bodyTask.Result, JsonDefaultSettings.Options);
						return peers;
					}
				}
			}
			catch (Exception ex)
			{
				logger.LogCritical($"Error al acceder a {publicUrl}: {ex}");
			}
			return Array.Empty<PeerInfo>();
		}

		public Block GetBlock(string publicUrl, byte[] hash)
		{
			// TODO: GET a $"{publicUrl}/api/chain/{Base58.Encode(hash)}"
			throw new System.NotImplementedException();
		}

		public void Send(string publicUrl, Block block)
		{
			var url = new Uri(new Uri(publicUrl), "api/chain");
			Send(url, JsonSerializer.Serialize(block, JsonDefaultSettings.Options));
		}

		private void Send(Uri url, string body)
		{
			try
			{
				using (var client = new HttpClient())
				{
					var content=new StringContent(body);
					var task = client.PostAsync(url, content);
					task.Wait(HttpDefaultTimeout);
					if (task.IsCompleted)
					{
						var result = task.Result;
						result.EnsureSuccessStatusCode();
					}
				}
			}
			catch (Exception ex)
			{
				logger.LogCritical($"Error al acceder a {url} {ex}");
			}
		}

		public void Send(string publicUrl, Community community)
		{
			var url = new Uri(new Uri(publicUrl), "api/community");
			Send(url, JsonSerializer.Serialize(community, JsonDefaultSettings.Options));
		}

		public void Send(string publicUrl, Question question)
		{
			var url = new Uri(new Uri(publicUrl), "api/question");
			Send(url, JsonSerializer.Serialize(question, JsonDefaultSettings.Options));
		}

		public void Send(string publicUrl, Member member)
		{
			var url = new Uri(new Uri(publicUrl), "api/member");
			Send(url, JsonSerializer.Serialize(member, JsonDefaultSettings.Options));
		}

		public void Send(string publicUrl, Document document)
		{
			var url = new Uri(new Uri(publicUrl), "api/document");
			Send(url, JsonSerializer.Serialize(document, JsonDefaultSettings.Options));
		}

		public void Send(string publicUrl, Vote vote)
		{
			var url = new Uri(new Uri(publicUrl), "api/vote");
			Send(url, JsonSerializer.Serialize(vote, JsonDefaultSettings.Options));
		}

		public void Send(string publicUrl, Fiscal fiscal)
		{
			var url = new Uri(new Uri(publicUrl), "api/fiscal");
			Send(url, JsonSerializer.Serialize(fiscal, JsonDefaultSettings.Options));
		}

		public void Send(string publicUrl, Urn urn)
		{
			var url = new Uri(new Uri(publicUrl), "api/urn");
			Send(url, JsonSerializer.Serialize(urn, JsonDefaultSettings.Options));
		}

		public void Send(string publicUrl, Recount recount)
		{
			var url = new Uri(new Uri(publicUrl), "api/recount");
			Send(url, JsonSerializer.Serialize(recount, JsonDefaultSettings.Options));
		}

		public Block GetLastBlock(string publicUrl)
		{
			// TODO: GET a $"{publicUrl}/api/chain/"
			throw new System.NotImplementedException();
		}
	}
}