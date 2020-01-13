using System.Collections.Generic;
using System.Linq;
using Domain.Elections;

namespace Domain
{
	public class Peers
	{
		private readonly INode node;
		private readonly IPeerChannel channel;
		private readonly IList<Peer> others = new List<Peer>();

		public Peers(INode node, IPeerChannel channel)
		{
			this.node = node;
			this.channel = channel;
		}

		public void Connect(string publicUrl, string url)
		{
			if (publicUrl.Equals(url)) return;
			if (others.Any(o => o.PublicUrl == url)) return;

			others.Add(new Peer {PublicUrl = url});
			channel.Connect(publicUrl, url);
		}


		public void GetLastBlock()
		{
			foreach (var other in others)
			{
				var block = channel.GetLastBlock(other.PublicUrl);
				if (block != null)
					node.Add(block);
			}
		}

		public void Discovery(string publicUrl)
		{
			var targets = new List<Peer>(others);
			
			foreach (var target in targets)
			{
				var peers = channel.ListPeers(target.PublicUrl);
				if (peers != null)
				{
					foreach (var peer in peers)
					{
						if (others.All(o => o.PublicUrl != peer.PublicUrl))
						{
							Connect(publicUrl, peer.PublicUrl);
						}
					}
				}
			}
		}

		public void Broadcast(Block block)
		{
			foreach (var other in others)
				channel.Send(other.PublicUrl, block);
		}

		public void Broadcast(Community community)
		{
			foreach (var other in others)
				channel.Send(other.PublicUrl, community);
		}

		public void Broadcast(Question question)
		{
			foreach (var other in others)
				channel.Send(other.PublicUrl, question);
		}

		public void Broadcast(Member member)
		{
			foreach (var other in others)
				channel.Send(other.PublicUrl, member);
		}

		public void Broadcast(Document document)
		{
			foreach (var other in others)
				channel.Send(other.PublicUrl, document);
		}

		public void Broadcast(Vote vote)
		{
			foreach (var other in others)
				channel.Send(other.PublicUrl, vote);
		}

		public void Broadcast(Fiscal fiscal)
		{
			foreach (var other in others)
				channel.Send(other.PublicUrl, fiscal);
		}

		public void Broadcast(Urn urn)
		{
			foreach (var other in others)
				channel.Send(other.PublicUrl, urn);
		}

		public void Broadcast(Recount recount)
		{
			foreach (var other in others)
				channel.Send(other.PublicUrl, recount);
		}

		public void Broadcast(Peer peer)
		{
			foreach (var other in others)
				channel.Send(other.PublicUrl, peer);
		}

		public void GetBlock(byte[] hash)
		{
			foreach (var other in others)
			{
				var block = channel.GetBlock(other.PublicUrl, hash);
				if (block != null)
					node.Add(block);
			}
		}

		public int Count => others.Count;

		public void Add(Peer peer)
		{
			if (others.All(o => o.PublicUrl != peer.PublicUrl))
			{
				others.Add(peer);
				Broadcast(peer);
			}
		}

		public IList<Peer> List()
		{
			return others;
		}
	}
}