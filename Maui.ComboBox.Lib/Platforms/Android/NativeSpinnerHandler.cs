#if ANDROID
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Handlers;
using Maui.ComboBox.Interfaces;
using System.Collections;
using Color = Android.Graphics.Color;
using View = Android.Views.View;

namespace Maui.ComboBox.Platforms.Android
{
    public class NativeSpinnerHandler : ViewHandler<INativeSpinner, AlwaysFireSpinner>
    {
        private SpinnerAdapter? _adapter;
        private bool _isUpdatingSelection;
        private bool _isInitialized = false;

        new private static IPropertyMapper<INativeSpinner, NativeSpinnerHandler> ViewMapper = new PropertyMapper<INativeSpinner, NativeSpinnerHandler>(ViewHandler.ViewMapper)
        {
            [nameof(INativeSpinner.ItemsSource)] = MapItemsSource,
            [nameof(INativeSpinner.SelectedIndex)] = MapSelectedIndex,
            [nameof(INativeSpinner.SelectedItem)] = MapSelectedItem,
            [nameof(INativeSpinner.Placeholder)] = MapTitle,
            [nameof(INativeSpinner.TextColor)] = MapTextColor,
            [nameof(INativeSpinner.FontSize)] = MapFontSize,
            [nameof(INativeSpinner.IsEnabled)] = MapIsEnabled,
        };

        new private static CommandMapper<INativeSpinner, NativeSpinnerHandler> ViewCommandMapper = new CommandMapper<INativeSpinner, NativeSpinnerHandler>(ViewHandler.ViewCommandMapper);

        public NativeSpinnerHandler() : base(ViewMapper, ViewCommandMapper) { }

        protected override AlwaysFireSpinner CreatePlatformView()
        {
            var context = Context ?? throw new InvalidOperationException("Context cannot be null");
            var spinner = new AlwaysFireSpinner(context, null, global::Android.Resource.Attribute.SpinnerStyle, (int)SpinnerMode.Dropdown);
            spinner.SetBackgroundResource(global::Android.Resource.Drawable.SpinnerBackground);

            spinner.Focusable = true;

            return spinner;
        }

        protected override void ConnectHandler(AlwaysFireSpinner platformView)
        {
            base.ConnectHandler(platformView);

            if (VirtualView != null)
            {
                CreateAdapter();
                UpdateItemsSource();
                UpdateSelectedIndex();
                UpdateTitle();
                UpdateTextColor();
                UpdateFontSize();
                UpdateIsEnabled();

                platformView.ItemSelected += OnItemSelected;
            }
        }

        private void CreateAdapter()
        {
            if (Context == null) return;

            SafeDisposeAdapter();

            try
            {
                _adapter = new SpinnerAdapter(
                    Context,
                    VirtualView?.ItemsSource,
                    VirtualView?.Placeholder ?? string.Empty,
                    VirtualView?.SelectedIndex ?? -1
                );

                PlatformView.Adapter = _adapter;
            }
            catch (Exception)
            {
                SafeDisposeAdapter();
                throw;
            }
        }

        private void OnItemSelected(object? sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (_isUpdatingSelection || VirtualView == null || _adapter?.IsDisposed == true)
                return;

            try
            {
                var position = e.Position;

                if (VirtualView.SelectedIndex == -1 && position == 0 && !_isInitialized)
                {
                    _isInitialized = true;
                }
                else if (VirtualView.ItemsSource != null && position >= 0 && position < VirtualView.ItemsSource.Count)
                {
                    VirtualView.SelectedIndex = position;
                    VirtualView.SelectedItem = VirtualView.ItemsSource[position];
                }
                else
                {
                    VirtualView.SelectedIndex = -1;
                    VirtualView.SelectedItem = null;
                }
            }
            catch (Exception) { }
        }

        #region Property Mappers

        public static void MapItemsSource(NativeSpinnerHandler handler, INativeSpinner spinner)
        {
            handler.UpdateItemsSource();
        }

        public static void MapSelectedIndex(NativeSpinnerHandler handler, INativeSpinner spinner)
        {
            handler.UpdateSelectedIndex();
        }

        public static void MapSelectedItem(NativeSpinnerHandler handler, INativeSpinner spinner)
        {
            handler.UpdateSelectedIndex();
        }

        public static void MapTitle(NativeSpinnerHandler handler, INativeSpinner spinner)
        {
            handler.UpdateTitle();
        }

        public static void MapTextColor(NativeSpinnerHandler handler, INativeSpinner spinner)
        {
            handler.UpdateTextColor();
        }

        public static void MapFontSize(NativeSpinnerHandler handler, INativeSpinner spinner)
        {
            handler.UpdateFontSize();
        }

        public static void MapIsEnabled(NativeSpinnerHandler handler, INativeSpinner spinner)
        {
            handler.UpdateIsEnabled();
        }

        #endregion

        #region Update Methods

        private void UpdateItemsSource()
        {
            if (_adapter?.IsDisposed != false || VirtualView == null) return;

            try
            {
                _adapter.UpdateItems(
                    VirtualView.ItemsSource,
                    VirtualView.Placeholder ?? string.Empty,
                    VirtualView.SelectedIndex
                );
            }
            catch (Exception)
            {
                CreateAdapter();
            }
        }

