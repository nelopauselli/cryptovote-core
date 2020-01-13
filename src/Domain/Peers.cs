using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Elections;

namespace Domain
{
	public class Peers
	{
		private readonly INode node;
		private readonly IPeerChannel channel;
		private readonly IList<PeerInfo> others=new List<PeerInfo>();

		public Peers(INode node, IPeerChannel channel)
		{
			this.node = node;
			this.channel = channel;
		}

		public void Connect(string publicUrl, string url)
		{
			if (publicUrl.Equals(url)) return;
			if (others.Any(o => o.PublicUrl == url)) return;

			others.Add(new PeerInfo {PublicUrl = url});
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
			var targets = new List<PeerInfo>(others);
			
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

		public void Broadcast(Member community)
		{
			throw new System.NotImplementedException();
		}

		public void Broadcast(Document community)
		{
			throw new System.NotImplementedException();
		}

		public void Broadcast(Vote community)
		{
			throw new System.NotImplementedException();
		}

		public void Broadcast(Fiscal community)
		{
			throw new System.NotImplementedException();
		}

		public void Broadcast(Urn community)
		{
			throw new System.NotImplementedException();
		}

		public void Broadcast(Recount community)
		{
			throw new System.NotImplementedException();
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

		public void Add(PeerInfo peer)
		{
			others.Add(peer);
		}

		public IList<PeerInfo> List()
		{
			return others;
		}
	}

	public interface IPeerChannel
	{
		void Connect(string myPublicUrl, string targetPublicUrl);
		IList<PeerInfo> ListPeers(string publicUrl);
		Block GetLastBlock(string publicUrl);
		Block GetBlock(string publicUrl, byte[] hash);

		void Send(string publicUrl, Block block);
		void Send(string publicUrl, Community community);
		void Send(string publicUrl, Question question);
	}
}