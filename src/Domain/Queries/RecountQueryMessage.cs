using System;
using System.Text;
using Domain.Protocol;
using Domain.Scrutiny;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class RecountQueryMessage : IMessage<Recount>
	{
		private readonly Guid issueId;

		public RecountQueryMessage(Guid issueId)
		{
			this.issueId = issueId;
		}
		public byte[] GetBytes()
		{
			var message = $"recount#{issueId}";
			return Encoding.UTF8.GetBytes($"{QueryCommand.CommandId}:{message.Length:D5}|{message}");
		}

		public Recount Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			return !string.IsNullOrWhiteSpace(body) ? JsonConvert.DeserializeObject<Recount>(body) : null;
		}
	}
}