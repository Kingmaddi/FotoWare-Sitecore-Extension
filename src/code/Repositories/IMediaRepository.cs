using System.Collections.Generic;
using Kingmaddi.Foundation.FotoWareExtension.Models;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace Kingmaddi.Foundation.FotoWareExtension.Repositories
{
  public interface IMediaRepository
  {
    /// <summary>
    /// Uploads the selected image from FotoWare to media-library.
    /// </summary>
    /// <param name="selectedImage">Selected Image</param>
    /// <param name="itemName">Item-Name</param>
    /// <param name="language">Content-Language</param>
    /// <returns></returns>
    MediaItem UploadToMediaLibrary(SelectedImageModel selectedImage, string itemName, Language language);

    /// <summary>
    /// Gets a media item by id.
    /// </summary>
    /// <param name="id">Sitecore-ID</param>
    /// <param name="language">Content-Language</param>
    /// <returns></returns>
    MediaItem GetMediaItemById(ID id, Language language);

    /// <summary>
    /// Get all FotoWare images uploaded to Sitecore.
    /// </summary>
    /// <param name="language">Content-Language</param>
    /// <returns></returns>
    List<Item> GetAllImages(Language language);

    /// <summary>
    /// Updates the meta data of an exisiting sitecore image.
    /// </summary>
    /// <param name="image">Image</param>
    /// <param name="metaData">MetaData</param>
    /// <param name="language">Content-Language</param>
    void UpdateMetaData(Item image, SitecoreMetaDataModel metaData, Language language);

    /// <summary>
    /// Checks if a specific image has links to other items.
    /// </summary>
    /// <param name="image">Sitecore image</param>
    /// <returns></returns>
    bool HasLinks(Item image);

    /// <summary>
    /// Deletes an specific image from Sitecore.
    /// </summary>
    /// <param name="image">Sitecore image</param>
    void DeleteImage(Item image);

    /// <summary>
    /// Sets the status of an Sitecore image to deleted from FotoWare.
    /// </summary>
    /// <param name="image">Sitecore image</param>
    void SetImageStatusToDeleted(Item image);

    /// <summary>
    /// Resets the status of an Sitecore image.
    /// </summary>
    /// <param name="image">Sitecore image</param>
    void ResetImageStatus(Item image);
  }
}
