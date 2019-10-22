﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Scrutiny;
using Newtonsoft.Json;

namespace DataSample
{
	internal class ConsoleAdapter : IPublisher
	{
		private readonly IList<Community> communities = new List<Community>();
		private readonly IList<Issue> issues = new List<Issue>();
		private readonly IList<Member> members = new List<Member>();
		private readonly IList<Urn> urns = new List<Urn>();
		private readonly IList<Fiscal> fiscals = new List<Fiscal>();
		
		public Task Add(Community community)
		{
			Console.WriteLine(JsonConvert.SerializeObject(community));
			communities.Add(community);
			return Task.FromResult(0);
		}

		public Task Add(Issue issue)
		{
			Console.WriteLine(JsonConvert.SerializeObject(issue));
			issues.Add(issue);
			return Task.FromResult(0);
		}

		public Task Add(Member member)
		{
			Console.WriteLine(JsonConvert.SerializeObject(member));
			members.Add(member);
			return Task.FromResult(0);
		}

		public Task Add(Urn urn)
		{
			Console.WriteLine(JsonConvert.SerializeObject(urn));
			urns.Add(urn);
			return Task.FromResult(0);
		}

		public Task Add(Fiscal fiscal)
		{
			Console.WriteLine(JsonConvert.SerializeObject(fiscal));
			fiscals.Add(fiscal);
			return Task.FromResult(0);
		}

		public Task<Community[]> ListCommunities()
		{
			return Task.FromResult(communities.ToArray());
		}

		public Task<Issue[]> ListIssues(Guid communityId)
		{
			return Task.FromResult(issues.Where(i=>i.CommunityId==communityId).ToArray());

		}

		public Task<Member[]> ListMembers(Guid communityId)
		{
			return Task.FromResult(members.Where(m => m.CommunityId == communityId).ToArray());
		}
	}
}