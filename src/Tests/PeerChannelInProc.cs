using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Elections;

namespace Tests
{
	public class PeerChannelInProc : IPeerChannel
	{
		private readonly IList<Node> nodes = new List<Node>();

		public void Add(Node node)
		{
			nodes.Add(node);
		}

		public IList<Peer> ListPeers(string publicUrl)
		{
			var node = nodes.SingleOrDefault(n => n.Host.PublicUrl == publicUrl);
			return node?.Peers.List();
		}

		
		public void Send(string publicUrl, Block block)
		{
			var node = nodes.SingleOrDefault(n => n.Host.PublicUrl == publicUrl);
			node?.Add(block);
		}

		public void Send(string publicUrl, Community community)
		{
			var node = nodes.SingleOrDefault(n => n.Host.PublicUrl == publicUrl);
			node?.Add(community);
		}

		public void Send(string publicUrl, Question question)
		{
			var node = nodes.SingleOrDefault(n => n.Host.PublicUrl == publicUrl);
			node?.Add(question);
		}

		public void Send(string publicUrl, Member member)
		{
			throw new System.NotImplementedException();
		}

		public void Send(string publicUrl, Document document)
		{
			throw new System.NotImplementedException();
		}

		public void Send(string publicUrl, Vote vote)
		{
			throw new System.NotImplementedException();
		}

		public void Send(string publicUrl, Fiscal fiscal)
		{
			throw new System.NotImplementedException();
		}

		public void Send(string publicUrl, Urn urn)
		{
			throw new System.NotImplementedException();
		}

		public void Send(string publicUrl, Recount recount)
		{
			throw new System.NotImplementedException();
		}

		public Peer GetNodeInfo(string publicUrl)
		{
			var other = nodes.SingleOrDefault(n => n.Host.PublicUrl == publicUrl);
			return other?.Host;
		}

		public void Send(string publicUrl, Peer peer)
		{
			var other = nodes.SingleOrDefault(n => n.Host.PublicUrl == publicUrl);
			other?.Register(peer);
		}

		public Block GetLastBlock(string publicUrl)
		{
			var node = nodes.SingleOrDefault(n => n.Host.PublicUrl == publicUrl);
			return node?.Blockchain.Last;
		}

		public Block GetBlock(string publicUrl, byte[] hash)
		{
			var node = nodes.SingleOrDefault(n => n.Host.PublicUrl == publicUrl);
			return node?.Blockchain.GetBlock(hash);
		}

	}
}