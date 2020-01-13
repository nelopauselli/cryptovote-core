using System;
using System.Collections.Generic;
using Domain;
using Domain.Elections;
using Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace crypto_vote.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FiscalController : ControllerBase
	{
		private readonly ILogger<FiscalController> logger;
		private readonly INode node;

		public FiscalController(INode node, ILogger<FiscalController> logger)
		{
			this.node = node;
			this.logger = logger;
		}

		[HttpGet("{questionId}")]
		public IEnumerable<Fiscal> Get(Guid questionId)
		{
			var query=new FiscalsQuery(node.Blockchain);
			return query.Execute(questionId);
		}

		[HttpPost]
		public ObjectResult Post(Fiscal fiscal)
		{
			node.Add(fiscal);
			//var url = Url.Action("Get", new { communityId = fiscal.QuestionId, FiscalId = fiscal.Id });
			return Accepted(fiscal);
		}
	}
}