using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Channels;
using Domain.Protocol;
using Domain.Queries;
using Domain.Elections;
using Domain.Utils;

namespace Domain
{
	public class Node : INode
	{
		private readonly INodeConfiguration configuration;
		private readonly INodeLogger logger;
		private readonly Blockchain blockchain;
		private readonly IDictionary<string, BlockItem> pendings;
		public Peers Peers { get; }

		private bool stop;
		private readonly object semaphore = new object();
		private Task worker;
		private readonly TcpChannel channel;

		public Node(INodeConfiguration configuration, IBlockBuilder blockBuilder, INodeLogger logger)
		{
			this.configuration = configuration;
			this.logger = logger;
			this.Peers = new Peers(configuration.Name, logger);

			blockchain = new Blockchain(new Miner(AddressRewards), blockBuilder, Dificulty);

			var path = "genesis.block";
			blockchain.LoadGenesisBlock(path);

			channel = new TcpChannel(this, configuration.MyPort, logger);

			pendings = new Dictionary<string, BlockItem>();
		}

		protected byte[] AddressRewards
		{
			get
			{
				byte[] addressRewards = null;

				var addressRewardsBase58 = configuration.MinerAddress;
				if (string.IsNullOrWhiteSpace(addressRewardsBase58))
				{
					logger.Error("Falta configurar 'Miner:Address'");
				}
				else
				{
					try
					{
						addressRewards = Base58.Decode(addressRewardsBase58);
					}
					catch (Exception ex)
					{
						logger.Error(ex.Message);
					}
				}

				return addressRewards ?? new byte[32];
			}
		}

		protected byte Dificulty
		{
			get
			{
				var dificultyAsString = configuration.BlockchainDificulty;

				byte dificulty = 2;
				if (string.IsNullOrWhiteSpace(dificultyAsString))
					logger.Error($"Falta configurar 'Blockchain:Dificulty'. Se minará con una dificultad de {dificulty} zeros");
				else if (!byte.TryParse(dificultyAsString, out dificulty))
					logger.Error($"El valor de 'Blockchain:Dificulty' no es un número entero. Se minará con una dificultad de {dificulty} zeros");

				return dificulty;
			}
		}

		protected int MinerInterval
		{
			get
			{
				var intervalAsString = configuration.MinerInterval;

				int interval = 10 * 60 * 1000;
				if (string.IsNullOrWhiteSpace(intervalAsString))
					logger.Warning($"Falta configurar 'Miner:Interval', se minará con un intervalo de {interval} ms");
				else if (!int.TryParse(intervalAsString, out interval))
					logger.Warning($"El valor de 'Miner:Interval' no es un número entero, se minará con un intervalo de {interval} ms");

				return interval;
			}
		}

		public string Name => configuration.Name;

		public NodeState State { get; set; }

		public IEnumerable<BlockItem> Pendings => pendings.Values;
		public int ChainLength => blockchain.Trunk.Count();
		public Blockchain Blockchain => blockchain;
		public ChannelState ChannelState => channel.State;

		public void Start()
		{
			worker = Task.Run(() =>
			{
				State = NodeState.Running;

				logger.Information("Inicializando Nodo de Blockchain");
				while (!stop)
				{
					Thread.Sleep(MinerInterval);
					MinePendingTransactions();
				}

				State = NodeState.Stoped;
			});
			Thread.Sleep(10);
		}

		public void Listen(int timeout = 1000)
		{
			channel.Start(timeout);
		}

		public void MinePendingTransactions()
		{
			logger.Debug("Buscando transacciones que minar");
			BlockItem[] pendingsToMine;
			lock (semaphore)
			{
				pendingsToMine = Pendings.ToArray();
			}

			if (pendingsToMine.Length > 0)
			{
				logger.Debug($"Minando {pendingsToMine.Length} transacciones");

				var block = blockchain.MineNextBlock(pendingsToMine);

				lock (semaphore)
				{
					if (block != null)
					{
						Peers.Broadcast(block);
						var transactions = block.GetTransactions();
						foreach (var item in transactions)
							pendings.Remove(item.GetKey());
					}
				}

				var chain = blockchain.Trunk.ToArray();
				foreach (var invalid in pendingsToMine.Where(p => !p.IsValid(chain)))
				{
					logger.Warning($"Item inválido: {string.Join("\n\r\t", invalid.Messages)}");
				}
			}
			else
			{
				logger.Debug("No se encontraron transacciones que minar");
			}
		}

