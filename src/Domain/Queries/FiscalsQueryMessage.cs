using System;
using System.Text;
using System.Text.Json;
using Domain.Converters;
using Domain.Protocol;
using Domain.Elections;

namespace Domain.Queries
{
	public class FiscalsQueryMessage : IMessage<Fiscal[]>
	{
		private readonly Guid questionId;

		public FiscalsQueryMessage(Guid questionId)
		{
			this.questionId = questionId;
		}
		public byte[] GetBytes()
		{
			var message = $"Fiscals#{questionId}";
			return Encoding.UTF8.GetBytes($"{QueryCommand.CommandId}:{message.Length:D5}|{message}");
		}

		public Fiscal[] Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			if (!string.IsNullOrWhiteSpace(body))
				return JsonSerializer.Deserialize<Fiscal[]>(body, JsonDefaultSettings.Options);

			return Array.Empty<Fiscal>();
		}
	}
}