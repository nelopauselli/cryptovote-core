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
		private readonly Blockchain blockchain;
		private readonly IList<IEventListener> listeners;
		private readonly INodeLogger logger;
		private readonly IChannel channel;

		public Conversation(Blockchain blockchain, IList<IEventListener> listeners, INodeLogger logger, IChannel channel)
		{
			this.blockchain = blockchain;
			this.listeners = listeners;
			this.logger = logger;
			this.channel = channel;
		}

		public void Talk(ProtocolMessageChannel channel)
		{
			try
			{
				char commandType = channel.GetCommandId();
				logger.Debug($"Recibiendo comando {commandType}");

				switch (commandType)
				{
					case SendVoteMessage.CommandId:
						ProcessVote(channel);
						break;
					case SendRecountMessage.CommandId:
						ProcessRecount(channel);
						break;
					case SendFiscalMessage.CommandId:
						ProcessFiscal(channel);
						break;
					case SendUrnMessage.CommandId:
						ProcessUrn(channel);
						break;
					case SendQuestionMessage.CommandId:
						ProcessQuestion(channel);
						break;
					case SendMemberMessage.CommandId:
						ProcessMember(channel);
						break;
					case SendCommunityMessage.CommandId:
						ProcessCommunity(channel);
						break;
					case SendBlockMessage.CommandId:
						ProcessBlock(channel);
						break;
					case SendDocumentMessage.CommandId:
						ProcessDocument(channel);
						break;
					case SendPeerInfoMessage.CommandId:
						ProcessPeer(channel);
						break;
					case LastBlockQueryMessage.CommandId:
						ProcessLastBlockRequest(channel);
						break;
					case BlockQueryMessage.CommandId:
						ProcessBlockRequest(channel);
						break;
					case QueryCommand.CommandId:
						ProcessQueryRequest(channel);
						break;
				}
			}
			catch (Exception ex)
			{
				foreach (var listener in listeners)
					listener.Error($"ERROR: {ex.Message}");
			}
		}

		private void ProcessPeer(ProtocolMessageChannel protocolMessageChannel)
		{
			var body = protocolMessageChannel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				var peerInfo = JsonConvert.DeserializeObject<PeerInfo>(body);
				
				var peer = new TcpPeer(peerInfo.Host, peerInfo.Port, channel);
				foreach (var listener in listeners)
					listener.Incomming(peer);
			}
		}

		private void ProcessVote(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				foreach (var listener in listeners)
					listener.Debug($"Incomming message: {body}");

				var vote = JsonConvert.DeserializeObject<Vote>(body);
				foreach (var listener in listeners)
					listener.Incomming(vote);
				channel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessRecount(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				foreach (var listener in listeners)
					listener.Debug($"Incomming message: {body}");

				var recount = JsonConvert.DeserializeObject<Recount>(body);
				foreach (var listener in listeners)
					listener.Incomming(recount);
				channel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessUrn(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				foreach (var listener in listeners)
					listener.Debug($"Incomming message: {body}");

				var urn = JsonConvert.DeserializeObject<Urn>(body);
				foreach (var listener in listeners)
					listener.Incomming(urn);
				channel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessFiscal(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				foreach (var listener in listeners)
					listener.Debug($"Incomming message: {body}");

				var fiscal = JsonConvert.DeserializeObject<Fiscal>(body);
				foreach (var listener in listeners)
					listener.Incomming(fiscal);
				channel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessQuestion(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				foreach (var listener in listeners)
					listener.Debug($"Incomming message: {body}");

				var question = JsonConvert.DeserializeObject<Question>(body);
				foreach (var listener in listeners)
					listener.Incomming(question);
				channel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessCommunity(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				foreach (var listener in listeners)
					listener.Debug($"Incomming message: {body}");

				var community = JsonConvert.DeserializeObject<Community>(body);
				foreach (var listener in listeners)
					listener.Incomming(community);
				channel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessDocument(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				foreach (var listener in listeners)
					listener.Debug($"Incomming message: {body}");

				var document = JsonConvert.DeserializeObject<Document>(body);
				foreach (var listener in listeners)
					listener.Incomming(document);
				channel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessMember(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				foreach (var listener in listeners)
					listener.Debug($"Incomming message: {body}");

				var member = JsonConvert.DeserializeObject<Member>(body);
				foreach (var listener in listeners)
					listener.Incomming(member);
				channel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessBlock(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				foreach (var listener in listeners)
					listener.Debug($"Incomming message: {body}");

				var block = JsonConvert.DeserializeObject<Block>(body);
				foreach (var listener in listeners)
					listener.Incomming(block);
				channel.Write(Encoding.UTF8.GetBytes("OK" + Environment.NewLine));
			}
		}

		private void ProcessLastBlockRequest(ProtocolMessageChannel channel)
		{
			logger.Debug("Enviando último bloque");
			var lastBlock = blockchain.Last;
			var command = new SendBlockMessage(lastBlock);
			var data = command.GetBytes();
			channel.Write(data);
		}

		private void ProcessBlockRequest(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				var hash = Base58.Decode(body);
				var block = blockchain.GetBlock(hash);
				var command = new SendBlockMessage(block);
				var data = command.GetBytes();
				channel.Write(data);
			}
		}

		private void ProcessQueryRequest(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				var chunks = body.Split('#', 2);
				var entity = chunks[0];
				var content = chunks.Length > 1 ? chunks[1]:null;
				switch (entity.ToLower())
				{
					case "communities":
						var queryCommunities = new CommunitiesQuery(blockchain);
						var commandCommiunities = new SendQueryResponseMessage<IEnumerable<Community>>(queryCommunities.Execute());
						var dataCommiunities = commandCommiunities.GetBytes();
						channel.Write(dataCommiunities);
						break;
					case "community":
						var queryCommunity = new CommunityQuery(blockchain);
						var commandCommunity = new SendQueryResponseMessage<Community>(queryCommunity.Execute(content));
						var dataCommunity = commandCommunity.GetBytes();
						channel.Write(dataCommunity);
						break;
					case "questions":
						var queryQuestions = new QuestionsQuery(blockchain);
						var commandQuestions = new SendQueryResponseMessage<IEnumerable<Question>>(queryQuestions.Execute(content));
						var dataQuestions = commandQuestions.GetBytes();
						channel.Write(dataQuestions);
						break;
					case "question":
						var queryQuestion = new QuestionQuery(blockchain);
						var commandQuestion = new SendQueryResponseMessage<Question>(queryQuestion.Execute(content));
						var dataQuestion = commandQuestion.GetBytes();
						channel.Write(dataQuestion);
						break;
					case "question-result":
						var queryQuestionResult = new QuestionResultQuery(blockchain);
						var commandQuestionResult = new SendQueryResponseMessage<QuestionResult>(queryQuestionResult.Execute(content));
						var dataQuestionResult = commandQuestionResult.GetBytes();
						channel.Write(dataQuestionResult);
						break;
					case "members":
						var queryMembers = new MembersQuery(blockchain);
						var commandMembers = new SendQueryResponseMessage<IEnumerable<Member>>(queryMembers.Execute(content));
						var dataMembers = commandMembers.GetBytes();
						channel.Write(dataMembers);
						break;
					case "member":
						var queryMember = new MemberQuery(blockchain);
						var commandMember = new SendQueryResponseMessage<Member>(queryMember.Execute(content));
						var dataMember = commandMember.GetBytes();
						channel.Write(dataMember);
						break;
					case "votes":
						var queryVotes = new VotesQuery(blockchain);
						var commandVotes = new SendQueryResponseMessage<IEnumerable<Vote>>(queryVotes.Execute(content));
						var dataVotes = commandVotes.GetBytes();
						channel.Write(dataVotes);
						break;
					case "fiscals":
						var queryFiscals = new FiscalsQuery(blockchain);
						var commandFiscals = new SendQueryResponseMessage<IEnumerable<Fiscal>>(queryFiscals.Execute(content));
						var dataFiscals = commandFiscals.GetBytes();
						channel.Write(dataFiscals);
						break;
					case "urns":
						var queryUrns = new UrnsQuery(blockchain);
						var commandUrns = new SendQueryResponseMessage<IEnumerable<Urn>>(queryUrns.Execute(content));
						var dataUrns = commandUrns.GetBytes();
						channel.Write(dataUrns);
						break;
					case "recount":
						var queryRecount = new RecountQuery(blockchain);
						var commandRecount = new SendQueryResponseMessage<Recount>(queryRecount.Execute(content));
						var dataRecount = commandRecount.GetBytes();
						channel.Write(dataRecount);
						break;
					default:
						channel.Write(Encoding.UTF8.GetBytes("Unknown Entity"));
						break;
				}
			}
		}
	}
}