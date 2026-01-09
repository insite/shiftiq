using System;

using Shift.Common;

namespace Shift.Sdk.UI
{
    [Serializable]
    public abstract class DropDownButtonBaseItem
    {
        #region Properties

        public string Name
        {
            get => _name;
            set => _name = !string.IsNullOrEmpty(value) ? StringHelper.RemoveNonAlphanumericCharacters(value) : null;
        }

        public bool Visible { get; set; }

        #endregion

        #region Fields

        private string _name;

        #endregion

        #region Construction

        public DropDownButtonBaseItem()
        {
            Visible = true;
        }

        #endregion

        #region Helper methods

        public DropDownButtonBaseItem Clone()
        {
            return (DropDownButtonBaseItem)MemberwiseClone();
        }

        #endregion
    }
}