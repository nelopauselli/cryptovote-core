﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Scrutiny;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CommunityController : ControllerBase
	{
		private readonly ILogger<CommunityController> logger;
		private readonly NodeAdapter nodeAdapter;

		public CommunityController(NodeAdapter nodeAdapter, ILogger<CommunityController> logger)
		{
			this.logger = logger;
			this.nodeAdapter = nodeAdapter;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Community>>> Get()
		{
			var communities = await nodeAdapter.Communities.ListCommunities();
			return Ok(communities);
		}

		[HttpGet("{communityId}")]
		public async Task<ActionResult<Community>> Get(Guid communityId)
		{
			var community = await nodeAdapter.Communities.Get(communityId);
			return Ok(community);
		}

		[HttpPost]
		public async Task<ActionResult<Community>> Post(Community community)
		{
			logger.LogInformation("Community: " + JsonConvert.SerializeObject(community));

			await nodeAdapter.Communities.AddCommunity(community);

			var url = Url.Action("Get", new { communityId = community.Id });
			return Accepted(url, community);
		}
	}
}