using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace StoicDreams.Middleware
{
	// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
	public class FileProxyMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly FileProxy.Service service;

		public FileProxyMiddleware(RequestDelegate next, IFileProxyOptions options)
		{
			_next = next;
			if (options?.Routes != null)
			{
				service = FileProxy.Service.StandardService(options.Routes);
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

	public interface IFileProxyOptions
	{
		FileProxy.Interface.IRoute[] Routes { get; set; }
	}

	public class FileProxyOptions : IFileProxyOptions
	{
		public FileProxy.Interface.IRoute[] Routes { get; set; }
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class FileProxyMiddlewareExtensions
	{
		public static IApplicationBuilder UseFileProxy(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<FileProxyMiddleware>();
		}

		public static void AddFileProxyOptions(this IServiceCollection services, Action<IFileProxyOptions> setupOptions)
		{
			IFileProxyOptions options = new FileProxyOptions();
			setupOptions(options);
			services.AddSingleton(options);
		}

	}
}
