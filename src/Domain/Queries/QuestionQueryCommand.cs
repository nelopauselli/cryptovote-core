using System;
using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Elections;
using Domain.Utils;

namespace Domain.Queries
{
	public class QuestionQueryCommand : ICommand<Question>
	{
		private readonly Guid communityId;
		private readonly Guid questionId;

		public QuestionQueryCommand(Guid communityId, Guid questionId)
		{
			this.communityId = communityId;
			this.questionId = questionId;
		}

		public string Name => "Question query";

		public void Send(Stream stream)
		{
			var bufferCommunity = communityId.ToOrderByteArray();
			var bufferQuestion = questionId.ToOrderByteArray();

			var header = new CommandHeader(CommandIds.QueryQuestion, bufferCommunity.Length+bufferQuestion.Length);
			header.CopyTo(stream);

			stream.Write(bufferCommunity, 0, bufferCommunity.Length);
			stream.Write(bufferQuestion, 0, bufferQuestion.Length);
		}

		public Question Parse(Stream stream, int length)
		{
			var buffer = new byte[length];
			stream.Read(buffer, 0, length);
			return Serializer.Parse<Question>(buffer);
		}
	}
}