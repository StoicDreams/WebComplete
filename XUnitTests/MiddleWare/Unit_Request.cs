using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using StoicDreams.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;

namespace XUnitTests.MiddleWare
{
	public class Unit_Request
	{
		private void HandleRequest(HttpContext context)
		{
			context.Response.ContentType = "text/plain";
			context.Response.WriteAsync("Hello World");
		}
		[Fact]
		public void Verify_RequestMiddlewareFromServices()
		{
			Mock<IServiceCollection> mockServices = new Mock<IServiceCollection>();

			mockServices.Object.AddRequestOptions(options => {
				options.OnRequest = HandleRequest;
			});
		}
		[Fact]
		public void Verify_RequestMiddleware()
		{
			var context = new DefaultHttpContext();
			context.Request.Path = "/";
			var middleware = new RequestMiddleware(next: (context) => Task.FromResult(0), options: new RequestOptions()
			{
				OnRequest = HandleRequest
			});
			middleware.InvokeAsync(context).GetAwaiter().GetResult();
			Assert.Equal("text/plain", context.Response.ContentType);
			Assert.Equal(200, context.Response.StatusCode);
		}
		[Fact]
		public void Verify_RequestMiddlewareSkipped()
		{
			var context = new DefaultHttpContext();
			context.Request.Path = "/";
			var middleware = new RequestMiddleware(next: (context) => {
				context.Response.ContentType = "text/plain";
				context.Response.WriteAsync("Skipped");
				return Task.FromResult(0);
			}, options: new RequestOptions()
			{
				OnRequest = context => {
				}
			});
			middleware.InvokeAsync(context).GetAwaiter().GetResult();
			Assert.Equal("text/plain", context.Response.ContentType);
			Assert.Equal(200, context.Response.StatusCode);
		}
	}
}
