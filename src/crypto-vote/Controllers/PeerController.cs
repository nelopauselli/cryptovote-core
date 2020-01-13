using System.Collections.Generic;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace crypto_vote.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PeerController : ControllerBase
	{
		private readonly INode node;
		private readonly ILogger<PeerController> logger;

		public PeerController(INode node, ILogger<PeerController> logger)
		{
			this.node = node;
			this.logger = logger;
		}

		[HttpGet]
		public IEnumerable<Peer> List()
		{
			logger.LogDebug("Listando pares");
			return node.Peers.List();
		}

		//[HttpGet("{id}")]
		//public Peer Get(Guid id)
		//{
		//	logger.LogDebug($"Buscando comunidad con Id {id}");
		//	return node.Peers.List().SingleOrDefault(p => p.Id == id);
		//}

		[HttpPost]
		public ObjectResult Post(Peer model)
		{
			logger.LogInformation($"Agregando comunidad '{model.Name}' en la url {model.PublicUrl}");

			node.Register(model);

			//var url = Url.Action("Get", new {id = model.Id});
			return Accepted(model);
		}
	}
}