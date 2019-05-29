using StoicDreams.Interfaces;

namespace StoicDreams.Catalog
{
	public class APIResponse<V> : IAPIResponse<V>
	{
		public APIResult Result { get; set; } = APIResult.Error;
		public V Data { get; set; }
		public int StatusCode { get; set; } = 200;
	}
}
