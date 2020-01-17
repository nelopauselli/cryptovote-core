using System;
using System.Collections.Generic;
using Domain;
using Domain.Elections;
using Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CryptoVote.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class QuestionController : ControllerBase
	{
		private readonly ILogger<QuestionController> logger;
		private readonly INode node;

		public QuestionController(INode node, ILogger<QuestionController> logger)
		{
			this.node = node;
			this.logger = logger;
		}

		[HttpGet("{communityId}")]
		public IEnumerable<Question> List(Guid communityId)
		{
			var query = new QuestionsQuery(node.Blockchain);
			return query.Execute(communityId);
		}

		[HttpGet("{communityId}/{questionId}")]
		public Question Get(Guid communityId, Guid questionId)
		{
			var query = new QuestionQuery(node.Blockchain);
			return query.Execute(communityId, questionId);
		}

		[HttpPost]
		public ObjectResult Post(Question question)
		{
			node.Add(question);
			//var url = Url.Action("Get", new { communityId = question.CommunityId, questionId = question.Id });
			return Accepted(question);
		}
	}
}