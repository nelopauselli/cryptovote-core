using System;
using System.Collections.Generic;
using System.Text;
using Domain.Protocol;
using Domain.Queries;
using Domain.Elections;

namespace Domain.Channels
{
	public class Conversation
	{
		private readonly INode node;
		private readonly INodeLogger logger;

		public Conversation(INode node, INodeLogger logger)
		{
			this.node = node;
			this.logger = logger;
		}

		public void Talk(ProtocolMessageChannel messageChannel)
		{
			try
			{
				char commandType = messageChannel.GetCommandId();
				logger.Information($"Recibiendo comando {commandType}");

				switch (commandType)
				{

					case QueryCommand.CommandId:
						ProcessQueryRequest(messageChannel);
						break;
				}
			}
			catch (Exception ex)
			{
					logger.Error($"ERROR: {ex.Message}");
			}
		}

		private void ProcessQueryRequest(ProtocolMessageChannel messageChannel)
		{
			var body = messageChannel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				var chunks = body.Split('#', 2);
				var entity = chunks[0];
				var content = chunks.Length > 1 ? chunks[1]:null;
				switch (entity.ToLower())
				{
					case "question-result":
						var queryQuestionResult = new QuestionResultQuery(node.Blockchain);
						var commandQuestionResult = new SendQueryResponseMessage<QuestionResult>(queryQuestionResult.Execute(content));
						var dataQuestionResult = commandQuestionResult.GetBytes();
						messageChannel.Write(dataQuestionResult);
						break;
					case "votes":
						var queryVotes = new VotesQuery(node.Blockchain);
						var commandVotes = new SendQueryResponseMessage<IEnumerable<Vote>>(queryVotes.Execute(content));
						var dataVotes = commandVotes.GetBytes();
						messageChannel.Write(dataVotes);
						break;
					case "fiscals":
						var queryFiscals = new FiscalsQuery(node.Blockchain);
						var commandFiscals = new SendQueryResponseMessage<IEnumerable<Fiscal>>(queryFiscals.Execute(content));
						var dataFiscals = commandFiscals.GetBytes();
						messageChannel.Write(dataFiscals);
						break;
					case "urns":
						var queryUrns = new UrnsQuery(node.Blockchain);
						var commandUrns = new SendQueryResponseMessage<IEnumerable<Urn>>(queryUrns.Execute(content));
						var dataUrns = commandUrns.GetBytes();
						messageChannel.Write(dataUrns);
						break;
					case "recount":
						var queryRecount = new RecountQuery(node.Blockchain);
						var commandRecount = new SendQueryResponseMessage<Recount>(queryRecount.Execute(content));
						var dataRecount = commandRecount.GetBytes();
						messageChannel.Write(dataRecount);
						break;
					default:
						messageChannel.Write(Encoding.UTF8.GetBytes("Unknown Entity"));
						break;
				}
			}
		}
	}
}