using System;
using System.Threading.Tasks;
using Domain.Protocol;
using Domain.Queries;
using Domain.Scrutiny;

namespace Web.Controllers
{
	public class NodeAdapterFiscals
	{
		private readonly NodeAdapter nodeAdapter;

		public NodeAdapterFiscals(NodeAdapter nodeAdapter)
		{
			this.nodeAdapter = nodeAdapter;
		}

		public async Task<Fiscal[]> List(Guid issueId)
		{
			var command = new FiscalsQueryMessage(issueId);
			return await nodeAdapter.GetResponse(command);
		}

		public async Task Add(Fiscal fiscal)
		{
			var command = new SendFiscalMessage(fiscal).GetBytes();
			await nodeAdapter.GetResponse(command);
		}
	}
}