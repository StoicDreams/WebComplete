using Microsoft.AspNetCore.Http;

namespace StoicDreams.Extensions
{
	public static class IHeaderDictionary_TryGetHeader
	{
		/// <summary>
		/// Get header value as string if available.
		/// Returns true if header found and not empty
		/// </summary>
		/// <param name="header"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool TryGetHeader(this IHeaderDictionary header, string name, out string value)
		{
			value = "";
			if (header.ContainsKey(name) && !string.IsNullOrWhiteSpace(header[name]))
			{
				value = header[name];
				return true;
			}
			return false;
		}
	}
}
