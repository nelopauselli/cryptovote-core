using Domain;
using Domain.Queries;
using Domain.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace crypto_vote.Controllers
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
		public Block Get(string id)
		{
			logger.LogDebug("Consuntado último bloque");
			var query = new BlockQuery(node.Blockchain);
			return query.Execute(Base58.Decode(id));
		}

		[HttpPost]
		public ObjectResult Post(Block block)
		{
			node.Add(block);
			//var url = Url.Action("Get", new {id = block.Hash});
			return Accepted(block);
		}
	}
}
