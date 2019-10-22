using System;
using System.Text;
using Domain.Protocol;
using Domain.Scrutiny;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class MembersQueryMessage : IMessage<Member[]>
	{
		private readonly Guid communityId;

		public MembersQueryMessage(Guid communityId)
		{
			this.communityId = communityId;
		}

		public byte[] GetBytes()
		{
			var message = $"Members#{communityId}";
			return Encoding.UTF8.GetBytes($"Q:{message.Length:D5}|{message}");
		}

		public Member[] Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			if (!string.IsNullOrWhiteSpace(body))
				return JsonConvert.DeserializeObject<Member[]>(body);

			return Array.Empty<Member>();
		}
	}
}