using System;
using System.Collections.Generic;
using System.Text.Json;
using Domain;
using Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CryptoVote.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DocumentController : ControllerBase
	{
		private readonly INode node;
		private readonly ILogger<DocumentController> logger;

		public DocumentController(INode node, ILogger<DocumentController> logger)
		{
			this.node = node;
			this.logger = logger;
		}

		[HttpGet]
		public IEnumerable<Document> List()
		{
			logger.LogDebug("Listando documentos");
			var query = new DocumentsQuery(node.Blockchain);
			return query.Execute();
		}

		[HttpGet("{id}")]
		public Document Get(Guid id)
		{
			logger.LogDebug($"Buscando documento con Id {id}");
			var query = new DocumentQuery(node.Blockchain);
			return query.Execute(id);
		}

		[HttpPost]
		public ObjectResult Post(Document model)
		{
			logger.LogInformation($"Recibiendo document: {JsonSerializer.Serialize(model)}");

			node.Add(model);
			return Accepted(model);
		}
	}
}