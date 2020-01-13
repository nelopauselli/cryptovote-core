﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Elections;
using Domain.Utils;

namespace Domain
{
	public class Node : INode
	{
		private readonly INodeConfiguration configuration;
		public string PublicUrl => configuration.PublicUrl;

		private readonly INodeLogger logger;
		private readonly Blockchain blockchain;
		private readonly IDictionary<string, BlockItem> pendings;

		private readonly Peers peers;
		public Peers Peers => peers;

		private bool stop;
		private readonly object semaphore = new object();
		private Task worker;
		private readonly CancellationTokenSource cts = new CancellationTokenSource();

		public Node(INodeConfiguration configuration, IBlockBuilder blockBuilder, INodeLogger logger, IPeerChannel channel=null)
		{
			this.configuration = configuration;
			this.logger = logger;
			
			peers = new Peers(this, channel ?? new HttpPeerChannel());
			
			blockchain = new Blockchain(new Miner(AddressRewards), blockBuilder, configuration.BlockchainDificulty);

			var path = "genesis.block";
			blockchain.LoadGenesisBlock(path);

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
					logger.Warning("Falta configurar 'Miner:Address'");
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

		public NodeState State { get; set; }

		public IEnumerable<BlockItem> Pendings => pendings.Values;
		public int ChainLength => blockchain.Trunk.Count();
		public Blockchain Blockchain => blockchain;


		public void Start()
		{
			worker = Task.Run(() =>
			{
				State = NodeState.Running;

				logger.Information("Inicializando Nodo de Blockchain");
				while (!stop)
				{
					Thread.Sleep(configuration.MinerInterval);
					MinePendingTransactions();
				}

				State = NodeState.Stoped;
			}, cts.Token);
			Thread.Sleep(10);
		}

		public void Stop()
		{
			stop = true;
			cts.Cancel();
		}

		public void Connect(string url)
		{
			peers.Connect(this.configuration.PublicUrl, url);
		}

		public void Syncronize()
		{
			peers.GetLastBlock();
		}

		public void Discovery()
		{
			peers.Discovery(this.configuration.PublicUrl);
		}

		public void MinePendingTransactions()
		{
			logger.Information("Buscando transacciones que minar");
			BlockItem[] pendingsToMine;
			lock (semaphore)
			{
				pendingsToMine = Pendings.ToArray();
			}

			if (pendingsToMine.Length > 0)
			{
				logger.Information($"Minando {pendingsToMine.Length} transacciones");
				try
				{
					var block = blockchain.MineNextBlock(pendingsToMine);

					lock (semaphore)
					{
						if (block != null)
						{
							peers.Broadcast(block);
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
				catch (Exception ex)
				{
					logger.Error($"Error minando transacciones pendientes: {ex}");
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

			peers.Broadcast(community);
		}

		public void Add(Question question)
		{
			lock (semaphore)
			{
				if (pendings.ContainsKey(question.GetKey()))
					return;

				pendings.Add(question.GetKey(), question);
			}

			peers.Broadcast(question);
		}

		public void Add(Member member)
		{
			lock (semaphore)
			{
				if (pendings.ContainsKey(member.GetKey()))
					return;

				pendings.Add(member.GetKey(), member);
			}

			peers.Broadcast(member);
		}

		public void Add(Vote vote)
		{
			lock (semaphore)
			{
				if (pendings.ContainsKey(vote.GetKey()))
					return;

				pendings.Add(vote.GetKey(), vote);
			}

			peers.Broadcast(vote);
		}

		public void Add(Fiscal fiscal)
		{
			lock (semaphore)
			{
				if (pendings.ContainsKey(fiscal.GetKey()))
					return;

				pendings.Add(fiscal.GetKey(), fiscal);
			}

			peers.Broadcast(fiscal);
		}

		public void Add(Urn urn)
		{
			lock (semaphore)
			{
				if (pendings.ContainsKey(urn.GetKey()))
					return;

				pendings.Add(urn.GetKey(), urn);
			}

			peers.Broadcast(urn);
		}

		public void Add(Recount recount)
		{
			lock (semaphore)
			{
				if (pendings.ContainsKey(recount.GetKey()))
					return;

				pendings.Add(recount.GetKey(), recount);
			}

			peers.Broadcast(recount);
		}

		public void Add(Document document)
		{
			lock (semaphore)
			{
				if (pendings.ContainsKey(document.GetKey()))
					return;

				pendings.Add(document.GetKey(), document);
			}

			peers.Broadcast(document);
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
				peers.GetBlock(block.PreviousHash);
				return;
			}

			lock (semaphore)
			{
				//TODO: stop miner
				stop = true;

				var transactions = block.GetTransactions();
				logger.Information($"Quitando {transactions.Length} transacciones pendientes");

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

			peers.GetLastBlock();
		}

		public void Add(PeerInfo peer)
		{
			peers.Add(peer);
		}
	}
}