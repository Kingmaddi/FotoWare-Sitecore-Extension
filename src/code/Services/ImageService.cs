using System;
using System.IO;
using Kingmaddi.Foundation.FotoWareExtension.Logging;
using Kingmaddi.Foundation.FotoWareExtension.Models;
using Kingmaddi.Foundation.FotoWareExtension.Repositories;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Kingmaddi.Foundation.FotoWareExtension.Services
{
  public class ImageService : IImageService
  {
    private readonly IMediaRepository _mediaRepository;
    private readonly ILanguageRepository _languageRepository;

    public ImageService(IMediaRepository mediaRepository, ILanguageRepository languageRepository)
    {
      _mediaRepository = mediaRepository;
      _languageRepository = languageRepository;
    }

    /// <inheritdoc />
    public MediaItem UploadImage(SelectedImageModel selectedImage)
    {
      var contentLanguage = _languageRepository.GetContentLanguage();
      try
      {
        return _mediaRepository.UploadToMediaLibrary(selectedImage, GetItemName(selectedImage), contentLanguage);
      }
      catch (Exception e)
      {
        FotoWareFieldsLog.WriteToLog("--- Image Service: FotoWare-Image could not be uploaded to sitecore media-library: ---", e);
        return null;
      }
    }

    /// <inheritdoc />
    public MediaItem GetImageById(ID id)
    {
      var contentLanguage = _languageRepository.GetContentLanguage();
      var mediaItem = _mediaRepository.GetMediaItemById(id, contentLanguage);
      if (mediaItem != null)
      {
        return mediaItem;
      }
      else
      {
        FotoWareFieldsLog.WriteToLog("--- Image Service: item not found. : ---");
        return null;
      }
    }

    /// <inheritdoc />
    public string GetImageUrlById(ID id)
    {
      var mediaItem = GetImageById(id);
      if (mediaItem != null)
      {
        var itemId = mediaItem.ID.Guid.ToString("N");
        return string.Format("/-/media/{0}.ashx", itemId);
      }
      else
      {
        FotoWareFieldsLog.WriteToLog("--- Image Service: Url to image is empty, because item not found ---");
        return string.Empty;
      }
    }

    /// <summary>
    /// Generates a specific sitecore item name for selected image.
    /// </summary>
    /// <param name="selectedImage">Selected image</param>
    /// <returns>Item-name</returns>
    private string GetItemName(SelectedImageModel selectedImage)
    {
      try
      {
        var fullImageName = Path.GetFileName(selectedImage.ImageUrl.LocalPath);
        var imageNameArray = fullImageName.Split('.');
        return imageNameArray[0];
      }
      catch (Exception e)
      {
        FotoWareFieldsLog.WriteToLog("--- Image Service: Could not generate item name for selected image: ---", e);
        return string.Empty;
      }
    }
  }
}
