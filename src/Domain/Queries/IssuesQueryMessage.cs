using System;
using System.Text;
using Domain.Protocol;
using Domain.Elections;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class IssuesQueryMessage : IMessage<Issue[]>
	{
		private readonly Guid communityId;

		public IssuesQueryMessage(Guid communityId)
		{
			this.communityId = communityId;
		}
		public byte[] GetBytes()
		{
			var message = $"Issues#{communityId}";
			return Encoding.UTF8.GetBytes($"{QueryCommand.CommandId}:{message.Length:D5}|{message}");
		}

		public Issue[] Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			if (!string.IsNullOrWhiteSpace(body))
				return JsonConvert.DeserializeObject<Issue[]>(body);

			return Array.Empty<Issue>();
		}
	}
}