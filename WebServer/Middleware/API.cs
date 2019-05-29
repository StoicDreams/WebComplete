using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StoicDreams.Interfaces;
using StoicDreams.Serialize;


namespace StoicDreams.Middleware
{
	public class APIMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IAPIOptions config;

		public APIMiddleware(RequestDelegate next, IAPIOptions options)
		{
			_next = next;
			config = options;
		}

		public async Task InvokeAsync(HttpContext httpContext)
		{
			string path = httpContext.Request?.Path.Value ?? "/";
			if(path.Length < config.APIFolder.Length)
			{
				await _next(httpContext);
				return;
			}
			if(path.Substring(0, config.APIFolder.Length).ToLower() != config.APIFolder)
			{
				await _next(httpContext);
				return;
			}
			IAPIResponse<object> result = await config.OnAPICall(httpContext);
			httpContext.Response.StatusCode = result.StatusCode;
			httpContext.Response.ContentType = config.ContentType;
			JSON serial = new JSON();
			string json = await serial.SerializeAsync(result);
			byte[] data = Encoding.UTF8.GetBytes(json);
			await httpContext.Response.Body.WriteAsync(data, 0, data.Length);
		}
	}

	public interface IAPIOptions
	{
		string APIFolder { get; set; }
		string ContentType { get; set; }
		Func<HttpContext, Task<IAPIResponse<object>>> OnAPICall { get; set; }
	}

	public class APIOptions : IAPIOptions
	{
		public string APIFolder { get; set; } = "/api/";
		/// <summary>
		/// Content type returned by API.
		/// Defaults to "application/json".
		/// </summary>
		public string ContentType { get; set; } = "application/json";
		public Func<HttpContext, Task<IAPIResponse<object>>> OnAPICall { get; set; }
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class APIExtensions
	{
		private const string defaultFolder = "/api/";
		public static IApplicationBuilder UseAPI(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<APIMiddleware>();
		}
		public static void AddAPIOptions(this IServiceCollection services, Action<IAPIOptions> setupOptions)
		{
			IAPIOptions options = new APIOptions();
			setupOptions(options);
			options.APIFolder = CleanFolderPath(options.APIFolder ?? defaultFolder);
			if(options.OnAPICall == null)
			{
				throw new Exception("APIMiddleware options was not assigned a processing method for OnAPICall.");
			}
			services.AddSingleton(options);
		}
		private static string CleanFolderPath(string input)
		{
			if(input.Length == 0) { return defaultFolder; }
			input = input.Replace('\\', '/').ToLower();
			if(input[0] != '/') { input = $"/{input}"; }
			if(input[input.Length-1] != '/') { input = $"{input}/"; }
			return input;
		}
	}
}
