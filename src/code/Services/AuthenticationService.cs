using Kingmaddi.Foundation.FotoWareExtension.Logging;
using Kingmaddi.Foundation.FotoWareExtension.Repositories;
using Sitecore.Configuration;

namespace Kingmaddi.Foundation.FotoWareExtension.Services
{
  public class AuthenticationService : IAuthenticationService
  {
    private readonly IFotoWareRepository _fotoWareRepository;

    public AuthenticationService(IFotoWareRepository fotoWareRepository)
    {
      _fotoWareRepository = fotoWareRepository;
    }

    /// <inheritdoc />
    public string GetAccessToken()
    {
      var tenantUrl = Settings.GetSetting("Foundation.FotoWareExtensions.FotoWare.TenantUrl", string.Empty);
      var clientId = Settings.GetSetting("Foundation.FotoWareExtensions.FotoWare.ClientId", string.Empty);
      var clientSecret = Settings.GetSetting("Foundation.FotoWareExtensions.FotoWare.ClientSecret", string.Empty);

      if (string.IsNullOrEmpty(tenantUrl)
          || string.IsNullOrEmpty(clientId)
          || string.IsNullOrEmpty(clientSecret))
      {
        FotoWareFieldsLog.WriteToLog("--- Authentication Service: FotoWare-Setings are incomplete or not exists ---");
        return string.Empty;
      }

      return _fotoWareRepository.GetAccessToken(tenantUrl, clientId, clientSecret);
    }
  }
}
