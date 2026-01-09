namespace Shift.Sdk.UI
{
    public interface IComboBoxOption
    {
        string Text { get; set; }
        string Value { get; set; }
        bool Selected { get; set; }
        bool AllowSelect { get; }
        bool Enabled { get; set; }
        bool Visible { get; set; }
    }
}
