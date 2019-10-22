using System.Text;
using Domain.Elections;
using Newtonsoft.Json;

namespace Domain.Protocol
{
	public class SendVoteMessage : ProtocolMessage
	{
		public const char CommandId = 'V';

		private readonly Vote vote;

		public SendVoteMessage(Vote vote)
		{
			this.vote = vote;
		}

		public override byte[] GetBytes()
		{
			var serialized = JsonConvert.SerializeObject(vote, Formatting.None);
			var message = $"{CommandId}:{serialized.Length:D5}|{serialized}";
			return Encoding.UTF8.GetBytes(message);
		}
	}
}