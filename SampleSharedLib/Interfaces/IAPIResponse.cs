//using StoicDreams.Catalog;

namespace StoicDreams.Interfaces
{
	public interface IAPIResponse
	{
		//APIResult Result { get; set; }
		object Data { get; set; }
		int StatusCode { get; set; }
	}
}
