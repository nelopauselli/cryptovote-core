using System;
using System.Collections.Generic;
using System.Text;
using Domain.Protocol;
using Domain.Queries;
using Domain.Elections;
using Domain.Utils;
using Newtonsoft.Json;

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
				logger.Debug($"Recibiendo comando {commandType}");

				switch (commandType)
				{
					case SendVoteMessage.CommandId:
						ProcessVote(messageChannel);
						break;
					case SendRecountMessage.CommandId:
						ProcessRecount(messageChannel);
						break;
					case SendFiscalMessage.CommandId:
						ProcessFiscal(messageChannel);
						break;
					case SendUrnMessage.CommandId:
						ProcessUrn(messageChannel);
						break;
					case SendQuestionMessage.CommandId:
						ProcessQuestion(messageChannel);
						break;
					case SendMemberMessage.CommandId:
						ProcessMember(messageChannel);
						break;
					case SendCommunityMessage.CommandId:
						ProcessCommunity(messageChannel);
						break;
					case SendBlockMessage.CommandId:
						ProcessBlock(messageChannel);
						break;
					case SendDocumentMessage.CommandId:
						ProcessDocument(messageChannel);
						break;
					case SendPeerInfoMessage.CommandId:
						ProcessPeer(messageChannel);
						break;
					case LastBlockQueryMessage.CommandId:
						ProcessLastBlockRequest(messageChannel);
						break;
					case BlockQueryMessage.CommandId:
						ProcessBlockRequest(messageChannel);
						break;
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

		private void ProcessPeer(ProtocolMessageChannel messageChannel)
		{
			var body = messageChannel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				var peerInfo = JsonConvert.DeserializeObject<PeerInfo>(body);
				node.Register(peerInfo.Host, peerInfo.Port);
			}
		}

		private void ProcessVote(ProtocolMessageChannel messageChannel)
		{
			var body = messageChannel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				logger.Debug($"Incomming message: {body}");

				var vote = JsonConvert.DeserializeObject<Vote>(body);
				node.Add(vote);
				messageChannel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessRecount(ProtocolMessageChannel messageChannel)
		{
			var body = messageChannel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				logger.Debug($"Incomming message: {body}");

				var recount = JsonConvert.DeserializeObject<Recount>(body);
				node.Add(recount);
				messageChannel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessUrn(ProtocolMessageChannel messageChannel)
		{
			var body = messageChannel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				logger.Debug($"Incomming message: {body}");

				var urn = JsonConvert.DeserializeObject<Urn>(body);
				node.Add(urn);
				messageChannel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessFiscal(ProtocolMessageChannel messageChannel)
		{
			var body = messageChannel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				logger.Debug($"Incomming message: {body}");

				var fiscal = JsonConvert.DeserializeObject<Fiscal>(body);
				node.Add(fiscal);
				messageChannel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessQuestion(ProtocolMessageChannel messageChannel)
		{
			var body = messageChannel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				logger.Debug($"Incomming message: {body}");

				var question = JsonConvert.DeserializeObject<Question>(body);
				node.Add(question);
				messageChannel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessCommunity(ProtocolMessageChannel messageChannel)
		{
			var body = messageChannel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				logger.Debug($"Incomming message: {body}");

				var community = JsonConvert.DeserializeObject<Community>(body);
				node.Add(community);
				messageChannel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessDocument(ProtocolMessageChannel messageChannel)
		{
			var body = messageChannel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				logger.Debug($"Incomming message: {body}");

				var document = JsonConvert.DeserializeObject<Document>(body);
				node.Add(document);
				messageChannel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessMember(ProtocolMessageChannel messageChannel)
		{
			var body = messageChannel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				logger.Debug($"Incomming message: {body}");

				var member = JsonConvert.DeserializeObject<Member>(body);
				node.Add(member);
				messageChannel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessBlock(ProtocolMessageChannel messageChannel)
		{
			var body = messageChannel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				logger.Debug($"Incomming message: {body}");

				var block = JsonConvert.DeserializeObject<Block>(body);
				node.Add(block);
				messageChannel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessLastBlockRequest(ProtocolMessageChannel messageChannel)
		{
			logger.Debug("Enviando último bloque");
			var lastBlock = node.Blockchain.Last;
			var command = new SendBlockMessage(lastBlock);
			var data = command.GetBytes();
			messageChannel.Write(data);
		}

		private void ProcessBlockRequest(ProtocolMessageChannel messageChannel)
		{
			var body = messageChannel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				var hash = Base58.Decode(body);
				var block = node.Blockchain.GetBlock(hash);
				var command = new SendBlockMessage(block);
				var data = command.GetBytes();
				messageChannel.Write(data);
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
					case "communities":
						var queryCommunities = new CommunitiesQuery(node.Blockchain);
						var commandCommiunities = new SendQueryResponseMessage<IEnumerable<Community>>(queryCommunities.Execute());
						var dataCommiunities = commandCommiunities.GetBytes();
						messageChannel.Write(dataCommiunities);
						break;
					case "community":
						var queryCommunity = new CommunityQuery(node.Blockchain);
						var commandCommunity = new SendQueryResponseMessage<Community>(queryCommunity.Execute(content));
						var dataCommunity = commandCommunity.GetBytes();
						messageChannel.Write(dataCommunity);
						break;
					case "questions":
						var queryQuestions = new QuestionsQuery(node.Blockchain);
						var commandQuestions = new SendQueryResponseMessage<IEnumerable<Question>>(queryQuestions.Execute(content));
						var dataQuestions = commandQuestions.GetBytes();
						messageChannel.Write(dataQuestions);
						break;
					case "question":
						var queryQuestion = new QuestionQuery(node.Blockchain);
						var commandQuestion = new SendQueryResponseMessage<Question>(queryQuestion.Execute(content));
						var dataQuestion = commandQuestion.GetBytes();
						messageChannel.Write(dataQuestion);
						break;
					case "question-result":
						var queryQuestionResult = new QuestionResultQuery(node.Blockchain);
						var commandQuestionResult = new SendQueryResponseMessage<QuestionResult>(queryQuestionResult.Execute(content));
						var dataQuestionResult = commandQuestionResult.GetBytes();
						messageChannel.Write(dataQuestionResult);
						break;
					case "members":
						var queryMembers = new MembersQuery(node.Blockchain);
						var commandMembers = new SendQueryResponseMessage<IEnumerable<Member>>(queryMembers.Execute(content));
						var dataMembers = commandMembers.GetBytes();
						messageChannel.Write(dataMembers);
						break;
					case "member":
						var queryMember = new MemberQuery(node.Blockchain);
						var commandMember = new SendQueryResponseMessage<Member>(queryMember.Execute(content));
						var dataMember = commandMember.GetBytes();
						messageChannel.Write(dataMember);
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