namespace InSite.Common.Web.UI
{
    public interface ICanHide
    {
        /// <summary>
        /// Gets or sets a value indicating whether the control is hidden from view
        /// </summary>
        /// <value>
        /// <c>true</c> if the control is hidden; otherwise, <c>false</c>. Default is <c>false</c>.
        /// </value>
        /// <remarks>
        /// <para>
        /// This property provides reliable visibility control that persists across postbacks using ViewState and is not 
        /// affected by parent control hierarchy cascading behavior. In a scenario where you need to set the visibility 
        /// of a child control independent of the visibility of its parent control, you can use this instead of the 
        /// default Visible property. (For example, this may be useful for implementing visibility rules based on 
        /// business logic.)
        /// </para>
        /// <para>
        /// <strong>Rationale:</strong> When a parent control is hidden by setting Visible = false, all child controls 
        /// automatically return false for their Visible property getter, regardless of their individual visibility 
        /// settings. This makes it impossible to reliably determine a child control's intended visibility state when 
        /// applying business rules "bottom-up" (i.e., making a control visible only if it contains visible child 
        /// controls).
        /// </para>
        /// <para>
        /// <strong>Example:</strong> If you set ChildControl.Visible = true but ParentControl.Visible = false, then 
        /// ChildControl.Visible getter will return false, losing the original intended state of the child control.
        /// </para>
        /// <para>
        /// <strong>Solution:</strong> In a scenario that requires a "bottom-up" implementation for visibility, use a
        /// separate property (IsHidden) instead, which maintains independent state for each control without hierarchy 
        /// interference.
        /// </para>
        /// </remarks>
        bool IsHidden { get; set; }

        /*
         * Here is an implementation example. Notice current state is preserved across postbacks using ViewState. If 
         * ViewState is disabled for this control, the property will reset to its default value (<c>false</c>) on each 
         * postback.
         * 
        public bool IsHidden
        {
            get => ViewState[nameof(IsHidden)] as bool? ?? false;
            set => ViewState[nameof(IsHidden)] = value;
        }
        */
    }
}