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

		public void Connect(string myPublicUrl, string targetPublicUrl)
		{
			var other = nodes.SingleOrDefault(n => n.PublicUrl == targetPublicUrl);
			other?.Add(new PeerInfo{PublicUrl = myPublicUrl});
		}

		public IList<PeerInfo> ListPeers(string publicUrl)
		{
			var node = nodes.SingleOrDefault(n => n.PublicUrl == publicUrl);
			return node?.Peers.List();
		}

		
		public void Send(string publicUrl, Block block)
		{
			var node = nodes.SingleOrDefault(n => n.PublicUrl == publicUrl);
			node?.Add(block);
		}

		public void Send(string publicUrl, Community community)
		{
			var node = nodes.SingleOrDefault(n => n.PublicUrl == publicUrl);
			node?.Add(community);
		}

		public void Send(string publicUrl, Question question)
		{
			var node = nodes.SingleOrDefault(n => n.PublicUrl == publicUrl);
			node?.Add(question);
		}

		public Block GetLastBlock(string publicUrl)
		{
			var node = nodes.SingleOrDefault(n => n.PublicUrl == publicUrl);
			return node?.Blockchain.Last;
		}

		public Block GetBlock(string publicUrl, byte[] hash)
		{
			var node = nodes.SingleOrDefault(n => n.PublicUrl == publicUrl);
			return node?.Blockchain.GetBlock(hash);
		}

	}
}