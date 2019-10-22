using System;
using System.Text;
using Domain.Protocol;
using Domain.Elections;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class FiscalsQueryMessage : IMessage<Fiscal[]>
	{
		private readonly Guid issueId;

		public FiscalsQueryMessage(Guid issueId)
		{
			this.issueId = issueId;
		}
		public byte[] GetBytes()
		{
			var message = $"Fiscals#{issueId}";
			return Encoding.UTF8.GetBytes($"{QueryCommand.CommandId}:{message.Length:D5}|{message}");
		}

		public Fiscal[] Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();
			if (!string.IsNullOrWhiteSpace(body))
				return JsonConvert.DeserializeObject<Fiscal[]>(body);

			return Array.Empty<Fiscal>();
		}
	}
}