using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Utils;

namespace Domain.Queries
{
	public class MembersQueryMiddleware : IMiddleware
	{
		private readonly INode node;

		public MembersQueryMiddleware(INode node)
		{
			this.node = node;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.QueryMembers) return false;

			var request = new byte[header.Length];
			peer.Read(request, 0, header.Length);

			var communityId = request.ToGuid();

			var membersQuery = new MembersQuery(node.Blockchain);
			var members = membersQuery.Execute(communityId);
			var response = Serializer.GetBytes(members);
			peer.Send(new QueryResponseCommand(response));

			return true;
		}
	}

	public class MemberQueryMiddleware : IMiddleware
	{
		private readonly INode node;

		public MemberQueryMiddleware(INode node)
		{
			this.node = node;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.QueryMember) return false;

			var communityRequest = new byte[16];
			var memberRequest = new byte[16];
			peer.Read(communityRequest, 0, 16);
			peer.Read(memberRequest, 0, 16);

			var communityId = communityRequest.ToGuid();
			var memberId = memberRequest.ToGuid();

			var memberQuery = new MemberQuery(node.Blockchain);
			var member = memberQuery.Execute(communityId, memberId);
			var response = Serializer.GetBytes(member);
			peer.Send(new QueryResponseCommand(response));

			return true;
		}
	}

	public class QuestionsQueryMiddleware : IMiddleware
	{
		private readonly INode node;

		public QuestionsQueryMiddleware(INode node)
		{
			this.node = node;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.QueryQuestions) return false;

			var request = new byte[header.Length];
			peer.Read(request, 0, header.Length);

			var communityId = request.ToGuid();

			var membersQuery = new QuestionsQuery(node.Blockchain);
			var members = membersQuery.Execute(communityId);
			var response = Serializer.GetBytes(members);
			peer.Send(new QueryResponseCommand(response));

			return true;
		}
	}

	public class QuestionQueryMiddleware : IMiddleware
	{
		private readonly INode node;

		public QuestionQueryMiddleware(INode node)
		{
			this.node = node;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.QueryQuestion) return false;

			var communityRequest = new byte[16];
			var questionRequest = new byte[16];
			peer.Read(communityRequest, 0, 16);
			peer.Read(questionRequest, 0, 16);

			var communityId = communityRequest.ToGuid();
			var questionId = questionRequest.ToGuid();

			var memberQuery = new QuestionQuery(node.Blockchain);
			var member = memberQuery.Execute(communityId, questionId);
			var response = Serializer.GetBytes(member);
			peer.Send(new QueryResponseCommand(response));

			return true;
		}
	}
}