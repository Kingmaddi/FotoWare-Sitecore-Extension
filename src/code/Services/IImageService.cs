using Kingmaddi.Foundation.FotoWareExtension.Models;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Kingmaddi.Foundation.FotoWareExtension.Services
{
  public interface IImageService
  {
    /// <summary>
    /// Uploads the Image from FotoWare to media library.
    /// </summary>
    /// <param name="selectedImage">Selected Image</param>
    /// <returns></returns>
    MediaItem UploadImage(SelectedImageModel selectedImage);

    /// <summary>
    /// Get a Image by id.
    /// </summary>
    /// <param name="id">Sitecore-ID</param>
    /// <returns></returns>
    MediaItem GetImageById(ID id);

    /// <summary>
    /// Returns the sitecore-url by sitecore-id.
    /// </summary>
    /// <param name="id">Sitecore-ID</param>
    /// <returns></returns>
    string GetImageUrlById(ID id);
  }
}
