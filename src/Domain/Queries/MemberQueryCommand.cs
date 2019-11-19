using System;
using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Elections;
using Domain.Utils;

namespace Domain.Queries
{
	public class MemberQueryCommand : ICommand<Member>
	{
		private readonly Guid communityId;
		private readonly Guid memberId;

		public MemberQueryCommand(Guid communityId, Guid memberId)
		{
			this.communityId = communityId;
			this.memberId = memberId;
		}

		public string Name => "Member query";

		public void Send(Stream stream)
		{
			var bufferCommunity = communityId.ToOrderByteArray();
			var bufferMember = memberId.ToOrderByteArray();

			var header = new CommandHeader(CommandIds.QueryMember, bufferCommunity.Length + bufferMember.Length);
			header.CopyTo(stream);

			stream.Write(bufferCommunity, 0, bufferCommunity.Length);
			stream.Write(bufferMember, 0, bufferMember.Length);
		}

		public Member Parse(Stream stream, int length)
		{
			var buffer = new byte[length];
			stream.Read(buffer, 0, length);
			return Serializer.Parse<Member>(buffer);
		}
	}
}