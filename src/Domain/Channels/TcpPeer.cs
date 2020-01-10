using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Domain.Channels.Protocol;
using Domain.Channels.Protocol.Authentication;
using Domain.Channels.Protocol.Documents;
using Domain.Channels.Protocol.Echo;
using Domain.Channels.Protocol.PingPong;
using Domain.Channels.Protocol.Queries;
using Domain.Protocol;
using Domain.Queries;

namespace Domain.Channels
{
	public class TcpPeer
	{
		private readonly object semaphore = new object();
		private const int MaxAttempt = 3;

		public string ID { get; private set; }
		public int Port { get; private set; }
		public string Host { get; private set; }
		public long LastActivityTime { get; private set; }
		public bool IsAutenticated { get; set; }
		public IList<string> History { get; } = new List<string>();


		private readonly TcpClient client;
		private Thread clientThread;
		private bool stop;

		private readonly INodeLogger logger;

		private readonly NetworkStream stream;

		private readonly IMiddleware[] handlers;

		public TcpPeer(IChannel owner, TcpClient client, INodeLogger logger)
		{
			ID = owner.ID;
			Host = owner.ListenHost;
			Port = owner.ListenPort;

			this.client = client;
			this.logger = logger;

			handlers = new IMiddleware[]
			{
				new PingMiddleware(),
				new PongMiddleware(),
				new EchoMiddleware(),
				new EchoReplyMiddleware(),
				new DocumentMiddleware(),
				new DocumentHashMiddleware(),
				new LoginMiddleware(owner),
				new AuthorizedMiddleware(),
				new PeersRequestMiddleware(owner),
				new PeersResponseMiddleware(owner),
				new SendBlockMiddleware(owner.Node), 
				new SendCommunityMiddleware(owner.Node), 
				new SendDocumentMiddleware(owner.Node), 
				new SendFiscalMiddleware(owner.Node), 
				new SendMemberMiddleware(owner.Node), 
				new SendQuestionMiddleware(owner.Node), 
				new SendRecountMiddleware(owner.Node), 
				new SendUrnMiddleware(owner.Node), 
				new SendVoteMiddleware(owner.Node), 

				new GetLastBlockMiddleware(owner.Node),
				new GetBlockMiddleware(owner.Node, logger), 

				new LastBlockQueryMiddleware(owner.Node),
				new BlockQueryMiddleware(owner.Node),
				new CommunitiesQueryMiddleware(owner.Node),
				new CommunityQueryMiddleware(owner.Node),
				new MembersQueryMiddleware(owner.Node),
				new MemberQueryMiddleware(owner.Node), 
				new QuestionsQueryMiddleware(owner.Node),
				new QuestionQueryMiddleware(owner.Node),

				new NotFoundMiddleware()
			};


			stream = client.GetStream();
		}



		public void Start()
		{
			clientThread = new Thread(HandleClient) { IsBackground = true };
			clientThread.Start();
		}

		private void HandleClient(object obj)
		{
			while (!stop)
			{
				try
				{
					lock (semaphore)
					{
						if (stream.CanRead && stream.DataAvailable)
						{
							logger.Debug($"[{ID}] Hilo bloqueado");

							var header = CommandHeader.Parse(stream);

							logger.Debug($"[{ID}] Recibiendo comando: {header.CommandId} de {header.Length} bytes");

							foreach (var handler in handlers)
							{
								if (handler.Invoke(header, this))
									break;
							}

							logger.Debug($"[{ID}] Liberando bloqueo del hilo");
						}
					}
				}
				catch (Exception ex)
				{
					logger.Error($"[{ID}] ERROR (HandlerClient): {ex}");
				}
			}

			logger.Information($"[{ID}] conexión terminada");
			client.Close();
		}

		public void Send(ICommand command)
		{
			bool sended = false;
			int attempt = 0;

			History.Add(command.Name);

			while (!sended && attempt++ < MaxAttempt)
			{
				logger.Information($"[{ID}] Intento #{attempt}");
				if (attempt > 0)
				{
					Thread.Sleep(100);
				}

				logger.Debug($"[{ID}] Bloqueando hilo");
				lock (semaphore)
				{
					try
					{
						logger.Debug($"[{ID}] Hilo bloqueado");

						logger.Information($"[{ID}] Enviando '{command.Name}' a {this.Host}");

						command.Send(stream);
						Thread.Sleep(100);

						sended = true;
					}
					catch (Exception ex)
					{
						if (attempt == MaxAttempt)
							logger.Error($"[{ID}] ERROR (Send): {ex}"); 
					}

					logger.Debug($"[{ID}] Liberando bloqueo del hilo");
				}
			}
		}

		public void Stop()
		{
			logger.Information($"[{ID}] Deteniendo comunicación");

			stop = true;
			int count = 0;
			while (clientThread.IsAlive && count++ < 10)
			{
				Thread.Sleep(100);
			}

			if (clientThread.IsAlive)
			{
				logger.Information($"[{ID}] abortando comunicación");

				clientThread.Abort();

				if (client.Connected)
					client.Close();
			}
			else
			{
				logger.Information($"[{ID}] Comunicación detenida");
			}
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			lock (semaphore)
			{
				return stream.Read(buffer, offset, count);
			}
		}

		public void KeepAlive()
		{
			this.LastActivityTime = DateTimeOffset.Now.ToUnixTimeSeconds();
		}

		public void Authenticate(PeerInfo peerInfo)
		{
			ID = peerInfo.Id;
			Host = peerInfo.Host;
			Port = peerInfo.Port;
			IsAutenticated = true;
		}

		public override string ToString()
		{
			return ID;
		}
	}
}