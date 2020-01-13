﻿using System;
using System.Collections.Generic;
using Domain;
using Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace crypto_vote.Controllers
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
			logger.LogDebug($"Buscando comunidad con Id {id}");
			var query = new DocumentQuery(node.Blockchain);
			return query.Execute(id);
		}

		[HttpPost]
		public ObjectResult Post(Document model)
		{
			logger.LogInformation($"Agregando documento");
			node.Add(model);
			return Accepted(model);
		}
	}
}