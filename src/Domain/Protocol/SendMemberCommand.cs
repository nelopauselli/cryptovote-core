using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Elections;

namespace Domain.Protocol
{
	public class SendMemberCommand : ICommand
	{
		private readonly Member member;

		public SendMemberCommand(Member member)
		{
			this.member = member;
		}

		public string Name => "Send Memeber";
		
		public void Send(Stream stream)
		{
			var buffer = Serializer.GetBytes(member);

			var header = new CommandHeader(CommandIds.SendMember, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer, 0, buffer.Length);
		}
	}
}