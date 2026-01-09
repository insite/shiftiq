<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchDownload.ascx.cs" Inherits="InSite.Admin.Records.Outcomes.Controls.SearchDownload" %>

<%@ Register TagPrefix="uc" TagName="ColumnTabs" Src="~/UI/Layout/Common/Controls/SearchDownloadColumnTabs.ascx" %>
<%@ Register TagPrefix="uc" TagName="DownloadSettings" Src="~/UI/Layout/Common/Controls/SearchDownloadSettings.ascx" %>
<%@ Register TagPrefix="uc" TagName="DownloadManager" Src="~/UI/Layout/Common/Controls/SearchDownloadManager.ascx" %>

<div class="row settings">
    <div class="col-md-4 col-search-download-columns">
        <h4>Columns (drag and drop to reorder)</h4>

        <uc:ColumnTabs runat="server" ID="ColumnTabs" />
    </div>
    <div class="col-md-4">
        <h4>Settings</h4>

        <uc:DownloadSettings runat="server" ID="Settings" />
    </div>
    <div class="col-md-4">
        <h4>Download Templates</h4>
        <uc:DownloadManager runat="server" ID="Manager" />

        <h4>Other</h4>

        <div>
            <insite:Button runat="server" ID="NotAchievedMastery" ButtonStyle="Default" style="width:100%; text-align:left;" Text="Not Achieved Mastery (*.xlsx)" Icon="far fa-file-excel" />
        </div>
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
