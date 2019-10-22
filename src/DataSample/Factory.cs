using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Converters;
using Domain.Crypto;
using Domain.Scrutiny;
using Domain.Utils;
using Newtonsoft.Json;

namespace DataSample
{
	public class Factory
	{
		private readonly ICryptoService service;
		private readonly IPublisher publisher;
		private readonly List<Community> communities = new List<Community>();
		private readonly List<Issue> issues = new List<Issue>();
		private readonly List<Member> members = new List<Member>();
		private readonly List<Urn> urns = new List<Urn>();
		private readonly List<Fiscal> fiscals = new List<Fiscal>();

		public Factory(ICryptoService service, IPublisher publisher)
		{
			this.service = service;
			this.publisher = publisher;
			JsonConvert.DefaultSettings = () =>
			{
				var settings = new JsonSerializerSettings();
				settings.Converters.Add(new GuidJsonConverter());
				settings.Converters.Add(new DatetimeOffsetJsonConverter());
				settings.Converters.Add(new ByteArrayJsonConverter());
				return settings;
			};
		}

		public async Task<Guid> Community(string name, KeysPair keys)
		{
			var exist = communities.SingleOrDefault(o => o.Name == name);
			if (exist != null) return exist.Id;

			var community = new Community
			{
				Id = Guid.NewGuid(),
				Name = name,
				CreateAt = DateTimeOffset.Now,
				Address = keys.PublicKey
			};

			var signer = new Signer(service);
			signer.Sign(community, keys);

			await publisher.Add(community);
			communities.Add(community);

			return community.Id;
		}

		public async Task<Guid> Issue(Guid communityId, string name, Choice[] choices, byte type, KeysPair keys)
		{
			var exist = issues.SingleOrDefault(o => o.CommunityId == communityId && o.Name == name);
			if (exist != null) return exist.Id;

			var issue = new Issue
			{
				Id = Guid.NewGuid(),
				CommunityId = communityId,
				Name = name,
				Type = type,
				EndTime = DateTimeOffset.Now.AddMonths(1).ToUnixTimeMilliseconds(),
				Choices = choices
			};

			var signer = new Signer(service);
			issue.Signature = signer.Sign(issue.GetData(), keys);
			issue.PublicKey = new byte[keys.PublicKey.Length];
			Buffer.BlockCopy(keys.PublicKey, 0, issue.PublicKey, 0, keys.PublicKey.Length);

			await publisher.Add(issue);
			issues.Add(issue);

			return issue.Id;
		}

		public async Task<Guid> Member(Guid communityId, string name, byte[] address, KeysPair keys)
		{
			var exist = members.SingleOrDefault(o => o.CommunityId == communityId && o.Name == name);
			if (exist != null) return exist.Id;

			var member = new Member
			{
				Id = Guid.NewGuid(),
				CommunityId = communityId,
				Name = name
			};
			member.Address = new byte[keys.PublicKey.Length];
			Buffer.BlockCopy(address, 0, member.Address, 0, keys.PublicKey.Length);

			var signer = new Signer(service);
			member.Signature = signer.Sign(member.GetData(), keys);
			member.PublicKey = new byte[keys.PublicKey.Length];
			Buffer.BlockCopy(keys.PublicKey, 0, member.PublicKey, 0, keys.PublicKey.Length);

			await publisher.Add(member);
			members.Add(member);

			return member.Id;
		}

		public async Task<Guid> Urn(Guid issueId, string name, byte[][] authorities, KeysPair keys)
		{
			var exist = urns.SingleOrDefault(o => o.IssueId == issueId && o.Name == name);
			if (exist != null) return exist.Id;

			var urn = new Urn
			{
				Id = Guid.NewGuid(),
				IssueId = issueId,
				Name = name,
				Authorities = authorities
			};
			var signer = new Signer(service);
			urn.Signature = signer.Sign(urn.GetData(), keys);
			urn.PublicKey = new byte[keys.PublicKey.Length];
			Buffer.BlockCopy(keys.PublicKey, 0, urn.PublicKey, 0, keys.PublicKey.Length);

			await publisher.Add(urn);
			urns.Add(urn);

			return urn.Id;
		}

		public async Task<Guid> Fiscal(Guid issueId, Guid choiceId, byte[] fiscalAddress, KeysPair guardianKey)
		{
			var exist = fiscals.SingleOrDefault(o => o.IssueId == issueId && o.ChoiceId == choiceId && o.Address == fiscalAddress);
			if (exist != null) return exist.Id;

			var fiscal = new Fiscal
			{
				Id = Guid.NewGuid(),
				IssueId = issueId,
				ChoiceId = choiceId,
				Address = fiscalAddress
			};
			var signer = new Signer(service);
			signer.Sign(fiscal, guardianKey);

			await publisher.Add(fiscal);
			fiscals.Add(fiscal);

			return fiscal.Id;
		}

		public void Vote()
		{
			var vote1 = new Vote
			{
				IssueId = new Guid("bd746b3b276e454a8B1e041cf53a8747"),
				ChoiceId = Guid.NewGuid(),
				Time = DateTimeOffset.Now.ToUnixTimeMilliseconds()
			};

			var keys = service.GeneratePair();

			var signer = new Signer(service);
			vote1.Signature = signer.Sign(vote1.GetData(), keys);
			var signatureBase58 = Base58.Encode(vote1.Signature);
			vote1.PublicKey = new byte[keys.PublicKey.Length];
			Buffer.BlockCopy(keys.PublicKey, 0, vote1.PublicKey, 0, keys.PublicKey.Length);

			var json = JsonConvert.SerializeObject(vote1);
			Console.WriteLine(json);

			var vote = JsonConvert.DeserializeObject<Vote>(json);
			var publicKeyBase58 = Base58.Encode(vote.PublicKey);

			var verifier = new SignatureVerify(service);
		}

		public void Load()
		{
			var communityTask = publisher.ListCommunities();
			communityTask.Wait();
			communities.AddRange(communityTask.Result);

			foreach (var community in communities)
			{

				var issueTask = publisher.ListIssues(community.Id);
				var memberTask = publisher.ListMembers(community.Id);

				Task.WaitAll(issueTask, memberTask);
				issues.AddRange(issueTask.Result);
				members.AddRange(memberTask.Result);
			}

		}
	}
}