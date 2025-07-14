using System.Diagnostics;
using Microsoft.Maui.Controls.Xaml.Diagnostics;

namespace Maui.Testing.ComboBox;

public partial class App : Application {
    public App() {
        InitializeComponent();
        BindingDiagnostics.BindingFailed += BindingDiagnosticsOnBindingFailed;
    }

    private void BindingDiagnosticsOnBindingFailed(object? sender, BindingBaseErrorEventArgs e) {
        Debug.WriteLine("Binding Failed: " + (e?.XamlSourceInfo?.SourceUri.ToString() ?? "?SourceURI") + " | " + (e?.XamlSourceInfo?.LineNumber.ToString() ?? "?LineNum") + " | " + (e?.Binding?.ToString() ?? "?Binding") + " | " + (e?.Message ?? "?Message") + " | " + (e?.Binding?.GetType().Name ?? "?BindingType"));
    }

    protected override Window CreateWindow(IActivationState? activationState) {
        return new Window(new AppShell());
    }
}