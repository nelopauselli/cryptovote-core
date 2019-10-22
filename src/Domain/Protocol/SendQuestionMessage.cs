using System.Text;
using Domain.Elections;
using Newtonsoft.Json;

namespace Domain.Protocol
{
	public class SendQuestionMessage : ProtocolMessage
	{
		public const char CommandId = SendQuestionCommandId;

		private readonly Question question;

		public SendQuestionMessage(Question question)
		{
			this.question = question;
		}

		public override byte[] GetBytes()
		{
			var serialized = JsonConvert.SerializeObject(question, Formatting.None);
			var message = $"{CommandId}:{serialized.Length:D5}|{serialized}";
			return Encoding.UTF8.GetBytes(message);

		}
	}
}