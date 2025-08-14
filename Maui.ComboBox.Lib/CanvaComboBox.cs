using Maui.ComboBox.Helpers;
using System.Collections;

namespace Maui.ComboBox
{
    public partial class CanvaComboBox : GraphicsView
    {
        #region Bindable properties

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(ICollection), typeof(CanvaComboBox), propertyChanged: OnItemsSourceChanged);
        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(CanvaComboBox), -1, propertyChanged: OnSelectedIndexChanged);
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(CanvaComboBox), null, propertyChanged: OnSelectedItemChanged);
        public static readonly BindableProperty LabelProperty = BindableProperty.Create(nameof(Label), typeof(string), typeof(CanvaComboBox), "Select item...");
        public static readonly BindableProperty FontColorProperty = BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(CanvaComboBox), Colors.Black);
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(float), typeof(CanvaComboBox), 16f);
        public static readonly BindableProperty OutlineColorProperty = BindableProperty.Create(nameof(OutlineColor), typeof(Color), typeof(CanvaComboBox), Colors.Gray);
        public static readonly BindableProperty OutlineWidthProperty = BindableProperty.Create(nameof(OutlineWidth), typeof(float), typeof(CanvaComboBox), 1f);
        public static readonly BindableProperty DropDownBackgroundColorProperty = BindableProperty.Create(nameof(DropDownBackgroundColor), typeof(Color), typeof(CanvaComboBox), Colors.White);
        public static readonly BindableProperty DropDownTextColorProperty = BindableProperty.Create(nameof(DropDownTextColor), typeof(Color), typeof(CanvaComboBox), Colors.Black);
        public static readonly BindableProperty HeaderBackgroundColorProperty = BindableProperty.Create(nameof(HeaderBackgroundColor), typeof(Color), typeof(CanvaComboBox), Colors.White);
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(CanvaComboBox), string.Empty);

        #endregion

        #region Properties

        public ICollection ItemsSource
        {
            get => (ICollection)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public Color FontColor
        {
            get => (Color)GetValue(FontColorProperty);
            set => SetValue(FontColorProperty, value);
        }

        public float FontSize
        {
            get => (float)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public Color OutlineColor
        {
            get => (Color)GetValue(OutlineColorProperty);
            set => SetValue(OutlineColorProperty, value);
        }

        public float OutlineWidth
        {
            get => (float)GetValue(OutlineWidthProperty);
            set => SetValue(OutlineWidthProperty, value);
        }

        public Color DropDownBackgroundColor
        {
            get => (Color)GetValue(DropDownBackgroundColorProperty);
            set => SetValue(DropDownBackgroundColorProperty, value);
        }

        public Color DropDownTextColor
        {
            get => (Color)GetValue(DropDownTextColorProperty);
            set => SetValue(DropDownTextColorProperty, value);
        }

        public Color HeaderBackgroundColor
        {
            get => (Color)GetValue(HeaderBackgroundColorProperty);
            set => SetValue(HeaderBackgroundColorProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        #endregion

        public event EventHandler<int>? SelectedIndexChanged;

        private bool _isDropDownOpen = false;
        private int _hoveredIndex = -1;
        private float _scrollOffset = 0f;
        private bool _isDragging = false;
        private PointF _lastDragPoint;

        private const int MaxVisibleItems = 6;
        private const float ItemHeight = 36f;
        private const float HeaderHeight = 48f;

        public CanvaComboBox()
        {
            Drawable = new ComboBoxDrawable(this);
            HeightRequest = HeaderHeight;
            WidthRequest = 200;
            BackgroundColor = Colors.Transparent;

            // Use a single tap gesture recognizer
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnTapped;
            GestureRecognizers.Add(tapGesture);

            // Handle pointer events for better control
            StartInteraction += OnStartInteraction;
            DragInteraction += OnDragInteraction;
            EndInteraction += OnEndInteraction;
        }

        private void OnTapped(object sender, TappedEventArgs e)
        {
            var position = e.GetPosition(this);
            if (position == null) return;

            var point = position.Value;

            // If clicking on header, toggle dropdown
            if (IsHeaderClick(point))
            {
                ToggleDropDown();
                return;
            }

            // If dropdown is open and clicking on an item
            if (_isDropDownOpen && IsDropDownClick(point))
            {
                SelectItemAtPoint(point);
                return;
            }

            // If clicking outside, close dropdown
            if (_isDropDownOpen)
            {
                CloseDropDown();
            }
        }

        private void OnStartInteraction(object sender, TouchEventArgs e)
        {
            if (e.Touches.Length == 0) return;

            var point = new PointF(e.Touches[0].X, e.Touches[0].Y);
            _lastDragPoint = point;
            _isDragging = false;

            // Check if starting drag in dropdown area
            if (_isDropDownOpen && IsDropDownClick(point))
            {
                UpdateHoveredIndex(point);
            }
        }

        private void OnDragInteraction(object sender, TouchEventArgs e)
        {
            if (e.Touches.Length == 0 || !_isDropDownOpen) return;

            var point = new PointF(e.Touches[0].X, e.Touches[0].Y);

            // Calculate drag distance to determine if this is a scroll or selection
            var dragDistance = Math.Sqrt(Math.Pow(point.X - _lastDragPoint.X, 2) + Math.Pow(point.Y - _lastDragPoint.Y, 2));

            if (dragDistance > 10) // Threshold for scroll vs tap
            {
                _isDragging = true;
            }

            if (_isDragging && IsDropDownClick(point))
            {
                // Handle scrolling
                if (ItemsSource != null && ItemsSource.Count > MaxVisibleItems)
                {
                    var deltaY = _lastDragPoint.Y - point.Y;
                    var maxScroll = Math.Max(0, (ItemsSource.Count * ItemHeight) - (MaxVisibleItems * ItemHeight));
                    _scrollOffset = Math.Clamp(_scrollOffset + deltaY, 0, maxScroll);
                    Invalidate();
                }
            }
            else if (!_isDragging && IsDropDownClick(point))
            {
                // Update hover for selection
                UpdateHoveredIndex(point);
            }

            _lastDragPoint = point;
        }

        private void OnEndInteraction(object sender, TouchEventArgs e)
        {
            if (e.Touches.Length == 0) return;

            var point = new PointF(e.Touches[0].X, e.Touches[0].Y);

            if (_isDropDownOpen)
            {
                // If we were dragging, don't select an item
                if (_isDragging)
                {
                    _isDragging = false;
                    _hoveredIndex = -1;
                    Invalidate();
                    return;
                }

                // If clicking on dropdown item, select it
                if (IsDropDownClick(point))
                {
                    SelectItemAtPoint(point);
                    return;
                }

                // If clicking outside dropdown (but not on header), close it
                if (!IsHeaderClick(point))
                {
                    CloseDropDown();
                }
            }

            _isDragging = false;
        }

        private bool IsHeaderClick(PointF point)
        {
            return point.Y >= 0 && point.Y <= HeaderHeight && point.X >= 0 && point.X <= Width;
        }

        private bool IsDropDownClick(PointF point)
        {
            if (!_isDropDownOpen || ItemsSource == null) return false;

            var dropDownRect = GetDropDownRect();
            return dropDownRect.Contains(point);
        }

        private void UpdateHoveredIndex(PointF point)
        {
            if (!_isDropDownOpen) return;

            var dropDownRect = GetDropDownRect();
            if (dropDownRect.Contains(point))
            {
                var relativeY = point.Y - dropDownRect.Top;
                var index = GetItemIndexAt(relativeY + _scrollOffset);

                if (index != _hoveredIndex)
                {
                    _hoveredIndex = index;
                    Invalidate();
                }
            }
            else
            {
                if (_hoveredIndex != -1)
                {
                    _hoveredIndex = -1;
                    Invalidate();
                }
            }
        }

        private void SelectItemAtPoint(PointF point)
        {
            if (!_isDropDownOpen || ItemsSource == null) return;

            var dropDownRect = GetDropDownRect();
            if (dropDownRect.Contains(point))
            {
                var relativeY = point.Y - dropDownRect.Top;
                var index = GetItemIndexAt(relativeY + _scrollOffset);

                if (index >= 0 && index < ItemsSource.Count)
                {
                    SelectedIndex = index;
                    var selectedItem = CollectionHelper.GetItemAt(ItemsSource, index);
                    SelectedItem = selectedItem;
                    SelectedIndexChanged?.Invoke(this, index);
                    CloseDropDown();
                }
            }
        }

        private void ToggleDropDown()
        {
            _isDropDownOpen = !_isDropDownOpen;
            _hoveredIndex = -1;
            _scrollOffset = 0f;
            _isDragging = false;
            UpdateHeightRequest();
            Invalidate();
        }

        private void CloseDropDown()
        {
            _isDropDownOpen = false;
            _hoveredIndex = -1;
            _isDragging = false;
            UpdateHeightRequest();
            Invalidate();
        }

        private void UpdateHeightRequest()
        {
            if (_isDropDownOpen && ItemsSource != null && ItemsSource.Count > 0)
            {
                int visibleCount = Math.Min(ItemsSource.Count, MaxVisibleItems);
                float dropDownHeight = visibleCount * ItemHeight;
                HeightRequest = HeaderHeight + dropDownHeight;
            }
            else
            {
                HeightRequest = HeaderHeight;
            }
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var combo = (CanvaComboBox)bindable;
            combo.SelectedIndex = -1;
            combo.SelectedItem = null;
            combo._scrollOffset = 0f;
            combo.CloseDropDown();
        }

        private static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var combo = (CanvaComboBox)bindable;
            int idx = (int)newValue;
            if (combo.ItemsSource != null && idx >= 0 && idx < combo.ItemsSource.Count)
            {
                var item = CollectionHelper.GetItemAt(combo.ItemsSource, idx);
                if (!Equals(combo.SelectedItem, item))
                {
                    combo.SelectedItem = item;
                }
            }
            else if (idx == -1)
            {
                combo.SelectedItem = null;
            }
            combo.Invalidate();
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var combo = (CanvaComboBox)bindable;
            int idx = -1;

            if (combo.ItemsSource != null && newValue != null)
            {
                int i = 0;
                foreach (var item in combo.ItemsSource)
                {
                    if (Equals(item, newValue))
                    {
                        idx = i;
                        break;
                    }
                    i++;
                }
            }

            if (idx != combo.SelectedIndex)
            {
                combo.SelectedIndex = idx;
            }
            combo.Invalidate();
        }

        private RectF GetDropDownRect()
        {
            if (!_isDropDownOpen || ItemsSource == null)
                return new RectF(0, 0, 0, 0);

            int visibleCount = Math.Min(ItemsSource.Count, MaxVisibleItems);
            float menuHeight = visibleCount * ItemHeight;
            return new RectF(0, HeaderHeight, (float)Width, menuHeight);
        }

        private int GetItemIndexAt(float y)
        {
            if (ItemsSource == null) return -1;

            int idx = (int)(y / ItemHeight);
            return (idx >= 0 && idx < ItemsSource.Count) ? idx : -1;
        }

        class ComboBoxDrawable : IDrawable
        {
            private readonly CanvaComboBox _combo;

            public ComboBoxDrawable(CanvaComboBox combo)
            {
                _combo = combo;
            }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                DrawHeader(canvas);

                if (_combo._isDropDownOpen && _combo.ItemsSource != null && _combo.ItemsSource.Count > 0)
                {
                    DrawDropDown(canvas);
                }
            }

            private void DrawHeader(ICanvas canvas)
            {
                float radius = 6f;
                var headerRect = new RectF(0, 0, (float)_combo.Width, HeaderHeight);

                canvas.SaveState();
                canvas.Antialias = true;

                // Draw header background
                canvas.FillColor = _combo.HeaderBackgroundColor;
                canvas.FillRoundedRectangle(headerRect, radius);

                // Draw header border
                canvas.StrokeColor = _combo.OutlineColor;
                canvas.StrokeSize = _combo.OutlineWidth;
                canvas.DrawRoundedRectangle(headerRect, radius);

                // Draw label
                if (!string.IsNullOrEmpty(_combo.Label))
                {
                    canvas.FontColor = Colors.Gray;
                    canvas.FontSize = 12;
                    canvas.DrawString(_combo.Label, 12, 4, (float)_combo.Width - 24, 16, HorizontalAlignment.Left, VerticalAlignment.Top);
                }

                // Draw selected text or placeholder
                string displayText = GetDisplayText();
                canvas.FontColor = _combo.FontColor;
                canvas.FontSize = _combo.FontSize;
                canvas.DrawString(displayText, 16, 20, (float)_combo.Width - 48, 24, HorizontalAlignment.Left, VerticalAlignment.Center);

                // Draw arrow
                DrawArrow(canvas, (float)_combo.Width - 24, HeaderHeight / 2, 12, _combo._isDropDownOpen);

                canvas.RestoreState();
            }

            private string GetDisplayText()
            {
                if (_combo.SelectedIndex >= 0 && _combo.ItemsSource != null && _combo.SelectedIndex < _combo.ItemsSource.Count)
                {
                    var selectedItem = CollectionHelper.GetItemAt(_combo.ItemsSource, _combo.SelectedIndex);
                    return selectedItem?.ToString() ?? "";
                }
                return _combo.Placeholder ?? "Select...";
            }

            private void DrawArrow(ICanvas canvas, float x, float y, float size, bool up)
            {
                canvas.SaveState();
                canvas.StrokeColor = Colors.Gray;
                canvas.StrokeSize = 2;
                canvas.Translate(x, y);

                float halfSize = size / 2f;
                float quarterSize = size / 4f;

                if (up)
                {
                    canvas.DrawLine(-halfSize, quarterSize, 0, -quarterSize);
                    canvas.DrawLine(0, -quarterSize, halfSize, quarterSize);
                }
                else
                {
                    canvas.DrawLine(-halfSize, -quarterSize, 0, quarterSize);
                    canvas.DrawLine(0, quarterSize, halfSize, -quarterSize);
                }
                canvas.RestoreState();
            }

            private void DrawDropDown(ICanvas canvas)
            {
                var itemsSource = _combo.ItemsSource;
                if (itemsSource == null) return;

                int totalCount = itemsSource.Count;
                int visibleCount = Math.Min(totalCount, MaxVisibleItems);
                float menuHeight = visibleCount * ItemHeight;
                var dropDownRect = new RectF(0, HeaderHeight, (float)_combo.Width, menuHeight);

                canvas.SaveState();

                // Draw dropdown background
                canvas.FillColor = _combo.DropDownBackgroundColor;
                canvas.FillRectangle(dropDownRect);

                // Draw dropdown border
                canvas.StrokeColor = Colors.Gray;
                canvas.StrokeSize = 1;
                canvas.DrawRectangle(dropDownRect);

                // Clip content to dropdown area
                canvas.ClipRectangle(dropDownRect);

                // Calculate which items to draw
                int startIndex = (int)(_combo._scrollOffset / ItemHeight);
                int endIndex = Math.Min(totalCount, startIndex + visibleCount + 2);

                // Draw items
                var itemsList = itemsSource.Cast<object>().ToList();
                for (int i = startIndex; i < endIndex && i < totalCount; i++)
                {
                    var item = itemsList[i];
                    float itemY = HeaderHeight + (i * ItemHeight) - _combo._scrollOffset;
                    var itemRect = new RectF(0, itemY, (float)_combo.Width, ItemHeight);

                    // Skip items that are completely outside visible area
                    if (itemRect.Bottom < HeaderHeight || itemRect.Top > HeaderHeight + menuHeight)
                        continue;

                    // Draw item background
                    if (i == _combo._hoveredIndex)
                    {
                        canvas.FillColor = Colors.LightGray.WithAlpha(0.5f);
                        canvas.FillRectangle(itemRect);
                    }
                    else if (i == _combo.SelectedIndex)
                    {
                        canvas.FillColor = Colors.LightBlue.WithAlpha(0.3f);
                        canvas.FillRectangle(itemRect);
                    }

                    // Draw item text
                    string itemText = item?.ToString() ?? "null";
                    canvas.FontColor = _combo.DropDownTextColor;
                    canvas.FontSize = _combo.FontSize;
                    canvas.DrawString(itemText, 12, itemRect.Y + (ItemHeight - _combo.FontSize) / 2,
                                    (float)_combo.Width - 24, ItemHeight,
                                    HorizontalAlignment.Left, VerticalAlignment.Center);
                }

                // Draw scrollbar if needed
                if (totalCount > MaxVisibleItems)
                {
                    DrawScrollBar(canvas, dropDownRect, totalCount);
                }

                canvas.RestoreState();
            }

            private void DrawScrollBar(ICanvas canvas, RectF dropDownRect, int totalCount)
            {
                float totalContentHeight = totalCount * ItemHeight;
                float visibleHeight = dropDownRect.Height;

                // Calculate scrollbar dimensions
                float scrollBarHeight = Math.Max(20f, visibleHeight * (visibleHeight / totalContentHeight));
                float scrollRange = visibleHeight - scrollBarHeight;
                float scrollRatio = _combo._scrollOffset / (totalContentHeight - visibleHeight);
                float scrollBarY = dropDownRect.Top + scrollRange * scrollRatio;

                var scrollBarRect = new RectF(
                    dropDownRect.Right - 8,
                    scrollBarY,
                    6,
                    scrollBarHeight
                );

                canvas.FillColor = Colors.DarkGray.WithAlpha(0.7f);
                canvas.FillRoundedRectangle(scrollBarRect, 3);
            }
        }
    }
}