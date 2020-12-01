using Sitecore.Globalization;

namespace Kingmaddi.Foundation.FotoWareExtension.Repositories
{
  public interface ILanguageRepository
  {
    /// <summary>
    /// Gets the Content-Language
    /// </summary>
    /// <returns>Language</returns>
    Language GetContentLanguage();
  }
}
