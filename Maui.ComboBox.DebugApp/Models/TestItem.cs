namespace Maui.ComboBox.DebugApp.Models
{
    public class TestItem()
    {
        public required string Text { get; set; }
        public required string Value { get; set; }
        public override string ToString() => Text;

        public override bool Equals(object? obj)
        {
            if (obj is TestItem toCompare)
                return Text == toCompare.Text && Value == toCompare.Value;
            else
                return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
