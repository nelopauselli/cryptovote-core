using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Elections;

namespace Domain.Protocol
{
	public class SendRecountCommand : ICommand
	{
		private readonly Recount recount;

		public SendRecountCommand(Recount recount)
		{
			this.recount = recount;
		}

		public string Name => "Send Recount";

		public void Send(Stream stream)
		{
			var buffer = Serializer.GetBytes(recount);
			var header = new CommandHeader(CommandIds.SendRecount, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer, 0, buffer.Length);
		}
	}
}