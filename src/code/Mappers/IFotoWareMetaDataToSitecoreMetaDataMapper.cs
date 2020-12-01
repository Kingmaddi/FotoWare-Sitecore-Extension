using Kingmaddi.Foundation.FotoWareExtension.Models;

namespace Kingmaddi.Foundation.FotoWareExtension.Mappers
{
  public interface IFotoWareMetaDataToSitecoreMetaDataMapper
  {
    /// <summary>
    /// Maps FotoWare image meta data to sitecore meta data.
    /// </summary>
    /// <param name="metaData">FotoWare MetaData</param>
    /// <returns>Selected Image model</returns>
    SitecoreMetaDataModel MapFotoWareMetaDataToSitecoreMetaData(FotoWareImageMetaDataModel metaData);
  }
}
