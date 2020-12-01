using System;
using System.Collections.Generic;

namespace Kingmaddi.Foundation.FotoWareExtension.Models
{
  public class SitecoreMetaDataModel
  {
    public DateTimeOffset LastModification { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public List<string> Keywords { get; set; }
  }
}
