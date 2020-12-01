using Newtonsoft.Json;

namespace Kingmaddi.Foundation.FotoWareExtension.Models
{
  public partial class AuthResponseModel
  {
    [JsonProperty(Templates.AuthResponseModel.Fields.AccessToken)]
    public string AccessToken { get; set; }

    [JsonProperty(Templates.AuthResponseModel.Fields.RefreshToken)]
    public string RefreshToken { get; set; }

    [JsonProperty(Templates.AuthResponseModel.Fields.TokenType)]
    public string TokenType { get; set; }

    [JsonProperty(Templates.AuthResponseModel.Fields.ExpiresIn)]
    public long ExpiresIn { get; set; }
  }
}
