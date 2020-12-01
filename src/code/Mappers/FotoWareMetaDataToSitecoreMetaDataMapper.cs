using System;
using Kingmaddi.Foundation.FotoWareExtension.Logging;
using Kingmaddi.Foundation.FotoWareExtension.Models;

namespace Kingmaddi.Foundation.FotoWareExtension.Mappers
{
  public class FotoWareMetaDataToSitecoreMetaDataMapper : IFotoWareMetaDataToSitecoreMetaDataMapper
  {
    public SitecoreMetaDataModel MapFotoWareMetaDataToSitecoreMetaData(FotoWareImageMetaDataModel metaData)
    {
      try
      {
        var lastModification = metaData.Modified;
        var title = metaData.BuiltinFields[0].Value.String;
        var description = metaData.BuiltinFields[1].Value.String;
        var keywords = metaData.BuiltinFields[2].Value.StringArray;

        return new SitecoreMetaDataModel
        {
          LastModification = lastModification,
          Title = title,
          Description = description,
          Keywords = keywords
        };
      }
      catch (Exception e)
      {
        FotoWareFieldsLog.WriteToLog("--- FotoWareMetaDataToSitecoreMetaData Mapper: Could not map FotoWare-MetaData to Sitecore MetaData: ---", e);
        return null;
      }
    }
  }
}
