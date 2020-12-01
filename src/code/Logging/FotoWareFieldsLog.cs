using System;
using log4net;

namespace Kingmaddi.Foundation.FotoWareExtension.Logging
{
  public class FotoWareFieldsLog
  {
    private static readonly ILog _logger = Sitecore.Diagnostics.LoggerFactory.GetLogger("KingmaddiFotoWareExtensionLogger");
    /// <summary>
    /// Info - Debug mode enabled
    /// </summary>
    /// <param name="message"></param>
    public static void WriteToLog(string message)
    {
      if (_logger.IsDebugEnabled)
        _logger.Info(message);
    }

    /// <summary>
    /// Info - Debug mode enabled
    /// </summary>
    /// <param name="message"></param>
    public static void WriteConfigurationErrorToLog(string message)
    {
      _logger.Error(message);
    }

    /// <summary>
    /// Error - Always enabled
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public static void WriteToLog(string message, Exception exception)
    {
      _logger.Error(message, exception);
    }
  }
}
