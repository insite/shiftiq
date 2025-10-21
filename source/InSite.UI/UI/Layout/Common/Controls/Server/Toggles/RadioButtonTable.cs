using System;
using System.Collections.Generic;
using System.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class RadioButtonTable : System.Web.UI.Control
    {
        public bool AutoPostBack
        {
            get { return ViewState[nameof(AutoPostBack)] != null && (bool)ViewState[nameof(AutoPostBack)]; }
            set { ViewState[nameof(AutoPostBack)] = value; }
        }

        public bool DisableColumnHeadingWrap
        {
            get { return (bool?)ViewState[nameof(DisableColumnHeadingWrap)] ?? false; }
            set { ViewState[nameof(DisableColumnHeadingWrap)] = value; }
        }

        public event EventHandler CheckedChanged;

        public List<ListItemArray> Rows { get; set; }
        public List<string> Titles { get; set; }

        public RadioButtonTable()
        {
            Rows = new List<ListItemArray>();
            Titles = new List<string>();
        }

        public void AddRow(string title, ListItemArray row)
        {
            Titles.Add(title);
            Rows.Add(row);
        }

        public override void DataBind()
        {
            for (var i = 0; i < Rows.Count; i++)
            {
                var row = Rows[i];

                for (var j = 0; j < row.Items.Count; j++)
                {
                    var item = row.Items[j];
                    var radio = new RadioButton
                    {
                        GroupName = $"Table_{ID}_Row_{i}",
                        ID = $"Table_{ID}_Row_{i}_Column_{j}",
                        Text = item.Text,
                        Value = item.Value,
                        Checked = item.Selected,
                        RenderMode = CheckBoxRenderMode.Input
                    };

                    if (AutoPostBack)
                    {
                        radio.AutoPostBack = true;
                        radio.CheckedChanged += CheckedChanged;
                    }

                    Controls.Add(radio);
                }
            }
        }

        protected override void RenderChildren(HtmlTextWriter output)
        {
            if (HasControls())
            {
                output.WriteLine("<table id='{0}' class='table table-bordered table-striped'>", ClientID);
                output.WriteLine("<tr>");
                output.WriteLine("<th></th>");
                foreach (ListItem column in Rows[0].Items)
                {
                    output.Write("<th class='text-center");
                    if (DisableColumnHeadingWrap)
                        output.Write(" text-nowrap");
                    output.Write("'>");
                    output.Write(column.Text);
                    output.WriteLine("</th>");
                }
                output.WriteLine("</tr>");

                for (var i = 0; i < Rows.Count; i++)
                {
                    var row = Rows[i];
                    output.WriteLine("<tr>");
                    output.WriteLine($"<th>{Titles[i]}</th>");
                    for (var j = 0; j < row.Items.Count; j++)
                    {
                        output.WriteLine("<td class='text-center likert-scale-col'>");
                        var id = $"Table_{ID}_Row_{i}_Column_{j}";
                        var radio = FindControl(id);
                        radio.RenderControl(output);
                        output.WriteLine("</td>");
                    }
                    output.WriteLine("</tr>");
                }
                output.WriteLine("</table>");
            }
        }
    }
}