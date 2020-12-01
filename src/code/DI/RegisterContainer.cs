using Kingmaddi.Foundation.FotoWareExtension.Jobs;
using Kingmaddi.Foundation.FotoWareExtension.Mappers;
using Kingmaddi.Foundation.FotoWareExtension.Repositories;
using Kingmaddi.Foundation.FotoWareExtension.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Kingmaddi.Foundation.FotoWareExtension.DI
{
  public class RegisterContainer : IServicesConfigurator
  {
    public void Configure(IServiceCollection serviceCollection)
    {
      //jobs
      serviceCollection.AddScoped<IImportFotoWareImageJob, ImportFotoWareImageJob>();

      //mapper
      serviceCollection.AddScoped<ISelectedImageModelMapper, SelectedImageModelMapper>();
      serviceCollection.AddScoped<IFotoWareMetaDataToSitecoreMetaDataMapper, FotoWareMetaDataToSitecoreMetaDataMapper>();

      //repositories
      serviceCollection.AddScoped<IFotoWareRepository, FotoWareRepository>();
      serviceCollection.AddScoped<IMediaRepository, MediaRepository>();
      serviceCollection.AddScoped<ILanguageRepository, LanguageRepository>();

      //services
      serviceCollection.AddScoped<IImageService, ImageService>();
      serviceCollection.AddScoped<IAuthenticationService, AuthenticationService>();
      serviceCollection.AddScoped<IStatusMessageService, StatusMessageService>();
      serviceCollection.AddScoped<ISynchronizationService, SynchronizationService>();
    }
  }
}
