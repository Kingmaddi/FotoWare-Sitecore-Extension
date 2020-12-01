using System.Net;

namespace Kingmaddi.Foundation.FotoWareExtension.Models
{
  public class MetaDataResponse
  {
    public HttpStatusCode StatusCode { get; set; }
    public FotoWareImageMetaDataModel MetaData { get; set; }
  }
}
