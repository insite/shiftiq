using System;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// The type and number for a discrete skill level. For example, Certificate of Qualification Level 2.
    /// </summary>
    [Serializable]
    public class Level
    {
        public string Type { get; set; }
        public int? Number { get; set; }

        public override string ToString()
        {
            string s = null;
            if (!string.IsNullOrEmpty(Type))
                s = Number.HasValue ? $"{Type} Level {Number}" : s = Type;
            else if (Number.HasValue)
                s = $"Level {Number}";
            return s;
        }

        public void Copy(Level source)
        {
            Type = source.Type;
            Number = source.Number;
        }

        public Level Clone()
        {
            var clone = new Level();
            clone.Copy(this);
            return clone;
        }
    }
}
