using Kingmaddi.Foundation.FotoWareExtension.Logging;
using Kingmaddi.Foundation.FotoWareExtension.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Kingmaddi.Foundation.FotoWareExtension.Cronjobs
{
  public class SynchronizationStarter
  {
    private readonly IAuthenticationService _authenticationService;
    private readonly ISynchronizationService _synchronizationService;

    public SynchronizationStarter()
    {
      _authenticationService = ServiceLocator.ServiceProvider.GetService<IAuthenticationService>();
      _synchronizationService = ServiceLocator.ServiceProvider.GetService<ISynchronizationService>();
    }

    /// <summary>
    /// Starts the synchronization routine
    /// </summary>
    public void Execute()
    {
      var accessToken = _authenticationService.GetAccessToken();
      if (!string.IsNullOrEmpty(accessToken))
      {
        _synchronizationService.SynchronizeData(accessToken);
      }
      else
      {
        FotoWareFieldsLog.WriteToLog("--- SynchronizationStarter: could not get access token. ---");
      }
    }
  }
}
