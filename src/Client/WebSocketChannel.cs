using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Newtonsoft.Json;

namespace Client
{
	public class WebSocketChannel
	{
		private readonly ClientWebSocket client;
		private Task listenerTask;
		private readonly Uri uri;
		private readonly ILogger logger;

		public WebSocketChannel(Uri uri, ILogger logger)
		{
			this.uri = uri;
			this.logger = logger;
			client = new ClientWebSocket();
		}

		public const int TimeOutMilliseconds = 5000;

		public event EventHandler<Vote> IncommingVote;
		public event EventHandler<Topic> IncommingTopic;
		public event EventHandler<Organization> IncommingOrganization;
		public string State => client?.State.ToString() ?? "Unknown";

		public async Task Connect()
		{
			if (client.State == WebSocketState.Connecting || client.State == WebSocketState.Open)
				return;

			try
			{
				using (var cts = new CancellationTokenSource(TimeOutMilliseconds))
				{
					logger.Info($"Connecting to {uri}");

					await client.ConnectAsync(uri, cts.Token);
					
					logger.Debug($"Client state: {client.State}");
				}

				if (client.State == WebSocketState.Open)
				{
					listenerTask = Listen();
				}
			}
			catch (Exception ex)
			{
				logger.Error("Error conectando con el nodo repetidor", ex);
			}
		}

		public void Dispose()
		{
			logger.Info("Disposing...");
			if (!client.CloseStatus.HasValue)
			{
				using (var cts = new CancellationTokenSource(TimeOutMilliseconds))
				{
					var task = client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cts.Token);
					task.Wait(TimeOutMilliseconds);
				}
			}

			listenerTask?.Dispose();
			client?.Dispose();
		}

		private async Task Listen()
		{
			logger.Info("Listen...");

			var buffer = new byte[1024];
			WebSocketReceiveResult result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			while (!result.CloseStatus.HasValue)
			{
				string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

				logger.Debug(message);
				IncommingMessage(message);

				logger.Debug("Re-listener");
				result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			}

			logger.Info("Closing ws");
			await client.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
		}

		public async Task Send(string message)
		{
			logger.Debug($"Sending: {message}");

			byte[] sendBytes = Encoding.UTF8.GetBytes(message);
			var segmentOut = new ArraySegment<byte>(sendBytes, 0, sendBytes.Length);
			using (var cts = new CancellationTokenSource(TimeOutMilliseconds))
			{
				await client.SendAsync(segmentOut, WebSocketMessageType.Text, true, cts.Token);
				logger.Debug("Send complete");
			}
		}

		private void IncommingMessage(string message)
		{
			var chunks = message.Split('|', 2);
			if (chunks.Length != 2)
			{
				logger.Warning("Mensaje inválido: " + message);
				return;
			}

			switch (chunks[0])
			{
				case "vote":
					var vote = JsonConvert.DeserializeObject<Vote>(chunks[1]);
					IncommingVote?.Invoke(this, vote);
					break;
				case "topic":
					var topic = JsonConvert.DeserializeObject<Topic>(chunks[1]);
					IncommingTopic?.Invoke(this, topic);
					break;
				case "organization":
					var organization = JsonConvert.DeserializeObject<Organization>(chunks[1]);
					IncommingOrganization?.Invoke(this, organization);
					break;
			}

		}

	}
}
