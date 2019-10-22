using System;
using System.Threading.Tasks;
using Domain.Protocol;
using Domain.Queries;
using Domain.Scrutiny;

namespace Web.Controllers
{
	public class NodeAdapterMembers
	{
		private readonly NodeAdapter nodeAdapter;

		public NodeAdapterMembers(NodeAdapter nodeAdapter)
		{
			this.nodeAdapter = nodeAdapter;
		}

		public async Task<Member[]> ListMembers(Guid communityId)
		{
			var command = new MembersQueryMessage(communityId);
			return await nodeAdapter.GetResponse(command);
		}

		public async Task<Member> GetMember(Guid communityId, Guid memberId)
		{
			var command = new MemberQueryMessage(communityId, memberId);
			return await nodeAdapter.GetResponse(command);
		}

		public async Task AddMember(Member member)
		{
			var command = new SendMemberMessage(member).GetBytes();
			await nodeAdapter.GetResponse(command);
		}
	}
}