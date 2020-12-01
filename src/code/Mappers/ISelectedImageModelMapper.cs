using Kingmaddi.Foundation.FotoWareExtension.Models;

namespace Kingmaddi.Foundation.FotoWareExtension.Mappers
{
  public interface ISelectedImageModelMapper
  {
    /// <summary>
    /// Maps the json-string of selected FotoWare image to model. 
    /// </summary>
    /// <param name="jsonString">JSON-string of selected image</param>
    /// <returns></returns>
    SelectedImageModel MapJsonStringToSelectedImageModel(string jsonString);
  }
}
