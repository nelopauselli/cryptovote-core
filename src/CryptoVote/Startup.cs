using Domain;
using Domain.Converters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CryptoVote
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			var options = new NodeOptions();
			Configuration.GetSection("Node").Bind(options);
			services.AddSingleton(options);

			services.AddBlockchain();

			services.AddCors(options =>
			{
				options.AddDefaultPolicy(builder => { builder.AllowAnyOrigin(); });
			});

			services.AddControllers()
				.AddJsonOptions(cfg =>
				{
					cfg.JsonSerializerOptions.Converters.Add(new GuidJsonConverter());
					cfg.JsonSerializerOptions.Converters.Add(new DatetimeOffsetJsonConverter());
					cfg.JsonSerializerOptions.Converters.Add(new ByteArrayJsonConverter());
				});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseBlockchain();
		
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			//app.UseHttpsRedirection();
			app.UseCors();

			app.UseRouting();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

		}
	}
}
