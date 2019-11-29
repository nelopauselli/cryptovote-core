using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Channels;
using Domain.Crypto;
using Domain.Elections;
using NUnit.Framework;
using Tests.Mocks;

namespace Tests
{
	public class Node_tests : TcpTestBase
	{
		private readonly Signer signer = new Signer(CryptoService.Instance);

		[Test]
		public void Start_node()
		{
			var cfg = new NodeConfiguration("Nodo 1", "1234", 1, 2000, port1);
			var logger = new ConsoleLogger();
			var node = new Node(cfg, new BlockBuilder(), logger);

			try
			{
				node.Start();
				Assert.AreEqual(NodeState.Running, node.State);
			}
			finally
			{
				node.Stop(2000);
				WaitFor(() => node.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node.ChannelState);
			}
		}

		[Test]
		public void Stop_node()
		{
			var cfg = new NodeConfiguration("Nodo 1", "1234", 1, 2000, port1);
			var logger = new ConsoleLogger();
			var node = new Node(cfg, new BlockBuilder(), logger);

			node.Start();
			WaitFor(() => node.State == NodeState.Running);
			Assert.AreEqual(NodeState.Running, node.State);

			node.Stop(2000);
			WaitFor(() => node.State == NodeState.Stoped);
			Assert.AreEqual(NodeState.Stoped, node.State);
		}

		[Test]
		public void Mensaje_entre_red_de_3_nodos()
		{
			var logger1 = new ConsoleLogger {Tag = "node-1"};
			var logger2 = new ConsoleLogger {Tag = "node-2"};
			var logger3 = new ConsoleLogger {Tag = "node-3"};

			var node1 = new Node(new NodeConfiguration("Nodo 1", "1111", 1, 2000, port1), new BlockBuilder(), logger1);
			var node2 = new Node(new NodeConfiguration("Nodo 2", "2222", 1, 2000, port2), new BlockBuilder(), logger2);
			var node3 = new Node(new NodeConfiguration("Nodo 3", "3333", 1, 2000, port3), new BlockBuilder(), logger3);

			try
			{
				Task.Run(() => node1.Listen());
				WaitFor(() => node1.ChannelState == ChannelState.Listening);
				Task.Run(() => node2.Listen());
				WaitFor(() => node2.ChannelState == ChannelState.Listening);
				Task.Run(() => node3.Listen());
				WaitFor(() => node3.ChannelState == ChannelState.Listening);

				node1.Connect(host2, port2);
				WaitFor(() => node1.Channel.Peers.Count() == 1);
				WaitFor(() => node2.Channel.Peers.Count() == 1);
				WaitFor(() => node3.Channel.Peers.Count() == 0);

				node2.Connect(host3, port3);
				WaitFor(() => node1.Channel.Peers.Count() == 1);
				WaitFor(() => node2.Channel.Peers.Count() == 2);
				WaitFor(() => node3.Channel.Peers.Count() == 1);

				node3.Discovery();
				WaitFor(() => node1.Channel.Peers.Count() == 1);
				WaitFor(() => node2.Channel.Peers.Count() == 2);
				WaitFor(() => node3.Channel.Peers.Count() == 2);

				node2.Syncronize();
				WaitFor(() => node2.Blockchain.Trunk.Count() == 1);
				node3.Syncronize();
				WaitFor(() => node3.Blockchain.Trunk.Count() == 1);

				node1.Add(new Community {Address = new byte[] {1, 2, 3}});
				WaitFor(() => node2.Pendings.Count() == 1 && node3.Pendings.Count() == 1);

				Assert.AreEqual(1, node2.Pendings.Count());
				Assert.AreEqual(1, node3.Pendings.Count());
			}
			finally
			{
				node1.Stop();
				WaitFor(() => node1.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node1.ChannelState);
				node2.Stop();
				WaitFor(() => node2.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node2.ChannelState);
				node3.Stop();
				WaitFor(() => node3.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node3.ChannelState);
			}
		}

