using System;
using Domain;

namespace CryptoVote.Loggers
{
	public class ColoredConsoleLogger : INodeLogger
	{
		private readonly VerbosityEnum verbosity;
		private object semaphore = new object();

		public ColoredConsoleLogger(VerbosityEnum verbosity)
		{
			this.verbosity = verbosity;
		}

		public void Debug(string message)
		{
			if (verbosity < VerbosityEnum.DEBUG) return;

			lock (semaphore)
			{
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine($"[DEBUG] {message}");
				Console.ResetColor();
			}
		}

		public void Information(string message)
		{
			if (verbosity < VerbosityEnum.INFO) return;

			lock (semaphore)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine($"[INFO] {message}");
				Console.ResetColor();
			}
		}

		public void Warning(string message)
		{
			if (verbosity < VerbosityEnum.WARN) return;
			lock (semaphore)
			{
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine($"[WARN] {message}");
				Console.ResetColor();
			}
		}

		public void Error(string message)
		{
			if (verbosity < VerbosityEnum.ERROR) return;
			lock (semaphore)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"[ERROR] {message}");
				Console.ResetColor();
			}
		}
	}
}