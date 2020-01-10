using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Converters;
using Domain.Elections;

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
			var body = JsonSerializer.Serialize(community, JsonDefaultSettings.Options);
			await Post("/api/community", body);
		}

		public async Task Add(Urn urn)
		{
			var body = JsonSerializer.Serialize(urn, JsonDefaultSettings.Options);
			await Post("/api/urn", body);
		}

		public async Task Add(Fiscal fiscal)
		{
			var body = JsonSerializer.Serialize(fiscal, JsonDefaultSettings.Options);
			await Post("/api/fiscal", body);
		}

		public async Task<Community[]> ListCommunities()
		{
			return await Get<Community>("/api/community");
		}

		public async Task Add(Question question)
		{
			var body = JsonSerializer.Serialize(question, JsonDefaultSettings.Options);
			await Post("/api/question", body);
		}

		public async Task<Question[]> ListQuestions(Guid communityId)
		{
			return await Get<Question>($"/api/question/{communityId}");
		}

		public async Task Add(Member member)
		{
			var body = JsonSerializer.Serialize(member, JsonDefaultSettings.Options);
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

				var entities = JsonSerializer.Deserialize<T[]>(response, JsonDefaultSettings.Options);

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