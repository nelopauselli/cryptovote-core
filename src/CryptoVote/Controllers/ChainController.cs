using System.Text.Json;
using Domain;
using Domain.Queries;
using Domain.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CryptoVote.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ChainController : ControllerBase
	{
		private readonly INode node;
		private readonly ILogger<ChainController> logger;

		public ChainController(INode node, ILogger<ChainController> logger)
		{
			this.node = node;
			this.logger = logger;
		}

		[HttpGet]
		public Block GetLast()
		{
			logger.LogDebug("Consuntado último bloque");
			var query = new LastBlockQuery(node.Blockchain);
			return query.Execute();
		}

		[HttpGet("{id}")]
		public ObjectResult Get(string id)
		{
			logger.LogDebug("Consuntado último bloque");
			var query = new BlockQuery(node.Blockchain);
			var block = query.Execute(Base58.Decode(id));
			if (block == null)
				return NotFound("No tengo ningún bloque con ese hash");
			
			return Ok(block);
		}

		[HttpPost]
		public ObjectResult Post(Block block)
		{
			logger.LogInformation($"Recibiendo block: {JsonSerializer.Serialize(block)} desde '{Request?.HttpContext?.Connection?.RemoteIpAddress}:{Request?.HttpContext?.Connection?.RemotePort}'");

			node.Add(block);
			//var url = Url.Action("Get", new {id = block.Hash});
			return Accepted(block);
		}
	}
}
