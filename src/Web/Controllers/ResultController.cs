using System;
using System.Threading.Tasks;
using Domain.Elections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ResultController : ControllerBase
	{
		private readonly ILogger<ResultController> logger;
		private readonly NodeAdapter node;

		public ResultController(ILogger<ResultController> logger, NodeAdapter node)
		{
			this.logger = logger;
			this.node = node;
		}

		[HttpGet("{communityId}/{questionId}")]
		public async Task<ActionResult<QuestionResult>> Get(Guid communityId, Guid questionId)
		{
			var result = await node.Questions.Result(communityId, questionId);
			if (result == null)
				return NotFound();
			return Ok(result);
		}
	}
}