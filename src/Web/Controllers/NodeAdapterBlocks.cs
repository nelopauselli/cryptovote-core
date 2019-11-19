using System.Threading.Tasks;
using Domain;
using Domain.Queries;

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
			var query = new LastBlockQueryCommand();
			var block = await nodeAdapter.GetResponse(query);
			return block;
		}

		public async Task<Block> GetBlockByHash(byte[] hash)
		{
			var command = new BlockQueryCommand(hash);
			return await nodeAdapter.GetResponse(command);
		}
	}
}