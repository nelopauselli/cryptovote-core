using System;
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
	public class MemberController : ControllerBase
	{
		private readonly ILogger<MemberController> logger;
		private readonly NodeAdapter node;

		public MemberController(ILogger<MemberController> logger, NodeAdapter node)
		{
			this.logger = logger;
			this.node = node;
		}

		[HttpGet("{communityId}")]
		public async Task<ActionResult<IEnumerable<Member>>> Get(Guid communityId)
		{
			var members = await node.Members.ListMembers(communityId);
			return Ok(members);
		}

		[HttpGet("{communityId}/{memberId}")]
		public async Task<ActionResult<Member>> Get(Guid communityId, Guid memberId)
		{
			var member = await node.Members.GetMember(communityId, memberId);
			return Ok(member);
		}

		[HttpPost]
		public async Task<ActionResult<Member>> Post(Member member)
		{
			logger.LogInformation("Member: " + JsonConvert.SerializeObject(member));

			await node.Members.AddMember(member);

			var url = Url.Action("Get", new { communityId = member.CommunityId, MemberId = member.Id });
			return Accepted(url);
		}
	}
}