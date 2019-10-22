using System;
using System.Text;
using Domain.Protocol;
using Domain.Elections;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class CommunityQueryMessage : IMessage<Community>
	{
		private readonly Guid communityId;

		public CommunityQueryMessage(Guid communityId)
		{
			this.communityId = communityId;
		}

		public byte[] GetBytes()
		{
			var message = $"community#{communityId}";
			return Encoding.UTF8.GetBytes($"{QueryCommand.CommandId}:{message.Length:D5}|{message}");
		}

		public Community Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			if (!string.IsNullOrWhiteSpace(body))
				return JsonConvert.DeserializeObject<Community>(body);
			return null;
		}
	}
}