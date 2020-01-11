using System;
using System.Collections.Generic;
using Domain;
using Domain.Elections;
using Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace crypto_vote.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class QuestionController : ControllerBase
	{
		private readonly ILogger<QuestionController> logger;
		private readonly Node node;

		public QuestionController(Node node, ILogger<QuestionController> logger)
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
			var url = Url.Action("Get", new { communityId = question.CommunityId, questionId = question.Id });
			return Accepted(url, question);
		}
	}

	[Route("api/[controller]")]
	[ApiController]
	public class UrnController : ControllerBase
	{
		private readonly ILogger<UrnController> logger;
		private readonly Node node;

		public UrnController(Node node, ILogger<UrnController> logger)
		{
			this.node = node;
			this.logger = logger;
		}

		[HttpGet("{questionId}")]
		public IEnumerable<Urn> Get(Guid questionId)
		{
			var query = new UrnsQuery(node.Blockchain);
			return query.Execute(questionId);
		}

		[HttpPost]
		public ObjectResult Post(Urn urn)
		{
			node.Add(urn);
			var url = Url.Action("Get", new { communityId = urn.QuestionId, UrnId = urn.Id });
			return Accepted(url);
		}
	}

	[Route("api/[controller]")]
	[ApiController]
	public class FiscalController : ControllerBase
	{
		private readonly ILogger<FiscalController> logger;
		private readonly Node node;

		public FiscalController(Node node, ILogger<FiscalController> logger)
		{
			this.node = node;
			this.logger = logger;
		}

		[HttpGet("{questionId}")]
		public IEnumerable<Fiscal> Get(Guid questionId)
		{
			var query=new FiscalsQuery(node.Blockchain);
			return query.Execute(questionId);
		}

		[HttpPost]
		public ObjectResult Post(Fiscal fiscal)
		{
			node.Add(fiscal);
			var url = Url.Action("Get", new { communityId = fiscal.QuestionId, FiscalId = fiscal.Id });
			return Accepted(url);
		}
	}
}