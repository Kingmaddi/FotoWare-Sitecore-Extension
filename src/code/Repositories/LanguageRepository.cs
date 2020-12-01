using System;
using Kingmaddi.Foundation.FotoWareExtension.Logging;
using Sitecore.Configuration;
using Sitecore.Globalization;

namespace Kingmaddi.Foundation.FotoWareExtension.Repositories
{
  public class LanguageRepository : ILanguageRepository
  {
    /// <inheritdoc />
    public Language GetContentLanguage()
    {
      Language contentLanguage = null;
      var code = Settings.GetSetting("Foundation.FotoWareExtensions.Language", "de-DE");
      if (string.IsNullOrEmpty(code))
      {
        contentLanguage = Sitecore.Context.Language;
      }
      else
      {
        try
        {
          contentLanguage = Language.Parse(code);
        }
        catch (Exception e)
        {
          FotoWareFieldsLog.WriteToLog("--- Language Repository: Language-Code could no be parsed: ---", e);
          contentLanguage = Sitecore.Context.Language;
        }
      }

      return contentLanguage;
    }
  }
}
