using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Protocol;
using Domain.Queries;
using Microsoft.Extensions.Configuration;

namespace Web.Controllers
{
	public class NodeAdapter
	{
		private string Host { get; }
		private int Port { get; }

		public NodeAdapter(IConfiguration config)
		{
			var section = config.GetSection("Node");
			if (section == null)
				throw new ApplicationException("Falta configurar el nodo al que conectarse");

			Host = section["Host"];

			if (int.TryParse(section["Port"], out var value))
				Port = value;
		}

		public NodeAdapterBlocks Blocks => new NodeAdapterBlocks(this);
		public NodeAdapterCommunities Communities => new NodeAdapterCommunities(this);
		public NodeAdapterIssues Issues => new NodeAdapterIssues(this);
		public NodeAdapterMembers Members => new NodeAdapterMembers(this);
		public NodeAdapterVotes Votes => new NodeAdapterVotes(this);
		public NodeAdapterUrns Urns => new NodeAdapterUrns(this);
		public NodeAdapterFiscals Fiscals => new NodeAdapterFiscals(this);
		public NodeAdapterRecounts Recounts => new NodeAdapterRecounts(this);

		public async Task<string> GetResponse(byte[] data)
		{
			Console.WriteLine($"Conectando con {Host}:{Port}");
			var client = new TcpClient();
			await client.ConnectAsync(Host, Port);

			string body = null;
			if (client.Connected)
			{
				using (var stream = client.GetStream())
				{
					CancellationToken ctsToken;
					await stream.WriteAsync(data, 0, data.Length, ctsToken);

					stream.Flush();

					if (!ctsToken.IsCancellationRequested)
					{
						byte[] bytesToRead = new byte[client.ReceiveBufferSize];
						int bytesRead = stream.Read(bytesToRead, 0, client.ReceiveBufferSize);
						var response = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);

						var chunks = response.Split('|');
						if (chunks.Length == 2)
							body = chunks[1];
					}
				}

				client.Close();
			}

			return body;
		}

		public async Task<T> GetResponse<T>(IMessage<T> message) where T : class
		{
			var client = new TcpClient();
			await client.ConnectAsync(Host, Port);

			if (client.Connected)
			{
				using (var stream = client.GetStream())
				{
					var data = message.GetBytes();

					CancellationToken ctsToken;
					await stream.WriteAsync(data, 0, data.Length, ctsToken);

					stream.Flush();

					if (!ctsToken.IsCancellationRequested)
					{
						return message.Parse(new ProtocolMessageChannel(stream));
					}
				}

				client.Close();
			}

			return null;
		}
	}
}