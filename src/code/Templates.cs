namespace Kingmaddi.Foundation.FotoWareExtension
{
  public static class Templates
  {
    public static class ImageMediaItem
    {
      public static class Fields
      {
        public const string Alt = "Alt";
        public const string Title = "Title";
        public const string Keywords = "Keywords";
        public const string Description = "Description";
        public const string Height = "Height";
        public const string Width = "Width";
        public const string FotoWareUrl = "LocationDescription";
        public const string LastModification = "DateTime";
        public const string Software = "Software";
        public const string DeletedFromFotoWare = "ImageDescription";
      }
    }

    public static class AuthResponseModel
    {
      public static class Fields
      {
        public const string AccessToken = "access_token";
        public const string RefreshToken = "refresh_token";
        public const string TokenType = "token_type";
        public const string ExpiresIn = "expires_in";
      }
    }

    public static class RichTextImageResponse
    {
      public static class Fields
      {
        public const string Url = "url";
        public const string Alt = "alt";
      }
    }

    public static class SelectedImageModel
    {
      public static class Fields
      {
        public const string LastModification = "lastModification";
        public const string Alt = "alt";
        public const string Title = "title";
        public const string Description = "description";
        public const string Keywords = "keywords";
        public const string Height = "height";
        public const string Width = "width";
        public const string ImageUrl = "imageUrl";
        public const string MetaDataUrl = "metaDataUrl";
      }
    }

    public static class ImportImageClientPipeline
    {
      public static class Parameters
      {
        public const string MediaItemId = "mediaItemId";
        public const string SelectedImageJson = "SelectedImageJson";
        public const string FieldValue = "fieldValue";
      }
    }
  }
}
