using System;
using System.Text;
using Domain.Protocol;
using Domain.Elections;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class UrnsQueryMessage : IMessage<Urn[]>
	{
		private readonly Guid questionId;

		public UrnsQueryMessage(Guid questionId)
		{
			this.questionId = questionId;
		}
		public byte[] GetBytes()
		{
			var message = $"Urns#{questionId}";
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