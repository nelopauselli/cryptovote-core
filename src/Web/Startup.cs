using System;
using Domain.Converters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Web.Controllers;

namespace Web
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
			services.AddMvc()
				.AddMvcOptions(options =>
				{
					options.EnableEndpointRouting = false;
				})
				.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.Converters.Add(new GuidJsonConverter());
					options.JsonSerializerOptions.Converters.Add(new DatetimeOffsetJsonConverter());
					options.JsonSerializerOptions.Converters.Add(new ByteArrayJsonConverter());
				})
				.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

			services.AddSingleton<NodeAdapter>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseMvc();
			app.UseWebSockets();
		}
	}
}
 