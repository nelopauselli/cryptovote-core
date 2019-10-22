using System;
using System.Threading.Tasks;
using Domain.Protocol;
using Domain.Queries;
using Domain.Elections;

namespace Web.Controllers
{
	public class NodeAdapterFiscals
	{
		private readonly NodeAdapter nodeAdapter;

		public NodeAdapterFiscals(NodeAdapter nodeAdapter)
		{
			this.nodeAdapter = nodeAdapter;
		}

		public async Task<Fiscal[]> List(Guid questionId)
		{
			var command = new FiscalsQueryMessage(questionId);
			return await nodeAdapter.GetResponse(command);
		}

		public async Task Add(Fiscal fiscal)
		{
			var command = new SendFiscalMessage(fiscal).GetBytes();
			await nodeAdapter.GetResponse(command);
		}
	}
}