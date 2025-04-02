using System.Collections;
using System.Diagnostics;
using System.Globalization;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using Application = Microsoft.Maui.Controls.Application;
using LayoutAlignment = Microsoft.Maui.Primitives.LayoutAlignment;
using ListView = Microsoft.Maui.Controls.ListView;

namespace Maui.DropDown;

public enum DropDownStyleEnum { Popup, Dropdown }

public class DropDownListBox() : DropDownBoxBase(DropDownStyleEnum.Dropdown);

public class PopUpListBox() : DropDownBoxBase(DropDownStyleEnum.Popup);

public abstract class DropDownBoxBase : ContentView, IDisposable {
    private Popup? _popup;
    private bool _disposed;
    public DropDownStyleEnum DropDownStyle;

    private readonly Border _popupContainer = new Border();
    private Image _arrowImage = new Image();

    protected DropDownBoxBase(DropDownStyleEnum dropdownStyle) {
        DropDownStyle = dropdownStyle;
        DrawDropDown();
    }

    #region Bindable Properties
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(DropDownBoxBase));
    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(DropDownBoxBase), null, BindingMode.TwoWay);
    public static readonly BindableProperty ItemDisplayPathProperty = BindableProperty.Create(nameof(ItemDisplayPath), typeof(string), typeof(DropDownBoxBase));
    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(DropDownBoxBase), string.Empty);
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(DropDownBoxBase), Colors.Black);
    public static readonly BindableProperty TextSizeProperty = BindableProperty.Create(nameof(TextSize), typeof(double), typeof(DropDownBoxBase), 12.0);
    public static readonly BindableProperty DropDownWidthProperty = BindableProperty.Create(nameof(DropDownWidth), typeof(double), typeof(DropDownBoxBase), -1.0);
    public static readonly BindableProperty DropDownHeightProperty = BindableProperty.Create(nameof(DropDownHeight), typeof(double), typeof(DropDownBoxBase), 200.0);
    public static readonly BindableProperty DropdownCornerRadiusProperty = BindableProperty.Create(nameof(DropdownCornerRadius), typeof(CornerRadius), typeof(DropDownBoxBase), new CornerRadius(10), propertyChanged: CornerRadiusChanged);
    public static readonly BindableProperty DropdownTextColorProperty = BindableProperty.Create(nameof(DropdownTextColor), typeof(Color), typeof(DropDownBoxBase), Colors.Black);
    public static readonly BindableProperty DropdownBackgroundColorProperty = BindableProperty.Create(nameof(DropdownBackgroundColor), typeof(Color), typeof(DropDownBoxBase), Colors.White);
    public static readonly BindableProperty DropdownBorderColorProperty = BindableProperty.Create(nameof(DropdownBorderColor), typeof(Color), typeof(DropDownBoxBase), Colors.DarkGrey);
    public static readonly BindableProperty DropdownBorderWidthProperty = BindableProperty.Create(nameof(DropdownBorderWidth), typeof(double), typeof(DropDownBoxBase), 1.0);
    public static readonly BindableProperty DropdownClosedImageSourceProperty = BindableProperty.Create(nameof(DropdownClosedImageSource), typeof(string), typeof(DropDownBoxBase), "chevron_right.png");
    public static readonly BindableProperty DropdownOpenImageSourceProperty = BindableProperty.Create(nameof(DropdownOpenImageSource), typeof(string), typeof(DropDownBoxBase), "chevron_down.png");
    public static readonly BindableProperty DropdownImageTintProperty = BindableProperty.Create(nameof(DropdownImageTint), typeof(Color), typeof(DropDownBoxBase));
    public static readonly BindableProperty DropdownShadowProperty = BindableProperty.Create(nameof(DropdownShadow), typeof(bool), typeof(DropDownBoxBase), true);
    public static readonly BindableProperty DropdownSeparatorProperty = BindableProperty.Create(nameof(DropdownSeparator), typeof(bool), typeof(DropDownBoxBase), true);

    /// <summary>
    /// The source of the dropdown list. This is either a collection of strings
    /// in which case Path should be null, or an object and Path should point to
    /// the property in the object that should be displayed. 
    /// </summary>
    public IEnumerable ItemsSource {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// If the ItemSource is an object then this should be the path, or
    /// name of the property within that object to display. 
    /// </summary>
    public string? ItemDisplayPath {
        get => (string)GetValue(ItemDisplayPathProperty);
        set => SetValue(ItemDisplayPathProperty, value);
    }

    /// <summary>
    /// The currently selected item. If an item is clicked, this will become
    /// the selected item. 
    /// </summary>
    public object SelectedItem {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    /// <summary>
    /// If the current object is blank, this is the text that should be
    /// displayed as a placeholder. 
    /// </summary>
    public string Placeholder {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    /// <summary>
    /// Part of the DropDown Configuration: How rounded should the dropdown be?
    /// The default is 10. Set to 0 for square corners. 
    /// </summary>
    public CornerRadius DropdownCornerRadius {
        get => (CornerRadius)GetValue(DropdownCornerRadiusProperty);
        set => SetValue(DropdownCornerRadiusProperty, value);
    }

    /// <summary>
    /// Specifies the width of the dropdown menu. If set to a value greater than 0,
    /// the dropdown will use the specified width. Otherwise, it defaults to the
    /// width of the main button layout.
    /// </summary>
    public double DropDownWidth {
        get => (double)GetValue(DropDownWidthProperty);
        set => SetValue(DropDownWidthProperty, value);
    }

    /// <summary>
    /// Defines the height of the dropdown component. This value determines
    /// the vertical size of the dropdown when it is displayed.
    /// </summary>
    public double DropDownHeight {
        get => (double)GetValue(DropDownHeightProperty);
        set => SetValue(DropDownHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the text within the DropDownBox.
    /// </summary>
    public Color TextColor {
        get => (Color)GetValue(TextColorProperty) ?? Colors.Black;
        set => SetValue(TextColorProperty, value);
    }

    /// <summary>
    /// Determines the font size of the text displayed in the drop-down box.
    /// </summary>
    public double TextSize {
        get => (double)GetValue(TextSizeProperty);
        set => SetValue(TextSizeProperty, value);
    }

    /// <summary>
    /// Specifies the text color for items displayed in the dropdown list.
    /// This property determines the `TextColor` of labels rendered within the dropdown.
    /// </summary>
    public Color DropdownTextColor {
        get => (Color)GetValue(DropdownTextColorProperty) ?? Colors.Black;
        set => SetValue(DropdownTextColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the border color of the dropdown. This property defines the color of the border
    /// surrounding the dropdown menu when it is displayed.
    /// </summary>
    public Color DropdownBorderColor {
        get => (Color)GetValue(DropdownBorderColorProperty) ?? Colors.DarkGrey;
        set => SetValue(DropdownBorderColorProperty, value);
    }

    /// <summary>
    /// Represents a brush that is derived from the <c>DropdownBorderColor</c> property,
    /// used to define the border color as a solid brush for the dropdown box.
    /// </summary>
    public SolidColorBrush DropdownBorderColorBrush => new SolidColorBrush(DropdownBorderColor);

    /// <summary>
    /// Gets or sets the width of the border surrounding the dropdown.
    /// </summary>
    public double DropdownBorderWidth {
        get => (double)GetValue(DropdownBorderWidthProperty);
        set => SetValue(DropdownBorderWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the background color of the dropdown list.
    /// This property determines the visual background appearance
    /// of the dropdown items when displayed.
    /// </summary>
    public Color DropdownBackgroundColor {
        get => (Color)GetValue(DropdownBackgroundColorProperty) ?? Colors.Gainsboro;
        set => SetValue(DropdownBackgroundColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the image source used to represent the closed state of the dropdown.
    /// </summary>
    public string DropdownClosedImageSource {
        get => (string)GetValue(DropdownClosedImageSourceProperty);
        set => SetValue(DropdownClosedImageSourceProperty, value);
    }

    /// <summary>
    /// Represents the image source to be used when the dropdown is in its open state.
    /// This property allows customization of the visual appearance by specifying
    /// the file path or resource name of the image displayed.
    /// </summary>
    public string DropdownOpenImageSource {
        get => (string)GetValue(DropdownOpenImageSourceProperty);
        set => SetValue(DropdownOpenImageSourceProperty, value);
    }

    /// <summary>
    /// Specifies the tint color applied to the dropdown indicator image.
    /// This property allows customization of the arrow image's color, enhancing visual consistency
    /// with the application's theme or design requirements.
    /// </summary>
    public Color? DropdownImageTint {
        get => (Color?)GetValue(DropdownImageTintProperty);
        set => SetValue(DropdownImageTintProperty, value);
    }

    /// <summary>
    /// Determines whether the dropdown box displays a shadow effect.
    /// </summary>
    public bool DropdownShadow {
        get => (bool)GetValue(DropdownShadowProperty);
        set => SetValue(DropdownShadowProperty, value);
    }

    /// <summary>
    /// Indicates whether a separator is displayed within the dropdown.
    /// </summary>
    public bool DropdownSeparator {
        get => (bool)GetValue(DropdownSeparatorProperty);
        set => SetValue(DropdownSeparatorProperty, value);
    }

    public SeparatorVisibility DropdownSeparatorVisibility => DropdownSeparator ? SeparatorVisibility.Default : SeparatorVisibility.None;
    #endregion

    /// <summary>
    /// Renders the dropdown menu, handling its visual update and ensuring
    /// that it is properly displayed within the parent container.
    /// </summary>
    private void DrawDropDown() {
        // The label that will be displayed containing the selected item
        // ----------------------------------------------------------------------------
        var selectedItemLabel = new Label {
            VerticalOptions = LayoutOptions.Center,
            VerticalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.TailTruncation,
        };
        selectedItemLabel.SetBinding(Label.TextColorProperty, new Binding(nameof(TextColor), BindingMode.OneWay, source: this));
        selectedItemLabel.SetBinding(Label.FontSizeProperty, new Binding(nameof(TextSize), BindingMode.OneWay, source: this));

        // The up/down image. Use properties to change what .png is used. (must be PNG)  
        // ----------------------------------------------------------------------------
        _arrowImage = new Image {
            Source = DropdownClosedImageSource,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(1, 1, 1, 1),
        };
        var togglePopupGesture = new TapGestureRecognizer();
        togglePopupGesture.Tapped += (_, _) => TogglePopup();
        selectedItemLabel.GestureRecognizers.Add(togglePopupGesture);
        _arrowImage.GestureRecognizers.Add(togglePopupGesture);

        // Main container for the label and icon
        // ----------------------------------------------------------------------------
        var mainButtonLayout = new Grid {
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
            HeightRequest = 40
        };
        mainButtonLayout.SizeChanged += (_, _) => {
            var dropdownWidth = DropDownWidth > 0 ? DropDownWidth - 2 : mainButtonLayout.Width - 2;
            AbsoluteLayout.SetLayoutBounds(_popupContainer, new Rect(0, mainButtonLayout.Height, dropdownWidth, DropDownHeight));
        };

        mainButtonLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        mainButtonLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        mainButtonLayout.Children.Add(selectedItemLabel);
        mainButtonLayout.SetColumn(selectedItemLabel, 0);
        mainButtonLayout.Children.Add(_arrowImage);
        mainButtonLayout.SetColumn(_arrowImage, 1);

        // This list of items as a list view
        // ----------------------------------------------------------------------------
        var itemListView = new ListView {
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(5),
            SelectionMode = ListViewSelectionMode.Single,
            ItemsSource = ItemsSource,
            ItemTemplate = new DataTemplate(() => {
                var label = new Label {
                    Margin = new Thickness(5, 0, 5, 0),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Fill,
                };
                label.SetBinding(Label.TextColorProperty, new Binding(nameof(DropdownTextColor), BindingMode.OneWay, source: this));
                label.SetBinding(Label.FontSizeProperty, new Binding(nameof(TextSize), BindingMode.OneWay, source: this));
                label.SetBinding(Label.BackgroundColorProperty, new Binding(nameof(DropdownBackgroundColor), BindingMode.OneWay, source: this));
                label.SetBinding(Label.TextProperty, new Binding("."));
                return label;
            }),
        };
        itemListView.SetBinding(ListView.ItemsSourceProperty, new Binding(nameof(ItemsSource), BindingMode.TwoWay, source: this));
        itemListView.SetBinding(ListView.BackgroundColorProperty, new Binding(nameof(DropdownBackgroundColor), BindingMode.OneWay, source: this));
        itemListView.SetBinding(ListView.SeparatorColorProperty, new Binding(nameof(DropdownTextColor), BindingMode.OneWay, source: this));
        itemListView.SetBinding(ListView.SeparatorVisibilityProperty, new Binding(nameof(DropdownSeparatorVisibility), BindingMode.OneWay, source: this));
        itemListView.On<iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);
        itemListView.ItemTapped += (_, e) => {
            if (e?.Item is { } item) {
                SelectedItem = item;
                TogglePopup();
                SetDropDownImage(false);
            }
        };

        // Popup container with a shadow and border
        // ----------------------------------------------------------------------------
        _popupContainer.Content = itemListView;
        _popupContainer.IsVisible = false;
        _popupContainer.Margin = new Thickness(1);
        _popupContainer.Padding = new Thickness(1);
        _popupContainer.SetBinding(Border.BackgroundColorProperty, new Binding(nameof(DropdownBackgroundColor), BindingMode.OneWay, source: this));
        _popupContainer.SetBinding(Border.StrokeProperty, new Binding(nameof(DropdownBorderColorBrush), BindingMode.OneWay, source: this));
        _popupContainer.SetBinding(Border.StrokeThicknessProperty, new Binding(nameof(DropdownBorderWidth), BindingMode.OneWay, source: this));
        _popupContainer.Unfocused += (_, _) => _popupContainer.IsVisible = false;

        // AbsoluteLayout to allow overlay over other content
        // ----------------------------------------------------------------------------
        if (DropDownStyle == DropDownStyleEnum.Dropdown) {
            var absoluteLayout = new AbsoluteLayout();
            AbsoluteLayout.SetLayoutBounds(mainButtonLayout, new Rect(0, 0, 1, 40)); // Layout button at (0, 0)
            AbsoluteLayout.SetLayoutFlags(mainButtonLayout, AbsoluteLayoutFlags.WidthProportional);
            absoluteLayout.Children.Add(mainButtonLayout);

            AbsoluteLayout.SetLayoutBounds(_popupContainer, new Rect(0, 40, DropDownWidth > 0 ? DropDownWidth - 2 : WidthRequest - 2, DropDownHeight));
            AbsoluteLayout.SetLayoutFlags(_popupContainer, AbsoluteLayoutFlags.None);
            absoluteLayout.Children.Add(_popupContainer);
            Content = absoluteLayout;
        } else {
            Content = mainButtonLayout;
        }
        selectedItemLabel.BindingContext = this;
        selectedItemLabel.SetBinding(Label.TextProperty, new Binding(nameof(SelectedItem), BindingMode.OneWay, source: this));

        // Placeholder management
        // ----------------------------------------------------------------------------
        PropertyChanged += (_, e) => {
            switch (e.PropertyName) {
            case nameof(SelectedItem):
            case nameof(Placeholder):
                selectedItemLabel.Text = SelectedItem?.ToString() ?? Placeholder ?? string.Empty;;
                break;

            case nameof(DropdownSeparator):
                OnPropertyChanged(nameof(DropdownSeparatorVisibility));
                break;

            case nameof(DropdownImageTint):
                SetDropDownImage(_popupContainer.IsVisible);
                break;

            case nameof(DropdownShadow):
                if (DropdownShadow) {
                    _popupContainer.Shadow = new Shadow {
                        Opacity = 0.25f,
                        Offset = new Point(5, 5),
                        Radius = 10
                    };
                } else _popupContainer.Shadow = null!;
                break;
            }
        };
    }
    
    /// <summary>
    /// Retrieves the value of a specified property from the given object based on
    /// the provided property path. If the value is null or the property path is invalid,
    /// a default value is returned.
    /// </summary>
    /// <param name="source">The object from which the property value should be retrieved.</param>
    /// <param name="propertyPath">The path or name of the property to retrieve the value from.</param>
    /// <param name="defaultValue">The value to return if the property path is null, invalid, or the resulting value is null.</param>
    /// <returns>The value of the specified property, or the default value if the property is null or not found.</returns>
    public static object? GetPropertyValue(object? source, string? propertyPath, string? defaultValue = null) {
        if (source == null) return defaultValue;
        if (string.IsNullOrEmpty(propertyPath)) return source;
        var property = source.GetType().GetProperty(propertyPath);
        var value = property?.GetValue(source);
        return string.IsNullOrEmpty(value?.ToString()) ? defaultValue : property?.GetValue(source);
    }

    private static void CornerRadiusChanged(BindableObject bindable, object oldValue, object newValue) {
        if (bindable is DropDownBoxBase container) container.UpdateCornerRadius();
    }

    /// <summary>
    /// Updates the corner radius of the dropdown's popup container to match the specified
    /// value in the `DropdownCornerRadius` property, ensuring the visual shape of the popup
    /// is consistent with the designated styling.
    /// This is needed as we can't bind to the Stroke in the dropdown.
    /// </summary>
    private void UpdateCornerRadius() {
        ArgumentNullException.ThrowIfNull(_popupContainer);
        _popupContainer.StrokeShape = new RoundRectangle { CornerRadius = DropdownCornerRadius };
    }

    /// <summary>
    /// Updates the dropdown arrow image to reflect its open or closed state.
    /// </summary>
    /// <param name="isOpen">A boolean value indicating whether the dropdown is open (true) or closed (false).</param>
    private void SetDropDownImage(bool isOpen) {
        try {
            _arrowImage.Source = isOpen ? DropdownOpenImageSource : DropdownClosedImageSource;
            if (DropdownImageTint != null) {
                _arrowImage.Behaviors.Add(new IconTintColorBehavior { TintColor = DropdownImageTint });
            }
        } catch {
            Debug.WriteLine($"Error setting dropdown image source: {(isOpen ? DropdownOpenImageSource : DropdownClosedImageSource)}");
        }
    }

    /// <summary>
    /// Toggles the visibility of the dropdown popup.
    /// Depending on the configured dropdown style, either shows or hides the popup
    /// container by updating its visibility or dynamically creating and displaying a popup.
    /// </summary>
    private void TogglePopup() {
        if (DropDownStyle == DropDownStyleEnum.Dropdown) {
            _popupContainer.IsVisible = !_popupContainer.IsVisible;
        } else {
            if (_popup?.Parent != null) {
                if (_popup is not null) {
                    _popup.Close();
                    _popup = null;
                }
                _popupContainer.IsVisible = false;
            } else {
                // Create and show popup
                var bounds = GetControlBounds();
                _popupContainer.WidthRequest = DropDownWidth > 0 ? DropDownWidth - 4 : bounds.Width - 4;
                _popup = new Popup {
                    Content = _popupContainer,
                    Size = new Size(_popupContainer.WidthRequest - 4, DropDownHeight - 4),
                    Color = DropdownBackgroundColor, // Make the popup background transparent
                    CanBeDismissedByTappingOutsideOfPopup = true,
                    Anchor = this,
                    VerticalOptions = LayoutAlignment.Center,
                    HorizontalOptions = LayoutAlignment.Center,
                };
                _popup.Closed += (sender, args) => {
                    _popupContainer.IsVisible = false;
                    _popup = null;
                };
                Application.Current?.Windows[0].Page?.ShowPopup(_popup);
                _popupContainer.IsVisible = true;
            }
        }
        SetDropDownImage(_popupContainer.IsVisible);
    }

    private Rect GetControlBounds() {
        var element = this;
        var x = element.X;
        var y = element.Y;

        // Get absolute position by walking up the visual tree
        var parent = element.Parent as Microsoft.Maui.Controls.VisualElement;
        while (parent != null) {
            x += parent.X;
            y += parent.Y;
            parent = parent.Parent as Microsoft.Maui.Controls.VisualElement;
        }

        return new Rect(x, y, Width, Height);
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (!_disposed) {
            if (disposing) {
                if (_popup is not null) {
                    _popup?.Close();
                    _popup = null;
                }
            }
            _disposed = true;
        }
    }
}

public static class ViewExtensions {
    public static Rect GetAbsoluteBounds(this IView view) {
        var element = view as Microsoft.Maui.Controls.VisualElement;
        var x = element?.X ?? 0;
        var y = element?.Y ?? 0;
        var parent = element?.Parent as Microsoft.Maui.Controls.VisualElement;
        while (parent != null) {
            x += parent.X;
            y += parent.Y;
            parent = parent.Parent as Microsoft.Maui.Controls.VisualElement;
        }
        return new Rect(x, y, element?.Width ?? 0, element?.Height ?? 0);
    }
}