﻿using Domain.Channels;
using Domain.Elections;

namespace Domain
{
	public interface INode
	{
		Blockchain Blockchain { get; }

		void Start();

		void Add(Document document);
		void Add(Community community);
		void Add(Member member);
		void Add(Question question);
		void Add(Vote vote);
		void Add(Fiscal fiscal);
		void Add(Urn urn);
		void Add(Recount recount);
		void Add(Block block, TcpPeer peer);
	}
}