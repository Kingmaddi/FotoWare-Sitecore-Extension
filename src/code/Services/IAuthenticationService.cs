namespace Kingmaddi.Foundation.FotoWareExtension.Services
{
  public interface IAuthenticationService
  {
    /// <summary>
    /// Gets FotoWare-OAUth2-AccessToken.
    /// </summary>
    /// <returns>AccessToken</returns>
    string GetAccessToken();
  }
}
