using System;
using System.Linq;
using System.Threading;
using Domain;
using Domain.Crypto;
using Domain.Elections;
using Domain.Queries;
using NUnit.Framework;
using Tests.Mocks;

namespace Tests
{
	public class Node_syncronize_over_tcp : TcpTestBase
	{
		private readonly Signer signer = new Signer(CryptoService.Instance);
		
		[Test]
		public void Copiar_blockchain_de_otro_nodo_en_ejecucion_con_2_bloques()
		{
			var logger1 = new ConsoleLogger {Tag = "node-1"};
			var logger2 = new ConsoleLogger {Tag = "node-2" };

			var node1 = new Node(new NodeConfiguration("Nodo 1", "1234", 1, 2000, port:port1), new BlockBuilder(), logger1);
			node1.Listen();
			Assert.AreEqual(1, node1.ChainLength);

			var community = new Community { Id = Guid.NewGuid(), Name = "My Company", Address = new byte[] { 1, 2, 3 } };
			var keys = CryptoService.Instance.GeneratePair();
			signer.Sign(community, keys);

			node1.Add(community);
			node1.MinePendingTransactions();

			Assert.AreEqual(2, node1.ChainLength);

			var node2 = new Node(new NodeConfiguration("Nodo 2", "1234", 1, 2000, port: port2), new BlockBuilder(), logger2);
			node2.Listen();
			Assert.AreEqual(1, node2.ChainLength);
			node2.Register(host1, port1);
			Thread.Sleep(1000);
			
			node2.Syncronize();
			Thread.Sleep(2000);

			Assert.AreEqual(2, node2.ChainLength);

			node1.Stop();
			node2.Stop();
		}

		[Test]
		public void Establecer_conexion_entre_pares()
		{
			var node1 = new Node(new NodeConfiguration("Nodo 1", "1234", 1, 2000, port: port1), new BlockBuilder(), new ConsoleLogger { Tag = "node-1" });
			node1.Listen();

			var node2 = new Node(new NodeConfiguration("Nodo 2", "1234", 1, 2000, port: port2), new BlockBuilder(), new ConsoleLogger { Tag = "node-2" });
			node2.Listen();

			node2.Register(host1, port1);

			Thread.Sleep(1000);

			Assert.AreEqual(1, node1.Peers.Count, $"Pares: {string.Join(", ", node1.Peers.Hosts.Select(p => $"{p.Host}:{p.Port}"))}");
			Assert.AreEqual(1, node2.Peers.Count, $"Pares: {string.Join(", ", node2.Peers.Hosts.Select(p => $"{p.Host}:{p.Port}"))}");

			node1.Stop();
			node2.Stop();
		}

		//[Test]
		//public void El_canal_debe_quedar_abierto()
		//{
		//	var node1 = new Node(new NodeConfiguration("Nodo 1", "1234", 1, 2000, port: port1), new BlockBuilder(), new ConsoleLogger { Tag = "node-1" });
		//	node1.Listen();

		//	var node2 = new Node(new NodeConfiguration("Nodo 2", "1234", 1, 2000, port: port2), new BlockBuilder(), new ConsoleLogger { Tag = "node-2" });
		//	node2.Listen();

		//	node2.Register(host1, port1);

		//	Thread.Sleep(1000);

		//	Assert.AreEqual(1, node1.Peers.Count, $"Pares: {string.Join(", ", node1.Peers.Hosts.Select(p => $"{p.Host}:{p.Port}"))}");
		//	Assert.AreEqual(1, node2.Peers.Count, $"Pares: {string.Join(", ", node2.Peers.Hosts.Select(p => $"{p.Host}:{p.Port}"))}");

		//	var peerNode1ToNode2 = node1.Peers.Hosts.First();
		//	var peerNode2ToNode1 = node2.Peers.Hosts.First();

		//	var data = new LastBlockQueryMessage().GetBytes();
		//	var cts = new CancellationTokenSource();
		//	peerNode2ToNode1.SendAsync(data, cts.Token);
		//	Assert


		//	node1.Stop();
		//	node2.Stop();
		//}
	}
}