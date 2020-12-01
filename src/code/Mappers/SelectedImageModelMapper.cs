using System;
using Kingmaddi.Foundation.FotoWareExtension.Logging;
using Kingmaddi.Foundation.FotoWareExtension.Models;
using Newtonsoft.Json;

namespace Kingmaddi.Foundation.FotoWareExtension.Mappers
{
  public class SelectedImageModelMapper : ISelectedImageModelMapper
  {
    /// <inheritdoc />
    public SelectedImageModel MapJsonStringToSelectedImageModel(string jsonString)
    {
      try
      {
        return JsonConvert.DeserializeObject<SelectedImageModel>(jsonString);
      }
      catch (Exception e)
      {
        FotoWareFieldsLog.WriteToLog("--- SelectedImageModel Mapper: JSON-String could not be mapped to Model: ---", e);
        return null;
      }
    }
  }
}