		[Test]
		public void Mensaje_entre_red_de_3_nodos_circular()
		{
			var logger1 = new ConsoleLogger {Tag = "node-1"};
			var logger2 = new ConsoleLogger {Tag = "node-2"};
			var logger3 = new ConsoleLogger {Tag = "node-3"};

			var node1 = new Node(new NodeConfiguration("Nodo 1", "1111", 1, 2000, port1), new BlockBuilder(), logger1);
			var node2 = new Node(new NodeConfiguration("Nodo 2", "2222", 1, 2000, port2), new BlockBuilder(), logger2);
			var node3 = new Node(new NodeConfiguration("Nodo 3", "3333", 1, 2000, port3), new BlockBuilder(), logger3);
			try
			{
				Task.Run(() => node1.Listen());
				WaitFor(() => node1.ChannelState == ChannelState.Listening);
				Task.Run(() => node2.Listen());
				WaitFor(() => node2.ChannelState == ChannelState.Listening);
				Task.Run(() => node3.Listen());
				WaitFor(() => node3.ChannelState == ChannelState.Listening);

				node1.Connect(host2, port2);
				WaitFor(() => node1.Channel.Peers.Count() == 1 && node2.Channel.Peers.Count() == 1);
				node2.Connect(host3, port3);
				WaitFor(() => node2.Channel.Peers.Count() == 2 && node3.Channel.Peers.Count() == 2);
				node3.Connect(host1, port1);
				WaitFor(() => node3.Channel.Peers.Count() == 2 && node1.Channel.Peers.Count() == 2);

				node1.Add(new Community {Address = new byte[] {1, 2, 3}});
				WaitFor(() => node1.Pendings.Count() == 1 && node2.Pendings.Count() == 1 && node3.Pendings.Count() == 1);

				Assert.AreEqual(1, node1.Pendings.Count());
				Assert.AreEqual(1, node2.Pendings.Count());
				Assert.AreEqual(1, node3.Pendings.Count());
			}
			finally
			{
				node1.Stop();
				WaitFor(() => node1.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node1.ChannelState);
				node2.Stop();
				WaitFor(() => node2.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node2.ChannelState);
				node3.Stop();
				WaitFor(() => node3.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node3.ChannelState);
			}
		}

		[Test]
		public void Compartiendo_pares_entre_3_nodos()
		{

			var logger1 = new ConsoleLogger {Tag = "node-1"};
			var logger2 = new ConsoleLogger {Tag = "node-2"};
			var logger3 = new ConsoleLogger {Tag = "node-3"};

			var node1 = new Node(new NodeConfiguration("Nodo 1", "1111", 1, 2000, port1), new BlockBuilder(), logger1);
			var node2 = new Node(new NodeConfiguration("Nodo 2", "2222", 1, 2000, port2), new BlockBuilder(), logger2);
			var node3 = new Node(new NodeConfiguration("Nodo 3", "3333", 1, 2000, port3), new BlockBuilder(), logger3);

			try
			{
				Task.Run(() => node1.Listen());
				WaitFor(() => node1.ChannelState == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, node1.ChannelState);
				
				Task.Run(() => node2.Listen());
				WaitFor(() => node2.ChannelState == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, node2.ChannelState);
				
				Task.Run(() => node3.Listen());
				WaitFor(() => node3.ChannelState == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, node3.ChannelState);

				Assert.AreEqual(0, node1.Channel.Peers.Count());
				Assert.AreEqual(0, node2.Channel.Peers.Count());
				Assert.AreEqual(0, node3.Channel.Peers.Count());

				node1.Connect(host2, port2);
				WaitFor(() => node2.Channel.Peers.Count() == 1);

				Assert.AreEqual(1, node1.Channel.Peers.Count());
				Assert.AreEqual(1, node2.Channel.Peers.Count());
				Assert.AreEqual(0, node3.Channel.Peers.Count());

				node2.Connect(host3, port3);
				WaitFor(() => node1.Channel.Peers.Count() == 1);
				WaitFor(() => node2.Channel.Peers.Count() == 2);
				WaitFor(() => node3.Channel.Peers.Count() == 1);

				node3.Discovery();
				WaitFor(() => node1.Channel.Peers.Count() == 2);
				WaitFor(() => node2.Channel.Peers.Count() == 2);
				WaitFor(() => node3.Channel.Peers.Count() == 2);

				Assert.AreEqual(2, node1.Channel.Peers.Count());
				Assert.AreEqual(2, node2.Channel.Peers.Count());
				Assert.AreEqual(2, node3.Channel.Peers.Count(), string.Join(", ", node3.Channel.Peers));
			}
			finally
			{
				node1.Stop();
				WaitFor(() => node1.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node1.ChannelState);
				node2.Stop();
				WaitFor(() => node2.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node2.ChannelState);
				node3.Stop();
				WaitFor(() => node3.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node3.ChannelState);
			}
		}

