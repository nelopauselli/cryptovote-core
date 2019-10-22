﻿using System;
using System.Threading.Tasks;
using Domain.Protocol;
using Domain.Queries;
using Domain.Scrutiny;

namespace Web.Controllers
{
	public class NodeAdapterUrns
	{
		private readonly NodeAdapter nodeAdapter;

		public NodeAdapterUrns(NodeAdapter nodeAdapter)
		{
			this.nodeAdapter = nodeAdapter;
		}

		public async Task<Urn[]> List(Guid issueId)
		{
			var command = new UrnsQueryMessage(issueId);
			return await nodeAdapter.GetResponse(command);
		}

		public async Task Add(Urn urn)
		{
			var command = new SendUrnMessage(urn).GetBytes();
			await nodeAdapter.GetResponse(command);
		}
	}
}