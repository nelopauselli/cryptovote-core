using System;
using System.Text;
using Domain.Protocol;
using Domain.Elections;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class VotesQueryMessage : IMessage<Vote[]>
	{
		private readonly Guid issueId;

		public VotesQueryMessage(Guid issueId)
		{
			this.issueId = issueId;
		}
		public byte[] GetBytes()
		{
			var message = $"votes#{issueId}";
			return Encoding.UTF8.GetBytes($"{QueryCommand.CommandId}:{message.Length:D5}|{message}");
		}

		public Vote[] Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			if (!string.IsNullOrWhiteSpace(body))
				return JsonConvert.DeserializeObject<Vote[]>(body);

			return Array.Empty<Vote>();
		}
	}
}