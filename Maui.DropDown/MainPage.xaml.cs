using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace Maui.DropDown;

public partial class MainPage : ContentPage {
    public MainPage() {
        InitializeComponent();
        BindingContext = this;
    }
    
    public string SelectedStringItem { get; set; } = "Item 1";
    public ObservableCollection<object> StringItems => ["Item 1", "Item 2", "Item 3", "Number 4", "Number 5", "Control", "Kaos", "Spectre"];

    public DropDownItem? SelectedObjectItem { get; set; }
    public ObservableCollection<DropDownItem> ObjectItems => [
        new() { Text = "Alpha", Value = "Item 1" },
        new() { Text = "Beta", Value = "Item 2" },
        new() { Text = "Charlie", Value = "Item 3" },
        new() { Text = "Delta", Value = "Item 4" },
        new() { Text = "Echo", Value = "Item 5" },
        new() { Text = "Foxtrot", Value = "Item 6" },
        new() { Text = "Gamma", Value = "Item 7" },
        new() { Text = "Hotel", Value = "Item 8" },
        new() { Text = "India", Value = "Item 9" },
        new() { Text = "Juliett", Value = "Item 10" },
        new() { Text = "Kilo", Value = "Item 11" },
        new() { Text = "Lima", Value = "Item 12" },
        new() { Text = "Mike", Value = "Item 13" },
        new() { Text = "November", Value = "Item 14" },
        new() { Text = "Oscar", Value = "Item 15" },
        new() { Text = "Papa", Value = "Item 16" },
        new() { Text = "Quebec", Value = "Item 17" },
        new() { Text = "Romeo", Value = "Item 18" },
        new() { Text = "Sierra", Value = "Item 19" },
        new() { Text = "Tango", Value = "Item 20" },
        new() { Text = "Uniform", Value = "Item 21" },
        new() { Text = "Victor", Value = "Item 22" },
        new() { Text = "Whisky", Value = "Item 23" },
        new() { Text = "Xray", Value = "Item 24" },
        new() { Text = "Yes", Value = "Item 25" },
        new() { Text = "Zulu", Value = "Item 26" }
    ];
}

public class DropDownItem() {
    public required string Text { get; set; }
    public required string Value { get; set; }
}