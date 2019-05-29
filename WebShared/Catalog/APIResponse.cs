using StoicDreams.Interfaces;

namespace StoicDreams.Catalog
{
	public class APIResponse : IAPIResponse
	{
		public APIResult Result { get; set; } = APIResult.Error;
		public object Data { get; set; }
		public int StatusCode { get; set; } = 200;
	}
}
