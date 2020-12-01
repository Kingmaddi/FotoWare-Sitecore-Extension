namespace Kingmaddi.Foundation.FotoWareExtension.Services
{
  public class StatusMessageService : IStatusMessageService
  {
    /// <inheritdoc />
    public string GetImageCouldNotUploadedMessage()
    {
      return "Es ist ein Fehler beim Upload des Bildes in die Medien-Bibliothek aufgetreten. Weitere Informationen finden Sie in den Logs.";
    }

    /// <inheritdoc />
    public string GetImageMustBeExportedMessage()
    {
      return
        "Das Bild muss vorher bei FotoWare veröffentlicht / exportiert werden, um in die Medie-Bibliothek importiert werden zu können.";
    }

    /// <inheritdoc />
    public string GetImageUploadSuccessfullyMessage()
    {
      return "Bild erfolgreich importiert."; 
    }

    /// <inheritdoc />
    public string GetJobStatusImporting()
    {
      return "Das ausgewählte Bild wird in die Media-Library importiert...";
    }

    /// <inheritdoc />
    public string GetJobStatusValueCouldNotSet()
    {
      return "Das Bild konnte nicht gespeichert werden.";
    }
  }
}
