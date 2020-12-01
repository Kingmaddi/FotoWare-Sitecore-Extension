using System;
using Kingmaddi.Foundation.FotoWareExtension.Jobs;
using Kingmaddi.Foundation.FotoWareExtension.Logging;
using Kingmaddi.Foundation.FotoWareExtension.Mappers;
using Kingmaddi.Foundation.FotoWareExtension.Models;
using Kingmaddi.Foundation.FotoWareExtension.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;

namespace Kingmaddi.Foundation.FotoWareExtension.Dialogs
{
  public class SelectImageFromFotoWareDialog : DialogForm
  {
    protected Edit RawValue;
    protected Edit AccessToken = new Edit();
    protected Edit TenantUrl = new Edit();
    private readonly IAuthenticationService _authenticationService;
    private readonly IImageService _imageService;
    private readonly IStatusMessageService _statusMessageService;
    private readonly ISelectedImageModelMapper _selectedImageModelMapper;
    private readonly IImportFotoWareImageJob _fotoWareImageJob;

    public SelectImageFromFotoWareDialog() : base()
    {
      _authenticationService = ServiceLocator.ServiceProvider.GetService<IAuthenticationService>();
      _imageService = ServiceLocator.ServiceProvider.GetService<IImageService>();
      _statusMessageService = ServiceLocator.ServiceProvider.GetService<IStatusMessageService>();
      _selectedImageModelMapper = ServiceLocator.ServiceProvider.GetService<ISelectedImageModelMapper>();
      _fotoWareImageJob = ServiceLocator.ServiceProvider.GetService<IImportFotoWareImageJob>();
    }

    /// <inheritdoc />
    protected override void OnLoad(EventArgs e)
    {
      //Authenticate with FotoWare
      AccessToken.Value = _authenticationService.GetAccessToken();

      //Get Tenant Url
      TenantUrl.Value = Settings.GetSetting("Foundation.FotoWareExtensions.FotoWare.TenantUrl", string.Empty);

      //Load Dialog
      base.OnLoad(e);

      if (Context.ClientPage.IsEvent) return;

      RawValue.Value = WebUtil.GetQueryString("val");
    }

    /// <inheritdoc />
    protected override void OnOK(object sender, EventArgs args)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(args, "args");

      var value = RawValue.Value;

      if (IsRichTextEditorMode())
      {
        var mediaResponse = _selectedImageModelMapper.MapJsonStringToSelectedImageModel(value);

        if (mediaResponse != null)
        {
          ClientPipelineArgs pipeline = new ClientPipelineArgs();
          pipeline.Parameters.Add(Templates.ImportImageClientPipeline.Parameters.SelectedImageJson, JsonConvert.SerializeObject(mediaResponse, Formatting.Indented));
          Context.ClientPage.Start(this, "StartImageImport", pipeline);
        }
        else
        {
          FotoWareFieldsLog.WriteToLog("--- SelectImageFromFotoWare Dialog: Selected image must be exported before upload to media-library ---");
          SheerResponse.Alert(_statusMessageService.GetImageMustBeExportedMessage());
        }
      }
      else
      {
        SheerResponse.SetDialogValue(value);
        base.OnOK(sender, args);
      }
    }

    /// <inheritdoc />
    protected override void OnCancel(object sender, EventArgs args)
    {
      if (IsRichTextEditorMode())
      {
        SheerResponse.Eval("scCancel()");
      }
      else
      {
        base.OnCancel(sender, args);
      }
    }

    /// <summary>
    /// Starts the image import-job.
    /// </summary>
    /// <param name="args">ClientPipelineArgs</param>
    protected void StartImageImport(ClientPipelineArgs args)
    {
      var selectedImage = _selectedImageModelMapper.MapJsonStringToSelectedImageModel(args.Parameters[Templates.ImportImageClientPipeline.Parameters.SelectedImageJson]);

      _fotoWareImageJob.StartJob(selectedImage, new ClientPipelineArgs(), ImportImageFinished);
    }

    /// <summary>
    /// Checks if the Dialog-Request was send by RichtText-Editor.
    /// </summary>
    /// <returns></returns>
    private bool IsRichTextEditorMode()
    {
      var fieldtype = WebUtil.GetQueryString("fieldtype");
      if (!string.IsNullOrEmpty(fieldtype))
      {
        if (fieldtype == "RichText")
        {
          return true;
        }
      }

      return false;
    }


    /// <summary>
    /// Sets the value to field.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="selectedImage"></param>
    private void ImportImageFinished(ClientPipelineArgs args, SelectedImageModel selectedImage)
    {
      var itemIdString = args.Parameters[Templates.ImportImageClientPipeline.Parameters.MediaItemId];

      if (!string.IsNullOrEmpty(itemIdString))
      {
        var itemId = new ID(itemIdString);
        var mediaUrl = _imageService.GetImageUrlById(itemId);

        if (!string.IsNullOrEmpty(mediaUrl))
        {
          var imageResponse = new RichTextImageResponse
          {
            Url = mediaUrl,
            Alt = selectedImage.Alt
          };

          var imageResponeJson = JsonConvert.SerializeObject(imageResponse);

          SheerResponse.Eval("scClose(" + StringUtil.EscapeJavascriptString(imageResponeJson) + ")");
        }
        else
        {
          SheerResponse.Alert(_statusMessageService.GetJobStatusValueCouldNotSet());
          FotoWareFieldsLog.WriteToLog("--- SelectImageFromFotoWare Dialog: Could not set field-value ---");
        }
      }
      else
      {
        SheerResponse.Alert(_statusMessageService.GetJobStatusValueCouldNotSet());
        FotoWareFieldsLog.WriteToLog("--- SelectImageFromFotoWare Dialog: Could not set field-value ---");
      }
    }
  }
}
