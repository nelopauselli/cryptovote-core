using Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace crypto_vote
{
	public static class ApplicationBuilderExtensions{
		public static IApplicationBuilder UseBlockchain(this IApplicationBuilder app)
		{
			var node = app.ApplicationServices.GetService<INode>();
			node.Start();
			return app;
		}
	}
}