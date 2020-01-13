using Domain;
using Domain.Elections;
using Microsoft.Extensions.DependencyInjection;

namespace crypto_vote
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddBlockchain(this IServiceCollection services)
		{
			var configuration = new NodeConfiguration();
			var node = new Node(configuration, new BlockBuilder(), new ColoredConsoleLogger());
			services.AddSingleton<INode>(node);

			return services;
		}
	}
}