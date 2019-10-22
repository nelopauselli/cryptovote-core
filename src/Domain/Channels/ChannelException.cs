using System;

namespace Domain.Channels
{
	public class ChannelException : Exception
	{
		public ChannelException(string message, Exception innerException)
			:base(message, innerException)
		{
			
		}
	}
}