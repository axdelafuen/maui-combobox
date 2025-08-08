using Maui.ComboBox.Lib.Helpers;
using System.Collections;
using Microsoft.Maui.Controls;

namespace Maui.ComboBox
{
    public partial class NewComboBox : GraphicsView
    {
        #region Bindable properties

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(ICollection), typeof(NewComboBox), propertyChanged: OnItemsSourceChanged);
        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(NewComboBox), -1, propertyChanged: OnSelectedIndexChanged);
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(NewComboBox), null, propertyChanged: OnSelectedItemChanged);
        public static readonly BindableProperty LabelProperty = BindableProperty.Create(nameof(Label), typeof(string), typeof(NewComboBox), "Select item...");
        public static readonly BindableProperty FontColorProperty = BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(NewComboBox), Colors.Black);
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(float), typeof(NewComboBox), 16f);
        public static readonly BindableProperty OutlineColorProperty = BindableProperty.Create(nameof(OutlineColor), typeof(Color), typeof(NewComboBox), Colors.Gray);
        public static readonly BindableProperty OutlineWidthProperty = BindableProperty.Create(nameof(OutlineWidth), typeof(float), typeof(NewComboBox), 1f);
        public static readonly BindableProperty DropDownBackgroundColorProperty = BindableProperty.Create(nameof(DropDownBackgroundColor), typeof(Color), typeof(NewComboBox), Colors.White);
        public static readonly BindableProperty DropDownTextColorProperty = BindableProperty.Create(nameof(DropDownTextColor), typeof(Color), typeof(NewComboBox), Colors.Black);
        public static readonly BindableProperty HeaderBackgroundColorProperty = BindableProperty.Create(nameof(HeaderBackgroundColor), typeof(Color), typeof(NewComboBox), Colors.White);
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(NewComboBox), string.Empty);

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
        private PointF _startDragPoint;
        private float _totalDragDistance = 0f;

        private const int MaxVisibleItems = 6;
        private const float ItemHeight = 36f;
        private const float HeaderHeight = 48f;
        private const float DragThreshold = 15f; // Minimum distance to start scrolling

        public NewComboBox()
        {
            Drawable = new ComboBoxDrawable(this);
            HeightRequest = HeaderHeight; // FIXED: Keep constant height for overlay
            WidthRequest = 200;
            BackgroundColor = Colors.Transparent;

            StartInteraction += OnStartInteraction;
            DragInteraction += OnDragInteraction;
            EndInteraction += OnEndInteraction;
        }

        private void OnGlobalTapped(object? sender, TappedEventArgs e)
        {
            if (!_isDropDownOpen) return;

            // Check if the tap was on this control
            var position = e.GetPosition((View?)sender);
            if (position == null) return;

            // Get this control's bounds relative to the main page
            var thisPosition = this.GetAbsolutePosition();
            var thisBounds = new RectF(
                (float)thisPosition.X,
                (float)thisPosition.Y,
                (float)Width,
                (float)(HeaderHeight + (_isDropDownOpen ? GetDropDownHeight() : 0))
            );

            // If tap is outside this control, close dropdown
            if (!thisBounds.Contains((float)position.Value.X, (float)position.Value.Y))
            {
                CloseDropDown();
            }
        }

        private void OnStartInteraction(object? sender, TouchEventArgs e)
        {
            if (e.Touches.Length == 0) return;

            var point = new PointF(e.Touches[0].X, e.Touches[0].Y);
            _startDragPoint = point;
            _isDragging = false;
            _totalDragDistance = 0f;

            // Handle header click to toggle dropdown
            if (IsHeaderClick(point))
            {
                ToggleDropDown();
                return;
            }

            // Update hover state for dropdown items
            if (_isDropDownOpen && IsDropDownClick(point))
            {
                UpdateHoveredIndex(point);
            }
        }

        private void OnDragInteraction(object? sender, TouchEventArgs e)
        {
            if (e.Touches.Length == 0 || !_isDropDownOpen) return;

            var currentPoint = new PointF(e.Touches[0].X, e.Touches[0].Y);

            // Calculate total drag distance from start point
            var dragDeltaFromStart = Math.Abs(currentPoint.Y - _startDragPoint.Y);
            _totalDragDistance = Math.Max(_totalDragDistance, dragDeltaFromStart);

            // Only start dragging if we've moved enough
            if (_totalDragDistance > DragThreshold)
            {
                _isDragging = true;
            }

            // Handle scrolling if we're dragging and in dropdown area
            if (_isDragging && IsDropDownClick(currentPoint) && ItemsSource != null && ItemsSource.Count > MaxVisibleItems)
            {
                var deltaY = _startDragPoint.Y - currentPoint.Y;
                var maxScroll = Math.Max(0, (ItemsSource.Count * ItemHeight) - (MaxVisibleItems * ItemHeight));

                _scrollOffset = Math.Clamp(_scrollOffset + deltaY * 2f, 0, maxScroll); // Multiply by 2 for faster scrolling
                _startDragPoint = currentPoint; // Update reference point for continuous scrolling

                Invalidate();
            }
            else if (!_isDragging && IsDropDownClick(currentPoint))
            {
                // Update hover state for potential selection
                UpdateHoveredIndex(currentPoint);
            }
        }

        private void OnEndInteraction(object? sender, TouchEventArgs e)
        {
            if (e.Touches.Length == 0) return;

            var point = new PointF(e.Touches[0].X, e.Touches[0].Y);

            // If we were dragging, don't select an item
            if (_isDragging)
            {
                _isDragging = false;
                _hoveredIndex = -1;
                Invalidate();
                return;
            }

            // Handle item selection on tap (not drag)
            if (_isDropDownOpen && IsDropDownClick(point))
            {
                SelectItemAtPoint(point);
            }
            // Handle clicking outside dropdown
            else if (_isDropDownOpen && !IsHeaderClick(point))
            {
                CloseDropDown();
            }

            _isDragging = false;
            _totalDragDistance = 0f;
        }

        private bool IsHeaderClick(PointF point)
        {
            return point.Y >= 0 && point.Y <= HeaderHeight && point.X >= 0 && point.X <= Width;
        }

        private bool IsDropDownClick(PointF point)
        {
            if (!_isDropDownOpen || ItemsSource == null) return false;

            var dropDownHeight = GetDropDownHeight();
            return point.Y >= HeaderHeight && point.Y <= HeaderHeight + dropDownHeight &&
                   point.X >= 0 && point.X <= Width;
        }

        private float GetDropDownHeight()
        {
            if (ItemsSource == null) return 0;
            int visibleCount = Math.Min(ItemsSource.Count, MaxVisibleItems);
            return visibleCount * ItemHeight;
        }

        private void UpdateHoveredIndex(PointF point)
        {
            if (!_isDropDownOpen || ItemsSource == null) return;

            var relativeY = point.Y - HeaderHeight;
            var index = GetItemIndexAt(relativeY + _scrollOffset);

            if (index != _hoveredIndex && index >= 0 && index < ItemsSource.Count)
            {
                _hoveredIndex = index;
                Invalidate();
            }
        }

        private void SelectItemAtPoint(PointF point)
        {
            if (!_isDropDownOpen || ItemsSource == null) return;

            var relativeY = point.Y - HeaderHeight;
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

        private void ToggleDropDown()
        {
            _isDropDownOpen = !_isDropDownOpen;
            _hoveredIndex = -1;
            _scrollOffset = 0f;
            _isDragging = false;
            _totalDragDistance = 0f;
            // DON'T change HeightRequest - keep overlay behavior
            Invalidate();
        }

        private void CloseDropDown()
        {
            _isDropDownOpen = false;
            _hoveredIndex = -1;
            _isDragging = false;
            _totalDragDistance = 0f;
            Invalidate();
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var combo = (NewComboBox)bindable;
            combo.SelectedIndex = -1;
            combo.SelectedItem = null;
            combo._scrollOffset = 0f;
            combo.CloseDropDown();
        }

        private static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var combo = (NewComboBox)bindable;
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
            var combo = (NewComboBox)bindable;
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

        private int GetItemIndexAt(float y)
        {
            if (ItemsSource == null) return -1;

            int idx = (int)(y / ItemHeight);
            return (idx >= 0 && idx < ItemsSource.Count) ? idx : -1;
        }

        class ComboBoxDrawable : IDrawable
        {
            private readonly NewComboBox _combo;

            public ComboBoxDrawable(NewComboBox combo)
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

                // Draw shadow for overlay effect
                var shadowRect = new RectF(dropDownRect.X + 2, dropDownRect.Y + 2, dropDownRect.Width, dropDownRect.Height);
                canvas.FillColor = Colors.Black.WithAlpha(0.1f);
                canvas.FillRoundedRectangle(shadowRect, 4f);

                // Draw dropdown background
                canvas.FillColor = _combo.DropDownBackgroundColor;
                canvas.FillRoundedRectangle(dropDownRect, 4f);

                // Draw dropdown border
                canvas.StrokeColor = Colors.Gray;
                canvas.StrokeSize = 1;
                canvas.DrawRoundedRectangle(dropDownRect, 4f);

                // Clip content to dropdown area
                canvas.ClipRectangle(dropDownRect);

                // Calculate which items to draw based on scroll
                int startIndex = Math.Max(0, (int)(_combo._scrollOffset / ItemHeight));
                int endIndex = Math.Min(totalCount, startIndex + visibleCount + 2);

                // Draw items
                var itemsList = itemsSource.Cast<object>().ToList();
                for (int i = startIndex; i < endIndex && i < totalCount; i++)
                {
                    var item = itemsList[i];
                    float itemY = HeaderHeight + (i * ItemHeight) - _combo._scrollOffset;
                    var itemRect = new RectF(4, itemY, (float)_combo.Width - 8, ItemHeight);

                    // Skip items that are completely outside visible area
                    if (itemRect.Bottom < HeaderHeight || itemRect.Top > HeaderHeight + menuHeight)
                        continue;

                    // Draw item background
                    if (i == _combo._hoveredIndex)
                    {
                        canvas.FillColor = Colors.LightGray.WithAlpha(0.5f);
                        canvas.FillRoundedRectangle(itemRect, 2f);
                    }
                    else if (i == _combo.SelectedIndex)
                    {
                        canvas.FillColor = Colors.LightBlue.WithAlpha(0.3f);
                        canvas.FillRoundedRectangle(itemRect, 2f);
                    }

                    // Draw item text
                    string itemText = item?.ToString() ?? "null";
                    canvas.FontColor = _combo.DropDownTextColor;
                    canvas.FontSize = _combo.FontSize;
                    canvas.DrawString(itemText, itemRect.X + 8, itemRect.Y + (ItemHeight - _combo.FontSize) / 2,
                                    itemRect.Width - 16, ItemHeight,
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
                float maxScrollOffset = Math.Max(0, totalContentHeight - visibleHeight);
                float scrollRatio = maxScrollOffset > 0 ? _combo._scrollOffset / maxScrollOffset : 0;
                float scrollBarY = dropDownRect.Top + 4 + (visibleHeight - scrollBarHeight - 8) * scrollRatio;

                var scrollBarRect = new RectF(
                    dropDownRect.Right - 12,
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

// Extension method to get absolute position
public static class ViewExtensions
{
    public static Point GetAbsolutePosition(this View view)
    {
        double x = 0, y = 0;
        var current = view;

        while (current != null)
        {
            x += current.X;
            y += current.Y;
            current = current.Parent as View;
        }

        return new Point(x, y);
    }
}