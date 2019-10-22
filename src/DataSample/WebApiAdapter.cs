using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Domain.Scrutiny;
using Newtonsoft.Json;

namespace DataSample
{
	public class WebApiAdapter : IPublisher
	{
		private readonly Uri restUrl;

		public WebApiAdapter(string url)
		{
			restUrl = new Uri(url);
		}

		public async Task Add(Community community)
		{
			var body = JsonConvert.SerializeObject(community);
			await Post("/api/community", body);
		}

		public async Task Add(Urn urn)
		{
			var body = JsonConvert.SerializeObject(urn);
			await Post("/api/urn", body);
		}

		public async Task Add(Fiscal fiscal)
		{
			var body = JsonConvert.SerializeObject(fiscal);
			await Post("/api/fiscal", body);
		}

		public async Task<Community[]> ListCommunities()
		{
			return await Get<Community>("/api/community");
		}

		public async Task Add(Issue issue)
		{
			var body = JsonConvert.SerializeObject(issue);
			await Post("/api/issue", body);
		}

		public async Task<Issue[]> ListIssues(Guid communityId)
		{
			return await Get<Issue>($"/api/issue/{communityId}");
		}

		public async Task Add(Member member)
		{
			var body = JsonConvert.SerializeObject(member);
			await Post("/api/member", body);
		}
		public async Task<Member[]> ListMembers(Guid communityId)
		{
			return await Get<Member>($"/api/member/{communityId}");
		}

		private async Task<T[]> Get<T>(string path)
		{
			using (var restClient = new HttpClient())
			{
				restClient.DefaultRequestHeaders.Accept.Clear();
				restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				restClient.DefaultRequestHeaders.Add("User-Agent", "Data Sample App");


				var url = new Uri(restUrl, path);
				Console.WriteLine($"GET: {url}");

				var response = await restClient.GetStringAsync(url);

				var entities = JsonConvert.DeserializeObject<T[]>(response);

				return entities;
			}
			
		}

		private async Task Post(string path, string body)
		{
			using (var restClient = new HttpClient())
			{
				restClient.DefaultRequestHeaders.Accept.Clear();
				restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				restClient.DefaultRequestHeaders.Add("User-Agent", "Data Sample App");
				
				var url = new Uri(restUrl, path);
				Console.WriteLine($"POST: {url}");

				var content = new StringContent(body, Encoding.UTF8, "application/json");
				var response = await restClient.PostAsync(url, content);
				response.EnsureSuccessStatusCode();
			}
		}
	}
}