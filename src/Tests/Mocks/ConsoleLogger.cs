using System;
using Domain;

namespace Tests.Mocks
{
	public class ConsoleLogger : INodeLogger
	{
		public void Error(string message)
		{
			Console.WriteLine($"[{Tag}][ERROR] {message}");
		}

		public void Warning(string message)
		{
			Console.WriteLine($"[{Tag}][WARN] {message}");
		}

		public void Information(string message)
		{
			Console.WriteLine($"[{Tag}][INFO] {message}");
		}

		public void Debug(string message)
		{
			Console.WriteLine($"[{Tag}][DEBG] {message}");
		}

		public string Tag { get; set; }
	}
}