using CommunityToolkit.Maui.Core.Extensions;
using Maui.ComboBox.DebugApp.Models;
using Maui.ComboBox.DebugApp.Stub;
using System.Collections.ObjectModel;

namespace Maui.ComboBox.DebugApp.SamplePages
{
    public partial class PopupComboBoxSamplePage : ContentPage
    {
        public TestItem? SelectedObjectItem { get; set; }

        public ObservableCollection<TestItem> ObjectItems => StubedModel.GetItems().ToObservableCollection();

        public string? SelectedStringItem { get; set; }

        public ObservableCollection<string> StringItems => StubedString.GetItems().ToObservableCollection();

        public PopupComboBoxSamplePage()
        {
            InitializeComponent();
            BindingContext = this;
        }
    }
}
