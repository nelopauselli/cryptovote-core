using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Elections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UrnController : ControllerBase
	{
		private readonly ILogger<UrnController> logger;
		private readonly NodeAdapter node;

		public UrnController(ILogger<UrnController> logger, NodeAdapter node)
		{
			this.logger = logger;
			this.node = node;
		}

		[HttpGet("{issueId}")]
		public async Task<ActionResult<IEnumerable<Urn>>> Get(Guid issueId)
		{
			var Urns = await node.Urns.List(issueId);
			return Ok(Urns);
		}

		[HttpPost]
		public async Task<ActionResult<Urn>> Post(Urn urn)
		{
			logger.LogInformation("Urn: " + JsonConvert.SerializeObject(urn));

			await node.Urns.Add(urn);

			var url = Url.Action("Get", new { communityId = urn.IssueId, UrnId = urn.Id });
			return Accepted(url);
		}
	}
}