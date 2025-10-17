namespace InSite.Domain
{
    public class StateFieldMeta : IStateFieldMeta
    {
        public bool Required { get; set; }
        public StateFieldType FieldType { get; set; }
        public bool DirectlyModifiable { get; set; } = true;
    }

    public interface IStateFieldMeta
    {
        bool Required { get; }
        StateFieldType FieldType { get; }
        bool DirectlyModifiable { get; }
    }
}
