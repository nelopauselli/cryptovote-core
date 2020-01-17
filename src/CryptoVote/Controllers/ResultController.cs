using System;
using Domain;
using Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CryptoVote.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ResultController : ControllerBase
	{
		private readonly ILogger<ResultController> logger;
		private readonly INode node;

		public ResultController(ILogger<ResultController> logger, INode node)
		{
			this.logger = logger;
			this.node = node;
		}

		[HttpGet("{communityId}/{questionId}")]
		public ObjectResult Get(Guid communityId, Guid questionId)
		{
			var query = new QuestionResultQuery(node.Blockchain);
			return Ok(query.Execute(communityId, questionId));
		}
	}
}