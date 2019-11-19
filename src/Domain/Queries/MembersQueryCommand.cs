using System;
using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Elections;
using Domain.Utils;

namespace Domain.Queries
{
	public class MembersQueryCommand : ICommand<Member[]>
	{
		private readonly Guid communityId;

		public MembersQueryCommand(Guid communityId)
		{
			this.communityId = communityId;
		}

		public string Name => "Members query";

		public void Send(Stream stream)
		{
			var buffer = communityId.ToOrderByteArray();

			var header = new CommandHeader(CommandIds.QueryMembers, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer, 0, buffer.Length);
		}

		public Member[] Parse(Stream stream, int length)
		{
			var buffer = new byte[length];
			stream.Read(buffer, 0, length);
			return Serializer.Parse<Member[]>(buffer);
		}
	}
}