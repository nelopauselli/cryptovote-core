using System;
using Domain;
using Domain.Channels;
using Domain.Scrutiny;

namespace CryptoVote.Loggers
{
	public class ConsoleLogger : IEventListener
	{
		public void Incomming(Recount recount)
		{
			Console.WriteLine($"[Incoming recount] {recount}");
		}

		public void Incomming(Fiscal fiscal)
		{
			Console.WriteLine($"[Incoming fiscal] {fiscal}");
		}

		public void Incomming(Urn urn)
		{
			Console.WriteLine($"[Incoming urn] {urn}");
		}

		public void Incomming(Vote vote)
		{
			Console.WriteLine($"[Incoming vote] {vote}");
		}

		public void Incomming(Issue issue)
		{
			Console.WriteLine($"[Incoming issue] {issue}");
		}

		public void Incomming(Member member)
		{
			Console.WriteLine($"[Incoming member] {member}");
		}

		public void Incomming(Community community)
		{
			Console.WriteLine($"[Incoming community] {community}");
		}

		public void Incomming(Document document)
		{
			Console.WriteLine($"[Incoming document] {document}");
		}

		public void Incomming(Block block)
		{
			Console.WriteLine($"[Incoming block] {block}");
		}

		public void Incomming(IPeer peer)
		{
			Console.WriteLine($"[Incoming peer] {peer}");
		}

		public void Debug(string message)
		{
			Console.WriteLine($"[DEBG] {message}");
		}

		public void Information(string message)
		{
			Console.WriteLine($"[INFO] {message}");
		}

		public void Warning(string message)
		{
			Console.WriteLine($"[WARN] {message}");
		}

		public void Error(string message)
		{
			Console.WriteLine($"[ERROR] {message}");
		}
	}
}