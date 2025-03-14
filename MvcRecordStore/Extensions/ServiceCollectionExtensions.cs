using MvcRecordStore.Services;
using Parbad.AspNetCore;
using Parbad.Builder;

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
        services.AddScoped<IOrderService, OrderService>();

        return services;
    }
}