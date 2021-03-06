﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Elections;
using Microsoft.Extensions.Logging;

namespace Domain
{
	public class Peers
	{
		private readonly INode node;
		private readonly IPeerChannel channel;
		private readonly ILogger<Peers> logger;
		private readonly IList<Peer> others = new List<Peer>();

		public Peers(INode node, IPeerChannel channel, ILogger<Peers> logger)
		{
			this.node = node;
			this.channel = channel;
			this.logger = logger;
		}

		public void GetLastBlock()
		{
			foreach (var other in others)
			{
				var block = channel.GetLastBlock(other.PublicUrl);
				if (block != null)
				{
					node.Add(block);
					other.LastActivity= DateTimeOffset.UtcNow;
				}
			}
		}

		public void Discovery()
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
							Add(peer);
						}
					}
					target.LastActivity = DateTimeOffset.UtcNow;
				}
			}
		}

		public void Broadcast(Block block)
		{
			logger.LogInformation($"Enviando block a {others.Count} pares");
			foreach (var other in others)
			{
				channel.Send(other.PublicUrl, block);
				other.LastActivity = DateTimeOffset.UtcNow;
			}
		}

		public void Broadcast(Community community)
		{
			logger.LogInformation($"Enviando community a {others.Count} pares");
			foreach (var other in others)
			{
				channel.Send(other.PublicUrl, community);
				other.LastActivity = DateTimeOffset.UtcNow;
			}
		}

		public void Broadcast(Question question)
		{
			logger.LogInformation($"Enviando question a {others.Count} pares");
			foreach (var other in others)
			{
				channel.Send(other.PublicUrl, question);
				other.LastActivity = DateTimeOffset.UtcNow;
			}
		}

		public void Broadcast(Member member)
		{
			logger.LogInformation($"Enviando member a {others.Count} pares");
			foreach (var other in others)
			{
				channel.Send(other.PublicUrl, member);
				other.LastActivity = DateTimeOffset.UtcNow;
			}
		}

		public void Broadcast(Document document)
		{
			logger.LogInformation($"Enviando document a {others.Count} pares");
			foreach (var other in others)
			{
				channel.Send(other.PublicUrl, document);
				other.LastActivity = DateTimeOffset.UtcNow;
			}
		}

		public void Broadcast(Vote vote)
		{
			logger.LogInformation($"Enviando vote a {others.Count} pares");
			foreach (var other in others)
			{
				channel.Send(other.PublicUrl, vote);
				other.LastActivity = DateTimeOffset.UtcNow;
			}
		}

		public void Broadcast(Fiscal fiscal)
		{
			logger.LogInformation($"Enviando fiscal a {others.Count} pares");
			foreach (var other in others)
			{
				channel.Send(other.PublicUrl, fiscal);
				other.LastActivity = DateTimeOffset.UtcNow;
			}
		}

		public void Broadcast(Urn urn)
		{
			logger.LogInformation($"Enviando urn a {others.Count} pares");
			foreach (var other in others)
			{
				channel.Send(other.PublicUrl, urn);
				other.LastActivity = DateTimeOffset.UtcNow;
			}
		}

		public void Broadcast(Recount recount)
		{
			logger.LogInformation($"Enviando recount a {others.Count} pares");
			foreach (var other in others)
			{
				channel.Send(other.PublicUrl, recount);
				other.LastActivity = DateTimeOffset.UtcNow;
			}
		}

		public void Broadcast(Peer peer)
		{
			logger.LogInformation($"Enviando peer a {others.Count} pares");
			foreach (var other in others)
			{
				channel.Send(other.PublicUrl, peer);
				other.LastActivity = DateTimeOffset.UtcNow;
			}
		}

		public void GetBlock(byte[] hash)
		{
			foreach (var other in others)
			{
				var block = channel.GetBlock(other.PublicUrl, hash);
				if (block != null)
				{
					node.Add(block);
					other.LastActivity = DateTimeOffset.UtcNow;
				}
			}
		}

		public int Count => others.Count;

		public void Add(Peer peer)
		{
			if (peer.PublicUrl.Equals(node.Host.PublicUrl)) return;
			if (others.Any(o => o.PublicUrl == peer.PublicUrl)) return;

			others.Add(peer);
			channel.Send(peer.PublicUrl, node.Host);
			//Broadcast(peer);
		}

		public IList<Peer> List()
		{
			return others;
		}

		public Peer GetNodeInfo(string url)
		{
			return channel.GetNodeInfo(url);
		}

		public bool Contains(string peerUrl)
		{
			return others.Any(p => p.PublicUrl == peerUrl);
		}
	}
}