using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using StoicDreams.FileProxy.Interface;
using StoicDreams.FileProxy.Routing;
using StoicDreams.Middleware;

namespace XUnitTests.MiddleWare
{
	public class UnitTestFileProxy
	{
		[Fact]
		public async Task TestFileProxyMiddleware()
		{
			var builder = new WebHostBuilder()
				.UseEnvironment("Development")
				.UseStartup<TestStartup>()
				;
			var testServer = new TestServer(builder);
			HttpClient client = testServer.CreateClient();
			HttpResponseMessage response = await client.GetAsync("/api/test");
			string fileContent = await File.ReadAllTextAsync(Path.GetFullPath("test/test.json"));
			Assert.Equal(fileContent, await response.Content.ReadAsStringAsync());
		}

		public class TestStartup
		{
			public void ConfigureServices(IServiceCollection services)
			{
				services.AddFileProxyOptions(options => {
					options.Routes = new IRoute[]
					{
						new FileRoute("/api/test", "/test/test.json")
					};
				});
			}
			public void Configure(IApplicationBuilder app)
			{
				app.UseFileProxy();
			}
		}
	}
}
