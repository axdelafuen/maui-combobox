#if ANDROID
using Android.Content;
using Android.Util;
using AndroidX.AppCompat.Widget;

namespace Maui.ComboBox.Platforms.Android
{
    public class AlwaysFireSpinner : AppCompatSpinner
    {
        public AlwaysFireSpinner(Context context) : base(context) { }
        public AlwaysFireSpinner(Context context, IAttributeSet attrs) : base(context, attrs) { }
        public AlwaysFireSpinner(Context context, IAttributeSet? attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { }

        public AlwaysFireSpinner(Context context, IAttributeSet? attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes) { }

        public override void SetSelection(int position)
        {
            bool sameSelected = position == SelectedItemPosition;
            base.SetSelection(position);
            if (sameSelected)
            {
                OnItemSelectedListener?.OnItemSelected(this, SelectedView, position, SelectedItemId);
            }
        }

        public override void SetSelection(int position, bool animate)
        {
            bool sameSelected = position == SelectedItemPosition;
            base.SetSelection(position, animate);
            if (sameSelected)
            {
                OnItemSelectedListener?.OnItemSelected(this, SelectedView, position, SelectedItemId);
            }
        }
    }
}
#endif