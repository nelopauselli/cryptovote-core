using System;
using Domain;

namespace CryptoVote.Loggers
{
	public class ConsoleLogger : INodeLogger
	{
		private readonly VerbosityEnum verbosity;

		public ConsoleLogger(VerbosityEnum verbosity)
		{
			this.verbosity = verbosity;
		}

		public void Debug(string message)
		{
			if (verbosity < VerbosityEnum.DEBUG) return;
			Console.WriteLine($"[DEBUG] {message}");
		}

		public void Information(string message)
		{
			if (verbosity < VerbosityEnum.INFO) return;
			Console.WriteLine($"[INFO] {message}");
		}

		public void Warning(string message)
		{
			if (verbosity < VerbosityEnum.WARN) return;
			Console.WriteLine($"[WARN] {message}");
		}

		public void Error(string message)
		{
			if (verbosity < VerbosityEnum.ERROR) return;
			Console.WriteLine($"[ERROR] {message}");
		}
	}
}