		[Test]
		public void Copiar_blockchain_de_otro_nodo_en_ejecucion_con_2_bloques()
		{
			var logger1 = new ConsoleLogger {Tag = "node-1"};
			var logger2 = new ConsoleLogger {Tag = "node-2"};

			var node1 = new Node(new NodeConfiguration("Nodo 1", "1111", 1, 2000, port1), new BlockBuilder(), logger1);
			var node2 = new Node(new NodeConfiguration("Nodo 2", "1234", 1, 2000, port2), new BlockBuilder(), logger2);

			try
			{
				Task.Run(() => node1.Listen());
				WaitFor(() => node1.ChannelState == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, node1.ChannelState);

				WaitFor(() => node1.ChainLength == 1);
				Assert.AreEqual(1, node1.ChainLength);

				var community = new Community {Id = Guid.NewGuid(), Name = "My Company", Address = new byte[] {1, 2, 3}};
				var keys = CryptoService.Instance.GeneratePair();
				signer.Sign(community, keys);

				node1.Add(community);
				node1.MinePendingTransactions();

				Assert.AreEqual(2, node1.ChainLength);

				Task.Run(() => node2.Listen());
				WaitFor(() => node2.ChannelState == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, node2.ChannelState);
				WaitFor(() => node2.ChainLength == 1);

				Assert.AreEqual(1, node2.ChainLength);
				node2.Connect(host1, port1);
				node2.Syncronize();
				WaitFor(() => node2.ChainLength == 2);

				Assert.AreEqual(2, node2.ChainLength);
			}
			finally
			{
				node1.Stop();
				WaitFor(() => node1.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node1.ChannelState);
				node2.Stop();
				WaitFor(() => node2.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node2.ChannelState);
			}
		}

		[Test]
		public void Copiar_blockchain_de_otro_nodo_en_ejecucion_con_3_bloques()
		{
			var owner = CryptoService.Instance.GeneratePair();
			var communityKeys = CryptoService.Instance.GeneratePair();

			var logger1 = new ConsoleLogger {Tag = "node-1"};
			var logger2 = new ConsoleLogger {Tag = "node-2"};

			var node1 = new Node(new NodeConfiguration("Nodo 1", "1234", 1, 2000, port1), new BlockBuilder(), logger1);
			var node2 = new Node(new NodeConfiguration("Nodo 2", "1234", 1, 2000, port2), new BlockBuilder(), logger2);

			try
			{
				Task.Run(() => node1.Listen());
				WaitFor(() => node1.ChannelState == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, node1.ChannelState); 
				
				WaitFor(() => node1.ChannelState == ChannelState.Listening);
				WaitFor(() => node1.ChainLength == 1);

				Assert.AreEqual(1, node1.ChainLength);

				var community = new Community {Id = Guid.NewGuid(), Name = "My Company", Address = new byte[] {1, 2, 3}};
				community.SetAddress(communityKeys.PublicKey);

				signer.Sign(community, owner);

				node1.Add(community);
				node1.MinePendingTransactions();

				Assert.AreEqual(2, node1.ChainLength);

				var question = new Question {Name = "My Question"};
				signer.Sign(question, communityKeys);
				node1.Add(question);
				node1.MinePendingTransactions();

				Assert.AreEqual(3, node1.ChainLength);

				Task.Run(() => node2.Listen());
				WaitFor(() => node2.ChannelState == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, node2.ChannelState); 
				
				WaitFor(() => node2.ChannelState == ChannelState.Listening);
				WaitFor(() => node2.ChainLength == 1);


				Assert.AreEqual(1, node2.ChainLength);
				node2.Connect(host1, port1);
				node2.Syncronize();

				WaitFor(() => node2.ChainLength == 3);

				Assert.AreEqual(3, node2.ChainLength);
			}
			finally
			{
				node1.Stop();
				WaitFor(() => node1.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node1.ChannelState);
				node2.Stop();
				WaitFor(() => node2.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node2.ChannelState);
			}
		}

