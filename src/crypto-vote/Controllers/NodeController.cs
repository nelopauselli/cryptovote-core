using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace crypto_vote.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class NodeController : ControllerBase
	{
		private readonly INode node;
		private readonly ILogger<NodeController> logger;

		public NodeController(INode node, ILogger<NodeController> logger)
		{
			this.node = node;
			this.logger = logger;
		}

		[HttpGet]
		public Peer Get()
		{
			logger.LogDebug("Mostrando información del nodo");
			return node.Host;
		}
	}
}