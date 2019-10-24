using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Domain.Protocol;

namespace Domain.Channels
{
	public class TcpChannel : IChannel
	{
		private readonly int port;
		private readonly INodeLogger logger;
		private readonly bool stop = false;
		private CancellationTokenSource cancellation;
		private TcpListener server;
		private readonly INode node;

		public TcpChannel(INode node, int port, INodeLogger logger)
		{
			this.node = node ?? throw new ArgumentNullException(nameof(node));
			this.port = port;
			this.logger = logger;

			State = ChannelState.Stop;
		}

		public ChannelState State { get; set; }

		public Task Start(int timeout)
		{
			cancellation = new CancellationTokenSource();

			var address = new IPAddress(new byte[] {0, 0, 0, 0});
			logger.Information($"Listening in {address}:{port}");

			var task = Task.Run(async () =>
			{
				try
				{
					server = new TcpListener(address, port);

					server.Start();
					State = ChannelState.Listening;

					while (!stop)
					{
						logger.Information("Waiting for a connection... ");
						TcpClient client = await server.AcceptTcpClientAsync();
						logger.Information("Connected!");

						Task.Run(() =>
						{
							TalkWithClient(client.GetStream());
							client.Close();
						});
					}
				}
				catch (SocketException e)
				{
					logger.Error($"SocketException: {e}");
				}
				finally
				{
					// Stop listening for new clients.
					server?.Stop();
					State = ChannelState.Stop;
				}
			}, cancellation.Token);

			var timeLimit = DateTime.Now.AddMilliseconds(timeout);
			while (State != ChannelState.Listening)
			{
				if (DateTime.Now > timeLimit)
					break;
				Thread.Sleep(10);
			}

			return task;
		}

		public void TalkWithClient(Stream stream)
		{
			try
			{
				var conversation = new Conversation(node, logger);
				conversation.Talk(new ProtocolMessageChannel(stream));
			}
			finally
			{
				// Shutdown and end connection
				logger.Information("Shutdown and end connection");

			}
		}

		public void Stop()
		{
			cancellation?.Cancel();

			logger.Information("Say NO MORE");
			server?.Stop();
			State = ChannelState.Stop;
		}
	}

	public interface IChannel
	{
		void TalkWithClient(Stream stream);
	}
}