using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace StoicDreams.Middleware
{
	// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
	public class FileProxyMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly FileProxy.Service service;
		private readonly FileProxyOptions options;

		public FileProxyMiddleware(RequestDelegate next, IOptions<FileProxyOptions> options = null)
		{
			_next = next;
			if (options?.Value?.Routes != null)
			{
				this.options = options.Value;
				service = FileProxy.Service.StandardService(this.options.Routes);
			}
		}

		public async Task InvokeAsync(HttpContext httpContext)
		{
			var (IsMatched, fileData) = await service.HandleProxyIfMatchedAsync(httpContext.Request.Path.Value);
			if (IsMatched)
			{
				httpContext.Response.ContentType = fileData.ContentType;
				await httpContext.Response.Body.WriteAsync(fileData.Data, 0, fileData.Data.Length);
				return;
			}

			await _next(httpContext);
		}
	}

	public class FileProxyOptions
	{
		public FileProxy.Interface.IRoute[] Routes { get; set; }
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class FileProxyMiddlewareExtensions
	{
		public static IApplicationBuilder UseFileProxy(this IApplicationBuilder builder, FileProxyOptions options = null)
		{
			return builder.UseMiddleware<FileProxyMiddleware>(Options.Create(options));
		}
	}
}
