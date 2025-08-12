#if ANDROID
using Microsoft.Maui.Handlers;
using AndroidX.AppCompat.Widget;
using Android.Widget;
using Android.Views;
using Android.Graphics;
using Android.Content;
using System.Collections;
using Maui.ComboBox.Interfaces;
using Color = Android.Graphics.Color;
using View = Android.Views.View;
using Resource = Microsoft.Maui.Resource;

namespace Maui.ComboBox.Platforms.Android
{
    public class NativeSpinnerHandler : ViewHandler<INativeSpinner, AppCompatSpinner>
    {

        private SpinnerAdapter? _adapter;
        private bool _isUpdatingSelection;

        public NativeSpinnerHandler() : base(ViewHandler.ViewMapper, ViewHandler.ViewCommandMapper) 
        { }

        protected override AppCompatSpinner CreatePlatformView()
        {
            var context = Context ?? throw new InvalidOperationException("Context cannot be null");
            var spinner = new AppCompatSpinner(context);

            // Set default styling to match native Android spinners
            spinner.SetBackgroundResource(global::Android.Resource.Drawable.SpinnerBackground);

            return spinner;
        }

        protected override void ConnectHandler(AppCompatSpinner platformView)
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

        protected override void DisconnectHandler(AppCompatSpinner platformView)
        {
            if (platformView != null)
            {
                platformView.ItemSelected -= OnItemSelected;
                platformView.Adapter = null;
            }

            _adapter?.Dispose();
            _adapter = null;

            base.DisconnectHandler(platformView);
        }

        private void CreateAdapter()
        {
            if (Context == null) return;

            _adapter = new SpinnerAdapter(Context, VirtualView.ItemsSource, VirtualView.Title);
            PlatformView.Adapter = _adapter;
        }

        private void OnItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (_isUpdatingSelection || VirtualView == null) return;

            // Account for title item at position 0
            var actualPosition = e.Position - 1;

            if (actualPosition >= 0 && VirtualView.ItemsSource != null && actualPosition < VirtualView.ItemsSource.Count)
            {
                VirtualView.SelectedIndex = actualPosition;
                VirtualView.SelectedItem = VirtualView.ItemsSource[actualPosition];
            }
            else
            {
                // Title was selected or invalid position
                VirtualView.SelectedIndex = -1;
                VirtualView.SelectedItem = null;
            }
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
            if (_adapter != null && VirtualView != null)
            {
                _adapter.UpdateItems(VirtualView.ItemsSource, VirtualView.Title);
                UpdateSelectedIndex();
            }
        }

        private void UpdateSelectedIndex()
        {
            if (PlatformView == null || VirtualView == null) return;

            _isUpdatingSelection = true;
            try
            {
                var selectedIndex = VirtualView.SelectedIndex;

                // Add 1 to account for title item at position 0
                var spinnerPosition = selectedIndex >= 0 ? selectedIndex + 1 : 0;

                if (spinnerPosition < PlatformView.Adapter?.Count)
                {
                    PlatformView.SetSelection(spinnerPosition, false);
                }
            }
            finally
            {
                _isUpdatingSelection = false;
            }
        }

        private void UpdateTitle()
        {
            if (_adapter != null && VirtualView != null)
            {
                _adapter.UpdateTitle(VirtualView.Title);
            }
        }

        private void UpdateTextColor()
        {
            // Text color will be handled by the adapter
            _adapter?.NotifyDataSetChanged();
        }

        private void UpdateFontSize()
        {
            // Font size will be handled by the adapter
            _adapter?.NotifyDataSetChanged();
        }

        private void UpdateIsEnabled()
        {
            if (PlatformView != null && VirtualView != null)
            {
                PlatformView.Enabled = VirtualView.IsEnabled;
                PlatformView.Alpha = VirtualView.IsEnabled ? 1.0f : 0.5f;
            }
        }
        #endregion
    }

    internal class SpinnerAdapter : BaseAdapter, ISpinnerAdapter
    {
        private readonly Context _context;
        private readonly LayoutInflater _inflater;
        private IList _items;
        private string _title;

        public SpinnerAdapter(Context context, IList items, string title)
        {
            _context = context;
            _inflater = LayoutInflater.From(context);
            _items = items ?? new List<object>();
            _title = title ?? string.Empty;
        }

        public override int Count => (_items?.Count ?? 0) + 1; // +1 for title

        public override Java.Lang.Object GetItem(int position)
        {
            if (position == 0)
                return new JavaObjectWrapper(_title);

            var actualIndex = position - 1;
            if (_items != null && actualIndex >= 0 && actualIndex < _items.Count)
                return new JavaObjectWrapper(_items[actualIndex]);

            return new JavaObjectWrapper(string.Empty);
        }

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            return CreateView(position, convertView, parent, global::Android.Resource.Layout.SimpleSpinnerItem);
        }

        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            var view = CreateView(position, convertView, parent, global::Android.Resource.Layout.SimpleSpinnerDropDownItem);

            // Style the title differently in dropdown
            if (position == 0 && view is TextView titleView)
            {
                titleView.SetTextColor(Color.Gray);
                titleView.SetTypeface(null, TypefaceStyle.Italic);
            }

            return view;
        }

        private View CreateView(int position, View convertView, ViewGroup parent, int layoutResource)
        {
            View view = convertView ?? _inflater.Inflate(layoutResource, parent, false);

            if (view is TextView textView)
            {
                var item = GetItem(position);
                var text = item?.ToString() ?? string.Empty;

                textView.Text = text;

                // Apply styling based on position
                if (position == 0)
                {
                    // Title styling
                    textView.SetTextColor(Color.Gray);
                    if (layoutResource == global::Android.Resource.Layout.SimpleSpinnerItem)
                    {
                        textView.SetTypeface(null, TypefaceStyle.Normal);
                    }
                }
                else
                {
                    // Regular item styling
                    textView.SetTextColor(Color.Black);
                    textView.SetTypeface(null, TypefaceStyle.Normal);
                }
            }

            return view;
        }

        public void UpdateItems(IList items, string title)
        {
            _items = items ?? new List<object>();
            _title = title ?? string.Empty;
            NotifyDataSetChanged();
        }

        public void UpdateTitle(string title)
        {
            _title = title ?? string.Empty;
            NotifyDataSetChanged();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _items = null;
            }
            base.Dispose(disposing);
        }
    }

    internal class JavaObjectWrapper : Java.Lang.Object
    {
        private readonly object _obj;

        public JavaObjectWrapper(object obj)
        {
            _obj = obj;
        }

        public override string ToString()
        {
            return _obj?.ToString() ?? string.Empty;
        }
    }
}
#endif