using System;

namespace Client
{
	public class Log
	{
		public LogType Type { get; }
		public string Message { get; }
		public Exception Exception { get; }
		public DateTimeOffset Time { get; }

		public Log(LogType type, string message)
		{
			Time = DateTimeOffset.Now;
			Type = type;
			Message = message;
		}

		public Log(LogType type, string message, Exception exception)
			: this(type, message)
		{
			Exception = exception;
		}
	}
}