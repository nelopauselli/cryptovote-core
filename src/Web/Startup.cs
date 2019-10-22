using System;
using Domain.Converters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Web.Controllers;
using DatetimeOffsetJsonConverter = Web.Converters.DatetimeOffsetJsonConverter;
using GuidJsonConverter = Web.Converters.GuidJsonConverter;

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
			JsonConvert.DefaultSettings = () =>
			{
				var settings = new JsonSerializerSettings()
				{
					Formatting = Formatting.None,
				};
				settings.Converters.Add(new GuidJsonConverter());
				settings.Converters.Add(new DatetimeOffsetJsonConverter());
				settings.Converters.Add(new ByteArrayJsonConverter());
				return settings;
			};

			services.AddMvc()
				.AddJsonOptions(options =>
				{
					options.SerializerSettings.Converters.Add(new GuidJsonConverter());
					options.SerializerSettings.Converters.Add(new DatetimeOffsetJsonConverter());
					options.SerializerSettings.Converters.Add(new ByteArrayJsonConverter());
				})
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

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
 