using System;
using System.Text;
using Domain.Protocol;
using Domain.Elections;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class QuestionsQueryMessage : IMessage<Question[]>
	{
		private readonly Guid communityId;

		public QuestionsQueryMessage(Guid communityId)
		{
			this.communityId = communityId;
		}
		public byte[] GetBytes()
		{
			var message = $"Questions#{communityId}";
			return Encoding.UTF8.GetBytes($"{QueryCommand.CommandId}:{message.Length:D5}|{message}");
		}

		public Question[] Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			if (!string.IsNullOrWhiteSpace(body))
				return JsonConvert.DeserializeObject<Question[]>(body);

			return Array.Empty<Question>();
		}
	}
}