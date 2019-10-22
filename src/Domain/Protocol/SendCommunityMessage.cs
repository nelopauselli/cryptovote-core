using System.Text;
using Domain.Scrutiny;
using Newtonsoft.Json;

namespace Domain.Protocol
{
	public class SendCommunityMessage : ProtocolMessage
	{
		public const char CommandId = 'C';

		private readonly Community community;

		public SendCommunityMessage(Community community)
		{
			this.community = community;
		}

		public override byte[] GetBytes()
		{
			var serialized = JsonConvert.SerializeObject(community, Formatting.None);
			var message = $"{CommandId}:{serialized.Length:D5}|{serialized}";
			return Encoding.UTF8.GetBytes(message);
		}
	}
}