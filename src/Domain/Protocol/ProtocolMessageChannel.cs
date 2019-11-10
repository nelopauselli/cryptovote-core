using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Domain.Protocol
{
	public class ProtocolMessageChannel
	{
		private readonly char commandId;
		private readonly Stream stream;

		public ProtocolMessageChannel(Stream stream)
		{
			this.stream = stream;

			var value = stream.ReadByte();
			if (value != -1)
				commandId = Encoding.UTF8.GetString(new[] {(byte) value})[0];
		}

		public string GetBody()
		{
			var sizeOfMessage = GetSizeOfMessage(stream);
			var body = ReadBody(stream, sizeOfMessage);
			return body;
		}

		private static int GetSizeOfMessage(Stream stream)
		{
			int sizeOfMessage = 0;

			byte[] sizeOfMessageBuffer = new byte[7];
			if (stream.Read(sizeOfMessageBuffer, 0, 7) == 7)
			{
				// el primer byte son los : y el último el |
				var sizeOfMessageAsString = Encoding.UTF8.GetString(sizeOfMessageBuffer, 1, 5);
				sizeOfMessage = int.Parse(sizeOfMessageAsString);
			}

			return sizeOfMessage;
		}

		private static string ReadBody(Stream stream, int sizeOfMessage)
		{
			// Loop to receive all the data sent by the client.
			byte[] bytes = new Byte[1024];

			int received = 0;
			var bodyBuilder = new StringBuilder(sizeOfMessage);

			while (received < sizeOfMessage)
			{
				int size;
				if ((size = stream.Read(bytes, 0, bytes.Length)) > 0)
				{
					if (size == 0) continue;
					received += size;

					var chunk = Encoding.UTF8.GetString(bytes, 0, size);
					bodyBuilder.Append(chunk);
				}
			}

			return bodyBuilder.Length > sizeOfMessage
				? bodyBuilder.ToString(0, sizeOfMessage)
				: bodyBuilder.ToString();
		}

		public char GetCommandId()
		{
			return commandId;
		}

		public void Write(byte[] buffer)
		{
			stream.Write(buffer);
		}
	}
}