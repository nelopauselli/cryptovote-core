using System;
using System.Text;
using Domain.Protocol;
using Domain.Elections;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class QuestionQueryMessage : IMessage<Question>
	{
		private readonly Guid communityId;
		private readonly Guid questionId;

		public QuestionQueryMessage(Guid communityId, Guid questionId)
		{
			this.communityId = communityId;
			this.questionId = questionId;
		}
		public byte[] GetBytes()
		{
			var message = $"Question#{communityId}#{questionId}";
			return Encoding.UTF8.GetBytes($"{QueryCommand.CommandId}:{message.Length:D5}|{message}");
		}

		public Question Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			if (!string.IsNullOrWhiteSpace(body))
				return JsonConvert.DeserializeObject<Question>(body);
			return null;
		}
	}
}