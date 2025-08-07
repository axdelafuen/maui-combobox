namespace Maui.ComboBox.DebugApp.Models
{
    public class TestItem()
    {
        public required string Text { get; set; }
        public required string Value { get; set; }
        public override string ToString() => Text;
    }
}
