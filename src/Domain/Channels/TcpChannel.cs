using System;
using System.Collections.Generic;
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
		private readonly IList<IEventListener> listeners = new List<IEventListener>();

		public TcpChannel(INode node, int port, INodeLogger logger)
		{
			this.node = node ?? throw new ArgumentNullException(nameof(node));
			AddListener(new NodeListener(node));

			this.port = port;
			this.logger = logger;

			State = ChannelState.Stop;
		}

		public ChannelState State { get; set; }

		public void AddListener(IEventListener listener)
		{
			listeners.Add(listener);
		}
		
		public Task Start(int timeout)
		{
			cancellation = new CancellationTokenSource();

			var address = new IPAddress(new byte[] {0, 0, 0, 0});
			foreach (var listener in listeners)
				listener.Information($"Listening in {address}:{port}");

			var task= Task.Run(async () =>
			{
				try
				{
					server = new TcpListener(address, port);

					// Start listening for client requests.
					server.Start();
					State = ChannelState.Listening;
					// Buffer for reading data

					// Enter the listening loop.
					while (!stop)
					{
						foreach (var listener in listeners)
							listener.Information("Waiting for a connection... ");
						// Perform a blocking call to accept requests.
						// You could also user server.AcceptSocket() here.
						TcpClient client = await server.AcceptTcpClientAsync();
						foreach (var listener in listeners)
							listener.Information("Connected!");

						Task.Run(() =>
						{
							TalkWithClient(client.GetStream());
							client.Close();
						});
					}
				}
				catch (SocketException e)
				{
					foreach (var listener in listeners)
						listener.Error($"SocketException: {e}");
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
				if(DateTime.Now> timeLimit)
					break;
				Thread.Sleep(10);
			}

			return task;
		}

		public void TalkWithClient(Stream stream)
		{
			try
			{
				var conversation = new Conversation(node.Blockchain, listeners, logger, this);
				conversation.Talk(new ProtocolMessageChannel(stream));
			}
			finally
			{
				// Shutdown and end connection
				foreach (var listener in listeners)
					listener.Information("Shutdown and end connection");
				
			}
		}

		public void Stop()
		{
			cancellation?.Cancel();

			foreach (var listener in listeners)
				listener.Information("Say NO MORE");
			server?.Stop();
			State = ChannelState.Stop;
		}
	}

	public interface IChannel
	{
		void TalkWithClient(Stream stream);
	}
}