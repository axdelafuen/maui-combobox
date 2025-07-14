using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Shapes;

namespace Maui.Testing.ComboBox;

public static class MauiProgram {
    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
               .UseMauiCommunityToolkit(static options =>
               {
                   options.SetPopupDefaults(new DefaultPopupSettings
                   {
                       Padding = 0,
                       Margin = 0,
                   });
                   options.SetPopupOptionsDefaults(new DefaultPopupOptionsSettings
                   {
                       PageOverlayColor = Colors.Transparent,
                       Shape = new Rectangle { StrokeThickness = 0, Stroke = Colors.Transparent },
                   });
               })
               .ConfigureFonts(fonts => {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}