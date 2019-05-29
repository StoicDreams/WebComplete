using StoicDreams.Catalog;
using Newtonsoft.Json;

namespace StoicDreams.Interfaces
{
	public interface IAPIResponse
	{
		APIResult Result { get; set; }
		[JsonProperty(NullValueHandling= NullValueHandling.Ignore)]
		object Data { get; set; }
		[JsonIgnore]
		int StatusCode { get; set; }
	}
}
