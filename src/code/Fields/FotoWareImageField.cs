using Kingmaddi.Foundation.FotoWareExtension.Jobs;
using Kingmaddi.Foundation.FotoWareExtension.Logging;
using Kingmaddi.Foundation.FotoWareExtension.Mappers;
using Kingmaddi.Foundation.FotoWareExtension.Models;
using Kingmaddi.Foundation.FotoWareExtension.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore;
using Sitecore.Data;
using Sitecore.DependencyInjection;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web.UI.Sheer;
using Message = Sitecore.Web.UI.Sheer.Message;

namespace Kingmaddi.Foundation.FotoWareExtension.Fields
{
  public class FotoWareImageField : Image
  {
    private readonly IImageService _imageService;
    private readonly IStatusMessageService _statusMessageService;
    private readonly ISelectedImageModelMapper _selectedImageModelMapper;
    private readonly IImportFotoWareImageJob _fotoWareImageJob;

    public FotoWareImageField() : base()
    {
      _imageService = ServiceLocator.ServiceProvider.GetService<IImageService>();
      _statusMessageService = ServiceLocator.ServiceProvider.GetService<IStatusMessageService>();
      _selectedImageModelMapper = ServiceLocator.ServiceProvider.GetService<ISelectedImageModelMapper>();
      _fotoWareImageJob = ServiceLocator.ServiceProvider.GetService<IImportFotoWareImageJob>();
    }

    /// <inheritdoc />
    public override void HandleMessage(Message message)
    {
      base.HandleMessage(message);
      if (message["id"] != this.ID || string.IsNullOrWhiteSpace(message.Name)) return;

      switch (message.Name)
      {
        case "fotowareimage:SelectImageFromFotoWare":
          Sitecore.Context.ClientPage.Start((object)this, "SelectFromFotoWare");
          break;
        default:
          if (this.Value.Length > 0)
          {
            this.SetModified();
            this.Value = string.Empty;
          }
          break;
      }
    }

    /// <summary>
    /// Uploads the selected image to media-library and set value to image-field.
    /// </summary>
    /// <param name="args">ClientPipeline</param>
    protected void SelectFromFotoWare(ClientPipelineArgs args)
    {
      if (args.IsPostBack)
      {
        if (!args.HasResult || this.Value.Equals(args.Result))
          return;

        SelectedImageModel selectedImage = null;
        if (args.Result == "Finished")
        {
          var selectedImageJson = args.Parameters[Templates.ImportImageClientPipeline.Parameters.SelectedImageJson];
          selectedImage = _selectedImageModelMapper.MapJsonStringToSelectedImageModel(selectedImageJson);
        }
        else
        {
          selectedImage = _selectedImageModelMapper.MapJsonStringToSelectedImageModel(args.Result);
          args.Parameters.Add(Templates.ImportImageClientPipeline.Parameters.SelectedImageJson, args.Result);
        }

        if (selectedImage != null)
        {
          _fotoWareImageJob.StartJob(selectedImage, args, this.ImportImageFinished);
        }
        else
        {
          //image has to be exported to upload
          FotoWareFieldsLog.WriteToLog("--- FotoWareImageField: Selected image has to be exported before ---");
          SheerResponse.Alert(_statusMessageService.GetImageMustBeExportedMessage());
        }
      }
      else
      {
        _fotoWareImageJob.ResetStatus();
        string url = UIUtil.GetUri("control:SelectImageFromFotoWareDialog");
        string str = this.GetValue();
        if (!string.IsNullOrEmpty(str))
          url = string.Format("{0}&value={1}", (object)url, (object)str);
        SheerResponse.ShowModalDialog(url, "1200", "750", "", true, "1200", "750", false);
        args.WaitForPostBack();
      }
    }

    /// <summary>
    /// Sets the value to field.
    /// </summary>
    /// <param name="args">ClientPipelineArgs</param>
    /// <param name="selectedImage">Selected image</param>
    private void ImportImageFinished(ClientPipelineArgs args, SelectedImageModel selectedImage)
    {
      var itemIdString = args.Parameters[Templates.ImportImageClientPipeline.Parameters.MediaItemId];

      if (!string.IsNullOrEmpty(itemIdString))
      {
        var itemId = new ID(itemIdString);

        var mediaItem = _imageService.GetImageById(itemId);

        if (mediaItem != null)
        {
          this.SetValue(mediaItem);
          this.Update();
          this.SetModified();
        }
        else
        {
          SheerResponse.Alert(_statusMessageService.GetJobStatusValueCouldNotSet());
          FotoWareFieldsLog.WriteToLog("--- FotoWareImageField: Could not set field-value ---");
        }
      }
      else
      {
        SheerResponse.Alert(_statusMessageService.GetJobStatusValueCouldNotSet());
        FotoWareFieldsLog.WriteToLog("--- FotoWareImageField: Could not set field-value ---");
      }
    }
  }
}
