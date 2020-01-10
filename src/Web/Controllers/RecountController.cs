using System;
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
	public class RecountController : ControllerBase
	{
		private readonly ILogger<RecountController> logger;
		private readonly NodeAdapter nodeAdapter;

		public RecountController(NodeAdapter nodeAdapter, ILogger<RecountController> logger)
		{
			this.logger = logger;
			this.nodeAdapter = nodeAdapter;
		}

		// GET api/values
		[HttpGet("{urnId}")]
		public async Task<ActionResult<Recount>> Get(Guid urnId)
		{
			var questions = await nodeAdapter.Recounts.Get(urnId);
			return Ok(questions);
		}

		// POST api/values
		[HttpPost]
		public async Task<ActionResult<Recount>> Post(Recount recount)
		{
			logger.LogInformation("Recount: " + JsonSerializer.Serialize(recount, JsonDefaultSettings.Options));

			await nodeAdapter.Recounts.Add(recount);

			var url = Url.Action("Get", new {urnId = recount.UrnId });
			return Created(url, recount);
		}
	}
}
