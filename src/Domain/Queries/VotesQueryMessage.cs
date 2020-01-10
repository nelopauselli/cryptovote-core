using System;
using System.Text;
using System.Text.Json;
using Domain.Protocol;
using Domain.Elections;

namespace Domain.Queries
{
	public class VotesQueryMessage : IMessage<Vote[]>
	{
		private readonly Guid questionId;

		public VotesQueryMessage(Guid questionId)
		{
			this.questionId = questionId;
		}
		public byte[] GetBytes()
		{
			var message = $"votes#{questionId}";
			return Encoding.UTF8.GetBytes($"{QueryCommand.CommandId}:{message.Length:D5}|{message}");
		}

		public Vote[] Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			if (!string.IsNullOrWhiteSpace(body))
				return JsonSerializer.Deserialize<Vote[]>(body);

			return Array.Empty<Vote>();
		}
	}
}