		[Test]
		public void Copiar_blockchain_de_otros_nodos_en_ejecucion_x3_con_5_bloques()
		{
			var owner = CryptoService.Instance.GeneratePair();
			var communityKeys = CryptoService.Instance.GeneratePair();

			var logger1 = new ConsoleLogger {Tag = "node-1"};
			var logger2 = new ConsoleLogger {Tag = "node-2"};
			var logger3 = new ConsoleLogger {Tag = "node-3"};

			var node1 = new Node(new NodeConfiguration("Nodo 1", "1111", 1, 2000, port1), new BlockBuilder(), logger1);
			var node2 = new Node(new NodeConfiguration("Nodo 2", "2222", 1, 2000, port2), new BlockBuilder(), logger2);
			var node3 = new Node(new NodeConfiguration("Mi Nodo", "3333", 1, 2000, port3), new BlockBuilder(), logger3);

			try
			{
				Task.Run(() => node1.Listen());
				WaitFor(() => node1.ChannelState == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, node1.ChannelState);

				Task.Run(() => node2.Listen());
				WaitFor(() => node2.ChannelState == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, node2.ChannelState);

				node2.Connect(host1, port1);
				node2.Syncronize();
				WaitFor(() => node2.ChainLength == 1);

				Assert.AreEqual(1, node1.ChainLength);
				Assert.AreEqual(1, node2.ChainLength);

				var community = new Community {Id = Guid.NewGuid(), Name = "My Company", Address = new byte[] {1, 2, 3}};
				community.SetAddress(communityKeys.PublicKey);

				signer.Sign(community, owner);

				node1.Add(community);
				node1.MinePendingTransactions();

				Assert.AreEqual(2, node1.ChainLength);

				var question = new Question {Name = "My Question"};
				signer.Sign(question, communityKeys);
				node1.Add(question);
				node1.MinePendingTransactions();

				Assert.AreEqual(3, node1.ChainLength);
				Assert.AreEqual(1, node3.ChainLength);

				Task.Run(() => node3.Listen());
				WaitFor(() => node3.ChannelState == ChannelState.Listening);
				Assert.AreEqual(ChannelState.Listening, node1.ChannelState);

				node3.Connect(host1, port1);
				WaitFor(() => node3.Channel.Peers.Count() == 1);
				node3.Connect(host2, port2);
				WaitFor(() => node3.Channel.Peers.Count() == 2);
				
				node3.Syncronize();
				WaitFor(() => node3.ChainLength == 3);
				Assert.AreEqual(3, node3.ChainLength);

			}
			finally
			{
				node1.Stop();
				WaitFor(() => node1.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node1.ChannelState);
				node2.Stop();
				WaitFor(() => node2.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node2.ChannelState);
				node3.Stop();
				WaitFor(() => node3.ChannelState != ChannelState.Listening);
				Assert.AreEqual(ChannelState.Stop, node3.ChannelState);
			}
		}
	}
}