using Kingmaddi.Foundation.FotoWareExtension.Jobs;
using Kingmaddi.Foundation.FotoWareExtension.Logging;
using Kingmaddi.Foundation.FotoWareExtension.Mappers;
using Kingmaddi.Foundation.FotoWareExtension.Models;
using Kingmaddi.Foundation.FotoWareExtension.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Shell.Applications.WebEdit.Commands;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;

namespace Kingmaddi.Foundation.FotoWareExtension.Commands
{
  public class SelectImageImageFieldCommand : WebEditImageCommand
  {
    private readonly IImageService _imageService;
    private readonly ISelectedImageModelMapper _selectedImageModelMapper;
    private readonly IImportFotoWareImageJob _fotoWareImageJob;
    private readonly IStatusMessageService _statusMessageService;

    public SelectImageImageFieldCommand() : base()
    {
      _imageService = ServiceLocator.ServiceProvider.GetService<IImageService>();
      _selectedImageModelMapper = ServiceLocator.ServiceProvider.GetService<ISelectedImageModelMapper>();
      _fotoWareImageJob = ServiceLocator.ServiceProvider.GetService<IImportFotoWareImageJob>();
      _statusMessageService = ServiceLocator.ServiceProvider.GetService<IStatusMessageService>();
    }

    /// <inheritdoc />
    public override void Execute(CommandContext context)
    {
      Assert.ArgumentNotNull((object)context, nameof(context));
      WebEditCommand.ExplodeParameters(context);
      string formValue = WebUtil.GetFormValue("scPlainValue");
      context.Parameters.Add(Templates.ImportImageClientPipeline.Parameters.FieldValue, formValue);
      Context.ClientPage.Start((object)this, "Run", context.Parameters);
    }

    /// <summary>
    /// Uploads the selected image to media-library and set value to image-field.
    /// </summary>
    /// <param name="args">ClientPipeline</param>
    protected void Run(ClientPipelineArgs args)
    {
      if (args.IsPostBack)
      {
        if (!(args.Result != "undefined"))
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
          FotoWareFieldsLog.WriteToLog("--- SelectImageImageFieldCommand: Selected image has to be exported before ---");
          SheerResponse.Alert(_statusMessageService.GetImageMustBeExportedMessage());
        }
      }
      else
      {
        _fotoWareImageJob.ResetStatus();
        string url = UIUtil.GetUri("control:SelectImageFromFotoWareDialog");
        url = string.Format("{0}", (object)url);
        SheerResponse.ShowModalDialog(url, "1200", "750", "", true, "1200", "750", false);
        args.WaitForPostBack();
      }
    }

    /// <summary>
    /// Sets the value to field.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="selectedImage"></param>
    private void ImportImageFinished(ClientPipelineArgs args, SelectedImageModel selectedImage)
    {
      Assert.ArgumentNotNull((object)args, nameof(args));
      Item itemNotNull = Client.GetItemNotNull(args.Parameters["itemid"], Language.Parse(args.Parameters["language"]));
      itemNotNull.Fields.ReadAll();
      Field field = itemNotNull.Fields[args.Parameters["fieldid"]];
      Assert.IsNotNull((object)field, "field");
      ImageField imageField = new ImageField(field, field.Value);
      string parameter = args.Parameters["controlid"];
      string xml = args.Parameters[Templates.ImportImageClientPipeline.Parameters.FieldValue];

      var itemIdString = args.Parameters[Templates.ImportImageClientPipeline.Parameters.MediaItemId];

      if (!string.IsNullOrEmpty(itemIdString))
      {
        var itemId = new ID(itemIdString);

        var mediaItem = _imageService.GetImageById(itemId);

        if (mediaItem != null)
        {
          imageField.SetAttribute("mediaid", mediaItem.ID.ToString());
          if (xml.Length > 0)
          {
            XmlValue xmlValue = new XmlValue(xml, "image");
            string attribute1 = xmlValue.GetAttribute("height");
            if (!string.IsNullOrEmpty(attribute1))
              imageField.Height = attribute1;
            string attribute2 = xmlValue.GetAttribute("width");
            if (!string.IsNullOrEmpty(attribute2))
              imageField.Width = attribute2;
          }

          SheerResponse.SetAttribute("scHtmlValue", "value", WebEditImageCommand.RenderImage(args, imageField.Value));
          SheerResponse.SetAttribute("scPlainValue", "value", imageField.Value);
          SheerResponse.Eval("scSetHtmlValue('" + parameter + "')");
        }
        else
        {
          SheerResponse.Alert(_statusMessageService.GetJobStatusValueCouldNotSet());
          FotoWareFieldsLog.WriteToLog("--- SelectImageImageFieldCommand: Could not set field-value ---");
        }
      }
      else
      {
        SheerResponse.Alert(_statusMessageService.GetJobStatusValueCouldNotSet());
        FotoWareFieldsLog.WriteToLog("--- SelectImageImageFieldCommand: Could not set field-value ---");
      }
    }
  }
}
