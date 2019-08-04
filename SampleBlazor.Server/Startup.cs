using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using StoicDreams.Middleware;
using StoicDreams.FileProxy.Interface;
using System;

namespace SampleBlazor.Server
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().AddNewtonsoftJson();
			services.AddResponseCompression(opts =>
			{
				opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
					new[] { "application/octet-stream" });
			});
			services.AddFileProxyOptions(options => {
				options.WebRoot = $@"{AppDomain.CurrentDomain.BaseDirectory}SampleBlazor.Client\Dist";
				options.Routes = new IRoute[]
				{
					new StoicDreams.FileProxy.Routing.FileRoute("/manifest.webmanifest", "/manifest.json")
				};
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseResponseCompression();
			app.UseHttpsRedirection();
			app.UseHsts();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBlazorDebugging();
			}

			app.UseClientSideBlazorFiles<Client.Startup>();

			app.UseFileProxy();

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
				endpoints.MapFallbackToClientSideBlazor<Client.Startup>("index.html");
			});
		}
	}
}
