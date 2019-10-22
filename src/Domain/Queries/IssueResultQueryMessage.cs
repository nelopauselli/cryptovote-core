using System;
using System.Text;
using Domain.Protocol;
using Domain.Elections;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class IssueResultQueryMessage : IMessage<IssueResult>
	{
		private readonly Guid communityId;
		private readonly Guid issueId;

		public IssueResultQueryMessage(Guid communityId, Guid issueId)
		{
			this.communityId = communityId;
			this.issueId = issueId;
		}
		public byte[] GetBytes()
		{
			var message = $"Issue-Result#{communityId}#{issueId}";
			return Encoding.UTF8.GetBytes($"{QueryCommand.CommandId}:{message.Length:D5}|{message}");
		}

		public IssueResult Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			if (!string.IsNullOrWhiteSpace(body))
				return JsonConvert.DeserializeObject<IssueResult>(body);
			return null;
		}
	}
}