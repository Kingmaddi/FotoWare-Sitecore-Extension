namespace Kingmaddi.Foundation.FotoWareExtension.Services
{
  public interface ISynchronizationService
  {
    /// <summary>
    ///  Synchronize Sitecore images with FotoWare.
    /// </summary>
    /// <param name="accessToken">FotoWare Accesstoken</param>
    void SynchronizeData(string accessToken);
  }
}
