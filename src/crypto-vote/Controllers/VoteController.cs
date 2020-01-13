using System;
using System.Text.Json;
using Domain;
using Domain.Elections;
using Domain.Queries;
using Domain.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace crypto_vote.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VoteController : ControllerBase
	{
		private readonly ILogger<VoteController> logger;
		private readonly INode node;

		public VoteController(INode node, ILogger<VoteController> logger)
		{
			this.logger = logger;
			this.node = node;
		}

		// GET api/values
		[HttpGet("{questionId}")]
		public ObjectResult Get(Guid questionId)
		{
			var query = new VotesQuery(node.Blockchain);
			return Ok(query.Execute(questionId));
		}

		// GET api/values/5
		[HttpGet("{questionId}/{publicKey}")]
		public ActionResult<Vote> Get(Guid questionId, string publicKey)
		{
			var query = new VoteQuery(node.Blockchain);
			return Ok(query.Execute(questionId, Base58.Decode(publicKey)));
		}

		// POST api/values
		[HttpPost]
		public ObjectResult Post(Vote vote)
		{
			logger.LogInformation("Vote: " + JsonSerializer.Serialize(vote));

			node.Add(vote);

			//var url = Url.Action("Get", new {questionId = vote.QuestionId, publicKey = vote.PublicKey});
			return Accepted(vote);
		}
	}
}
