using System;
using System.Text;
using Domain.Protocol;
using Domain.Elections;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class QuestionResultQueryMessage : IMessage<QuestionResult>
	{
		private readonly Guid communityId;
		private readonly Guid questionId;

		public QuestionResultQueryMessage(Guid communityId, Guid questionId)
		{
			this.communityId = communityId;
			this.questionId = questionId;
		}
		public byte[] GetBytes()
		{
			var message = $"Question-Result#{communityId}#{questionId}";
			return Encoding.UTF8.GetBytes($"{QueryCommand.CommandId}:{message.Length:D5}|{message}");
		}

		public QuestionResult Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			if (!string.IsNullOrWhiteSpace(body))
				return JsonConvert.DeserializeObject<QuestionResult>(body);
			return null;
		}
	}
}