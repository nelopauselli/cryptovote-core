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
	public class FiscalController : ControllerBase
	{
		private readonly ILogger<FiscalController> logger;
		private readonly NodeAdapter node;

		public FiscalController(ILogger<FiscalController> logger, NodeAdapter node)
		{
			this.logger = logger;
			this.node = node;
		}

		[HttpGet("{questionId}")]
		public async Task<ActionResult<IEnumerable<Fiscal>>> Get(Guid questionId)
		{
			var fiscals = await node.Fiscals.List(questionId);
			return Ok(fiscals);
		}

		[HttpPost]
		public async Task<ActionResult<Fiscal>> Post(Fiscal fiscal)
		{
			logger.LogInformation("Fiscal: " + JsonConvert.SerializeObject(fiscal));

			await node.Fiscals.Add(fiscal);

			var url = Url.Action("Get", new { communityId = fiscal.QuestionId, FiscalId = fiscal.Id });
			return Accepted(url);
		}
	}
}