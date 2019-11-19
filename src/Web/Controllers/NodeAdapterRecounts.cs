using System;
using System.Threading.Tasks;
using Domain.Protocol;
using Domain.Queries;
using Domain.Elections;

namespace Web.Controllers
{
	public class NodeAdapterRecounts
	{
		private readonly NodeAdapter nodeAdapter;

		public NodeAdapterRecounts(NodeAdapter nodeAdapter)
		{
			this.nodeAdapter = nodeAdapter;
		}

		public async Task<Recount> Get(Guid urnId)
		{
			var command = new RecountQueryMessage(urnId);
			return await nodeAdapter.GetResponse(command);
		}

		public async Task Add(Recount recount)
		{
			var command = new SendRecountCommand(recount);
			await nodeAdapter.Send(command);
		}
	}
}