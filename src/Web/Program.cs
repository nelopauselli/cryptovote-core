using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.AddCommandLine(args)
				.Build();

			var hostUrl = configuration["hosturl"];
			if (string.IsNullOrEmpty(hostUrl))
				hostUrl = "http://0.0.0.0:5000";

			Console.WriteLine($"Host: {configuration["Node:Host"]}");
			Console.WriteLine($"Port: {configuration["Node:Port"]}");
			CreateWebHostBuilder(args, hostUrl).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args, string hostUrl) =>
			WebHost.CreateDefaultBuilder(args)
				.UseUrls(hostUrl)
				.UseStartup<Startup>();
	}
}
