using System;
using System.Linq;
using System.Threading;
using Domain;
using Domain.Crypto;
using Domain.Elections;
using NUnit.Framework;
using Tests.Mocks;

namespace Tests
{
	public class Tests : TestWithServices
	{
		private readonly Signer signer = new Signer(CryptoService.Instance);

		[Test]
		public void Start_node()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var cfg = new NodeConfiguration("Nodo 1", "1234", 1, 2000, "http://localhost:8000");
				var node = new Node(cfg, new BlockBuilder(), loggerFactory, new PeerChannelInProc());

				try
				{
					node.Start();
					Thread.Sleep(1000);
					Assert.AreEqual(NodeState.Running, node.State);
				}
				finally
				{
					node.Stop();
				}
			}
		}

		[Test]
		public void Stop_node()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var cfg = new NodeConfiguration("Nodo 1", "1234", 1, 2000, "http://localhost:8001");
				var node = new Node(cfg, new BlockBuilder(), loggerFactory, new PeerChannelInProc());

				node.Start();
				Thread.Sleep(1000);
				Assert.AreEqual(NodeState.Running, node.State);

				node.Stop();
				Thread.Sleep(1000);
				Assert.AreEqual(NodeState.Stoped, node.State);
			}
		}

		[Test]
		public void Mensaje_entre_red_de_3_nodos()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var channel = new PeerChannelInProc();
				var node1 = new Node(new NodeConfiguration("Nodo 1", "1111", 1, 2000, "http://localhost:8001"), new BlockBuilder(), loggerFactory, channel);
				var node2 = new Node(new NodeConfiguration("Nodo 2", "2222", 1, 2000, "http://localhost:8002"), new BlockBuilder(), loggerFactory, channel);
				var node3 = new Node(new NodeConfiguration("Nodo 3", "3333", 1, 2000, "http://localhost:8003"), new BlockBuilder(), loggerFactory, channel);
				channel.Add(node1);
				channel.Add(node2);
				channel.Add(node3);

				node1.Connect(node2.Host.PublicUrl);
				Assert.AreEqual(1, node1.Peers.Count);
				Assert.AreEqual(1, node2.Peers.Count);
				Assert.AreEqual(0, node3.Peers.Count);

				node2.Connect(node3.Host.PublicUrl);
				Assert.AreEqual(1, node1.Peers.Count);
				Assert.AreEqual(2, node2.Peers.Count);
				Assert.AreEqual(1, node3.Peers.Count);

				node3.Discovery();
				Assert.AreEqual(2, node1.Peers.Count);
				Assert.AreEqual(2, node2.Peers.Count);
				Assert.AreEqual(2, node3.Peers.Count);

				node2.Syncronize();
				node3.Syncronize();

				node1.Add(new Community {Address = new byte[] {1, 2, 3}});
				Assert.AreEqual(1, node2.Pendings.Count());
				Assert.AreEqual(1, node3.Pendings.Count());
			}
		}

		[Test]
		public void Mensaje_entre_red_de_3_nodos_circular()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var channel = new PeerChannelInProc();
				var node1 = new Node(new NodeConfiguration("Nodo 1", "1111", 1, 2000, "http://localhost:8001"), new BlockBuilder(), loggerFactory, channel);
				var node2 = new Node(new NodeConfiguration("Nodo 2", "2222", 1, 2000, "http://localhost:8002"), new BlockBuilder(), loggerFactory, channel);
				var node3 = new Node(new NodeConfiguration("Nodo 3", "3333", 1, 2000, "http://localhost:8003"), new BlockBuilder(), loggerFactory, channel);
				channel.Add(node1);
				channel.Add(node2);
				channel.Add(node3);

				node1.Connect(node2.Host.PublicUrl);
				node2.Connect(node3.Host.PublicUrl);
				node3.Connect(node1.Host.PublicUrl);

				node1.Add(new Community {Address = new byte[] {1, 2, 3}});

				Assert.AreEqual(1, node1.Pendings.Count());
				Assert.AreEqual(1, node2.Pendings.Count());
				Assert.AreEqual(1, node3.Pendings.Count());
			}
		}

		[Test]
		public void Compartiendo_pares_entre_3_nodos()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var channel = new PeerChannelInProc();
				var node1 = new Node(new NodeConfiguration("Nodo 1", "1111", 1, 2000, $"http://{host1}:{port1}"), new BlockBuilder(), loggerFactory, channel);
				var node2 = new Node(new NodeConfiguration("Nodo 2", "2222", 1, 2000, $"http://{host2}:{port2}"), new BlockBuilder(), loggerFactory, channel);
				var node3 = new Node(new NodeConfiguration("Nodo 3", "3333", 1, 2000, $"http://{host3}:{port3}"), new BlockBuilder(), loggerFactory, channel);
				channel.Add(node1);
				channel.Add(node2);
				channel.Add(node3);

				Assert.AreEqual(0, node1.Peers.Count);
				Assert.AreEqual(0, node2.Peers.Count);
				Assert.AreEqual(0, node3.Peers.Count);

				node1.Connect(node2.Host.PublicUrl);

				Assert.AreEqual(1, node1.Peers.Count, string.Join(", ", node1.Peers.List()));
				Assert.AreEqual(1, node2.Peers.Count, string.Join(", ", node2.Peers.List()));
				Assert.AreEqual(0, node3.Peers.Count, string.Join(", ", node3.Peers.List()));

				node2.Connect(node3.Host.PublicUrl);
				Assert.AreEqual(1, node1.Peers.Count, string.Join(", ", node1.Peers.List()));
				Assert.AreEqual(2, node2.Peers.Count, string.Join(", ", node2.Peers.List()));
				Assert.AreEqual(1, node3.Peers.Count, string.Join(", ", node3.Peers.List()));

				node3.Discovery();
				Assert.AreEqual(2, node1.Peers.Count, string.Join(", ", node1.Peers.List()));
				Assert.AreEqual(2, node2.Peers.Count, string.Join(", ", node2.Peers.List()));
				Assert.AreEqual(2, node3.Peers.Count, string.Join(", ", node3.Peers.List()));
			}
		}

		[Test]
		public void Copiar_blockchain_de_otro_nodo_en_ejecucion_con_2_bloques()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var channel = new PeerChannelInProc();
				var node1 = new Node(new NodeConfiguration("Nodo 1", "1111", 1, 2000, "http://localhost:8001"), new BlockBuilder(), loggerFactory, channel);
				var node2 = new Node(new NodeConfiguration("Nodo 2", "1234", 1, 2000, "http://localhost:8002"), new BlockBuilder(), loggerFactory, channel);
				channel.Add(node1);
				channel.Add(node2);

				Assert.AreEqual(1, node1.ChainLength);

				var community = new Community {Id = Guid.NewGuid(), Name = "My Company", Address = new byte[] {1, 2, 3}};
				var keys = CryptoService.Instance.GeneratePair();
				signer.Sign(community, keys);

				node1.Add(community);
				node1.MinePendingTransactions();

				Assert.AreEqual(2, node1.ChainLength);

				Assert.AreEqual(1, node2.ChainLength);
				node2.Connect(node1.Host.PublicUrl);
				node2.Syncronize();

				Assert.AreEqual(2, node2.ChainLength);
			}
		}

		[Test]
		public void Copiar_blockchain_de_otro_nodo_en_ejecucion_con_3_bloques()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var owner = CryptoService.Instance.GeneratePair();
				var communityKeys = CryptoService.Instance.GeneratePair();

				var channel = new PeerChannelInProc();
				var node1 = new Node(new NodeConfiguration("Nodo 1", "1234", 1, 2000, "http://localhost:8001"), new BlockBuilder(), loggerFactory, channel);
				var node2 = new Node(new NodeConfiguration("Nodo 2", "1234", 1, 2000, "http://localhost:8002"), new BlockBuilder(), loggerFactory, channel);
				channel.Add(node1);
				channel.Add(node2);

				node1.Start();
				Assert.AreEqual(1, node1.ChainLength);

				var community = new Community {Id = Guid.NewGuid(), Name = "My Company", Address = new byte[] {1, 2, 3}};
				community.SetAddress(communityKeys.PublicKey);

				signer.Sign(community, owner);

				node1.Add(community);
				node1.MinePendingTransactions();

				Assert.AreEqual(2, node1.ChainLength);

				var question = new Question {CommunityId = community.Id, Name = "My Question"};
				signer.Sign(question, communityKeys);
				node1.Add(question);
				node1.MinePendingTransactions();

				Assert.AreEqual(3, node1.ChainLength);

				node2.Start();

				Assert.AreEqual(1, node2.ChainLength);
				node2.Connect($"http://{host1}:{port1}");
				node2.Syncronize();

				Assert.AreEqual(3, node2.ChainLength);
			}
		}

		[Test]
		public void Copiar_blockchain_de_otros_nodos_en_ejecucion_x3_con_5_bloques()
		{
			using (var loggerFactory = CreateLoggerFactory())
			{
				var owner = CryptoService.Instance.GeneratePair();
				var communityKeys = CryptoService.Instance.GeneratePair();

				var channel = new PeerChannelInProc();
				var node1 = new Node(new NodeConfiguration("Nodo 1", "1111", 1, 2000, "http://localhost:8001"), new BlockBuilder(), loggerFactory, channel);
				var node2 = new Node(new NodeConfiguration("Nodo 2", "2222", 1, 2000, "http://localhost:8002"), new BlockBuilder(), loggerFactory, channel);
				var node3 = new Node(new NodeConfiguration("Mi Nodo", "3333", 1, 2000, "http://localhost:8003"), new BlockBuilder(), loggerFactory, channel);
				channel.Add(node1);
				channel.Add(node2);
				channel.Add(node3);

				node1.Start();

				node2.Start();
				node2.Connect($"http://{host1}:{port1}");
				node2.Syncronize();

				Assert.AreEqual(1, node1.ChainLength);
				Assert.AreEqual(1, node2.ChainLength);

				var community = new Community {Id = Guid.NewGuid(), Name = "My Company", Address = new byte[] {1, 2, 3}};
				community.SetAddress(communityKeys.PublicKey);

				signer.Sign(community, owner);

				node1.Add(community);
				node1.MinePendingTransactions();

				Assert.AreEqual(2, node1.ChainLength);

				var question = new Question { CommunityId = community.Id, Name = "My Question"};
				signer.Sign(question, communityKeys);
				node1.Add(question);
				node1.MinePendingTransactions();

				Assert.AreEqual(3, node1.ChainLength);
				Assert.AreEqual(1, node3.ChainLength);

				node3.Start();
				node3.Connect($"http://{host1}:{port1}");
				Assert.AreEqual(1, node3.Peers.Count);
				node3.Connect($"http://{host2}:{port2}");
				Assert.AreEqual(2, node3.Peers.Count);

				node3.Syncronize();
				Assert.AreEqual(3, node3.ChainLength);
			}
		}
	}
}