using System;
using System.Collections.Generic;
using Domain;
using Domain.Elections;
using Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CryptoVote.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UrnController : ControllerBase
	{
		private readonly ILogger<UrnController> logger;
		private readonly INode node;

		public UrnController(INode node, ILogger<UrnController> logger)
		{
			this.node = node;
			this.logger = logger;
		}

		[HttpGet("{questionId}")]
		public IEnumerable<Urn> Get(Guid questionId)
		{
			var query = new UrnsQuery(node.Blockchain);
			return query.Execute(questionId);
		}

		[HttpPost]
		public ObjectResult Post(Urn urn)
		{
			node.Add(urn);
			//var url = Url.Action("Get", new { communityId = urn.QuestionId, UrnId = urn.Id });
			return Accepted(urn);
		}
	}
}