using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain.Scrutiny;
using Newtonsoft.Json;

namespace DataSample
{
	internal class FileAdapter : IPublisher
	{
		private readonly IList<Community> communities = new List<Community>();
		private readonly IList<Issue> issues = new List<Issue>();
		private readonly IList<Member> members = new List<Member>();
		private readonly IList<Urn> urns = new List<Urn>();
		private readonly IList<Fiscal> fiscals = new List<Fiscal>();

		public async Task Add(Community community)
		{
			await File.WriteAllTextAsync($"community-{communities.Count+1}.json", JsonConvert.SerializeObject(community));
			communities.Add(community);
		}

		public async Task Add(Issue issue)
		{
			await File.WriteAllTextAsync($"issue-{issues.Count+1}.json", JsonConvert.SerializeObject(issue));
			issues.Add(issue);
		}

		public async Task Add(Member member)
		{
			await File.WriteAllTextAsync($"member{members.Count+1}.json", JsonConvert.SerializeObject(member));
			members.Add(member);
		}

		public async Task Add(Urn urn)
		{
			await File.WriteAllTextAsync($"urn{urns.Count + 1}.json", JsonConvert.SerializeObject(urn));
			urns.Add(urn);
		}

		public async Task Add(Fiscal fiscal)
		{
			await File.WriteAllTextAsync($"fiscal{fiscals.Count + 1}.json", JsonConvert.SerializeObject(fiscal));
			fiscals.Add(fiscal);
		}

		public Task<Community[]> ListCommunities()
		{
			return Task.FromResult(communities.ToArray());
		}

		public Task<Issue[]> ListIssues(Guid communityId)
		{
			return Task.FromResult(issues.ToArray());
		}

		public Task<Member[]> ListMembers(Guid communityId)
		{
			return Task.FromResult(members.ToArray());
		}
	}
}