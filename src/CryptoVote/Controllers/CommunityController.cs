using System;
using System.Collections.Generic;
using System.Text.Json;
using Domain;
using Domain.Elections;
using Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CryptoVote.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CommunityController : ControllerBase
	{
		private readonly INode node;
		private readonly ILogger<CommunityController> logger;

		public CommunityController(INode node, ILogger<CommunityController> logger)
		{
			this.node = node;
			this.logger = logger;
		}

		[HttpGet]
		public IEnumerable<Community> List()
		{
			logger.LogDebug("Listando comunidades");
			var query = new CommunitiesQuery(node.Blockchain);
			return query.Execute();
		}
		
		[HttpGet("{id}")]
		public Community Get(Guid id)
		{
			logger.LogDebug($"Buscando comunidad con Id {id}");
			var query = new CommunityQuery(node.Blockchain);
			return query.Execute(id);
		}

		[HttpPost]
		public ObjectResult Post(Community model)
		{
			logger.LogInformation($"Recibiendo comunidad: {JsonSerializer.Serialize(model)} desde '{Request?.HttpContext?.Connection?.RemoteIpAddress}:{Request?.HttpContext?.Connection?.RemotePort}'");

			node.Add(model);
			
			//var url = Url.Action("Get", new {id = model.Id});
			return Accepted(model);
		}
	}
}