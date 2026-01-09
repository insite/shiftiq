using System;
using System.Collections.Generic;
using System.Web.UI;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Common.Controls.Editor
{
    public partial class SectionTiles : SectionBase
    {
        #region Classes

        [Serializable]
        private class TileInfo
        {
            public string ID { get; set; }
            public string Path { get; set; }
        }

        #endregion

        #region Properties

        private Dictionary<string, int> Identifiers
        {
            get => (Dictionary<string, int>)(ViewState[nameof(Identifiers)]
                ?? (ViewState[nameof(Identifiers)] = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)));
            set => ViewState[nameof(Identifiers)] = value;
        }

        private TileSize TileSize
        {
            get => (TileSize)ViewState[nameof(TileSize)];
            set => ViewState[nameof(TileSize)] = value;
        }

        private List<TileInfo> TileItems
        {
            get => (List<TileInfo>)(ViewState[nameof(TileItems)]
                ?? (ViewState[nameof(TileItems)] = new List<TileInfo>()));
            set => ViewState[nameof(TileItems)] = value;
        }

        #endregion

        #region Fields

        private List<SectionBase> _controls = new List<SectionBase>();

        #endregion

        #region Initialization

        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();

            base.OnLoad(e);
        }

        protected override void CreateChildControls()
        {
            if (Container.Controls.Count == 0 && TileItems.Count > 0)
            {
                for (var i = 0; i < TileItems.Count; i++)
                    AddTile(TileItems[i].Path, out _);
            }

            base.CreateChildControls();
        }

        #endregion

        #region Methods (set input values)

        public override void SetOptions(LayoutContentSection options)
        {
            if (options is LayoutContentSection.TileList tileList)
            {
                TileSize = tileList.Size;

                foreach (var tile in tileList.Options)
                {
                    AddTile(tile.Id, tile.ControlPath, out var tileCtrl);

                    tileCtrl.SetOptions(tile);
                }
            }
            else
            {
                throw new NotImplementedException("Section type: " + options.GetType().FullName);
            }
        }

        public override void SetValidationGroup(string groupName)
        {
            foreach (var tile in _controls)
                tile.SetValidationGroup(groupName);
        }

        public override void SetLanguage(string lang)
        {
            foreach (var ctrl in _controls)
                ctrl.SetLanguage(lang);
        }

        #endregion

        #region Methods (get input values)

        public override MultilingualString GetValue() =>
            throw new NotImplementedException();

        public override MultilingualString GetValue(string id)
        {
            if (!string.IsNullOrEmpty(id) && Identifiers.TryGetValue(id, out var index))
                return _controls[index].GetValue();
            else
                throw ApplicationError.Create("Section not found: " + id);
        }

        public override IEnumerable<MultilingualString> GetValues()
        {
            foreach (var tile in _controls)
                yield return tile.GetValue();
        }

        public override void GetValues(MultilingualDictionary dictionary) => throw new NotImplementedException();

        #endregion

        #region Methods (tile management)

        private void AddTile(string id, string tilePath, out SectionBase tile)
        {
            if (Identifiers.ContainsKey(id))
                throw ApplicationError.Create("Invalid tile ID: " + id);

            AddTile(tilePath, out tile);

            Identifiers.Add(id, TileItems.Count);
            TileItems.Add(new TileInfo
            {
                ID = id,
                Path = tilePath
            });
        }

        private void AddTile(string tilePath, out SectionBase tile)
        {
            var size = TileSize == TileSize.Full
                ? 12
                : TileSize == TileSize.Quarter
                    ? 3
                    : 6;

            Container.Controls.Add(new LiteralControl($"<div class='col-md-{size}'>"));
            Container.Controls.Add(tile = (SectionBase)LoadControl(tilePath));
            Container.Controls.Add(new LiteralControl("</div>"));

            _controls.Add(tile);
        }

        public override void OpenTab(string id) { }

        #endregion
    }
}