using System;
using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Elections;
using Domain.Utils;

namespace Domain.Queries
{
	public class QuestionsQueryCommand : ICommand<Question[]>
	{
		private readonly Guid communityId;

		public QuestionsQueryCommand(Guid communityId)
		{
			this.communityId = communityId;
		}

		public string Name => "Questions query";

		public void Send(Stream stream)
		{
			var buffer = communityId.ToOrderByteArray();

			var header = new CommandHeader(CommandIds.QueryQuestions, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer, 0, buffer.Length);
		}

		public Question[] Parse(Stream stream, int length)
		{
			var buffer = new byte[length];
			stream.Read(buffer, 0, length);
			return Serializer.Parse<Question[]>(buffer);
		}
	}
}