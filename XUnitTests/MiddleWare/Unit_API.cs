using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using StoicDreams.Middleware;
using StoicDreams.Catalog;
using Microsoft.AspNetCore.Http;
using Moq;
using StoicDreams.Interfaces;

namespace XUnitTests.MiddleWare
{
	public class Unit_API
	{
		private IAPIResponse<object> HandleOnAPICall(HttpContext context)
		{
			APIResponse<object> response = new APIResponse<object>()
			{
				Result = APIResult.Success
			};
			return response;
		}
		[Fact]
		public void Verify_APIMiddlewareFromServices()
		{
			Mock<IServiceCollection> mockServices = new Mock<IServiceCollection>();

			mockServices.Object.AddAPIOptions(options => {
				options.OnAPICall = HandleOnAPICall;
			});
		}

		[Fact]
		public void Verify_APIMiddleware()
		{
			var context = new DefaultHttpContext();
			context.Request.Path = "/api/hello";
			var middleware = new APIMiddleware(next: (context) => Task.FromResult(0), options: new APIOptions()
			{
				OnAPICall = HandleOnAPICall
			});
			middleware.InvokeAsync(context).GetAwaiter().GetResult();
			Assert.Equal("application/json", context.Response.ContentType);
			Assert.Equal(200, context.Response.StatusCode);
		}
		[Theory]
		[InlineData("/")]
		[InlineData("/other")]
		public void Verify_APIMiddlewareNotCalled(string path)
		{
			var context = new DefaultHttpContext();
			context.Request.Path = path;
			var middleware = new APIMiddleware(next: (context) =>
			{
				context.Response.ContentType = "text/plain";
				return Task.FromResult(0);
			}, options: new APIOptions() { OnAPICall = HandleOnAPICall });
			middleware.InvokeAsync(context).GetAwaiter().GetResult();
			Assert.Equal("text/plain", context.Response.ContentType);
			Assert.Equal(200, context.Response.StatusCode);
		}
	}
}
