using System;

namespace InSite.Domain.Surveys.Sessions
{
    /// <summary>
    /// A survey response option indicates the items selected (and not selected) by the respondent.
    /// </summary>
    [Serializable]
    public class ResponseOption
    {
        /// <summary>
        /// The option item that may be selected or unselected.
        /// </summary>
        public Guid Item { get; set; }

        /// <summary>
        /// True if the item is selected; False if the item is not selected.
        /// </summary>
        public bool IsSelected { get; set; }

        #region Construction

        public ResponseOption() { }

        public ResponseOption(Guid item)
        {
            Item = item;
        }

        #endregion
    }
}
