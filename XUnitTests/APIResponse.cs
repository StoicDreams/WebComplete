using StoicDreams.Catalog;
using StoicDreams.Interfaces;

namespace XUnitTests
{
	public class APIResponse : IAPIResponse
	{
		public APIResult Result { get; set; }
		public object Data { get; set; }
		public int StatusCode { get; set; } = 200;
	}
}
