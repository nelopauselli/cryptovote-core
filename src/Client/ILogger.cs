using System;
using System.Collections.Generic;

namespace Client
{
	public interface ILogger
	{
		IList<Log> Logs { get; }

		void Debug(string message);
		void Info(string message);
		void Error(string message, Exception exception);
		void Warning(string message);
	}

	class MemoryLogger : ILogger
	{
		private readonly IList<Log> messages = new List<Log>();
		private readonly object semaphore = new object();

		public void Debug(string message)
		{
			lock (semaphore)
			{
				messages.Add(new Log(LogType.DEBUG, message));
			}
		}

		public void Info(string message)
		{
			lock (semaphore)
			{
				messages.Add(new Log(LogType.INFO, message));
			}
		}

		public void Error(string message, Exception exception)
		{
			lock (semaphore)
			{
				messages.Add(new Log(LogType.ERROR, message, exception));
			}
		}

		public void Warning(string message)
		{
			lock (semaphore)
			{
				messages.Add(new Log(LogType.WARN, message));
			}
		}

		public IList<Log> Logs
		{
			get
			{
				lock (semaphore)
				{
					return messages;
				}
			}
		}
	}
}