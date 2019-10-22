using System;
using System.Threading.Tasks;
using Domain.Protocol;
using Domain.Queries;
using Domain.Elections;

namespace Web.Controllers
{
	public class NodeAdapterUrns
	{
		private readonly NodeAdapter nodeAdapter;

		public NodeAdapterUrns(NodeAdapter nodeAdapter)
		{
			this.nodeAdapter = nodeAdapter;
		}

		public async Task<Urn[]> List(Guid questionId)
		{
			var command = new UrnsQueryMessage(questionId);
			return await nodeAdapter.GetResponse(command);
		}

		public async Task Add(Urn urn)
		{
			var command = new SendUrnMessage(urn).GetBytes();
			await nodeAdapter.GetResponse(command);
		}
	}
}