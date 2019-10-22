﻿using System.Threading.Tasks;
using Domain;
using Domain.Queries;
using Newtonsoft.Json;

namespace Web.Controllers
{
	public class NodeAdapterBlocks
	{
		private readonly NodeAdapter nodeAdapter;

		public NodeAdapterBlocks(NodeAdapter nodeAdapter)
		{
			this.nodeAdapter = nodeAdapter;
		}

		public async Task<Block> GetLastBlock()
		{
			var query = new LastBlockQueryMessage();
			var block = await nodeAdapter.GetResponse(query);
			return block;
		}

		public async Task<Block> GetBlockByHash(byte[] hash)
		{
			var command = new BlockQueryMessage(hash);
			return await nodeAdapter.GetResponse(command);
		}
	}
}