#if ANDROID
using Maui.ComboBox.Platforms.Android;
using Maui.ComboBox.Interfaces;
#endif

namespace Maui.ComboBox.Handlers
{
    public static class AppBuilderExtension
    {
        public static MauiAppBuilder UseAxDLFComboBox(this MauiAppBuilder builder)
        {
            builder.ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                 handlers.AddHandler<AndroidComboBox, NativeSpinnerHandler>();

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

            return builder;
        }
    }
}
