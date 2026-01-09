using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Web.Infrastructure;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public static class ControlHelper
    {
        #region Classes

        public class SectionSubtitleInfo
        {
            public string Text { get; set; }
            public int? Count { get; set; }
        }

        public class TabSubtitleCollection : IEnumerable<SectionSubtitleInfo>
        {
            #region Properties

            public int Count => _list.Count;

            #endregion

            #region Fields

            private Func<SectionSubtitleInfo, bool> _filter;
            private List<SectionSubtitleInfo> _list;

            #endregion

            #region Construction

            public TabSubtitleCollection()
            {
                _filter = null;
                _list = new List<SectionSubtitleInfo>();
            }

            public TabSubtitleCollection(Func<SectionSubtitleInfo, bool> filter)
                : this()
            {
                _filter = filter;
            }

            #endregion

            public void Add(string text, int? count)
            {
                if (string.IsNullOrEmpty(text) && !count.HasValue)
                    return;

                var item = new SectionSubtitleInfo
                {
                    Text = text,
                    Count = count
                };

                if (_filter != null && !_filter(item))
                    return;

                _list.Add(item);
            }

            #region IEnumerator

            public IEnumerator<SectionSubtitleInfo> GetEnumerator() => _list.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion
        }

        #endregion

        #region Properties

        private static Domain.Foundations.User User => CurrentSessionState.Identity.User;

        #endregion

        #region Fields

        private static readonly FieldInfo _controlFlagsFieldInfo;
        private static readonly PropertyInfo _controlFlagsItemPropertyInfo;
        private static readonly object[] _controlFlagsVisibleBit = new object[] { 0x10 };

        #endregion

        #region Construction

        static ControlHelper()
        {
            _controlFlagsFieldInfo = typeof(Control)
                .GetField("flags", BindingFlags.Instance | BindingFlags.NonPublic);

            using (var dummyControl = new Control())
            {
                var dummyFlags = _controlFlagsFieldInfo.GetValue(dummyControl);
                _controlFlagsItemPropertyInfo = dummyFlags.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(x => x.Name == "Item" && x.PropertyType == typeof(bool))
                    .Single();
            }
        }

        #endregion

        #region Methods (control searching)

        /// <summary>
        /// Use with caution, for now it works only when one of this is true:
        /// - 'container' is a NamingContainer for the 'id' control
        /// - 'id' control is located on the ASPX but not on ASCX
        /// 
        /// Improve when it will be needed or try to use GetControl method instead
        /// </summary>
        public static Control GetControlSafe(Control container, string id)
        {
            var control = container.FindControl(id);
            if (control != null)
                return control;

            var type = container.Page.GetType();
            var field = type.GetField(id, BindingFlags.Instance | BindingFlags.NonPublic);

            return field?.GetValue(container.Page) as Control;
        }

        public static Control GetControl(Control container, string id)
        {
            var foundControl = container.FindControl(id);

            if (foundControl != null && StringHelper.Equals(foundControl.ID, id))
                return foundControl;

            foreach (Control c in container.Controls)
            {
                foundControl = GetControl(c, id);

                if (foundControl != null && StringHelper.Equals(foundControl.ID, id))
                    return foundControl;
            }

            return null;
        }

        public static Control GetControl(Control container, string id, params string[] excludes)
        {
            var result = container.FindControl(id);

            if (result == null)
            {
                if (excludes.IsNotEmpty())
                {
                    var excludeIndex = new HashSet<string>(excludes);

                    return GetControlInternal(container, id, c => excludeIndex.Contains(c.ID));
                }
                else
                {
                    return GetControlInternal(container, id, c => false);
                }
            }

            return result;
        }

        public static Control GetControl(Control container, string id, params Control[] excludes)
        {
            var result = container.FindControl(id);

            if (result == null)
            {
                if (excludes.IsNotEmpty())
                {
                    var excludeIndex = new HashSet<Control>(excludes);

                    return GetControlInternal(container, id, c => excludeIndex.Contains(c));
                }
                else
                {
                    return GetControlInternal(container, id, c => false);
                }
            }

            return result;
        }

        private static Control GetControlInternal(Control container, string id, Func<Control, bool> isExclude)
        {
            Control result = null;

            for (var i = 0; result == null && i < container.Controls.Count; i++)
            {
                var c = container.Controls[i];

                if (c.Controls.Count == 0 || isExclude(c))
                    continue;

                if (c is INamingContainer)
                {
                    result = c.FindControl(id);
                    if (result != null && result != c)
                        break;
                }

                result = GetControlInternal(c, id, isExclude);
            }

            return result;
        }

        #endregion

        #region Methods (JS/CSS helpers)

        public static string MergeCssClasses(params string[] classes)
        {
            return string.Join(
                " ",
                classes
                    .Where(x => x != null)
                    .SelectMany(x => x.Split(' '))
                    .Where(x => x.IsNotEmpty())
                    .Distinct(StringComparer.OrdinalIgnoreCase));
        }

        public static bool ContainCssClass(string sourceClass, string findClass) => ContainCssClass(sourceClass, findClass, out _);

        public static bool ContainCssClass(string sourceClass, string findClass, out int startIndex)
        {
            startIndex = -1;

            if (sourceClass.IsEmpty() || findClass.IsEmpty())
                return false;

            var index = sourceClass.IndexOf(findClass);
            var lastIndex = sourceClass.Length - 1;
            var findLength = findClass.Length;

            while (index != -1)
            {
                var endIndex = index + findLength;

                if ((index == 0 || sourceClass[index - 1] == ' ') && (endIndex - 1 == lastIndex || sourceClass[endIndex] == ' '))
                {
                    startIndex = index;
                    return true;
                }

                index = sourceClass.IndexOf(findClass, index + 1);
            }

            return false;
        }

        public static string AddCssClass(string sourceClass, string newClass)
        {
            newClass = newClass.EmptyIfNull().Trim();
            if (newClass.Length == 0)
                return sourceClass.EmptyIfNull();

            if (sourceClass.IsEmpty())
                return newClass;

            if (ContainCssClass(sourceClass, newClass))
                return sourceClass;

            return sourceClass[sourceClass.Length - 1] == ' '
                ? sourceClass + newClass
                : sourceClass + ' ' + newClass;
        }

        public static string RemoveCssClass(string sourceClass, string removeClass)
        {
            removeClass = removeClass.EmptyIfNull().Trim();
            if (removeClass.Length == 0)
                return sourceClass.EmptyIfNull();

            int index, removeLen = removeClass.Length;

            while (ContainCssClass(sourceClass, removeClass, out index))
            {
                sourceClass = index > 0
                    ? sourceClass.Remove(index - 1, removeLen + 1)
                    : sourceClass.Remove(index, removeLen);
            }

            return sourceClass;
        }

        public static string MergeScripts(params string[] scripts)
        {
            var result = new StringBuilder();

            foreach (var script in scripts)
                AddScript(result, script);

            return result.ToString();
        }

        public static void AddScript(StringBuilder sb, string script)
        {
            if (string.IsNullOrEmpty(script))
                return;

            script = script.Trim();
            if (!script.EndsWith(";") && !script.EndsWith("}"))
                script += ";";

            if (sb.Length > 0)
                sb.Append(" ");

            sb.Append(script);
        }

        #endregion

        #region Methods (tab title)

        public static void SetTitle(this NavItem tab, string title, int count)
        {
            tab.IsTitleLocalizable = false;
            tab.Title = BuildTabTitle(title, count > 0 ? count.ToString("n0") : null);
        }

        public static void SetTitle(this NavItem tab, string title, string subtitle)
        {
            tab.IsTitleLocalizable = false;
            tab.Title = BuildTabTitle(title, subtitle);
        }

        public static void SetTitle(this NavItem tab, string title, TabSubtitleCollection subtitles, bool humanize = true)
        {
            tab.IsTitleLocalizable = false;
            tab.Title = BuildTabTitle(title, subtitles, humanize);
        }

        public static string BuildTabTitle(string title, string subtitle)
        {
            if (string.IsNullOrEmpty(title))
                return string.Empty;

            var sb = new StringBuilder();

            sb.Append(title);

            if (subtitle.IsNotEmpty())
                sb.Append(" <small class='text-body-secondary ms-1'>(").Append(subtitle).Append(")</small>");

            return sb.ToString();
        }

        public static string BuildTabTitle(string title, TabSubtitleCollection subtitles, bool humanize = true)
        {
            if (string.IsNullOrEmpty(title))
                return string.Empty;

            var sb = new StringBuilder(title);

            if (subtitles.Count > 0)
            {
                sb.Append(" <small class='text-body-secondary ms-1'>(");

                var isFirstItem = true;

                foreach (var subtitle in subtitles)
                {
                    if (isFirstItem)
                        isFirstItem = false;
                    else
                        sb.Append(", ");

                    if (!subtitle.Count.HasValue)
                        sb.Append(subtitle.Text);
                    else if (string.IsNullOrEmpty(subtitle.Text))
                        sb.Append(subtitle.Count.Value.ToString("n0"));
                    else if (humanize)
                        sb.AppendFormat(subtitle.Text.ToQuantity(subtitle.Count.Value, "N0"));
                    else
                        sb.AppendFormat("{0:N0} {1}", subtitle.Count.Value, subtitle.Text);
                }

                sb.Append(")</small>");
            }

            return sb.ToString();
        }

        #endregion

        #region Methods (other)

        public static bool IsContentItem(GridViewRowEventArgs e)
            => IsContentItem(e.Row);

        public static bool IsContentItem(GridViewRow row)
            => row.RowType == DataControlRowType.DataRow;

        public static bool IsContentItem(RepeaterItemEventArgs e)
            => IsContentItem(e.Item);

        public static bool IsContentItem(RepeaterItem item)
            => item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem;

        public static string FormatDate(object date)
        {
            return date != null && date is DateTime
                ? TimeZones.Format((DateTime)date)
                : string.Empty;
        }

        public static string LocalizeDate(object time, bool shortFormat = true)
        {
            if (!(time is DateTimeOffset dto) || dto == Shift.Common.Calendar.UnknownUtc)
                return string.Empty;

            return shortFormat
                ? dto.FormatDateOnly(User.TimeZone, null, "{0:MMM d, yyyy}")
                : dto.FormatDateOnly(User.TimeZone);
        }

        public static string LocalizeTime(object time, string element = null, bool isHtml = true)
        {
            if (time == null)
                return string.Empty;

            if (time is DateTimeOffset dto)
            {
                return dto == Shift.Common.Calendar.UnknownUtc
                    ? string.Empty
                    : element != null
                        ? $"{dto.Format(User.TimeZone, isHtml)} <{element} class='fs-sm text-body-secondary small-print'>{dto.Humanize()}</{element}>"
                        : dto.Format(User.TimeZone, isHtml, false);
            }
            else if (time is DateTime dt)
            {
                return dt.Format();
            }

            return string.Empty;
        }

        public static void SendZipFile(HttpResponse response, byte[] data, string fileName, string ext)
        {
            var zipPath = string.Empty;
            var storageId = TempFileStorage.Create();

            TempFileStorage.Open(storageId, dir =>
            {
                zipPath = Path.Combine(dir.FullName, "archive.bin");

                using (var fileStream = System.IO.File.Open(zipPath, FileMode.Create, FileAccess.Write))
                {
                    using (var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Create))
                    {
                        var entry = zipArchive.CreateEntry(fileName + "." + ext);
                        using (var entryStream = entry.Open())
                        {
                            entryStream.Write(data, 0, data.Length);
                        }
                    }
                }
            });

            response.SendFile(fileName + ".zip", zipPath);
        }

        public static bool GetInternalVisibleState(Control control)
        {
            var flags = _controlFlagsFieldInfo.GetValue(control);
            var bitValue = (bool)_controlFlagsItemPropertyInfo.GetValue(flags, _controlFlagsVisibleBit);
            return !bitValue;
        }

        #endregion
    }
}