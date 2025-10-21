using System;

namespace InSite.Persistence
{
    [Serializable]
    public class ControlModel
    {
        public int Id { get; set; }
        
        public string Description { get; set; }
        public bool IsList { get; set; }
        public bool IsQualitative { get; set; }
        public bool IsQuantitative { get; set; }
        public string Name { get; set; }
        public string Scope { get; set; }
        public int Sequence { get; set; }
        public string Title { get; set; }

        public ControlModel Clone()
        {
            return (ControlModel)MemberwiseClone();
        }
    }
}