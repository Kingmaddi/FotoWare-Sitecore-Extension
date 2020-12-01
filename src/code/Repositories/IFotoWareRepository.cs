using Kingmaddi.Foundation.FotoWareExtension.Models;

namespace Kingmaddi.Foundation.FotoWareExtension.Repositories
{
  public interface IFotoWareRepository
  {
    /// <summary>
    /// Gets the FotoWare-OAuth2-AccessToken.
    /// </summary>
    /// <param name="tenantUrl">Tenant-Url</param>
    /// <param name="clientId">Client-ID</param>
    /// <param name="clientSecret">Client-Secret</param>
    /// <returns></returns>
    string GetAccessToken(string tenantUrl, string clientId, string clientSecret);

    /// <summary>
    /// Gets the FotoWare-Image metadata.
    /// </summary>
    /// <param name="imageInfoUrl">Image info url</param>
    /// <param name="accessToken">Access-Token</param>
    /// <returns></returns>
    MetaDataResponse GetMetaData(string imageInfoUrl, string accessToken);

    /// <summary>
    /// Checks if an exported image allready exists on FotoWare.
    /// </summary>
    /// <param name="imageUrl"></param>
    /// <returns></returns>
    bool IsImageExists(string imageUrl);
  }
}
