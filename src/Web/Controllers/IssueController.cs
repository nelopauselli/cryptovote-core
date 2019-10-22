using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Scrutiny;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class IssueController : ControllerBase
	{
		private readonly ILogger<IssueController> logger;
		private readonly NodeAdapter node;

		public IssueController(ILogger<IssueController> logger, NodeAdapter node)
		{
			this.logger = logger;
			this.node = node;
		}

		[HttpGet("{communityId}")]
		public async Task<ActionResult<IEnumerable<Issue>>> List(Guid communityId)
		{
			var issues = await node.Issues.List(communityId);
			return Ok(issues);
		}

		[HttpGet("{communityId}/{issueId}")]
		public async Task<ActionResult<Issue>> Get(Guid communityId, Guid issueId)
		{
			var issue = await node.Issues.Get(communityId, issueId);
			if (issue == null)
				return NotFound();
			return Ok(issue);
		}

		[HttpPost]
		public async Task<ActionResult<Issue>> Post(Issue issue)
		{
			logger.LogInformation("Issue: " + JsonConvert.SerializeObject(issue));

			await node.Issues.Add(issue);

			var url = Url.Action("Get", new {communityId = issue.CommunityId, issueId = issue.Id});
			return Accepted(url, issue);
		}
	}
}