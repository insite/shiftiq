<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchDownload.ascx.cs" Inherits="InSite.Admin.Events.Registrations.Controls.SearchDownload" %>

<%@ Register TagPrefix="uc" TagName="ColumnTabs" Src="~/UI/Layout/Common/Controls/SearchDownloadColumnTabs.ascx" %>
<%@ Register TagPrefix="uc" TagName="DownloadSettings" Src="~/UI/Layout/Common/Controls/SearchDownloadSettings.ascx" %>
<%@ Register TagPrefix="uc" TagName="DownloadManager" Src="~/UI/Layout/Common/Controls/SearchDownloadManager.ascx" %>

<div class="row settings">

    <div class="col-md-4 col-search-download-columns">
        <h4 class="mb-0">Columns</h4>
        <div class="text-body-secondary fs-sm mb-2">Drag and drop to reorder</div>
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

        <div class="mb-2">
            <insite:Button runat="server" ID="ExportRegistrationReport" ButtonStyle="Default" style="width:100%; text-align:left;" Icon="far fa-file-pdf" Text="Registration Report (*.pdf)" />
        </div>
        <div class="mb-2">
            <insite:Button runat="server" ID="ExportAverageAgeByAchievement" ButtonStyle="Default" style="width:100%; text-align:left;" Icon="far fa-file-pdf" Text="Average Age by Achievements (*.pdf)" />
        </div>
        <div class="mb-2">
            <insite:Button runat="server" ID="ExportApprenticeScores" ButtonStyle="Default" style="width:100%; text-align:left;" Icon="far fa-file-excel" Text="Apprentice Scores(*.xlsx)" />
        </div>
        <div class="mb-2">
            <insite:Button runat="server" ID="ExportApprenticeCompletionRate" ButtonStyle="Default" style="width:100%; text-align:left;" Icon="far fa-file-pdf" Text="Apprentice Completion Rate (*.pdf)" />
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
