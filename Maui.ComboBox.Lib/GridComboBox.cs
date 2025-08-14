using Maui.ComboBox.Helpers;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Maui.ComboBox
{
    public class GridComboBox : ContentView
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(ICollection), typeof(Dropdown));
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(PopupComboBox), null, BindingMode.TwoWay);

        public ICollection ItemsSource
        {
            get => (ICollection)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private readonly Dropdown _dropdown;
        private readonly Label _header;
        private readonly Grid _container;

        public GridComboBox()
        {
            _dropdown = new()
            {
                IsVisible = false,
                ZIndex = 10,
            };
            _dropdown.SetBinding(Dropdown.ItemsSourceProperty, new Binding(nameof(ItemsSource), source: this));
            _dropdown.SetBinding(Dropdown.SelectedItemProperty, new Binding(nameof(SelectedItem), source: this));
            
            _header = new Label { 
                Text = "Select item...",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            var gestuerRecognize = new TapGestureRecognizer();
            gestuerRecognize.Tapped +=  (_, _) => 
            {
                _dropdown.IsVisible = !_dropdown.IsVisible;
            };
            _header.GestureRecognizers.Add(gestuerRecognize);

            _container = new Grid()
            {
                RowDefinitions = new RowDefinitionCollection(
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                ),
                Padding = new Thickness(8),
                BackgroundColor = Colors.LightGray
            };
            _container.Children.Add(_header);
            _container.Children.Add(_dropdown);

            _container.SetRow(_header, 0);
            _container.SetRow(_dropdown, 1);

            Content = _container;
        }
    }

    public class Dropdown : Border
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(ICollection), typeof(Dropdown));
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(Dropdown), null, BindingMode.TwoWay);
        
        public ICollection ItemsSource
        {
            get => (ICollection)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private readonly VerticalStackLayout _ContentLayout = new VerticalStackLayout();

        public Dropdown()
        {
            Content = _ContentLayout;
        }

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == ItemsSourceProperty.PropertyName)
            {
                LayoutItems();
            }
        }

        private void LayoutItems()
        {
            if (_ContentLayout == null) return;

            _ContentLayout.Clear();

            if (ItemsSource == null) return;

            for (int i = 0; i < ItemsSource.Count; i++)
            {
                var dropdownItem = new DropdownItemControl();
                dropdownItem.BindingContext = CollectionHelper.GetItemAt(ItemsSource, i);
                dropdownItem.SetBinding(DropdownItemControl.TextProperty, ".");
                
                var gestureRecognizer = new TapGestureRecognizer();
                gestureRecognizer.Tapped += (s, e) => {
                    if (s is DropdownItemControl control && control.BindingContext is object item)
                    {
                        SelectedItem = item;
                    }
                };
                dropdownItem.GestureRecognizers.Add(gestureRecognizer);

                _ContentLayout.Add(dropdownItem);

                if (i < (ItemsSource.Count - 1))
                {
                    _ContentLayout.Add(new BoxView());
                }
            }
        }
    }

    public class DropdownItemControl : Grid
    {
        public event EventHandler? Tapped;

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(DropdownItemControl), string.Empty);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private readonly Label _Label = new Label
        {
            Padding = new Thickness(0, 8, 8, 8),
            HorizontalTextAlignment = TextAlignment.Start,
            VerticalTextAlignment = TextAlignment.Center
        };

        public DropdownItemControl()
        {
            ColumnDefinitions = new ColumnDefinitionCollection(
                new ColumnDefinition { Width = GridLength.Auto }
            );

            ColumnSpacing = 8;

            Children.Add(_Label);
            this.SetColumn(_Label, 0);

            var gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (s, e) => Tapped?.Invoke(this, EventArgs.Empty);
            GestureRecognizers.Add(gestureRecognizer);
        }

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == TextProperty.PropertyName)
            {
                _Label.Text = Text;
            }
        }
    }
}
