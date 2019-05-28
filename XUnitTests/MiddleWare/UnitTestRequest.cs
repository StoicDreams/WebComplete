using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System.Net.Http;
using System.Threading.Tasks;
using StoicDreams.Middleware;
using System.Text;

namespace XUnitTests.MiddleWare
{
	public class UnitTestRequest
	{
		private const string testContent = "Test";
		[Fact]
		public async Task TestRequestMiddleware()
		{
			var builder = new WebHostBuilder()
				.UseEnvironment("Development")
				.UseStartup<TestStartup>()
				;
			var testServer = new TestServer(builder);
			HttpClient client = testServer.CreateClient();
			HttpResponseMessage response = await client.GetAsync("/api/test");
			Assert.Equal(testContent, await response.Content.ReadAsStringAsync());
		}

		public class TestStartup
		{
			public void ConfigureServices(IServiceCollection services)
			{
				services.AddRequestOptions(options => {
					options.OnRequest = async context => {
						byte[] data = Encoding.UTF8.GetBytes(testContent);
						await context.Response.Body.WriteAsync(data, 0, data.Length);
					};
				});
			}
			public void Configure(IApplicationBuilder app)
			{
				app.UseRequest();
			}
		}
	}
}
