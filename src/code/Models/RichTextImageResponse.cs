using Newtonsoft.Json;

namespace Kingmaddi.Foundation.FotoWareExtension.Models
{
  public class RichTextImageResponse
  {
    [JsonProperty(Templates.RichTextImageResponse.Fields.Url)]
    public string Url { get; set; }

    [JsonProperty(Templates.RichTextImageResponse.Fields.Alt)]
    public string Alt { get; set; }
  }
}
