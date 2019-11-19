using System;
using System.Threading.Tasks;
using Domain.Protocol;
using Domain.Queries;
using Domain.Elections;

namespace Web.Controllers
{
	public class NodeAdapterQuestions
	{
		private readonly NodeAdapter nodeAdapter;

		public NodeAdapterQuestions(NodeAdapter nodeAdapter)
		{
			this.nodeAdapter = nodeAdapter;
		}

		public async Task<Question[]> List(Guid communityId)
		{
			var command = new QuestionsQueryCommand(communityId);
			return await nodeAdapter.GetResponse(command);
		}

		public async Task<Question> Get(Guid communityId, Guid questionId)
		{
			var message = new QuestionQueryCommand(communityId, questionId);
			return await nodeAdapter.GetResponse(message);
		}

		public async Task Add(Question question)
		{
			var command = new SendQuestionCommand(question);
			await nodeAdapter.Send(command);
		}

		public async Task<QuestionResult> Result(Guid communityId, Guid questionId)
		{
			var message = new QuestionResultQueryMessage(communityId, questionId);
			return await nodeAdapter.GetResponse(message);
		}
	}
}