using Domain;
using Domain.Channels;
using Domain.Elections;
using Tests.Mocks;

namespace Tests
{
	public class ConsoleListener : IEventListener
	{
		private readonly ConsoleLogger logger;

		public ConsoleListener(ConsoleLogger logger)
		{
			this.logger = logger;
		}

		public ConsoleListener() : this(new ConsoleLogger())
		{
		}

		public void Incomming(Recount recount)
		{
			logger.Information($"Incoming recount: {recount}");
		}

		public void Incomming(Fiscal fiscal)
		{
			logger.Information($"Incoming fiscal: {fiscal}");
		}

		public void Incomming(Urn urn)
		{
			logger.Information($"Incoming urn: {urn}");
		}

		public void Incomming(Vote vote)
		{
			logger.Information($"Incoming vote: {vote}");
		}

		public void Incomming(Question question)
		{
			logger.Information($"Incoming question: {question}");
		}

		public void Incomming(Member member)
		{
			logger.Information($"Incoming member: {member}");
		}

		public void Incomming(Community community)
		{
			logger.Information($"Incoming community: {community}");
		}

		public void Incomming(Document document)
		{
			logger.Information($"Incoming document: {document}");
		}

		public void Incomming(Block block)
		{
			logger.Information($"Incoming block: {block}");
		}

		public void Incomming(IPeer peer)
		{
			logger.Information($"Incoming peer: {peer}");
		}

		public void Debug(string message)
		{
			logger.Debug(message);
		}

		public void Information(string message)
		{
			logger.Information(message);
		}

		public void Warning(string message)
		{
			logger.Warning(message);
		}

		public void Error(string message)
		{
			logger.Error(message);
		}
	}
}