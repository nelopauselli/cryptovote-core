using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Converters;
using Domain.Elections;

namespace DataSample
{
	internal class FileAdapter : IPublisher
	{
		private readonly IList<Community> communities = new List<Community>();
		private readonly IList<Question> questions = new List<Question>();
		private readonly IList<Member> members = new List<Member>();
		private readonly IList<Urn> urns = new List<Urn>();
		private readonly IList<Fiscal> fiscals = new List<Fiscal>();

		public async Task Add(Community community)
		{
			await File.WriteAllTextAsync($"community-{communities.Count+1}.json", JsonSerializer.Serialize(community, JsonDefaultSettings.Options));
			communities.Add(community);
		}

		public async Task Add(Question question)
		{
			await File.WriteAllTextAsync($"question-{questions.Count+1}.json", JsonSerializer.Serialize(question, JsonDefaultSettings.Options));
			questions.Add(question);
		}

		public async Task Add(Member member)
		{
			await File.WriteAllTextAsync($"member{members.Count+1}.json", JsonSerializer.Serialize(member, JsonDefaultSettings.Options));
			members.Add(member);
		}

		public async Task Add(Urn urn)
		{
			await File.WriteAllTextAsync($"urn{urns.Count + 1}.json", JsonSerializer.Serialize(urn, JsonDefaultSettings.Options));
			urns.Add(urn);
		}

		public async Task Add(Fiscal fiscal)
		{
			await File.WriteAllTextAsync($"fiscal{fiscals.Count + 1}.json", JsonSerializer.Serialize(fiscal, JsonDefaultSettings.Options));
			fiscals.Add(fiscal);
		}

		public Task<Community[]> ListCommunities()
		{
			return Task.FromResult(communities.ToArray());
		}

		public Task<Question[]> ListQuestions(Guid communityId)
		{
			return Task.FromResult(questions.ToArray());
		}

		public Task<Member[]> ListMembers(Guid communityId)
		{
			return Task.FromResult(members.ToArray());
		}
	}
}