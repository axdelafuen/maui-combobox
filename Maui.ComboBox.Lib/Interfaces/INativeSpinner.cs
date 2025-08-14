using System.Collections;

namespace Maui.ComboBox.Interfaces
{
    public interface INativeSpinner : IView
    {
        /// <summary>
        /// Collection of items to display in the spinner
        /// </summary>
        IList ItemsSource { get; set; }

        /// <summary>
        /// Currently selected item
        /// </summary>
        object? SelectedItem { get; set; }

        /// <summary>
        /// Index of the currently selected item
        /// </summary>
        int SelectedIndex { get; set; }

        /// <summary>
        /// Title/hint text shown when no item is selected
        /// </summary>
        string Placeholder { get; set; }

        /// <summary>
        /// Text color for the spinner
        /// </summary>
        Color TextColor { get; set; }

        /// <summary>
        /// Font size for the spinner text
        /// </summary>
        double FontSize { get; set; }

        /// <summary>
        /// Whether the spinner is enabled
        /// </summary>
        new bool IsEnabled { get; set; }

        /// <summary>
        /// Event fired when selection changes
        /// </summary>
        event EventHandler<SpinnerSelectionChangedEventArgs> SelectionChanged;
    }

    public class SpinnerSelectionChangedEventArgs : EventArgs
    {
        public object? SelectedItem { get; }
        public int SelectedIndex { get; }
        public object PreviousSelection { get; }

        public SpinnerSelectionChangedEventArgs(object? selectedItem, int selectedIndex, object previousSelection)
        {
            SelectedItem = selectedItem;
            SelectedIndex = selectedIndex;
            PreviousSelection = previousSelection;
        }
    }
}
