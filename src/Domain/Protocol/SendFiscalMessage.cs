using System.Text;
using Domain.Elections;
using Newtonsoft.Json;

namespace Domain.Protocol
{
	public class SendFiscalMessage : ProtocolMessage
	{
		public const char CommandId = 'F';

		private readonly Fiscal fiscal;

		public SendFiscalMessage(Fiscal fiscal)
		{
			this.fiscal = fiscal;
		}

		public override byte[] GetBytes()
		{
			var serialized = JsonConvert.SerializeObject(fiscal, Formatting.None);
			var message = $"{CommandId}:{serialized.Length:D5}|{serialized}";
			return Encoding.UTF8.GetBytes(message);
		}
	}
}