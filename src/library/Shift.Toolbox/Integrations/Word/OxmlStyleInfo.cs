namespace Shift.Toolbox
{
    public class OxmlStyleInfo
    {
        public string Id { get; }
        public string Name { get; }

        public OxmlStyleInfo(string id)
            : this(id, id)
        {

        }

        public OxmlStyleInfo(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
