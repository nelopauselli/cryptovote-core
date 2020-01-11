using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace crypto_vote.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ChainController : ControllerBase
	{
		private readonly Node node;
		private readonly ILogger<ChainController> _logger;

		public ChainController(Node node, ILogger<ChainController> logger)
		{
			this.node = node;
			_logger = logger;
		}

		[HttpGet]
		public Block Get(string id)
		{
			_logger.LogDebug("Consuntado último bloque");
			return node.Blockchain.Last;
			
		}
	}
}
