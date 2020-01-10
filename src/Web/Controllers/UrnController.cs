using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Converters;
using Domain.Elections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

		[HttpGet("{questionId}")]
		public async Task<ActionResult<IEnumerable<Urn>>> Get(Guid questionId)
		{
			var urns = await node.Urns.List(questionId);
			return Ok(urns);
		}

		[HttpPost]
		public async Task<ActionResult<Urn>> Post(Urn urn)
		{
			logger.LogInformation("Urn: " + JsonSerializer.Serialize(urn, JsonDefaultSettings.Options));

			await node.Urns.Add(urn);

			var url = Url.Action("Get", new { communityId = urn.QuestionId, UrnId = urn.Id });
			return Accepted(url);
		}
	}
}