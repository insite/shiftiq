using System;

namespace Shift.Constant
{
    [AttributeUsage(AttributeTargets.All)]
    public class IconAttribute : Attribute
    {
        #region Constants

        public static readonly IconAttribute Default = new IconAttribute();

        #endregion

        #region Properties

        public string Name
        {
            get;
            private set;
        }

        #endregion

        #region Construction

        public IconAttribute()
            : this(string.Empty)
        {
        }

        public IconAttribute(string name)
        {
            Name = name;
        }

        #endregion

        #region Methods (comparings)

        public override bool Equals(object obj)
        {
            return obj == this || obj is IconAttribute attr && attr.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion

        #region Methods (overrides)

        public override bool IsDefaultAttribute()
        {
            return Equals(Default);
        }

        #endregion
    }
}
