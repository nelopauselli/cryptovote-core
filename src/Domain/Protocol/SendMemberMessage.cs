using System.Text;
using Domain.Scrutiny;
using Newtonsoft.Json;

namespace Domain.Protocol
{
	public class SendMemberMessage : ProtocolMessage
	{
		public const char CommandId = 'M';
		
		private readonly Member member;

		public SendMemberMessage(Member member)
		{
			this.member = member;
		}

		public override byte[] GetBytes()
		{
			var serialized = JsonConvert.SerializeObject(member, Formatting.None);
			var message = $"{CommandId}:{serialized.Length:D5}|{serialized}";
			return Encoding.UTF8.GetBytes(message);
		}
	}
}