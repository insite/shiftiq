<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchDownload.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.SearchDownload" %>

<%@ Register TagPrefix="uc" TagName="ColumnTabs" Src="~/UI/Layout/Common/Controls/SearchDownloadColumnTabs.ascx" %>
<%@ Register TagPrefix="uc" TagName="DownloadSettings" Src="~/UI/Layout/Common/Controls/SearchDownloadSettings.ascx" %>
<%@ Register TagPrefix="uc" TagName="DownloadManager" Src="~/UI/Layout/Common/Controls/SearchDownloadManager.ascx" %>

<div class="row settings">

    <div class="col-md-4 col-search-download-columns">
        <h4 class="mb-0">
            <insite:Literal runat="server" Text="Columns" />
        </h4>
        <div class="text-body-secondary fs-sm mb-2">
            <insite:Literal runat="server" Text="Drag and drop to reorder" />
        </div>
        <uc:ColumnTabs runat="server" ID="ColumnTabs" />
    </div>

    <div class="col-md-4">
        <h4>
            <insite:Literal runat="server" Text="Settings" />
        </h4>
        <uc:DownloadSettings runat="server" ID="Settings" />
    </div>

    <div class="col-md-4">
        <h4>
            <insite:Literal runat="server" Text="Download Templates" />
        </h4>
        <uc:DownloadManager runat="server" ID="Manager" />
    </div>

    <div class="mt-5">
        <div class="search-download-buttons">
            <div class="border-1 shadow-lg">
                <insite:DownloadButton runat="server" ID="DownloadButton" />
                <insite:ResetButton runat="server" ID="ResetButton" />
            </div>
        </div>
    </div>

</div>