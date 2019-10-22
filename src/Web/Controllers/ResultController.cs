using System;
using System.Threading.Tasks;
using Domain.Scrutiny;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ResultController : ControllerBase
	{
		private readonly ILogger<ResultController> logger;
		private readonly NodeAdapter node;

		public ResultController(ILogger<ResultController> logger, NodeAdapter node)
		{
			this.logger = logger;
			this.node = node;
		}

		[HttpGet("{communityId}/{issueId}")]
		public async Task<ActionResult<IssueResult>> Get(Guid communityId, Guid issueId)
		{
			var result = await node.Issues.Result(communityId, issueId);
			if (result == null)
				return NotFound();
			return Ok(result);
		}
	}
}