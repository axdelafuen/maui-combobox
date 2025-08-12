using Maui.ComboBox.Interfaces;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Maui.ComboBox
{
    public class NativeSpinner : View, INativeSpinner
    {
        private object _previousSelection;

        #region Bindable Properties

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(NativeSpinner), default(IList), propertyChanged: OnItemsSourceChanged);
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(NativeSpinner), default(object), defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);
        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int),typeof(NativeSpinner), -1, defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSelectedIndexChanged);
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(NativeSpinner), string.Empty);
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(NativeSpinner), Colors.Black);
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(NativeSpinner), 14.0);
        public static new readonly BindableProperty IsEnabledProperty = BindableProperty.Create(nameof(IsEnabled), typeof(bool), typeof(NativeSpinner), true);

        #endregion

        #region Properties

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public new bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        #endregion

        public event EventHandler<SpinnerSelectionChangedEventArgs> SelectionChanged;

        public NativeSpinner()
        {
            ItemsSource = new ObservableCollection<object>();
        }

        #region Property Changed Handlers

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var spinner = (NativeSpinner)bindable;

            if (oldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= spinner.OnCollectionChanged;
            }

            if (newValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += spinner.OnCollectionChanged;
            }

            spinner.SelectedIndex = -1;
            spinner.SelectedItem = null;
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var spinner = (NativeSpinner)bindable;
            spinner._previousSelection = oldValue;

            if (spinner.ItemsSource != null && newValue != null)
            {
                var index = -1;
                for (int i = 0; i < spinner.ItemsSource.Count; i++)
                {
                    if (Equals(spinner.ItemsSource[i], newValue))
                    {
                        index = i;
                        break;
                    }
                }

                if (spinner.SelectedIndex != index)
                {
                    spinner.SelectedIndex = index;
                }
            }
            else if (newValue == null)
            {
                spinner.SelectedIndex = -1;
            }

            spinner.OnSelectionChanged();
        }

        private static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var spinner = (NativeSpinner)bindable;
            var index = (int)newValue;

            if (spinner.ItemsSource != null && index >= 0 && index < spinner.ItemsSource.Count)
            {
                var item = spinner.ItemsSource[index];
                if (!Equals(spinner.SelectedItem, item))
                {
                    spinner.SelectedItem = item;
                }
            }
            else if (index == -1)
            {
                if (spinner.SelectedItem != null)
                {
                    spinner.SelectedItem = null;
                }
            }
        }

        #endregion

        #region Event Handlers

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Validate current selection when collection changes
            if (SelectedIndex >= ItemsSource?.Count)
            {
                SelectedIndex = -1;
                SelectedItem = null;
            }
        }

        protected virtual void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new SpinnerSelectionChangedEventArgs(
                SelectedItem,
                SelectedIndex,
                _previousSelection));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears the current selection
        /// </summary>
        public void ClearSelection()
        {
            SelectedIndex = -1;
            SelectedItem = null;
        }

        /// <summary>
        /// Selects an item by its value
        /// </summary>
        public void SelectItem(object item)
        {
            SelectedItem = item;
        }

        /// <summary>
        /// Selects an item by its index
        /// </summary>
        public void SelectItemAt(int index)
        {
            SelectedIndex = index;
        }

        #endregion
    }
}