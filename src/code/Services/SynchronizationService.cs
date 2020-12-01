using System;
using System.Collections.Generic;
using System.Net;
using Kingmaddi.Foundation.FotoWareExtension.Logging;
using Kingmaddi.Foundation.FotoWareExtension.Mappers;
using Kingmaddi.Foundation.FotoWareExtension.Models;
using Kingmaddi.Foundation.FotoWareExtension.Repositories;
using Sitecore.Configuration;
using Sitecore.Data.Items;

namespace Kingmaddi.Foundation.FotoWareExtension.Services
{
  public class SynchronizationService : ISynchronizationService
  {
    private readonly IFotoWareRepository _fotoWareRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly IFotoWareMetaDataToSitecoreMetaDataMapper _fotoWareMetaDataToSitecoreMetaDataMapper;

    public SynchronizationService(IFotoWareRepository fotoWareRepository, IMediaRepository mediaRepository, ILanguageRepository languageRepository, IFotoWareMetaDataToSitecoreMetaDataMapper fotoWareMetaDataToSitecoreMetaDataMapper)
    {
      _fotoWareRepository = fotoWareRepository;
      _mediaRepository = mediaRepository;
      _languageRepository = languageRepository;
      _fotoWareMetaDataToSitecoreMetaDataMapper = fotoWareMetaDataToSitecoreMetaDataMapper;
    }

    /// <inheritdoc />
    public void SynchronizeData(string accessToken)
    {
      var targetMediaPath = Settings.GetSetting("Foundation.FotoWareExtensions.TargetPath", "/sitecore/media library/FotoWare");
      if (!string.IsNullOrEmpty(targetMediaPath))
      {
        var deletedImages = new List<Tuple<Item, bool>>();
        var modifiedImages = new List<Tuple<Item, FotoWareImageMetaDataModel>>();

        var allImages = _mediaRepository.GetAllImages(_languageRepository.GetContentLanguage());
        if (allImages != null)
        {
          foreach (var image in allImages)
          {
            try
            {
              if (_mediaRepository.HasLinks(image))
              {
                var imageUrls = image.Fields[Templates.ImageMediaItem.Fields.FotoWareUrl].Value;
                var urlArray = imageUrls.Split('|');
                var infoUrl = urlArray[0];
                if (!string.IsNullOrEmpty(infoUrl))
                {
                  var metaData = _fotoWareRepository.GetMetaData(infoUrl, accessToken);
                  if (metaData?.MetaData != null)
                  {
                    if (urlArray.Length > 1)
                    {
                      if (_fotoWareRepository.IsImageExists(urlArray[1]))
                      {
                        //reset status
                        _mediaRepository.ResetImageStatus(image);

                        //check modification
                        if (IsModified(image, metaData.MetaData))
                        {
                          //image was modified in FotoWare
                          modifiedImages.Add(new Tuple<Item, FotoWareImageMetaDataModel>(image, metaData.MetaData));
                        }
                      }
                      else
                      {
                        //exported image not exists on FotoWare
                        deletedImages.Add(new Tuple<Item, bool>(image, true));
                      }
                    }
                    else
                    {
                      FotoWareFieldsLog.WriteToLog($"--- Synchronization Service: Image with ID {image.ID} has no FotoWare-Embed-Url: ---");
                    }
                  }
                  else if (metaData?.StatusCode == HttpStatusCode.NotFound)
                  {
                    //image was deleted from FotoWare
                    deletedImages.Add(new Tuple<Item, bool>(image, true));
                  }
                  else
                  {
                    FotoWareFieldsLog.WriteToLog($"--- Synchronization Service: Could not get FotoWare-MetaData for Image with ID {image.ID}: ---");
                  }
                }
                else
                {
                  FotoWareFieldsLog.WriteToLog($"--- Synchronization Service: Image with ID {image.ID} has no FotoWare-Info-Url: ---");
                }
              }
              else
              {
                //image not used in sitecore
                deletedImages.Add(new Tuple<Item, bool>(image, false));
              }
            }
            catch (Exception e)
            {
              FotoWareFieldsLog.WriteToLog($"--- Synchronization Service: Error while synchronize metadata for Image with ID {image.ID}: ---", e);
            }
          }
        }
        else
        {
          FotoWareFieldsLog.WriteToLog("--- Synchronization Service: There was an error while getting all images ---");
        }

        DeleteRemovedImages(deletedImages);
        ModifyOutdatedImages(modifiedImages);
      }
      else
      {
        FotoWareFieldsLog.WriteToLog("--- Synchronization Service: target-path-setting is not set in config-file. : ---");
      }
    }

    /// <summary>
    /// Check if image metadata is outdated.
    /// </summary>
    /// <param name="image">Sitecore-Image</param>
    /// <param name="metaData">MetaData</param>
    /// <returns></returns>
    private bool IsModified(Item image, FotoWareImageMetaDataModel metaData)
    {
      var lastModificationString = image.Fields[Templates.ImageMediaItem.Fields.LastModification].Value;
      var lastModificationInSitecore = DateTimeOffset.Parse(lastModificationString);

      var isModified = DateTimeOffset.Compare(lastModificationInSitecore, metaData.Modified);

      if (isModified < 0)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// Deletes removed images from FotoWare.
    /// </summary>
    /// <param name="deletedImages">Deleted images</param>
    private void DeleteRemovedImages(List<Tuple<Item, bool>> deletedImages)
    {
      foreach (var imageData in deletedImages)
      {
        if (imageData.Item2)
        {
          //could not delete image, because there are links to other items
          _mediaRepository.SetImageStatusToDeleted(imageData.Item1);
        }
        else
        {
          //delete image
          _mediaRepository.DeleteImage(imageData.Item1);
        }
      }
    }

    /// <summary>
    /// Modify outdated MetaData.
    /// </summary>
    /// <param name="outdatedImages"></param>
    private void ModifyOutdatedImages(List<Tuple<Item, FotoWareImageMetaDataModel>> outdatedImages)
    {
      var contentLanguage = _languageRepository.GetContentLanguage();

      foreach (var outdatedImageTuple in outdatedImages)
      {
        var sitecoreMetaData =
          _fotoWareMetaDataToSitecoreMetaDataMapper.MapFotoWareMetaDataToSitecoreMetaData(outdatedImageTuple.Item2);
        if (sitecoreMetaData != null)
        {
          _mediaRepository.UpdateMetaData(outdatedImageTuple.Item1, sitecoreMetaData, contentLanguage);
        }
      }
    }
  }
}
