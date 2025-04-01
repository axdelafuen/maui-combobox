using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using CommunityToolkit.Maui.Behaviors;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;

namespace Maui.DropDown;

public class DropDownBox : ContentView {
    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(DropDownBox), null, BindingMode.TwoWay, propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(DropDownBox), new List<object>(), propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty ItemPathProperty = BindableProperty.Create(nameof(ItemPath), typeof(string), typeof(DropDownBox), propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(DropDownBox), string.Empty, propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty DropDownWidthProperty = BindableProperty.Create(nameof(DropDownWidth), typeof(double), typeof(DropDownBox), -1.0, propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty DropDownHeightProperty = BindableProperty.Create(nameof(DropDownHeight), typeof(double), typeof(DropDownBox), 200.0, propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(DropDownBox), Colors.Black, propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty TextSizeProperty = BindableProperty.Create(nameof(TextSize), typeof(double), typeof(DropDownBox), 12.0, propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty DropdownCornerRadiusProperty = BindableProperty.Create(nameof(DropdownCornerRadius), typeof(double), typeof(DropDownBox), 10.0, propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty DropdownTextColorProperty = BindableProperty.Create(nameof(DropdownTextColor), typeof(Color), typeof(DropDownBox), Colors.Black, propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty DropdownBackgroundColorProperty = BindableProperty.Create(nameof(DropdownBackgroundColor), typeof(Color), typeof(DropDownBox), Colors.White, propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty DropdownBorderColorProperty = BindableProperty.Create(nameof(DropdownBorderColor), typeof(Color), typeof(DropDownBox), Colors.DarkGrey, propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty DropdownBorderWidthProperty = BindableProperty.Create(nameof(DropdownBorderWidth), typeof(double), typeof(DropDownBox), 1.0, propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty DropdownClosedImageSourceProperty = BindableProperty.Create(nameof(DropdownClosedImageSource), typeof(string), typeof(DropDownBox), "chevron_right.png", propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty DropdownOpenImageSourceProperty = BindableProperty.Create(nameof(DropdownOpenImageSource), typeof(string), typeof(DropDownBox), "chevron_down.png", propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty DropdownImageTintProperty = BindableProperty.Create(nameof(DropdownImageTint), typeof(Color), typeof(DropDownBox), propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty DropdownShadowProperty = BindableProperty.Create(nameof(DropdownShadow), typeof(bool), typeof(DropDownBox), true, propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty DropdownSeparatorProperty = BindableProperty.Create(nameof(DropdownSeparator), typeof(bool), typeof(DropDownBox), true, propertyChanged: OnPropertyChanged);

    public string? ItemPath {
        get => (string)GetValue(ItemPathProperty);
        set => SetValue(ItemPathProperty, value);
    }

    public object SelectedItem {
        get => (object)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public IEnumerable ItemsSource {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public string Placeholder {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public double DropdownCornerRadius {
        get => (double)GetValue(DropdownCornerRadiusProperty);
        set => SetValue(DropdownCornerRadiusProperty, value);
    }

    public double DropDownWidth {
        get => (double)GetValue(DropDownWidthProperty);
        set => SetValue(DropDownWidthProperty, value);
    }

    public double DropDownHeight {
        get => (double)GetValue(DropDownHeightProperty);
        set => SetValue(DropDownHeightProperty, value);
    }

    public Color TextColor {
        get => (Color)GetValue(TextColorProperty) ?? Colors.Black;
        set => SetValue(TextColorProperty, value);
    }

    public double TextSize {
        get => (double)GetValue(TextSizeProperty);
        set => SetValue(TextSizeProperty, value);
    }

    public Color DropdownTextColor {
        get => (Color)GetValue(DropdownTextColorProperty) ?? Colors.Black;
        set => SetValue(DropdownTextColorProperty, value);
    }

    public Color DropdownBorderColor {
        get => (Color)GetValue(DropdownBorderColorProperty) ?? Colors.DarkGrey;
        set => SetValue(DropdownBorderColorProperty, value);
    }

    public SolidColorBrush DropdownBorderColorBrush => new SolidColorBrush(DropdownBorderColor);

    public double DropdownBorderWidth {
        get => (double)GetValue(DropdownBorderWidthProperty);
        set => SetValue(DropdownBorderWidthProperty, value);
    }

    public Color DropdownBackgroundColor {
        get => (Color)GetValue(DropdownBackgroundColorProperty) ?? Colors.Gainsboro;
        set => SetValue(DropdownBackgroundColorProperty, value);
    }

    public string DropdownClosedImageSource {
        get => (string)GetValue(DropdownClosedImageSourceProperty);
        set => SetValue(DropdownClosedImageSourceProperty, value);
    }

    public string DropdownOpenImageSource {
        get => (string)GetValue(DropdownOpenImageSourceProperty);
        set => SetValue(DropdownOpenImageSourceProperty, value);
    }

    public Color? DropdownImageTint {
        get => (Color?)GetValue(DropdownImageTintProperty);
        set => SetValue(DropdownImageTintProperty, value);
    }

    public bool DropdownShadow {
        get => (bool)GetValue(DropdownShadowProperty);
        set => SetValue(DropdownShadowProperty, value);
    }

    public bool DropdownSeparator {
        get => (bool)GetValue(DropdownSeparatorProperty);
        set => SetValue(DropdownSeparatorProperty, value);
    }

    public DropDownBox() {
        DrawDropDown();
    }

    private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue) { }

    private void DrawDropDown() {
        var popupContainer = new Border();
        var selectedItemLabel = new Label {
            VerticalOptions = LayoutOptions.Center,
            VerticalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.TailTruncation,
        };
        selectedItemLabel.SetBinding(Label.TextColorProperty, new Binding(nameof(TextColor), BindingMode.OneWay, source: this));
        selectedItemLabel.SetBinding(Label.FontSizeProperty, new Binding(nameof(TextSize), BindingMode.OneWay, source: this));

        var arrowImage = new Image {
            Source = DropdownClosedImageSource,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 20,
            HeightRequest = 20
        };

        // Tap gesture to toggle the popup
        var togglePopupGesture = new TapGestureRecognizer();
        togglePopupGesture.Tapped += (_, _) => { TogglePopup(popupContainer, arrowImage); };

        selectedItemLabel.GestureRecognizers.Add(togglePopupGesture);
        arrowImage.GestureRecognizers.Add(togglePopupGesture);

        // Main container for the label and icon
        var mainButtonLayout = new Grid {
            Padding = new Thickness(10),
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
            HeightRequest = 40
        };

        mainButtonLayout.SizeChanged += (sender, args) => {
            var dropdownWidth = DropDownWidth > 0 ? DropDownWidth : mainButtonLayout.Width;
            AbsoluteLayout.SetLayoutBounds(popupContainer, new Rect(0, mainButtonLayout.Height, dropdownWidth, DropDownHeight));
        };

        mainButtonLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        mainButtonLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = 30 });
        mainButtonLayout.Children.Add(selectedItemLabel);
        mainButtonLayout.Children.Add(arrowImage);
        mainButtonLayout.SetColumn(selectedItemLabel, 0);
        mainButtonLayout.SetColumn(arrowImage, 1);

        var itemListView = new ListView {
            ItemsSource = ItemsSource,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(5),
            SelectionMode = ListViewSelectionMode.Single,
            ItemTemplate = new DataTemplate(() => {
                var label = new Label {
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
        itemListView.SetBinding(ListView.BackgroundColorProperty, new Binding(nameof(DropdownBackgroundColor), BindingMode.OneWay, source: this));
        itemListView.SetBinding(ListView.SeparatorColorProperty, new Binding(nameof(DropdownTextColor), BindingMode.OneWay, source: this));
        itemListView.SetBinding(ListView.SeparatorVisibilityProperty, new Binding(nameof(DropdownSeparator), BindingMode.OneWay, source: this));

        itemListView.ItemTapped += (_, e) => {
            if (e?.Item is { } item) {
                SelectedItem = item;
                if (popupContainer is not null) popupContainer.IsVisible = false; // Hide dropdown
                SetDropDownImage(arrowImage, false);
            }
        };

        // Popup container with a shadow and border
        popupContainer.Content = itemListView;
        popupContainer.WidthRequest = DropDownWidth < 0 ? WidthRequest : DropDownWidth;
        popupContainer.IsVisible = false;
        popupContainer.StrokeShape = new RoundRectangle {
            CornerRadius = new CornerRadius(DropdownCornerRadius) // Customize the corner radius here
        };
        popupContainer.SetBinding(Border.BackgroundColorProperty, new Binding(nameof(DropdownBackgroundColor), BindingMode.OneWay, source: this));
        popupContainer.SetBinding(Border.StrokeProperty, new Binding(nameof(DropdownBorderColorBrush), BindingMode.OneWay, source: this));
        popupContainer.SetBinding(Border.StrokeThicknessProperty, new Binding(nameof(DropdownBorderWidth), BindingMode.OneWay, source: this));
        if (DropdownShadow) {
            popupContainer.Shadow = new Shadow {
                Opacity = 0.2f,
                Offset = new Point(5, 5),
                Radius = 10
            };
        }

        // AbsoluteLayout to allow overlay over other content
        var absoluteLayout = new AbsoluteLayout();

        // Main button setup
        AbsoluteLayout.SetLayoutBounds(mainButtonLayout, new Rect(0, 0, 1, 40)); // Layout button at (0, 0)
        AbsoluteLayout.SetLayoutFlags(mainButtonLayout, AbsoluteLayoutFlags.WidthProportional);
        absoluteLayout.Children.Add(mainButtonLayout);

        // Popup placement below the button
        AbsoluteLayout.SetLayoutBounds(popupContainer, new Rect(0, 40, DropDownWidth > 0 ? DropDownWidth : mainButtonLayout.Width, DropDownHeight));
        AbsoluteLayout.SetLayoutFlags(popupContainer, AbsoluteLayoutFlags.None);
        absoluteLayout.Children.Add(popupContainer);

        Content = absoluteLayout;
        selectedItemLabel.BindingContext = this;
        selectedItemLabel.SetBinding(Label.TextProperty, new Binding(nameof(SelectedItem), BindingMode.OneWay, source: this));

        // Placeholder management
        PropertyChanged += (_, e) => {
            switch (e.PropertyName) {
            case nameof(SelectedItem):
            case nameof(Placeholder):
                selectedItemLabel.Text = GetPropertyValue(SelectedItem, ItemPath, Placeholder) as string ?? string.Empty;
                break;

            case nameof(DropdownShadow):
                if (DropdownShadow) {
                    popupContainer.Shadow = new Shadow {
                        Opacity = 0.2f,
                        Offset = new Point(5, 5),
                        Radius = 10
                    };
                } else popupContainer.Shadow = new Shadow();
                break;

            case nameof(ItemsSource):
                itemListView.ItemsSource = ItemsSource;
                break;
            }
        };
    }

    private static object? GetPropertyValue(object? source, string? propertyPath, string? defaultValue = null) {
        if (source == null) return defaultValue;
        if (string.IsNullOrEmpty(propertyPath)) return source;
        var property = source.GetType().GetProperty(propertyPath);
        var value = property?.GetValue(source);
        return string.IsNullOrEmpty(value?.ToString()) ? defaultValue : property?.GetValue(source);
    }

    private void SetDropDownImage(Image arrowImage, bool isOpen) {
        try {
            arrowImage.Source = isOpen ? DropdownOpenImageSource : DropdownClosedImageSource;
            if (DropdownImageTint != null) {
                arrowImage.Behaviors.Add(new IconTintColorBehavior { TintColor = DropdownImageTint });
            }
        } catch {
            Debug.WriteLine($"Error setting dropdown image source: {(isOpen ? DropdownOpenImageSource : DropdownClosedImageSource)}");
        }
    }

    private void TogglePopup(Border container, Image arrowImage) {
        container.IsVisible = !container.IsVisible;
        SetDropDownImage(arrowImage, container.IsVisible);
    }
}