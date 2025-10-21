using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.UI;

using Shift.Common;
namespace InSite.Common.Web.UI
{
    [ParseChildren(false), PersistChildren(false)]
    [SuppressMessage("NDepend", "ND3101:DontUseSystemRandomForSecurityPurposes", Scope = "method", Justification = "Random number generation is not security-sensitive here, therefore weak psuedo-random numbers are permitted.")]
    public class DynamicControl : Control, INamingContainer
    {
        public event EventHandler ControlAdded;

        private void OnControlAdded() => ControlAdded?.Invoke(this, EventArgs.Empty);

        private Type ControlType
        {
            get => (Type)ViewState[nameof(ControlType)];
            set => ViewState[nameof(ControlType)] = value;
        }

        private string ControlPath
        {
            get => (string)ViewState[nameof(ControlPath)];
            set => ViewState[nameof(ControlPath)] = value;
        }

        private string ControlID
        {
            get => (string)ViewState[nameof(ControlID)];
            set => ViewState[nameof(ControlID)] = value;
        }

        private Random Random => (Random)(Context.Items[_randomKey] ?? (Context.Items[_randomKey] = new Random()));
        private static readonly string _randomKey = typeof(DynamicControl).FullName + "." + nameof(Random);

        private Control _ctrl;

        public DynamicControl()
        {
            ViewState.SetDirty(true);
        }

        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();

            base.OnLoad(e);
        }

        protected override void CreateChildControls()
        {
            AddControl();

            base.CreateChildControls();
        }

        public Control LoadControl(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            UnloadControl();

            ControlPath = path;
            ControlID = RandomStringGenerator.Create(Random, 8);

            return AddControl();
        }

        public T LoadControl<T>() where T : Control
        {
            UnloadControl();

            ControlType = typeof(T);
            ControlID = RandomStringGenerator.Create(Random, 8);

            return (T)AddControl();
        }

        public void UnloadControl()
        {
            ControlPath = null;
            ControlType = null;
            ControlID = null;

            Controls.Clear();

            _ctrl = null;
        }

        private Control AddControl()
        {
            Control result = null;

            if (_ctrl != null)
                return result;

            result = _ctrl = !string.IsNullOrEmpty(ControlPath)
                ? Page.LoadControl(ControlPath)
                : ControlType != null
                    ? (Control)Activator.CreateInstance(ControlType)
                    : null;

            if (result != null)
            {
                result.ID = ControlID;

                Controls.Add(result);

                OnControlAdded();
            }

            return result;
        }

        public Control GetControl() => _ctrl;

        public bool HasControl() => _ctrl != null;
    }
}