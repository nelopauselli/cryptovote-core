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
	public class MemberController : ControllerBase
	{
		private readonly ILogger<MemberController> logger;
		private readonly INode node;

		public MemberController(INode node, ILogger<MemberController> logger)
		{
			this.node = node;
			this.logger = logger;
		}

		[HttpGet("{communityId}")]
		public IEnumerable<Member> Get(Guid communityId)
		{
			var query = new MembersQuery(node.Blockchain);
			return query.Execute(communityId);
		}

		[HttpGet("{communityId}/{memberId}")]
		public Member Get(Guid communityId, Guid memberId)
		{
			var query = new MemberQuery(node.Blockchain);
			return query.Execute(communityId, memberId);
		}

		[HttpPost]
		public ObjectResult Post(Member member)
		{
			node.Add(member);

			var url = Url.Action("Get", new { communityId = member.CommunityId, memberId = member.Id });
			return Accepted(url);
		}
	}
}