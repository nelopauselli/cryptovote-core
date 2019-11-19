using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Elections;

namespace Domain.Protocol
{
	public class SendQuestionCommand : ICommand
	{
		private readonly Question question;

		public SendQuestionCommand(Question question)
		{
			this.question = question;
		}

		public string Name => "Send Question";

		public void Send(Stream stream)
		{
			var buffer = Serializer.GetBytes(question);

			var header = new CommandHeader(CommandIds.SendQuestion, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer, 0, buffer.Length);
		}
	}
}