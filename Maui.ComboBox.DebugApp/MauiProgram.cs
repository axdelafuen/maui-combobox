using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

#if ANDROID
using Maui.ComboBox.Platforms.Android;
using Maui.ComboBox.Interfaces;
#endif

namespace Maui.ComboBox.DebugApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureMauiHandlers(handlers =>
                {
#if ANDROID
                    handlers.AddHandler<NativeSpinner, NativeSpinnerHandler>();
                
                    Microsoft.Maui.Handlers.ViewHandler.ViewMapper.AppendToMapping("NativeSpinner", (handler, view) =>
                    {
                        if (handler is NativeSpinnerHandler spinnerHandler && view is INativeSpinner spinner)
                        {
                            NativeSpinnerHandler.MapItemsSource(spinnerHandler, spinner);
                            NativeSpinnerHandler.MapSelectedIndex(spinnerHandler, spinner);
                            NativeSpinnerHandler.MapTitle(spinnerHandler, spinner);
                            NativeSpinnerHandler.MapTextColor(spinnerHandler, spinner);
                            NativeSpinnerHandler.MapFontSize(spinnerHandler, spinner);
                            NativeSpinnerHandler.MapIsEnabled(spinnerHandler, spinner);
                        }
                    });
#endif
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
