using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Maui.Behaviors;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

namespace Maui.DropDown;

public class DropDownBox : ContentView, INotifyPropertyChanged {
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(DropDownBox));
    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(DropDownBox), null, BindingMode.TwoWay);
    public static readonly BindableProperty ItemPathProperty = BindableProperty.Create(nameof(ItemPath), typeof(string), typeof(DropDownBox));
    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(DropDownBox), string.Empty);
    public static readonly BindableProperty DropDownWidthProperty = BindableProperty.Create(nameof(DropDownWidth), typeof(double), typeof(DropDownBox), -1.0);
    public static readonly BindableProperty DropDownHeightProperty = BindableProperty.Create(nameof(DropDownHeight), typeof(double), typeof(DropDownBox), 200.0);
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(DropDownBox), Colors.Black);
    public static readonly BindableProperty TextSizeProperty = BindableProperty.Create(nameof(TextSize), typeof(double), typeof(DropDownBox), 12.0);
    public static readonly BindableProperty DropdownCornerRadiusProperty = BindableProperty.Create(nameof(DropdownCornerRadius), typeof(double), typeof(DropDownBox), 10.0);
    public static readonly BindableProperty DropdownTextColorProperty = BindableProperty.Create(nameof(DropdownTextColor), typeof(Color), typeof(DropDownBox), Colors.Black);
    public static readonly BindableProperty DropdownBackgroundColorProperty = BindableProperty.Create(nameof(DropdownBackgroundColor), typeof(Color), typeof(DropDownBox), Colors.White);
    public static readonly BindableProperty DropdownBorderColorProperty = BindableProperty.Create(nameof(DropdownBorderColor), typeof(Color), typeof(DropDownBox), Colors.DarkGrey);
    public static readonly BindableProperty DropdownBorderWidthProperty = BindableProperty.Create(nameof(DropdownBorderWidth), typeof(double), typeof(DropDownBox), 1.0);
    public static readonly BindableProperty DropdownClosedImageSourceProperty = BindableProperty.Create(nameof(DropdownClosedImageSource), typeof(string), typeof(DropDownBox), "chevron_right.png");
    public static readonly BindableProperty DropdownOpenImageSourceProperty = BindableProperty.Create(nameof(DropdownOpenImageSource), typeof(string), typeof(DropDownBox), "chevron_down.png");
    public static readonly BindableProperty DropdownImageTintProperty = BindableProperty.Create(nameof(DropdownImageTint), typeof(Color), typeof(DropDownBox));
    public static readonly BindableProperty DropdownShadowProperty = BindableProperty.Create(nameof(DropdownShadow), typeof(bool), typeof(DropDownBox), true);
    public static readonly BindableProperty DropdownSeparatorProperty = BindableProperty.Create(nameof(DropdownSeparator), typeof(bool), typeof(DropDownBox), true);

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
    public string? ItemPath {
        get => (string)GetValue(ItemPathProperty);
        set => SetValue(ItemPathProperty, value);
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
    public double DropdownCornerRadius {
        get => (double)GetValue(DropdownCornerRadiusProperty);
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

    public DropDownBox() {
        DrawDropDown();
    }
    
    private Border popupContainer = new Border();
    private Image arrowImage = new Image();

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
        
        // the up/down image. Use properties to change what .png is used.  
        // ----------------------------------------------------------------------------
        arrowImage = new Image {
            Source = DropdownClosedImageSource,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 20,
            HeightRequest = 20
        };
        var togglePopupGesture = new TapGestureRecognizer();
        togglePopupGesture.Tapped += (_, _) => TogglePopup();
        selectedItemLabel.GestureRecognizers.Add(togglePopupGesture);
        arrowImage.GestureRecognizers.Add(togglePopupGesture);

        // Main container for the label and icon
        // ----------------------------------------------------------------------------
        var mainButtonLayout = new Grid {
            Padding = new Thickness(10),
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
            HeightRequest = 40
        };
        mainButtonLayout.SizeChanged += (sender, _) => {
            var dropdownWidth = DropDownWidth > 0 ? DropDownWidth : mainButtonLayout.Width;
            AbsoluteLayout.SetLayoutBounds(popupContainer, new Rect(0, mainButtonLayout.Height, dropdownWidth, DropDownHeight));
        };
        mainButtonLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        mainButtonLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = 30 });
        mainButtonLayout.Children.Add(selectedItemLabel);
        mainButtonLayout.Children.Add(arrowImage);
        mainButtonLayout.SetColumn(selectedItemLabel, 0);
        mainButtonLayout.SetColumn(arrowImage, 1);

        // This list of items as a list view
        // ----------------------------------------------------------------------------
        var itemListView = new ListView {
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(5),
            SelectionMode = ListViewSelectionMode.Single,
            ItemTemplate = new DataTemplate(() => {
                var label = new Label {
                    Margin = new Thickness(5,0,0,0),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Fill,
                };
                label.SetBinding(Label.TextColorProperty, new Binding(nameof(DropdownTextColor), BindingMode.OneWay, source: this));
                label.SetBinding(Label.FontSizeProperty, new Binding(nameof(TextSize), BindingMode.OneWay, source: this));
                label.SetBinding(Label.BackgroundColorProperty, new Binding(nameof(DropdownBackgroundColor), BindingMode.OneWay, source: this));
                label.SetBinding(Label.TextProperty, new Binding(ItemPath ?? "."));
                return new ViewCell { View = label };
            }),
        };
        itemListView.SetBinding(ListView.ItemsSourceProperty, new Binding(nameof(ItemsSource), BindingMode.TwoWay, source: this));
        itemListView.SetBinding(ListView.BackgroundColorProperty, new Binding(nameof(DropdownBackgroundColor), BindingMode.OneWay, source: this));
        itemListView.SetBinding(ListView.SeparatorColorProperty, new Binding(nameof(DropdownTextColor), BindingMode.OneWay, source: this));
        itemListView.SetBinding(ListView.SeparatorVisibilityProperty, new Binding(nameof(DropdownSeparatorVisibility), BindingMode.TwoWay, source: this));

        itemListView.ItemTapped += (_, e) => {
            if (e?.Item is { } item) {
                SelectedItem = item;
                popupContainer.IsVisible = false; // Hide dropdown
                SetDropDownImage(false);
            }
        };

        // Popup container with a shadow and border
        // ----------------------------------------------------------------------------
        popupContainer.Content = itemListView;
        popupContainer.WidthRequest = DropDownWidth < 0 ? WidthRequest : DropDownWidth;
        popupContainer.IsVisible = false;
        popupContainer.StrokeShape = new RoundRectangle {
            CornerRadius = new CornerRadius(DropdownCornerRadius) // Customize the corner radius here
        };
        popupContainer.SetBinding(Border.BackgroundColorProperty, new Binding(nameof(DropdownBackgroundColor), BindingMode.OneWay, source: this));
        popupContainer.SetBinding(Border.StrokeProperty, new Binding(nameof(DropdownBorderColorBrush), BindingMode.OneWay, source: this));
        popupContainer.SetBinding(Border.StrokeThicknessProperty, new Binding(nameof(DropdownBorderWidth), BindingMode.OneWay, source: this));
        popupContainer.Unfocused += (_,_) => popupContainer.IsVisible = false;

        // AbsoluteLayout to allow overlay over other content
        // ----------------------------------------------------------------------------
        var absoluteLayout = new AbsoluteLayout();
        AbsoluteLayout.SetLayoutBounds(mainButtonLayout, new Rect(0, 0, 1, 40)); // Layout button at (0, 0)
        AbsoluteLayout.SetLayoutFlags(mainButtonLayout, AbsoluteLayoutFlags.WidthProportional);
        absoluteLayout.Children.Add(mainButtonLayout);
        AbsoluteLayout.SetLayoutBounds(popupContainer, new Rect(0, 40, DropDownWidth > 0 ? DropDownWidth : mainButtonLayout.Width, DropDownHeight));
        AbsoluteLayout.SetLayoutFlags(popupContainer, AbsoluteLayoutFlags.None);
        absoluteLayout.Children.Add(popupContainer);
        
        Content = absoluteLayout;
        selectedItemLabel.BindingContext = this;
        selectedItemLabel.SetBinding(Label.TextProperty, new Binding(nameof(SelectedItem), BindingMode.OneWay, source: this));

        // Placeholder management
        // ----------------------------------------------------------------------------
        PropertyChanged += (_, e) => {
            switch (e.PropertyName) {
            case nameof(SelectedItem):
            case nameof(Placeholder):
                selectedItemLabel.Text = GetPropertyValue(SelectedItem, ItemPath, Placeholder) as string ?? string.Empty;
                break;
            
            case nameof(DropdownSeparator):
                OnPropertyChanged(nameof(DropdownSeparatorVisibility));
                break;

            case nameof(DropdownShadow):
                if (DropdownShadow) {
                    popupContainer.Shadow = new Shadow {
                        Opacity = 0.2f,
                        Offset = new Point(5, 5),
                        Radius = 10
                    };
                } else popupContainer.Shadow = null!;
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
    private static object? GetPropertyValue(object? source, string? propertyPath, string? defaultValue = null) {
        if (source == null) return defaultValue;
        if (string.IsNullOrEmpty(propertyPath)) return source;
        var property = source.GetType().GetProperty(propertyPath);
        var value = property?.GetValue(source);
        return string.IsNullOrEmpty(value?.ToString()) ? defaultValue : property?.GetValue(source);
    }

    /// <summary>
    /// Updates the dropdown arrow image to reflect its open or closed state.
    /// </summary>
    /// <param name="isOpen">A boolean value indicating whether the dropdown is open (true) or closed (false).</param>
    private void SetDropDownImage(bool isOpen) {
        try {
            arrowImage.Source = isOpen ? DropdownOpenImageSource : DropdownClosedImageSource;
            if (DropdownImageTint != null) {
                arrowImage.Behaviors.Add(new IconTintColorBehavior { TintColor = DropdownImageTint });
            }
        } catch {
            Debug.WriteLine($"Error setting dropdown image source: {(isOpen ? DropdownOpenImageSource : DropdownClosedImageSource)}");
        }
    }

    /// <summary>
    /// Toggles the visibility of the popup container and updates the associated
    /// dropdown image based on its current visibility state.
    /// </summary>
    private void TogglePopup() {
        popupContainer.IsVisible = !popupContainer.IsVisible;
        SetDropDownImage(popupContainer.IsVisible);
    }
}