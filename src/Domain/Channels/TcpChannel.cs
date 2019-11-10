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
						logger.Debug("Waiting for a client... ");
						TcpClient client = await server.AcceptTcpClientAsync();
						logger.Debug("Client connected!");

						Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
						t.Start();
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

		private void HandleClient(object obj)
		{
			var client = (TcpClient) obj;
			var stream = client.GetStream();
			TalkWithClient(stream);
			stream.Flush();
			Thread.Sleep(100);
			client.Close();
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
				logger.Debug("Shutdown and end connection");
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
}