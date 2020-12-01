using System;
using Kingmaddi.Foundation.FotoWareExtension.Models;
using Sitecore.Web.UI.Sheer;

namespace Kingmaddi.Foundation.FotoWareExtension.Jobs
{
  public interface IImportFotoWareImageJob
  {
    /// <summary>
    /// Starts the background job for downloading the selected image from FotoWare and uploading to media-library.
    /// </summary>
    /// <param name="selectedImage">Selected image</param>
    /// <param name="args">ClientPipelineArgs</param>
    /// <returns>Job name</returns>
    void StartJob(SelectedImageModel selectedImage, ClientPipelineArgs args, Action<ClientPipelineArgs, SelectedImageModel> postAction);

    /// <summary>
    /// Resets the Job-Status-Handler.
    /// </summary>
    void ResetStatus();
  }
}
