using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System.Net.Http;
using System.Threading.Tasks;
using StoicDreams.Middleware;
using System.Collections.Generic;

namespace XUnitTests.MiddleWare
{
	public class UnitTestAPI
	{
		[Fact]
		public async Task TestAPIMiddleware()
		{
			var builder = new WebHostBuilder()
				.UseEnvironment("Development")
				.UseStartup<TestStartup>()
				;
			var testServer = new TestServer(builder);
			HttpClient client = testServer.CreateClient();
			HttpResponseMessage response = await client.GetAsync("/api/test");
			Assert.Equal(@"{""Result"":1}", await response.Content.ReadAsStringAsync());
		}
		[Fact]
		public async Task TestAPIMiddlewareWithData()
		{
			var builder = new WebHostBuilder()
				.UseEnvironment("Development")
				.UseStartup<TestStartupWithData>()
				;
			var testServer = new TestServer(builder);
			HttpClient client = testServer.CreateClient();
			HttpResponseMessage response = await client.GetAsync("/api/test");
			Assert.Equal(@"{""Result"":1,""Data"":[{""Foo"":""Bar""}]}", await response.Content.ReadAsStringAsync());
		}

		public class TestStartup
		{
			public void ConfigureServices(IServiceCollection services)
			{
				services.AddAPIOptions(options => {
					options.OnAPICall = async context => {
						APIResponse response = new APIResponse()
						{
							Result = StoicDreams.Catalog.APIResult.Success
						};
						return response;
					};
				});
			}
			public void Configure(IApplicationBuilder app)
			{
				app.UseAPI();
			}
		}

		public class TestStartupWithData
		{
			public void ConfigureServices(IServiceCollection services)
			{
				services.AddAPIOptions(options => {
					options.OnAPICall = async context => {
						APIResponse response = new APIResponse()
						{
							Result = StoicDreams.Catalog.APIResult.Success,
							Data = new Dictionary<string, object>[] {
								new Dictionary<string, object>()
								{
									{"Foo", "Bar" }
								}
							}
						};
						return response;
					};
				});
			}
			public void Configure(IApplicationBuilder app)
			{
				app.UseAPI();
			}
		}
	}
}
