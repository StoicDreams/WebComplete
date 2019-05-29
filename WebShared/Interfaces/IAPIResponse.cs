using StoicDreams.Catalog;
using Newtonsoft.Json;

namespace StoicDreams.Interfaces
{
	public interface IAPIResponse<T>
	{
		APIResult Result { get; set; }
		[JsonProperty(NullValueHandling= NullValueHandling.Ignore)]
		T Data { get; set; }
		[JsonIgnore]
		int StatusCode { get; set; }
	}
}
