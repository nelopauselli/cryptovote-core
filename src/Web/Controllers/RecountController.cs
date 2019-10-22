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
			var issues = await nodeAdapter.Recounts.Get(urnId);
			return Ok(issues);
		}

		// POST api/values
		[HttpPost]
		public async Task<ActionResult<Recount>> Post(Recount recount)
		{
			logger.LogInformation("Recount: " + JsonConvert.SerializeObject(recount));

			await nodeAdapter.Recounts.Add(recount);

			var url = Url.Action("Get", new {urnId = recount.UrnId });
			return Created(url, recount);
		}
	}
}
