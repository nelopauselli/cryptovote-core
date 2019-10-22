using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Crypto;
using Domain.Scrutiny;
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
		[HttpGet("{issueId}")]
		public async Task<ActionResult<IEnumerable<Vote>>> Get(Guid issueId)
		{
			var issues = await nodeAdapter.Votes.List(issueId);
			return Ok(issues);
		}

		// GET api/values/5
		[HttpGet("{issueId}/{publicKey}")]
		public ActionResult<Vote> Get(Guid issueId, string publicKey)
		{
			throw new NotImplementedException();
		}

		// POST api/values
		[HttpPost]
		public async Task<ActionResult<Vote>> Post(Vote vote)
		{
			logger.LogInformation("Vote: " + JsonConvert.SerializeObject(vote));

			await nodeAdapter.Votes.Add(vote);

			var url = Url.Action("Get", new {issueId = vote.IssueId, publicKey = vote.PublicKey});
			return Created(url, vote);
		}
	}
}
