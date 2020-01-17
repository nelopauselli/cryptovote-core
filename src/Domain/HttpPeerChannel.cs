using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Domain.Converters;
using Domain.Elections;
using Domain.Utils;
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

		public Peer GetNodeInfo(string publicUrl)
		{
			var url = new Uri(new Uri(publicUrl), "api/node");
			return Get<Peer>(url);
		}

		public IList<Peer> ListPeers(string publicUrl)
		{
			var url = new Uri(new Uri(publicUrl), "api/peer");
			return Get<Peer[]>(url) ?? Array.Empty<Peer>();
		}


		public Block GetBlock(string publicUrl, byte[] hash)
		{
			var url = new Uri(new Uri(publicUrl), $"api/chain/{Base58.Encode(hash)}");
			return Get<Block>(url);
		}

		public Block GetLastBlock(string publicUrl)
		{
			var url = new Uri(new Uri(publicUrl), "api/chain");
			return Get<Block>(url);
		}

		public void Send(string publicUrl, Block block)
		{
			var url = new Uri(new Uri(publicUrl), "api/chain");
			Post(url, JsonSerializer.Serialize(block, JsonDefaultSettings.Options));
		}

		public void Send(string publicUrl, Community community)
		{
			var url = new Uri(new Uri(publicUrl), "api/community");
			Post(url, JsonSerializer.Serialize(community, JsonDefaultSettings.Options));
		}

		public void Send(string publicUrl, Question question)
		{
			var url = new Uri(new Uri(publicUrl), "api/question");
			Post(url, JsonSerializer.Serialize(question, JsonDefaultSettings.Options));
		}

		public void Send(string publicUrl, Member member)
		{
			var url = new Uri(new Uri(publicUrl), "api/member");
			Post(url, JsonSerializer.Serialize(member, JsonDefaultSettings.Options));
		}

		public void Send(string publicUrl, Document document)
		{
			var url = new Uri(new Uri(publicUrl), "api/document");
			Post(url, JsonSerializer.Serialize(document, JsonDefaultSettings.Options));
		}

		public void Send(string publicUrl, Vote vote)
		{
			var url = new Uri(new Uri(publicUrl), "api/vote");
			Post(url, JsonSerializer.Serialize(vote, JsonDefaultSettings.Options));
		}

		public void Send(string publicUrl, Fiscal fiscal)
		{
			var url = new Uri(new Uri(publicUrl), "api/fiscal");
			Post(url, JsonSerializer.Serialize(fiscal, JsonDefaultSettings.Options));
		}

		public void Send(string publicUrl, Urn urn)
		{
			var url = new Uri(new Uri(publicUrl), "api/urn");
			Post(url, JsonSerializer.Serialize(urn, JsonDefaultSettings.Options));
		}

		public void Send(string publicUrl, Recount recount)
		{
			var url = new Uri(new Uri(publicUrl), "api/recount");
			Post(url, JsonSerializer.Serialize(recount, JsonDefaultSettings.Options));
		}

		
		public void Send(string publicUrl, Peer peer)
		{
			var url = new Uri(new Uri(publicUrl), "api/peer");
			Post(url, JsonSerializer.Serialize(peer, JsonDefaultSettings.Options));
		}

		private T Get<T>(Uri url) where T : class
		{
			try
			{
				using (var client = new HttpClient())
				{
					var task = client.GetAsync(url);
					task.Wait(HttpDefaultTimeout);
					if (task.IsCompleted)
					{
						var result = task.Result;
						result.EnsureSuccessStatusCode();

						var bodyTask = result.Content.ReadAsStringAsync();
						bodyTask.Wait();

						var response = JsonSerializer.Deserialize<T>(bodyTask.Result, JsonDefaultSettings.Options);
						return response;
					}
				}
			}
			catch (Exception ex)
			{
				logger.LogCritical($"Error al acceder a {url}: {ex}");
			}

			return null;
		}

		private void Post(Uri url, string body)
		{
			try
			{
				using (var client = new HttpClient())
				{
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					var content = new StringContent(body, Encoding.UTF8, "application/json");
					var task = client.PostAsync(url, content);
					task.Wait(HttpDefaultTimeout);

					var result = task.Result;
					result.EnsureSuccessStatusCode();
				}
			}
			catch (Exception ex)
			{
				logger.LogCritical($"Error al acceder a {url} {ex}");
			}
		}
	}
}