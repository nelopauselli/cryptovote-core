using System;
using System.IO;

namespace Domain.Channels.Protocol
{
	public class CommandHeader
	{
		public byte CommandId { get; }
		public int Length { get; }

		public CommandHeader(byte commandId, int length)
		{
			CommandId = commandId;
			Length = length;
		}


		public void CopyTo(Stream stream)
		{
			stream.WriteByte(CommandId);
			stream.Write(BitConverter.GetBytes(Length), 0, sizeof(int));
		}

		public static CommandHeader Parse(Stream stream)
		{
			var commandId = (byte)stream.ReadByte();

			var buffer = new byte[sizeof(int)];
			stream.Read(buffer, 0, sizeof(int));
			
			return new CommandHeader(commandId, BitConverter.ToInt32(buffer, 0));
		}
	}
}