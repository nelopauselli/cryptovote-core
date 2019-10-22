using System;
using System.Text;
using Domain.Protocol;
using Domain.Scrutiny;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class UrnsQueryMessage : IMessage<Urn[]>
	{
		private readonly Guid issueId;

		public UrnsQueryMessage(Guid issueId)
		{
			this.issueId = issueId;
		}
		public byte[] GetBytes()
		{
			var message = $"Urns#{issueId}";
			return Encoding.UTF8.GetBytes($"{QueryCommand.CommandId}:{message.Length:D5}|{message}");
		}

		public Urn[] Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			if (!string.IsNullOrWhiteSpace(body))
				return JsonConvert.DeserializeObject<Urn[]>(body);

			return Array.Empty<Urn>();
		}
	}
}