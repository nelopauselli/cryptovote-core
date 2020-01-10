using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Converters;
using Domain.Elections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class QuestionController : ControllerBase
	{
		private readonly ILogger<QuestionController> logger;
		private readonly NodeAdapter node;

		public QuestionController(ILogger<QuestionController> logger, NodeAdapter node)
		{
			this.logger = logger;
			this.node = node;
		}

		[HttpGet("{communityId}")]
		public async Task<ActionResult<IEnumerable<Question>>> List(Guid communityId)
		{
			var questions = await node.Questions.List(communityId);
			return Ok(questions);
		}

		[HttpGet("{communityId}/{questionId}")]
		public async Task<ActionResult<Question>> Get(Guid communityId, Guid questionId)
		{
			var question = await node.Questions.Get(communityId, questionId);
			if (question == null)
				return NotFound();
			return Ok(question);
		}

		[HttpPost]
		public async Task<ActionResult<Question>> Post(Question question)
		{
			logger.LogInformation("Question: " + JsonSerializer.Serialize(question, JsonDefaultSettings.Options));

			await node.Questions.Add(question);

			var url = Url.Action("Get", new {communityId = question.CommunityId, questionId = question.Id});
			return Accepted(url, question);
		}
	}
}