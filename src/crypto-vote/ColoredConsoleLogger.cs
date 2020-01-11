using System;
using Domain;

namespace crypto_vote
{
	public class ColoredConsoleLogger : INodeLogger
	{
		private readonly object semaphore = new object();

		public ColoredConsoleLogger()
		{
		}

		public void Debug(string message)
		{
			lock (semaphore)
			{
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine($"[DEBUG] {message}");
				Console.ResetColor();
			}
		}

		public void Information(string message)
		{
			lock (semaphore)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine($"[INFO] {message}");
				Console.ResetColor();
			}
		}

		public void Warning(string message)
		{
			lock (semaphore)
			{
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine($"[WARN] {message}");
				Console.ResetColor();
			}
		}

		public void Error(string message)
		{
			lock (semaphore)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"[ERROR] {message}");
				Console.ResetColor();
			}
		}
	}
}