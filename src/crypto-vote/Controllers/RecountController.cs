using System;
using System.Text.Json;
using Domain;
using Domain.Converters;
using Domain.Elections;
using Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace crypto_vote.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RecountController : ControllerBase
	{
		private readonly ILogger<RecountController> logger;
		private readonly INode node;

		public RecountController(INode node, ILogger<RecountController> logger)
		{
			this.logger = logger;
			this.node = node;
		}

		// GET api/values
		[HttpGet("{urnId}")]
		public ObjectResult Get(Guid urnId)
		{
			var query = new RecountQuery(node.Blockchain);
			return Ok(query.Execute(urnId));
		}

		// POST api/values
		[HttpPost]
		public ObjectResult Post(Recount recount)
		{
			logger.LogInformation("Recount: " + JsonSerializer.Serialize(recount, JsonDefaultSettings.Options));
			
			node.Add(recount);

			var url = Url.Action("Get", new {urnId = recount.UrnId });
			return Created(url, recount);
		}
	}
}
