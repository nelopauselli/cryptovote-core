using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Channels
{
	public class TcpPeer : IPeer
	{
		private readonly IChannel channel;
		private DateTime? disabledTo;
		private TcpClient client;

		public string Host { get; }
		public int Port { get; }

		public TcpPeer(string host, int port, IChannel channel)
		{
			this.channel = channel;
			Host = host;
			Port = port;
			disabledTo = null;
		}

		public string Id => $"{Host}:{Port}";

		public async Task SendAsync(byte[] data, CancellationToken ctsToken)
		{
			try
			{
				client = new TcpClient {SendTimeout = Peers.TimeoutMillicesonds, ReceiveTimeout = Peers.TimeoutMillicesonds};
				await client.ConnectAsync(Host, Port);

				if (client.Connected)
				{
					using (var stream = client.GetStream())
					{
						stream.Write(data, 0, data.Length);
						stream.Flush();

						Thread.Sleep(1000);
						channel.TalkWithClient(stream);
						stream.Flush();

						Thread.Sleep(1000);
					}
				}
			}
			catch (Exception ex)
			{
				disabledTo = DateTime.Now.AddSeconds(10);
				throw new ChannelException($"Error de comunicación con {Host}:{Port}: {ex.Message}", ex);
			}
		}

		public override string ToString()
		{
			return $"{Host}:{Port}";
		}

		public bool IsReady => !disabledTo.HasValue || disabledTo.Value < DateTime.Now;
	}
}