        private void UpdateSelectedIndex()
        {
            if (PlatformView == null || VirtualView == null || _adapter?.IsDisposed != false)
                return;

            _isUpdatingSelection = true;
            try
            {
                var selectedIndex = VirtualView.SelectedIndex;
                _adapter.SelectedIndex = selectedIndex;

                var adapterCount = PlatformView.Adapter?.Count ?? 0;
                if (selectedIndex < adapterCount)
                    PlatformView.SetSelection(selectedIndex, false);

                _adapter.NotifyDataSetChanged();
            }
            catch (Exception) { }
            finally
            {
                _isUpdatingSelection = false;
            }
        }

        private void UpdateTitle()
        {
            if (_adapter?.IsDisposed != false || VirtualView == null) return;

            try
            {
                _adapter.UpdateTitle(VirtualView.Placeholder ?? string.Empty);
            }
            catch (Exception) { }
        }

        private void UpdateTextColor()
        {
            if (_adapter?.IsDisposed != false) return;

            try
            {
                _adapter.NotifyDataSetChanged();
            }
            catch (Exception) { }
        }

        private void UpdateFontSize()
        {
            if (_adapter?.IsDisposed != false) return;

            try
            {
                _adapter.NotifyDataSetChanged();
            }
            catch (Exception) { }
        }

        private void UpdateIsEnabled()
        {
            if (PlatformView == null || VirtualView == null) return;

            try
            {
                PlatformView.Enabled = VirtualView.IsEnabled;
                PlatformView.Alpha = VirtualView.IsEnabled ? 1.0f : 0.5f;
            }
            catch (Exception) { }
        }
        #endregion

        private void SafeDisposeAdapter()
        {
            try
            {
                if (_adapter != null && !_adapter.IsDisposed)
                {
                    if (PlatformView != null)
                    {
                        PlatformView.Adapter = null;
                    }

                    _adapter.Dispose();
                }
            }
            catch (Exception) { }
            finally
            {
                _adapter = null;
                _isInitialized = false;
            }
        }

        protected override void DisconnectHandler(AlwaysFireSpinner platformView)
        {
            try
            {
                if (platformView != null)
                {
                    platformView.ItemSelected -= OnItemSelected;
                }
            }
            catch (Exception) { }
            finally
            {
                SafeDisposeAdapter();
                base.DisconnectHandler(platformView);
                _isInitialized = false;
            }
        }
    }

    internal class SpinnerAdapter : BaseAdapter, ISpinnerAdapter
    {
        private readonly LayoutInflater? _inflater;
        private IList? _items;
        private string _placeholder;
        private bool _isDisposed;

        public int SelectedIndex { get; set; } = -1;
        public bool IsDisposed => _isDisposed;

        public SpinnerAdapter(Context context, IList? items, string placeholder, int selectedIndex) : base()
        {
            _inflater = LayoutInflater.From(context);
            _items = items ?? new List<object>();
            _placeholder = placeholder ?? string.Empty;
            SelectedIndex = selectedIndex;
            _isDisposed = false;
        }

        public override int Count
        {
            get
            {
                if (_isDisposed) return 0;
                return _items?.Count ?? 0;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            if (_isDisposed || _items == null || position < 0 || position >= _items.Count)
            {
                return Java.Lang.String.ValueOf(string.Empty)!; // Safe ?
            }

            var item = _items[position];
            return Java.Lang.String.ValueOf(item?.ToString())!; // Safe ?
        }

        public override long GetItemId(int position) => _isDisposed ? 0 : position;

        public override View? GetView(int position, View? convertView, ViewGroup? parent)
        {
            if (_isDisposed) return null;

            try
            {
                View? view = convertView ?? _inflater?.Inflate(global::Android.Resource.Layout.SimpleSpinnerItem, parent, false);

                if (view is TextView textView)
                {
                    if (SelectedIndex == -1)
                    {
                        textView.Text = _placeholder;
                        textView.SetTextColor(Color.Gray);
                        textView.SetTypeface(null, TypefaceStyle.Italic);
                    }
                    else if (_items != null && position >= 0 && position < _items.Count)
                    {
                        var item = _items[position];
                        textView.Text = item?.ToString() ?? string.Empty;
                        textView.SetTextColor(Color.Black);
                        textView.SetTypeface(null, TypefaceStyle.Normal);
                    }
                }

                return view;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override View? GetDropDownView(int position, View? convertView, ViewGroup? parent)
        {
            if (_isDisposed) return null;

            try
            {
                View? view = convertView ?? _inflater?.Inflate(global::Android.Resource.Layout.SimpleSpinnerDropDownItem, parent, false);

                if (view is TextView textView)
                {
                    if (_items != null && position >= 0 && position < _items.Count)
                    {
                        var item = _items[position];
                        textView.Text = item?.ToString() ?? string.Empty;
                        textView.SetTextColor(Color.Black);
                        textView.SetTypeface(null, TypefaceStyle.Normal);
                    }
                }

                return view;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void UpdateItems(IList? items, string title, int selectedIndex)
        {
            if (_isDisposed) return;

            try
            {
                _items = items ?? new List<object>();
                _placeholder = title ?? string.Empty;
                SelectedIndex = selectedIndex;
                NotifyDataSetChanged();
            }
            catch (Exception) { }
        }

        public void UpdateTitle(string title)
        {
            if (_isDisposed) return;

            try
            {
                _placeholder = title ?? string.Empty;
                NotifyDataSetChanged();
            }
            catch (Exception) { }
        }

        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed && disposing)
            {
                _isDisposed = true;
                _items = null;
                _placeholder = string.Empty;
            }

            try
            {
                base.Dispose(disposing);
            }
            catch (Exception) { }
        }
    }
}
#endif