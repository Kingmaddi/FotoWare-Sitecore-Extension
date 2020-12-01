using System;
using Kingmaddi.Foundation.FotoWareExtension.Logging;
using Kingmaddi.Foundation.FotoWareExtension.Models;
using Kingmaddi.Foundation.FotoWareExtension.Services;
using Sitecore;
using Sitecore.Jobs;
using Sitecore.Shell.Framework.Jobs;
using Sitecore.Web.UI.Sheer;

namespace Kingmaddi.Foundation.FotoWareExtension.Jobs
{
  public class ImportFotoWareImageJob : IImportFotoWareImageJob
  {
    public static string HandleString { get; set; }
    private readonly IImageService _imageService;
    private readonly IStatusMessageService _statusMessageService;

    public ImportFotoWareImageJob(IImageService imageService, IStatusMessageService statusMessageService)
    {
      _imageService = imageService;
      _statusMessageService = statusMessageService;
    }

    /// <inheritdoc />
    public void StartJob(SelectedImageModel selectedImage, ClientPipelineArgs args, Action<ClientPipelineArgs, SelectedImageModel> postAction)
    {
      ClientPipelineArgs currentPipelineArgs = Sitecore.Context.ClientPage.CurrentPipelineArgs as ClientPipelineArgs;

      if (currentPipelineArgs != null)
      {
        ExecuteSync("ImportFotoWareImageJob", "", ImportImage, postAction, selectedImage);
      }
      else
      {
        FotoWareFieldsLog.WriteToLog(
          "--- ImportFotoWareImageTest Job: ClientPipelineArgs is null ---");
      }
    }

    /// <inheritdoc />
    public void ResetStatus()
    {
      HandleString = string.Empty;
    }

    /// <summary>
    /// Starts the import job with progress bar.
    /// </summary>
    /// <param name="jobName">Job-Name</param>
    /// <param name="icon">Icon</param>
    /// <param name="action">Start-Action</param>
    /// <param name="postAction">Finish-Action</param>
    /// <param name="selectedImage">Selected image</param>
    private static void ExecuteSync(string jobName, string icon, Action<ClientPipelineArgs, SelectedImageModel> action, Action<ClientPipelineArgs, SelectedImageModel> postAction, SelectedImageModel selectedImage)
    {
      ClientPipelineArgs currentPipelineArgs = Context.ClientPage.CurrentPipelineArgs as ClientPipelineArgs;
      if (currentPipelineArgs == null)
      {
        return;
      }
      if (currentPipelineArgs.IsPostBack && !string.IsNullOrEmpty(HandleString))
      {
        if (postAction != null)
        {
          postAction(currentPipelineArgs, selectedImage);
        }
        return;
      }
      string name = Client.Site.Name;
      object target = action.Target;
      string str = action.Method.Name;
      object[] objArray = new object[] { currentPipelineArgs, selectedImage };
      JobOptions jobOption = new JobOptions(jobName, "FotoWareFields", name, target, str, objArray)
      {
        ContextUser = Context.User,
        ClientLanguage = Context.Language
      };
      Job job = JobManager.Start(jobOption);
      LongRunningOptions longRunningOption = new LongRunningOptions(job.Handle.ToString())
      {
        Title = "Bild aus FotoWare importieren",
        Icon = icon,
        Threshold = 5
      };

      HandleString = job.Handle.ToString();

      longRunningOption.ShowModal(true);
      currentPipelineArgs.WaitForPostBack();
    }

    /// <summary>
    /// Imports the selected Image to media-library.
    /// </summary>
    /// <param name="args">ClientPipelineArgs</param>
    /// <param name="selectedImage">Selected image</param>
    private void ImportImage(ClientPipelineArgs args, SelectedImageModel selectedImage)
    {
      Job job = Sitecore.Context.Job;
      job.Status.State = JobState.Running;
      job.Status.AddMessage(_statusMessageService.GetJobStatusImporting());

      var uploadedImage = _imageService.UploadImage(selectedImage);
      if (uploadedImage != null)
      {
        args.Parameters.Add(Templates.ImportImageClientPipeline.Parameters.MediaItemId, uploadedImage.ID.ToString());
        job.Status.AddMessage(_statusMessageService.GetImageUploadSuccessfullyMessage());
        job.Status.State = JobState.Finished;
      }
      else
      {
        job.Status.AddMessage(_statusMessageService.GetImageCouldNotUploadedMessage());
        job.Status.LogError(_statusMessageService.GetImageCouldNotUploadedMessage());
        FotoWareFieldsLog.WriteToLog(
          "--- ImportFotoWareImageProgressBar Job: Selected image could not be added to media library ---");
        job.Status.Failed = true;
      }
    }
  }
}
