using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Elections;
using Domain.Utils;
using Microsoft.Extensions.Logging;

namespace Domain
{
	public class Node : INode
	{
		private readonly ILogger<Node> logger;
		private readonly object semaphore = new object();

		private readonly INodeConfiguration configuration;

		private readonly IDictionary<string, BlockItem> pendings;
		public IEnumerable<BlockItem> Pendings => pendings.Values;
		private readonly IList<string> searchingBlocks = new List<string>();

		public Blockchain Blockchain { get; }
		public int ChainLength => Blockchain.Trunk.Count();
		
		public Peer Host { get; }
		public Peers Peers { get; }

		private bool stop;
		private Task worker;
		private readonly CancellationTokenSource cts = new CancellationTokenSource();

		private byte[] AddressRewards
		{
			get
			{
				byte[] addressRewards = null;

				var addressRewardsBase58 = configuration.MinerAddress?.Trim();
				if (string.IsNullOrWhiteSpace(addressRewardsBase58))
				{
					logger.LogWarning("Falta configurar 'MinerAddress'");
				}
				else
				{
					try
					{
						addressRewards = Base58.Decode(addressRewardsBase58);
					}
					catch (Exception ex)
					{
						logger.LogError(ex.Message);
					}
				}

				return addressRewards ?? new byte[32];
			}
		}

		public NodeState State { get; private set; }

		

		public Node(INodeConfiguration configuration, IBlockBuilder blockBuilder, ILoggerFactory loggerFactory, IPeerChannel channel)
		{
			this.configuration = configuration;
			this.logger = loggerFactory.CreateLogger<Node>();

			logger.LogInformation($"Configuración del nodo:\n"+
			                      $"NodeId: {configuration.NodeId}\n" +
			                      $"NodeName: {configuration.NodeName}\n" +
			                      $"NodePublicUrl: {configuration.NodePublicUrl}\n" +
			                      $"PeerUrl: {configuration.PeerUrl}\n" +
			                      $"MinerAddress: {configuration.MinerAddress}\n" +
			                      $"BlockchainDificulty: {configuration.BlockchainDificulty}");

			Host = new Peer
			{
				Id = configuration.NodeId,
				Name = configuration.NodeName,
				PublicUrl = configuration.NodePublicUrl
			};

			Peers = new Peers(this, channel, loggerFactory.CreateLogger<Peers>());

			Blockchain = new Blockchain(new Miner(AddressRewards), blockBuilder, configuration.BlockchainDificulty);

			var path = "genesis.block";
			Blockchain.LoadGenesisBlock(path);

			pendings = new Dictionary<string, BlockItem>();
		}



		public void Start()
		{
			worker = Task.Run(() =>
			{
				var scheduler = new Scheduler(configuration.MinerInterval, configuration.PeersCheckInterval, configuration.SyncronizeInterval);

				State = NodeState.Running;

				logger.LogInformation("Inicializando Nodo de Blockchain");
				
				while (!stop)
				{
					Thread.Sleep(1000);
					
					if (scheduler.IsTimeToCheckPeers())
						CheckPeers();
					if (scheduler.IsTimeToSyncronize())
						Syncronize(); 
					if (scheduler.IsTimeToMine())
						MinePendingTransactions();

				}

				State = NodeState.Stoped;
			}, cts.Token);
			Thread.Sleep(10);
		}

		private void CheckPeers()
		{
			logger.LogInformation("Verificando conexiones con pares");

			if (!string.IsNullOrWhiteSpace(configuration.PeerUrl))
			{
				if(!Peers.Contains(configuration.PeerUrl))
					Connect(configuration.PeerUrl);
			}

			Discovery();
		}

		public void Stop()
		{
			stop = true;
			cts.Cancel();
		}

		public void Connect(string url)
		{
			logger.LogInformation($"Conectando con el peer: {url}");

			var peer = Peers.GetNodeInfo(url);
			if (peer != null)
				Register(peer);
			else
				logger.LogWarning($"No se pudo establecer la conexión con {url}");
		}

		public void Syncronize()
		{
			logger.LogInformation("Sincronizando blockchain con pares");
			Peers.GetLastBlock();
		}

		public void Discovery()
		{
			logger.LogInformation("Descubriendo nuevos pares");

			Peers.Discovery();
		}

		public void MinePendingTransactions()
		{
			logger.LogInformation($"Buscando transacciones que minar.\n" +
			                      $"\tÚltimo bloque #{Blockchain.Last.BlockNumber}.\n" +
			                      $"\tCadenas secundarias #{Blockchain.BranchesCount}\n"+
			                      $"\tTransacciones pendientes #{pendings.Count}\n"+
			                      $"\tPares registrados #{Peers.Count}: {string.Join(", ",Peers.List())}"
				                      );

			BlockItem[] pendingsToMine;
			lock (semaphore)
			{
				pendingsToMine = Pendings.ToArray();
			}

			if (pendingsToMine.Length > 0)
			{
				logger.LogInformation($"Minando {pendingsToMine.Length} transacciones");
				try
				{
					var block = Blockchain.MineNextBlock(pendingsToMine);

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

					var chain = Blockchain.Trunk.ToArray();
					KeyValuePair<string, BlockItem>[] invalids;
					lock (semaphore)
					{
						invalids = pendings.Where(p => !p.Value.IsValid(chain)).ToArray();
					}

					foreach (var invalid in invalids)
					{
						logger.LogWarning($"Item {invalid.Value.GetType().Name} inválido: {string.Join("\n\r\t", invalid.Value.Messages)}");
					}
				}
				catch (Exception ex)
				{
					logger.LogCritical($"Error minando transacciones pendientes: {ex}");
				}
			}
			else
			{
				logger.LogDebug("No se encontraron transacciones que minar");
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
			logger.LogInformation($"Recibiendo bloque {block.BlockNumber} con hash {block.Hash.ByteArrayToHexString()}");

			var other = Blockchain.GetBlock(block.Hash);
			if (other != null)
			{
				logger.LogInformation("El bloque ya existe");
				return;
			}

			if (!Blockchain.Last.Hash.SequenceEqual(block.PreviousHash))
			{
				var hashAsHex = block.PreviousHash.ByteArrayToHexString();
				if (!searchingBlocks.Contains(hashAsHex))
				{
					logger.LogInformation("El bloque no existe ni es el siguiente al último, buscando más");
					searchingBlocks.Add(hashAsHex);
					Peers.GetBlock(block.PreviousHash);
					searchingBlocks.Remove(hashAsHex);
				}
				return;
			}

			lock (semaphore)
			{
				//TODO: stop miner
				stop = true;

				var transactions = block.GetTransactions();
				logger.LogInformation($"Quitando {transactions.Length} transacciones pendientes");

				foreach (var item in transactions)
				{
					int count = 0;
					if (pendings.ContainsKey(item.GetKey()))
					{
						count++;
						pendings.Remove(item.GetKey());
					}

					logger.LogInformation($"Se quitaron {count} transacciones de la lista de pendientes");
				}

				logger.LogInformation($"Agregando bloque #{block.BlockNumber} a la cadena");
				Blockchain.AddBlock(block);
			}

			Peers.GetLastBlock();
		}

		public void Register(Peer peer)
		{
			Peers.Add(peer);
		}
	}
}