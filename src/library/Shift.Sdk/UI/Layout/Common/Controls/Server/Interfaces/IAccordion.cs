namespace Shift.Sdk.UI
{
    public interface IAccordion
    {
        int SelectedIndex { get; set; }

        int GetIndex(IAccordionPanel panel);
    }

    public interface IAccordionPanel
    {
        bool Visible { get; set; }
    }
}
