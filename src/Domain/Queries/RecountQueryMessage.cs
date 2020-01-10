using System;
using System.Text;
using System.Text.Json;
using Domain.Converters;
using Domain.Protocol;
using Domain.Elections;

namespace Domain.Queries
{
	public class RecountQueryMessage : IMessage<Recount>
	{
		private readonly Guid questionId;

		public RecountQueryMessage(Guid questionId)
		{
			this.questionId = questionId;
		}
		public byte[] GetBytes()
		{
			var message = $"recount#{questionId}";
			return Encoding.UTF8.GetBytes($"{QueryCommand.CommandId}:{message.Length:D5}|{message}");
		}

		public Recount Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			return !string.IsNullOrWhiteSpace(body) ? JsonSerializer.Deserialize<Recount>(body, JsonDefaultSettings.Options) : null;
		}
	}
}