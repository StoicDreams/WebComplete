using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StoicDreams.Serialize;

namespace StoicDreams.Middleware
{
	public class RequestMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IRequestOptions config;

		public RequestMiddleware(RequestDelegate next, IRequestOptions options)
		{
			_next = next;
			config = options;
		}

		public async Task InvokeAsync(HttpContext httpContext)
		{
			config?.OnRequest?.Invoke(httpContext);
			if(httpContext.Response.ContentLength == 0)
			{
				await _next(httpContext);
			}
		}
	}

	public interface IRequestOptions
	{
		Action<HttpContext> OnRequest { get; set; }
	}

	public class RequestOptions : IRequestOptions
	{
		/// <summary>
		/// General request processing.
		/// Make sure to write content to HttpContext.Response.Body.
		/// </summary>
		public Action<HttpContext> OnRequest { get; set; }
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class RequestExtensions
	{
		public static IApplicationBuilder UseRequest(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<RequestMiddleware>();
		}
		public static void AddRequestOptions(this IServiceCollection services, Action<IRequestOptions> setupOptions)
		{
			IRequestOptions options = new RequestOptions();
			setupOptions(options);
			services.AddSingleton(options);
		}
	}
}
