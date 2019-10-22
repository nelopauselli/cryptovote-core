using System;
using System.Text;
using Domain.Protocol;
using Domain.Scrutiny;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class IssueQueryMessage : IMessage<Issue>
	{
		private readonly Guid communityId;
		private readonly Guid issueId;

		public IssueQueryMessage(Guid communityId, Guid issueId)
		{
			this.communityId = communityId;
			this.issueId = issueId;
		}
		public byte[] GetBytes()
		{
			var message = $"Issue#{communityId}#{issueId}";
			return Encoding.UTF8.GetBytes($"{QueryCommand.CommandId}:{message.Length:D5}|{message}");
		}

		public Issue Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			if (!string.IsNullOrWhiteSpace(body))
				return JsonConvert.DeserializeObject<Issue>(body);
			return null;
		}
	}
}