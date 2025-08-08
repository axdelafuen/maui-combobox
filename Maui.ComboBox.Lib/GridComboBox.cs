using System.Runtime.CompilerServices;

namespace Maui.ComboBox
{
    public partial class DropdownItem
    {
        public string TranslateKey = string.Empty;

        public string MaterialIcon = string.Empty;
    }

    public class DropdownEventArgs : EventArgs
    {
        public DropdownItem? SelectedItem { get; set; } = null;
    }

    public class Dropdown : Border
    {
        public event EventHandler<DropdownEventArgs>? SelectedItem;

        public static readonly BindableProperty ItemsProperty = BindableProperty.Create(
            nameof(Items),
            typeof(List<DropdownItem>),
            typeof(Dropdown),
            new List<DropdownItem>()
        );
        public List<DropdownItem> Items
        {
            get => (List<DropdownItem>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        private readonly VerticalStackLayout _ContentLayout = new VerticalStackLayout();

        public Dropdown()
        {
            Content = _ContentLayout;
        }

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == ItemsProperty.PropertyName)
            {
                LayoutItems();
            }
        }

        private void LayoutItems()
        {
            if (_ContentLayout == null) return;

            _ContentLayout.Clear();

            if (Items == null) return;

            for (int i = 0; i < Items.Count; i++)
            {
                var dropdownItem = new DropdownItemControl();
                dropdownItem.BindingContext = Items[i];
                dropdownItem.SetBinding(DropdownItemControl.TranslateKeyProperty, nameof(DropdownItem.TranslateKey));
                dropdownItem.SetBinding(DropdownItemControl.MaterialIconProperty, nameof(DropdownItem.MaterialIcon));
                
                var gestureRecognizer = new TapGestureRecognizer();
                gestureRecognizer.Tapped += (s, e) => {
                    if (s is DropdownItemControl control && control.BindingContext is DropdownItem item)
                    {
                        SelectedItem?.Invoke(this, new DropdownEventArgs
                        {
                            SelectedItem = item,
                        });
                    }
                };
                dropdownItem.GestureRecognizers.Add(gestureRecognizer);

                _ContentLayout.Add(dropdownItem);

                if (i < (Items.Count - 1))
                {
                    _ContentLayout.Add(new BoxView());
                }
            }
        }
    }


    public class DropdownItemControl : Grid
    {
        public event EventHandler? Tapped;

        public static readonly BindableProperty TranslateKeyProperty = BindableProperty.Create(
            nameof(TranslateKey),
            typeof(string),
            typeof(DropdownItemControl),
            string.Empty
        );
        public string TranslateKey
        {
            get => (string)GetValue(TranslateKeyProperty);
            set => SetValue(TranslateKeyProperty, value);
        }

        public static readonly BindableProperty MaterialIconProperty = BindableProperty.Create(
            nameof(MaterialIcon),
            typeof(string),
            typeof(DropdownItemControl),
            string.Empty
        );
        public string MaterialIcon
        {
            get => (string)GetValue(MaterialIconProperty);
            set => SetValue(MaterialIconProperty, value);
        }

        private readonly Label _Label = new Label
        {
            Padding = new Thickness(0, 8, 8, 8),
            HorizontalTextAlignment = TextAlignment.Start,
            VerticalTextAlignment = TextAlignment.Center
        };

        private readonly Label _Icon = new Label
        {
            Padding = new Thickness(8, 0, 0, 0),
            FontSize = 24,
            FontFamily = nameof(MaterialIcon),
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };

        public DropdownItemControl()
        {
            ColumnDefinitions = new ColumnDefinitionCollection(
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star }
            );

            ColumnSpacing = 8;

            Children.Add(_Icon);
            Children.Add(_Label);
            this.SetColumn(_Icon, 0);
            this.SetColumn(_Label, 1);

            var gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (s, e) => Tapped?.Invoke(this, EventArgs.Empty);
            GestureRecognizers.Add(gestureRecognizer);
        }

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == TranslateKeyProperty.PropertyName)
            {
                _Label.Text = TranslateKey;
            }
        }
    }
}
