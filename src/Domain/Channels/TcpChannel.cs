using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Domain.Channels.Protocol.Authentication;
using Domain.Channels.Protocol.Queries;

namespace Domain.Channels
{
	public class TcpChannel : IChannel
	{
		public string ID { get; }
		public string ListenHost { get; }
		public int ListenPort { get; }

		private readonly INodeLogger logger;

		TcpListener server;
		private readonly object peersSemaphore =new object();
		private readonly IList<TcpPeer> peers = new List<TcpPeer>();
		private readonly CancellationTokenSource cancellationToken = new CancellationTokenSource(2000);
		private bool stoped;

		public IEnumerable<TcpPeer> Peers => peers;
		public INode Node { get; }
		public ChannelState State { get; private set; }

		public TcpChannel(string id, string host, int port, INode node, INodeLogger logger)
		{
			ID = id;
			ListenHost = host;
			ListenPort = port;
			Node = node;
			this.logger = logger;
			State = ChannelState.Stop;
		}

		public async Task Listen()
		{
			try
			{
				logger.Information($"[#{Thread.CurrentThread.ManagedThreadId}] Iniciando canal de comunicaciones...");

				var addr = IPAddress.Parse(ListenHost);
				//var addr = IPAddress.Parse("0.0.0.0");
				server = new TcpListener(addr, ListenPort);
				server.Start();

				try
				{
					State = ChannelState.Listening;

					while (!cancellationToken.IsCancellationRequested)
					{
						logger.Information($"[#{Thread.CurrentThread.ManagedThreadId}] Canal iniciado");
						
						TcpClient client;
						try
						{
							client = await Task.Run(() => server.AcceptTcpClientAsync(), cancellationToken.Token);
						}
						catch
						{
							client = null;
						}

						if (client != null)
						{
							logger.Information($"[#{Thread.CurrentThread.ManagedThreadId}] Nueva comunicación entrante");

							var peer = new TcpPeer(this, client, logger);
							peer.Start();

							lock (peersSemaphore)
							{
								peers.Add(peer);
							}
						}
					}
				}
				finally
				{
					logger.Information($"[#{Thread.CurrentThread.ManagedThreadId}] Listener detenido");
					if (server.Server.Connected)
						server.Stop();
					State = ChannelState.Stop;
					stoped = true;
				}
			}
			catch (Exception ex)
			{
				State = ChannelState.Error;
				logger.Error(ex.ToString());
			}
		}

		public Task Stop()
		{
			return Task.Run(() =>
			{
				logger.Information($"[#{Thread.CurrentThread.ManagedThreadId}] Deteniendo canal de comunicaciones...");

				cancellationToken.Cancel();

				lock (peersSemaphore)
				{
					foreach (var peer in peers)
						peer.Stop();
				}

				var attemtp = 0;
				while (!stoped && attemtp++ < 20)
				{
					Thread.Sleep(100);
				}

				if (!stoped)
				{
					logger.Information($"[#{Thread.CurrentThread.ManagedThreadId}] Listener abortado");
					server.Stop();
					stoped = true;
				}

				logger.Information($"[#{Thread.CurrentThread.ManagedThreadId}] Canal detenido");

				//serverThread.Abort();
				//server.Stop();
			});
		}

		public void Connect(PeerInfo peerInfo)
		{
			lock (peersSemaphore)
			{
				if (peers.Any(p => p.ID == peerInfo.Id))
				{
					logger.Information($"El peer {peerInfo.Id} ya está registrado");
					return;
				}
			}

			Connect(peerInfo.Host, peerInfo.Port);
		}

		public void Connect(string host, int port)
		{
			logger.Information($"[#{Thread.CurrentThread.ManagedThreadId}] Conectando con {host}:{port}");

			var client = new TcpClient(host, port);

			var peer = new TcpPeer(this, client, logger);
			peer.Start();

			peer.Send(new LoginCommand(new PeerInfo { Id = ID, Host = ListenHost, Port = ListenPort }));

			lock (peersSemaphore)
			{
				peers.Add(peer);
			}
		}

		public void Discovery()
		{
			lock (peersSemaphore)
			{
				foreach (var peer in peers)
					peer.Send(new PeersRequestCommand());
			}
		}

		public void Broadcast(ICommand command)
		{
			lock (peersSemaphore)
			{
				foreach (var peer in peers)
					peer.Send(command);
			}
		}
	}
}