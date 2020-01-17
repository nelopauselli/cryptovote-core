using Domain;
using Domain.Elections;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoVote
{
	public static class BlockchainExtensions
	{
		public static IServiceCollection AddBlockchain(this IServiceCollection services)
		{
			services.AddSingleton<INodeConfiguration, NodeConfiguration>();
			services.AddSingleton<IBlockBuilder, BlockBuilder>();
			services.AddSingleton<IPeerChannel, HttpPeerChannel>();
			services.AddSingleton<INode, Node>();

			return services;
		}

		public static IApplicationBuilder UseBlockchain(this IApplicationBuilder app)
		{
			var node = app.ApplicationServices.GetService<INode>();
			node.Start();
			return app;
		}
	}
}