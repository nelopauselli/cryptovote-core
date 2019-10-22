using System;
using System.Text;
using Domain.Protocol;
using Domain.Scrutiny;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class CommunitiesQueryMessage : IMessage<Community[]>
	{
		public byte[] GetBytes()
		{
			var message = "Communities";
			return Encoding.UTF8.GetBytes($"{QueryCommand.CommandId}:{message.Length:D5}|{message}");
		}

		public Community[] Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			if (!string.IsNullOrWhiteSpace(body))
				return JsonConvert.DeserializeObject<Community[]>(body);

			return Array.Empty<Community>();
		}
	}
}