using Xunit;
using System.Threading.Tasks;
using StoicDreams.FileProxy.Interface;
using StoicDreams.FileProxy.Routing;
using StoicDreams.Middleware;
using Microsoft.AspNetCore.Http;

namespace XUnitTests.MiddleWare
{
	public class Unit_FileProxy
	{
		[Fact]
		public void Verify_FileProxyMiddleware()
		{
			var context = new DefaultHttpContext();
			context.Request.Path = "/";
			var middleware = new FileProxyMiddleware(next: (context) => Task.FromResult(0), options: new FileProxyOptions()
			{
				Routes = new IRoute[]
				{
					new FileRoute("/", "/routed")
				}
			});
			middleware.InvokeAsync(context).GetAwaiter().GetResult();
			Assert.Equal("text/plain", context.Response.ContentType);
			Assert.Equal(404, context.Response.StatusCode);
		}
	}
}
