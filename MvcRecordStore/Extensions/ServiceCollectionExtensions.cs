using MvcRecordStore.Services;

namespace MvcRecordStore.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IRecordService, RecordService>();
        services.AddScoped<IArtistService, ArtistService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<ILabelService, LabelService>();
        services.AddScoped<ICartService, CartService>();

        return services;
    }
}