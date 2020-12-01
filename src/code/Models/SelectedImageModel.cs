using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kingmaddi.Foundation.FotoWareExtension.Models
{
  public class SelectedImageModel
  {
    [JsonProperty(Templates.SelectedImageModel.Fields.LastModification)]
    public DateTimeOffset LastModification { get; set; }

    [JsonProperty(Templates.SelectedImageModel.Fields.Alt)]
    public string Alt { get; set; }

    [JsonProperty(Templates.SelectedImageModel.Fields.Title)]
    public string Title { get; set; }

    [JsonProperty(Templates.SelectedImageModel.Fields.Description)]
    public string Description { get; set; }

    [JsonProperty(Templates.SelectedImageModel.Fields.Keywords)]
    public List<string> Keywords { get; set; }

    [JsonProperty(Templates.SelectedImageModel.Fields.Height)]
    public long Height { get; set; }

    [JsonProperty(Templates.SelectedImageModel.Fields.Width)]
    public long Width { get; set; }

    [JsonProperty(Templates.SelectedImageModel.Fields.ImageUrl)]
    public Uri ImageUrl { get; set; }

    [JsonProperty(Templates.SelectedImageModel.Fields.MetaDataUrl)]
    public Uri MetaDataUrl { get; set; }
  }
}
