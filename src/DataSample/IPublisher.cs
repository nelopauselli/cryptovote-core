using System;
using System.Threading.Tasks;
using Domain.Elections;

namespace DataSample
{
	public interface IPublisher
	{
		Task Add(Community community);
		Task Add(Question question);
		Task Add(Member member);
		Task Add(Urn urn);
		Task Add(Fiscal fiscal);
		Task<Community[]> ListCommunities();
		Task<Question[]> ListQuestions(Guid communityId);
		Task<Member[]> ListMembers(Guid communityId);
	}
}