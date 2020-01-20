using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CryptoVote.Controllers
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
		public ObjectResult Get()
		{
			logger.LogDebug("Mostrando información del nodo");
			var nodeInfo = new
			{
				node.Host.Id,
				node.Host.Name,
				node.Host.PublicUrl,
				Branches = node.Blockchain.BranchesCount,
				LastBlockNumber = node.Blockchain.Last.BlockNumber,
				LastBlockHash = node.Blockchain.Last.Hash
			};

			return Ok(nodeInfo);
		}
	}
}