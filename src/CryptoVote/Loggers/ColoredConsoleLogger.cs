using System;
using Domain;
using Domain.Channels;
using Domain.Scrutiny;

namespace CryptoVote.Loggers
{
	public class ColoredConsoleLogger : IEventListener
	{
		public void Incomming(Recount recount)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"[Incoming recount] {recount}");
			Console.ResetColor();
		}

		public void Incomming(Fiscal fiscal)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"[Incoming fiscal] {fiscal}");
			Console.ResetColor();
		}

		public void Incomming(Urn urn)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"[Incoming urn] {urn}");
			Console.ResetColor();
		}

		public void Incomming(Vote vote)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"[Incoming vote] {vote}");
			Console.ResetColor();
		}

		public void Incomming(Issue issue)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"[Incoming issue] {issue}");
			Console.ResetColor();
		}

		public void Incomming(Member member)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"[Incoming member] {member}");
			Console.ResetColor();
		}

		public void Incomming(Community community)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"[Incoming community] {community}");
			Console.ResetColor();
		}

		public void Incomming(Document document)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"[Incoming document] {document}");
			Console.ResetColor();
		}

		public void Incomming(Block block)
		{
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine($"[Incoming block] {block}");
			Console.ResetColor();
		}

		public void Incomming(IPeer peer)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"[Incoming peer] {peer}");
			Console.ResetColor();
		}

		public void Debug(string message)
		{
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine($"[DEBG] {message}");
			Console.ResetColor();
		}

		public void Information(string message)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine($"[INFO] {message}");
			Console.ResetColor();
		}

		public void Warning(string message)
		{
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.WriteLine($"[WARN] {message}");
			Console.ResetColor();
		}

		public void Error(string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"[ERROR] {message}");
			Console.ResetColor();
		}
	}
}