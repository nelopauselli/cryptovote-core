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
				client = new TcpClient {SendTimeout = 1000, ReceiveTimeout = 1000};
				await client.ConnectAsync(Host, Port);
				if (client.Connected)
				{
					using (var stream = client.GetStream())
					{
						await stream.WriteAsync(data, 0, data.Length, ctsToken);
						stream.Flush();

						Thread.Sleep(1000);
						channel.TalkWithClient(stream);
					}
				}
			}
			catch (Exception ex)
			{
				disabledTo = DateTime.Now.AddMinutes(1);
				throw new ChannelException($"Error de comunicación con {Host}:{Port}: {ex.Message}", ex);
			}
		}

		public override string ToString()
		{
			return $"{Host}:{Port}";
		}

		public bool IsReady => !disabledTo.HasValue || disabledTo.Value < DateTime.Now;
		public void Close()
		{
			if (client?.Connected ?? false)
				client?.Close();
		}
	}
}