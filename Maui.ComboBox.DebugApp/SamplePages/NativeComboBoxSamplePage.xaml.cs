using CommunityToolkit.Maui.Core.Extensions;
using Maui.ComboBox.DebugApp.Models;
using Maui.ComboBox.DebugApp.Stub;
using System.Collections.ObjectModel;

namespace Maui.ComboBox.DebugApp.SamplePages
{
    public partial class NativeComboBoxSamplePage : ContentPage
    {
        private TestItem? _selectedObjectItem;
        public TestItem? SelectedObjectItem
        {
            get => _selectedObjectItem;
            set
            {
                if (_selectedObjectItem != value)
                {
                    _selectedObjectItem = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<TestItem> ObjectItemsSource => StubedModel.GetItems().ToObservableCollection();
        
        public NativeComboBoxSamplePage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            SelectedObjectItem = null;
        }
    }
}
