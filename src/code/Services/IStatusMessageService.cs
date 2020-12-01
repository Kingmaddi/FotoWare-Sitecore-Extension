namespace Kingmaddi.Foundation.FotoWareExtension.Services
{
  public interface IStatusMessageService
  {
    /// <summary>
    /// Gets the image could not uploaded error message.
    /// </summary>
    /// <returns>Image could not uploaded error message.</returns>
    string GetImageCouldNotUploadedMessage();

    /// <summary>
    /// Gets the image must be exported error message.
    /// </summary>
    /// <returns>Image must be exported error message.</returns>
    string GetImageMustBeExportedMessage();

    /// <summary>
    /// Image was uploaded successfully.
    /// </summary>
    /// <returns></returns>
    string GetImageUploadSuccessfullyMessage();

    /// <summary>
    /// Job is importing image from FotoWare.
    /// </summary>
    /// <returns></returns>
    string GetJobStatusImporting();

    /// <summary>
    /// Job for importing image failed. Value could not set to field.
    /// </summary>
    /// <returns></returns>
    string GetJobStatusValueCouldNotSet();
  }
}
