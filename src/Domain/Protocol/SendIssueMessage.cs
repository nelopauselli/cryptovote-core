using System.Text;
using Domain.Scrutiny;
using Newtonsoft.Json;

namespace Domain.Protocol
{
	public class SendIssueMessage : ProtocolMessage
	{
		public const char CommandId = 'I';

		private readonly Issue issue;

		public SendIssueMessage(Issue issue)
		{
			this.issue = issue;
		}

		public override byte[] GetBytes()
		{
			var serialized = JsonConvert.SerializeObject(issue, Formatting.None);
			var message = $"{CommandId}:{serialized.Length:D5}|{serialized}";
			return Encoding.UTF8.GetBytes(message);

		}
	}
}