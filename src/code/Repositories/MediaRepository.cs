using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Kingmaddi.Foundation.FotoWareExtension.Logging;
using Kingmaddi.Foundation.FotoWareExtension.Models;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Resources.Media;
using Sitecore.SecurityModel;

namespace Kingmaddi.Foundation.FotoWareExtension.Repositories
{
  public class MediaRepository : IMediaRepository  
  {
    private readonly Database _database; 

    public MediaRepository()
    {
      _database = Factory.GetDatabase("master");
    }

    /// <inheritdoc />
    public MediaItem UploadToMediaLibrary(SelectedImageModel selectedImage, string itemName, Language language)
    {
      var targetSitecorePath = Settings.GetSetting("Foundation.FotoWareExtensions.TargetPath", "/sitecore/media library/FotoWare");
      var tenantUrl = Settings.GetSetting("Foundation.FotoWareExtensions.FotoWare.TenantUrl", string.Empty);
      if (!string.IsNullOrEmpty(tenantUrl))
      {
        if (!string.IsNullOrEmpty(targetSitecorePath))
        {
          try
          {
            var webRequest = WebRequest.Create(selectedImage.ImageUrl);
            using (var webResponse = webRequest.GetResponse())
            {
              using (var stream = webResponse.GetResponseStream())
              {
                using (var memoryStream = new MemoryStream())
                {
                  if (stream != null)
                  {
                    stream.CopyTo(memoryStream);
                  }
                  else
                  {
                    return null;
                  }

                  var mediaCreator = new MediaCreator();
                  var options = new MediaCreatorOptions
                  {
                    Versioned = false,
                    IncludeExtensionInItemName = false,
                    Database = _database,
                    Destination = string.Format("{0}/{1}", targetSitecorePath, itemName),
                    AlternateText = selectedImage.Alt
                  };

                  using (new SecurityDisabler())
                  {
                    Item item = null;

                    using (new EventDisabler())
                    {
                      item = mediaCreator.CreateFromStream(memoryStream,
                        Path.GetFileName(selectedImage.ImageUrl.LocalPath), options);
                    }

                    if (item != null)
                    {
                      return UpdateMetaData(item.ID, selectedImage, tenantUrl, language);
                    }

                    return null;
                  }
                }
              }
            }
          }

          catch (Exception e)
          {
            FotoWareFieldsLog.WriteToLog(
              "--- Media Repository: FotoWare-Image could not be uploaded to sitecore media-library: ---", e);
            return null;
          }
        }
        else
        {
          FotoWareFieldsLog.WriteToLog("--- Media Repository: target-path-setting is not set in config-file. : ---");
          return null;
        }
      }
      else
      {
        FotoWareFieldsLog.WriteToLog("--- Media Repository: tenant-url-setting is not set in config-file. : ---");
        return null;
      }
    }

    /// <inheritdoc />
    public MediaItem GetMediaItemById(ID id, Language language)
    {
      try
      {
        using (new LanguageSwitcher(language))
        {
          var item = _database.GetItem(id);
          return new MediaItem(item);
        }
      }
      catch (Exception e)
      {
        FotoWareFieldsLog.WriteToLog("--- Media Repository: Could not get media-item by id: ---", e);
        return null;
      }
    }

    /// <inheritdoc />
    public List<Item> GetAllImages(Language language)
    {
      var targetSitecorePath = Settings.GetSetting("Foundation.FotoWareExtensions.TargetPath", "/sitecore/media library/FotoWare");
      if (!string.IsNullOrEmpty(targetSitecorePath))
      {
        var item = GetItemByPath(targetSitecorePath, language);
        if (item != null)
        {
          return item.GetChildren().ToList();
        }
      }
      else
      {
        FotoWareFieldsLog.WriteToLog("--- Media Repository: target-path-setting is not set in config-file. : ---");
        return null;
      }

      return null;
    }

    /// <inheritdoc />
    public void UpdateMetaData(Item image, SitecoreMetaDataModel metaData, Language language)
    {
      using (new SecurityDisabler())
      {
        using (new LanguageSwitcher(language))
        {
          try
          {
            image.Editing.BeginEdit();
            image.Fields[Templates.ImageMediaItem.Fields.LastModification].Value = metaData.LastModification.ToString();
            image.Appearance.DisplayName = metaData.Title;
            image.Fields[Templates.ImageMediaItem.Fields.Alt].Value = metaData.Title;
            image.Fields[Templates.ImageMediaItem.Fields.Title].Value = metaData.Title;
            image.Fields[Templates.ImageMediaItem.Fields.Keywords].Value = string.Join(" ", metaData.Keywords.ToArray());
            image.Fields[Templates.ImageMediaItem.Fields.Description].Value = metaData.Description;
            image.Editing.EndEdit();
          }
          catch (Exception e)
          {
            FotoWareFieldsLog.WriteToLog("--- Media Repository: could not updates the meta data. : ---", e);
            throw;
          }
        }
      }
    }

