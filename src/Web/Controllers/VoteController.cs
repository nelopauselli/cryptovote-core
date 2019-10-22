using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Crypto;
using Domain.Elections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VoteController : ControllerBase
	{
		private readonly ILogger<VoteController> logger;
		private readonly NodeAdapter nodeAdapter;

		public VoteController(NodeAdapter nodeAdapter, ILogger<VoteController> logger)
		{
			this.logger = logger;
			this.nodeAdapter = nodeAdapter;
		}

		// GET api/values
		[HttpGet("{questionId}")]
		public async Task<ActionResult<IEnumerable<Vote>>> Get(Guid questionId)
		{
			var questions = await nodeAdapter.Votes.List(questionId);
			return Ok(questions);
		}

		// GET api/values/5
		[HttpGet("{questionId}/{publicKey}")]
		public ActionResult<Vote> Get(Guid questionId, string publicKey)
		{
			throw new NotImplementedException();
		}

		// POST api/values
		[HttpPost]
		public async Task<ActionResult<Vote>> Post(Vote vote)
		{
			logger.LogInformation("Vote: " + JsonConvert.SerializeObject(vote));

			await nodeAdapter.Votes.Add(vote);

			var url = Url.Action("Get", new {questionId = vote.QuestionId, publicKey = vote.PublicKey});
			return Created(url, vote);
		}
	}
}