		public void Add(Community community)
		{
			lock (semaphore)
			{
				if (pendings.ContainsKey(community.GetKey()))
					return;

				pendings.Add(community.GetKey(), community);
			}

			Peers.Broadcast(community);
		}

		public void Add(Question question)
		{
			lock (semaphore)
			{
				if (pendings.ContainsKey(question.GetKey()))
					return;

				pendings.Add(question.GetKey(), question);
			}

			Peers.Broadcast(question);
		}

		public void Add(Member member)
		{
			lock (semaphore)
			{
				if (pendings.ContainsKey(member.GetKey()))
					return;

				pendings.Add(member.GetKey(), member);
			}

			Peers.Broadcast(member);
		}

		public void Add(Vote vote)
		{
			lock (semaphore)
			{
				if (pendings.ContainsKey(vote.GetKey()))
					return;

				pendings.Add(vote.GetKey(), vote);
			}

			Peers.Broadcast(vote);
		}

		public void Add(Fiscal fiscal)
		{
			lock (semaphore)
			{
				if (pendings.ContainsKey(fiscal.GetKey()))
					return;

				pendings.Add(fiscal.GetKey(), fiscal);
			}

			Peers.Broadcast(fiscal);
		}

		public void Add(Urn urn)
		{
			lock (semaphore)
			{
				if (pendings.ContainsKey(urn.GetKey()))
					return;

				pendings.Add(urn.GetKey(), urn);
			}

			Peers.Broadcast(urn);
		}

		public void Add(Recount recount)
		{
			lock (semaphore)
			{
				if (pendings.ContainsKey(recount.GetKey()))
					return;

				pendings.Add(recount.GetKey(), recount);
			}

			Peers.Broadcast(recount);
		}

		public void Add(Document document)
		{
			lock (semaphore)
			{
				if (pendings.ContainsKey(document.GetKey()))
					return;

				pendings.Add(document.GetKey(), document);
			}

			Peers.Broadcast(document);
		}

		public void Add(Block block)
		{
			logger.Information("Recibiendo bloque");

			var other = blockchain.GetBlock(block.Hash);
			if (other != null)
			{
				logger.Information("El bloque ya existe");
				return;
			}

			if (!blockchain.Last.Hash.SequenceEqual(block.PreviousHash))
			{
				logger.Information("El bloque no existe ni es el siguiente al último, buscando más");
				Peers.GetBlockByHash(block.PreviousHash);
				return;
			}

			lock (semaphore)
			{
				//TODO: stop miner
				stop = true;

				logger.Information("Quitando transacciones pendientes");
				var transactions = block.GetTransactions();

				foreach (var item in transactions)
				{
					int count = 0;
					if (pendings.ContainsKey(item.GetKey()))
					{
						count++;
						pendings.Remove(item.GetKey());
					}

					logger.Information($"Se quitaron {count} transacciones de la lista de pendientes");
				}

				logger.Information($"Agregando bloque #{block.BlockNumber} a la cadena");
				blockchain.AddBlock(block);
			}

			Peers.Broadcast(new LastBlockQueryMessage());
		}

		public Block GetLastBlock()
		{
			return blockchain.Last;
		}

		public Block GetByHash(byte[] hash)
		{
			return blockchain.GetBlock(hash);
		}

		public void Register(string host, int port)
		{
			var peer = new TcpPeer(host, port, channel);
			if (Peers.Contains(peer))
			{
				logger.Information($"Ya tenemos al peer ${peer.Host}:{peer.Port} entre los pares");
				return;
            }

			if (peer.Host == configuration.MyHost && peer.Port == configuration.MyPort)
			{
				logger.Information($"Soy yo mismo el par");
				return;
			}

			logger.Information($"Registrando el par: {peer}");

			Peers.Add(peer);

			Peers.Send(peer, new SendPeerInfoMessage(new PeerInfo {Host = configuration.MyHost, Port = configuration.MyPort}));
			foreach (var other in Peers.Hosts)
			{
				if(other.Host != peer.Host || other.Port != peer.Port) // No mandamos información de un par a ese par
					Peers.Send(peer, new SendPeerInfoMessage(new PeerInfo {Host = other.Host, Port = other.Port}));
			}
		}

		public void Syncronize()
		{
			foreach (var peer in Peers.Hosts)
				Peers.Send(peer, new LastBlockQueryMessage());
		}

		public void Stop(int timeout=1000)
		{
			stop = true;

			channel.Stop();

			var startAt = DateTime.Now;
			while (State == NodeState.Running)
			{
				Thread.Sleep(100);
				if (DateTime.Now.Subtract(startAt).TotalMilliseconds > timeout)
					break;
			}
		}
	}
}