    /// <inheritdoc />
    public bool HasLinks(Item image)
    {
      try
      {
        var links = Sitecore.Globals.LinkDatabase.GetItemReferrers(image, true);
        if (links.Length > 0)
        {
          return true;
        }

        return false;
      }
      catch (Exception e)
      {
        FotoWareFieldsLog.WriteToLog(
          $"--- Media Repository: could not delete image with id  {image.ID} from Sitecore. : ---", e);
        return true;
      }
    }

    /// <inheritdoc />
    public void DeleteImage(Item image)
    {
      using (new SecurityDisabler())
      {
        try
        { 
          image.Delete();
        }
        catch (Exception e)
        {
          FotoWareFieldsLog.WriteToLog(
            $"--- Media Repository: could not delete image with id  {image.ID} from Sitecore. : ---", e);
        }
      }
    }

    /// <inheritdoc />
    public void SetImageStatusToDeleted(Item image)
    {
      using (new SecurityDisabler())
      {
        try
        {
          image.Editing.BeginEdit();
          image.Fields[Templates.ImageMediaItem.Fields.DeletedFromFotoWare].Value = "DELETED";
          image.Editing.EndEdit();
        }
        catch (Exception e)
        {
          FotoWareFieldsLog.WriteToLog(
            $"--- Media Repository: could not set image status to deleted for image with id  {image.ID} : ---", e);
        }
      }
    }

    /// <inheritdoc />
    public void ResetImageStatus(Item image)
    {
      using (new SecurityDisabler())
      {
        try
        {
          image.Editing.BeginEdit();
          image.Fields[Templates.ImageMediaItem.Fields.DeletedFromFotoWare].Value = string.Empty;
          image.Editing.EndEdit();
        }
        catch (Exception e)
        {
          FotoWareFieldsLog.WriteToLog(
            $"--- Media Repository: could not reset image status for image with id  {image.ID} : ---", e);
        }
      }
    }

    /// <summary>
    /// Sets the metadata from FotoWare for image-item.
    /// </summary>
    /// <param name="itemId">ID of Image</param>
    /// <param name="imageData">Metadata of Image</param>
    /// <param name="tenantUrl">Tenant-Url</param>
    /// <param name="language">Content-Language</param>
    /// <returns></returns>
    private MediaItem UpdateMetaData(ID itemId, SelectedImageModel imageData, string tenantUrl, Language language)
    {
      try
      {
        using (new SecurityDisabler())
        {
          using (new LanguageSwitcher(language))
          {
            var mediaItem = _database.GetItem(itemId);
            if (mediaItem != null)
            {
              mediaItem.Editing.BeginEdit();
              mediaItem.Fields[Templates.ImageMediaItem.Fields.Software].Value = "FotoWare";
              mediaItem.Fields[Templates.ImageMediaItem.Fields.LastModification].Value = imageData.LastModification.ToString();
              mediaItem.Appearance.DisplayName = imageData.Title;
              mediaItem.Fields[Templates.ImageMediaItem.Fields.Alt].Value = imageData.Alt;
              mediaItem.Fields[Templates.ImageMediaItem.Fields.Title].Value = imageData.Title;
              mediaItem.Fields[Templates.ImageMediaItem.Fields.Keywords].Value = string.Join(" ", imageData.Keywords.ToArray());
              mediaItem.Fields[Templates.ImageMediaItem.Fields.Description].Value = imageData.Description;
              mediaItem.Fields[Templates.ImageMediaItem.Fields.Height].Value = imageData.Height.ToString();
              mediaItem.Fields[Templates.ImageMediaItem.Fields.Width].Value = imageData.Width.ToString();
              mediaItem.Fields[Sitecore.Buckets.Util.Constants.BucketableField].Value = "1"; //make Item bucketable
              mediaItem.Fields[Templates.ImageMediaItem.Fields.FotoWareUrl].Value = string.Format("{0}|{1}", tenantUrl + imageData.MetaDataUrl, imageData.ImageUrl);
              mediaItem.Editing.EndEdit();

              return new MediaItem(mediaItem);
            }
          }
        }
      }
      catch (Exception e)
      {
        FotoWareFieldsLog.WriteToLog("--- Media Repository: Metadata could not be updated: ---", e);
        return null;
      }

      return null;
    }

    /// <summary>
    /// Get image by path.
    /// </summary>
    /// <param name="path">Path</param>
    /// <param name="language">Content-Language</param>
    /// <returns></returns>
    private Item GetItemByPath(string path, Language language)
    {
      using (new LanguageSwitcher(language))
      {
        try
        {
          var item = _database.GetItem(path);
          if (item != null)
          {
            return item;
          }
          else
          {
            FotoWareFieldsLog.WriteToLog($"--- Media Repository: image with path { path } not found : ---");
            return null;
          }
        }
        catch (Exception e)
        {
          FotoWareFieldsLog.WriteToLog("--- Media Repository: could not get image by path. : ---", e);
          return null;
        }
      }
    }
  }
}
