using System;
using System.Web.UI;

using Shift.Constant;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public sealed class Flag : Control
    {
        public string ToolTip
        {
            get { return (string)ViewState[nameof(ToolTip)]; }
            set { ViewState[nameof(ToolTip)] = value; }
        }

        public CmdsFlagType Type
        {
            get { return (CmdsFlagType)(ViewState[nameof(Type)] ?? CmdsFlagType.Red); }
            set { ViewState[nameof(Type)] = value; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!Visible)
                return;

            string cssClass;

            switch (Type)
            {
                case CmdsFlagType.Red:
                    cssClass = "red text-danger fas fa-flag";
                    break;
                case CmdsFlagType.Green:
                    cssClass = "green text-success fas fa-flag-checkered";
                    break;
                case CmdsFlagType.Yellow:
                    cssClass = "yellow text-warning fas fa-flag";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown flag type: " + Type);
            }

            writer.Write("<i class=\"flag {0}\" title=\"{1}\"></i>", cssClass, ToolTip);
        }
    }
}