using System.Text;
using Newtonsoft.Json;

namespace Domain.Protocol
{
	public class SendDocumentMessage : ProtocolMessage
	{
		public const char CommandId = SendDocumentCommandId;

		private readonly Document document;

		public SendDocumentMessage(Document document)
		{
			this.document = document;
		}

		public override byte[] GetBytes()
		{
			var serialized = JsonConvert.SerializeObject(document, Formatting.None);
			var message = $"{CommandId}:{serialized.Length:D5}|{serialized}";
			return Encoding.UTF8.GetBytes(message);
		}
	}
}