using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web.UI;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    [ParseChildren(true), PersistChildren(true)]
    public class ResourceBundle : Control
    {
        #region Properties

        public ResourceLink.ResourceType Type
        {
            get;
            set;
        }

        public bool EnableBundling
        {
            get => _enableBundling;
            set => _enableBundling = value;
        }

        public bool EnableDebug
        {
            get => _enableDebug;
            set => _enableDebug = value;
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Collection<ResourceBundleItem> Items => _items;

        #endregion

        #region Fields

        private readonly Collection<ResourceBundleItem> _items;

        private IEnumerable<ResourceBundleItem> _bundleFiles;
        private bool _enableBundling;
        private bool _enableDebug;

        private static readonly List<Tuple<string[], string[]>> _bundles;
        private static readonly ConcurrentDictionary<string, string[]> _bundleMapping;
        private static readonly string _query;
        private static readonly bool _enableBundlingDefault;
        private static readonly bool _enableDebugDefault;

        #endregion

        #region Construction

        static ResourceBundle()
        {
            var configValue = ServiceLocator.AppSettings.Application.ResourceBundle;
            var isLocalEnv = ServiceLocator.AppSettings.Environment.Name == EnvironmentName.Local;

            _bundles = new List<Tuple<string[], string[]>>();
            _bundleMapping = new ConcurrentDictionary<string, string[]>();

            _enableDebugDefault = configValue == "Debug";
            _enableBundlingDefault = !_enableDebugDefault && configValue != "Disabled"
                && (!isLocalEnv || configValue == "Enabled");
            _query = isLocalEnv
                ? "_=" + DateTime.UtcNow.Ticks
                : "v=" + ServiceLocator.AppSettings.Release.Version;
        }

        public ResourceBundle()
        {
            _enableBundling = _enableBundlingDefault;
            _enableDebug = _enableDebugDefault;
            _items = new Collection<ResourceBundleItem>();
        }

        #endregion

        #region Loading

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Type == ResourceLink.ResourceType.None || !EnableBundling || Items.Count == 0)
                return;

            var key = GetMappingKey();

            _bundleFiles = _bundleMapping
                .GetOrAdd(key, _ => CreateBundle())
                .Select(x => new ResourceBundleFile { Url = x })
                .ToArray()
                .NullIfEmpty();
        }

        private string[] CreateBundle()
        {
            var cNames = new HashSet<string>();
            var cContents = new List<ResourceHelper.BundleContent>();

            foreach (IResourceBundleItem item in Items)
            {
                var bundle = item.GetBundleContent();
                if (bundle.Name.IsNotEmpty() && cNames.Add(bundle.Name))
                    cContents.Add(bundle);
            }

            Tuple<string[], string[]> result = null;

            lock (_bundles)
            {
                for (var i = 0; i < _bundles.Count; i++)
                {
                    var bundle = _bundles[i];
                    var bNames = bundle.Item1;

                    if (bNames.Length != cContents.Count)
                        continue;

                    result = bundle;

                    for (var j = 0; j < bNames.Length; j++)
                    {
                        if (bNames[j] != cContents[j].Name)
                        {
                            result = null;
                            break;
                        }
                    }

                    if (result != null)
                        break;
                }

                if (result == null)
                {
                    var urls = ResourceHelper.CreateBundle(Type, cContents);

                    _bundles.Add(result = new Tuple<string[], string[]>(cContents.Select(x => x.Name).ToArray(), urls));
                }
            }

            return result.Item2;
        }

        #endregion

        #region Render

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (!Visible)
                return;

            var items = _bundleFiles ?? _items;
            var query = _enableDebug ? "_=" + DateTime.UtcNow.Ticks : _query;

            if (Type == ResourceLink.ResourceType.Css)
            {
                foreach (IResourceBundleItem item in items)
                    item.WriteCss(writer, _query);
            }
            else
            {
                foreach (IResourceBundleItem item in items)
                    item.WriteJs(writer, _query);
            }
        }

        #endregion

        #region Helpers

        private string GetMappingKey()
        {
            var result = new StringBuilder();

            result.Append("[").Append(ClientID).Append("]");

            Control container = this;
            while ((container = container.NamingContainer) != null)
                result.Append(".[").Append(container.GetType().Name).Append("]");

            return result.ToString();
        }

        #endregion
    }
}