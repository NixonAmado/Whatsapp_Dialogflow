using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebHook8.Services;
using WebHook8.Util;

public static class ApplicacionServiceExtension
{
    public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()    //WithOrigins("https://domini.com")
                .AllowAnyMethod()           //WithMethods(*GET", "POST")
                .AllowAnyHeader());         //WithHeaders(*accept*, "content-type")
            });
        public static void AddProjectServices(this IServiceCollection services, IConfiguration configuration)
        {
        services.AddSingleton<IDialogflowAPI>(provider => new DialogflowAPI(configuration));
        services.Configure<EWhatsAppSettings>(configuration.GetSection("AppSettings"));
        services.AddSingleton<IUtil, Util>();   

        //services.AddTransient<WhatsAppDialogflowHandler>();
        }
    //public static void AddWhatsappAPI(this IServiceCollection services, IConfiguration configuration)
    //{
    //    // Configurar WhatsAppSettings desde appsettings.json
    //    services.Configure<WhatsAppSettings>(configuration.GetSection("WhatsApp"));
    //    services.AddSingleton<TokenHelper>(); // Registrar TokenHelper como Singleton
    //    // Otros servicios relacionados con WhatsApp pueden ser registrados aquí
}
