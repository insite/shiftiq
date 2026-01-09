using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Assessments.Web.UI
{
    public partial class AssetTitleDisplay : Control
    {
        #region Classes

        private sealed class ControlHub
        {
            #region Properties

            public static ControlHub Current
            {
                get
                {
                    var key = typeof(ControlHub) + "." + nameof(Current);
                    var value = (ControlHub)HttpContext.Current.Items[key];

                    if (value == null)
                        HttpContext.Current.Items[key] = value = new ControlHub();

                    return value;
                }
            }

            #endregion

            #region Fields

            private bool _isInited = false;
            private List<AssetTitleDisplay> _controls = new List<AssetTitleDisplay>();
            private Dictionary<MultiKey<Guid, int>, SnippetBuilder.StandardModel> _numberIndex = null;
            private Dictionary<Guid, SnippetBuilder.StandardModel> _idIndex = null;

            #endregion

            #region Construction

            private ControlHub()
            {

            }

            #endregion

            #region Methods

            public void Register(AssetTitleDisplay ctrl)
            {
                if (!_isInited)
                    _controls.Add(ctrl);
            }

            public SnippetBuilder.StandardModel GetAsset(AssetTitleDisplay ctrl)
            {
                if (ctrl.AssetID != Guid.Empty)
                    return GetAsset(ctrl.AssetID);
                else if (ctrl.AssetOrganization != Guid.Empty && ctrl.AssetNumber > 0)
                    return GetAsset(ctrl.AssetOrganization, ctrl.AssetNumber);
                else
                    return null;
            }

            public SnippetBuilder.StandardModel GetAsset(Guid id)
            {
                if (!_isInited)
                    Init();

                if (!_idIndex.TryGetValue(id, out var info))
                    info = LoadAssets(new HashSet<Guid> { id }, null).FirstOrDefault();

                return info;
            }

            public SnippetBuilder.StandardModel GetAsset(Guid organization, int number)
            {
                if (!_isInited)
                    Init();

                var key = new MultiKey<Guid, int>(organization, number);

                if (!_numberIndex.TryGetValue(key, out var info))
                    info = LoadAssets(null, new Dictionary<Guid, HashSet<int>> { { organization, new HashSet<int> { number } } }).FirstOrDefault();

                return info;
            }

            private void Init()
            {
                var thumbprintFilter = new HashSet<Guid>();
                var numberFilter = new Dictionary<Guid, HashSet<int>>();

                foreach (var ctrl in _controls)
                {
                    if (ctrl.AssetID != Guid.Empty)
                    {
                        if (!thumbprintFilter.Contains(ctrl.AssetID))
                            thumbprintFilter.Add(ctrl.AssetID);
                    }
                    else if (ctrl.AssetOrganization != Guid.Empty && ctrl.AssetNumber > 0)
                    {
                        if (!numberFilter.TryGetValue(ctrl.AssetOrganization, out var numbers))
                            numberFilter.Add(ctrl.AssetOrganization, numbers = new HashSet<int>());

                        if (!numbers.Contains(ctrl.AssetNumber))
                            numbers.Add(ctrl.AssetNumber);
                    }
                }

                _numberIndex = new Dictionary<MultiKey<Guid, int>, SnippetBuilder.StandardModel>();
                _idIndex = new Dictionary<Guid, SnippetBuilder.StandardModel>();

                LoadAssets(thumbprintFilter, numberFilter);

                _isInited = true;
            }

            private IEnumerable<SnippetBuilder.StandardModel> LoadAssets(HashSet<Guid> thumbprintFilter, Dictionary<Guid, HashSet<int>> numberFilter)
            {
                var standards = new List<SnippetBuilder.StandardModel>();

                {
                    var queryFilter = LinqExtensions1.Expr((Standard x) => false);

                    if (thumbprintFilter.IsNotEmpty())
                        queryFilter = queryFilter.Or(x => thumbprintFilter.Contains(x.StandardIdentifier));

                    if (numberFilter.IsNotEmpty())
                    {
                        foreach (var g in numberFilter)
                            queryFilter = queryFilter.Or(x => x.Organization.OrganizationIdentifier == g.Key && g.Value.Contains(x.AssetNumber));
                    }

                    var list = StandardSearch.Bind(
                        x => new
                        {
                            Child = new SnippetBuilder.StandardModel
                            {
                                Identifier = x.StandardIdentifier,
                                Organization = x.Organization.OrganizationIdentifier,
                                Label = x.StandardLabel,
                                Type = x.StandardType,
                                Name = x.ContentName,
                                Number = x.AssetNumber,
                                Title = x.ContentTitle,
                                Code = x.Code
                            },
                            Parent = x.Parent == null ? null : new SnippetBuilder.StandardModel
                            {
                                Identifier = x.Parent.StandardIdentifier,
                                Organization = x.Parent.OrganizationIdentifier,
                                Label = x.Parent.StandardLabel,
                                Type = x.Parent.StandardType,
                                Name = x.Parent.ContentName,
                                Number = x.Parent.AssetNumber,
                                Title = x.Parent.ContentTitle,
                                Code = x.Parent.Code
                            }
                        },
                        queryFilter.Expand());

                    foreach (var item in list)
                    {
                        if (item.Parent != null)
                            item.Child.Parent = item.Parent;

                        standards.Add(item.Child);
                    }
                }

                foreach (var standard in standards)
                {
                    _idIndex.Add(standard.Identifier, standard);
                    _numberIndex.Add(new MultiKey<Guid, int>(standard.Organization, standard.Number), standard);
                }

                return standards;
            }

            #endregion
        }

        #endregion

        #region Properties

        private Guid _assetID;
        public Guid AssetID
        {
            get => _assetID;
            set
            {
                _assetID = value;
                InitOutput(true);
            }
        }

        private bool _showLink = true;
        public bool ShowLink
        {
            get => _showLink;
            set
            {
                _showLink = value;
                InitOutput(true);
            }
        }

        private bool _showParent = true;
        public bool ShowParent
        {
            get => _showParent;
            set
            {
                _showParent = value;
                InitOutput(true);
            }
        }

        private Guid _assetOrganization;
        public Guid AssetOrganization
        {
            get => _assetOrganization;
            set
            {
                _assetOrganization = value;
                InitOutput(true);
            }
        }

        private int _assetNumber;
        public int AssetNumber
        {
            get => _assetNumber;
            set
            {
                _assetNumber = value;
                InitOutput(true);
            }
        }

        private string _format;
        public string Format
        {
            get => _format;
            set
            {
                _format = value;
                InitOutput(true);
            }
        }

        private string Output
        {
            get => (string)ViewState[nameof(Output)];
            set => ViewState[nameof(Output)] = value;
        }

        #endregion

        #region Construction

        public AssetTitleDisplay()
        {
            ControlHub.Current.Register(this);
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Page.PreRender += Page_PreRender;
        }

        private void Page_PreRender(object sender, EventArgs e)
        {
            InitOutput(false);
        }

        private void InitOutput(bool reset)
        {
            if (reset)
                Output = null;

            if (Output != null)
                return;

            var asset = AssetID == Guid.Empty && (AssetNumber == 0 || AssetOrganization == Guid.Empty)
                ? null
                : ControlHub.Current.GetAsset(this);

            if (asset == null)
            {
                Output = "None";
                return;
            }

            var title = SnippetBuilder.GetHtml(asset, true, ShowParent, true, ShowLink, asset.Identifier);

            Output = string.IsNullOrEmpty(Format) ? title : string.Format(Format, title);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (Visible && !string.IsNullOrEmpty(Output))
                writer.Write(Output);
        }

        #endregion
    }
}