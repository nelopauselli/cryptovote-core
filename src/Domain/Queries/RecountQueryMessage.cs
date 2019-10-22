using System;
using System.Text;
using Domain.Protocol;
using Domain.Elections;
using Newtonsoft.Json;

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
			return !string.IsNullOrWhiteSpace(body) ? JsonConvert.DeserializeObject<Recount>(body) : null;
		}
	}
}