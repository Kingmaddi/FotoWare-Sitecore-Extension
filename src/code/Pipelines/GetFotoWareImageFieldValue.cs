using Sitecore.Pipelines.RenderField;

namespace Kingmaddi.Foundation.FotoWareExtension.Pipelines
{
  public class GetFotoWareImageFieldValue : GetImageFieldValue
  {
    /// <inheritdoc />
    protected override bool IsImage(RenderFieldArgs args)
    {
      return args.FieldTypeKey == "fotoware image";
    }
  }
}
