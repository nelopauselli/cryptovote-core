using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Domain.Channels;
using Domain.Channels.Protocol;
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
		public NodeAdapterQuestions Questions => new NodeAdapterQuestions(this);
		public NodeAdapterMembers Members => new NodeAdapterMembers(this);
		public NodeAdapterVotes Votes => new NodeAdapterVotes(this);
		public NodeAdapterUrns Urns => new NodeAdapterUrns(this);
		public NodeAdapterFiscals Fiscals => new NodeAdapterFiscals(this);
		public NodeAdapterRecounts Recounts => new NodeAdapterRecounts(this);

		public async Task Send(ICommand command)
		{
			Console.WriteLine($"Conectando con {Host}:{Port}");
			using (var client = new TcpClient())
			{
				await client.ConnectAsync(Host, Port);

				if (client.Connected)
				{
					using (var stream = client.GetStream())
					{
						command.Send(stream);
					}
				}
			}
		}

		public async Task<T> GetResponse<T>(ICommand<T> command) where T : class
		{
			Console.WriteLine($"Conectando con {Host}:{Port}");
			using (var client = new TcpClient())
			{
				await client.ConnectAsync(Host, Port);

				if (client.Connected)
				{
					using (var stream = client.GetStream())
					{
						command.Send(stream);
						var header = CommandHeader.Parse(stream);
						return command.Parse(stream, header.Length);
					}
				}

				return null;
			}
		}

		public async Task<T> GetResponse<T>(IMessage<T> message) where T : class
		{
			using (var client = new TcpClient())
			{
				await client.ConnectAsync(Host, Port);

				if (client.Connected)
				{
					using (var stream = client.GetStream())
					{
						var data = message.GetBytes();

						CancellationToken ctsToken;
						stream.Write(data, 0, data.Length);
						stream.Flush();

						if (!ctsToken.IsCancellationRequested)
							return message.Parse(new ProtocolMessageChannel(stream));
					}

				}
				return null;
			}
		}
	}
}