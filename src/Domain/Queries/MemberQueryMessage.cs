using System;
using System.Text;
using Domain.Protocol;
using Domain.Scrutiny;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class MemberQueryMessage : IMessage<Member>
	{
		private readonly Guid communityId;
		private readonly Guid memberId;

		public MemberQueryMessage(Guid communityId, Guid memberId)
		{
			this.communityId = communityId;
			this.memberId = memberId;
		}
		public byte[] GetBytes()
		{
			var message = $"Member#{communityId}#{memberId}";
			return Encoding.UTF8.GetBytes($"{QueryCommand.CommandId}:{message.Length:D5}|{message}");
		}

		public Member Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			if (!string.IsNullOrWhiteSpace(body))
				return JsonConvert.DeserializeObject<Member>(body);
			return null;
		}
	}
}