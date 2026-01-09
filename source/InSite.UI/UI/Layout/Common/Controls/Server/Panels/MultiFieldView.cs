using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class MultiFieldView : Control
    {
        #region Properties

        public bool IsActive
        {
            get => _parent != null ? _parent.IsActive(this) : _isActive == true;
            set
            {
                if (_parent != null)
                {
                    if (value)
                        _parent.SetView(this);
                }
                else
                    _isActive = value;
            }
        }

        [TypeConverter(typeof(StringArrayConverter))]
        public string[] Inputs
        {
            get => (string[])ViewState[nameof(Inputs)];
            set => ViewState[nameof(Inputs)] = value;
        }

        #endregion

        #region Fields

        private bool? _isActive;
        private MultiField _parent = null;

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack || !_parent.ManageValidators)
                return;

            var validators = FindValidators();
            if (validators.Length == 0)
                return;

            var isEnabled = IsActive;

            for (var i = 0; i < validators.Length; i++)
                validators[i].Enabled = isEnabled;
        }

        #endregion

        #region Render

        protected override void Render(HtmlTextWriter writer)
        {
            if (!Visible)
                return;

            var inputs = FindInputs();
            if (inputs.Length > 0)
                writer.AddAttribute("data-inputs", string.Join(",", inputs.Select(x => x.ClientID)));

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            base.Render(writer);

            writer.RenderEndTag();
        }

        internal void SetupParent(MultiField parent)
        {
            if (_parent != null)
                throw ApplicationError.Create("The multi field is already assigned to this view: {0}", UniqueID);

            _parent = parent;

            if (_isActive.HasValue)
                IsActive = _isActive.Value;
        }

        private Control[] FindInputs()
        {
            if (Inputs.IsEmpty())
                return new Control[0];

            var result = new List<Control>(Inputs.Length);

            foreach (var i in Inputs)
            {
                var ctrl = FindControl(i);
                if (ctrl == null)
                    throw ApplicationError.Create($"Input control not found: " + i);

                result.Add(ctrl);
            }

            return result.ToArray();
        }

        private BaseValidator[] FindValidators()
        {
            var inputs = FindInputs();
            if (inputs.Length == 0)
                return new BaseValidator[0];

            var result = new List<BaseValidator>();

            foreach (BaseValidator v in Page.Validators)
            {
                if (inputs.Any(x => x.ID == v.ControlToValidate))
                    result.Add(v);
            }

            return result.ToArray();
        }

        #endregion
    }
}