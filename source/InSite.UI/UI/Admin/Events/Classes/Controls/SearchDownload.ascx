<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchDownload.ascx.cs" Inherits="InSite.Admin.Events.Classes.Controls.SearchDownload" %>

<%@ Register TagPrefix="uc" TagName="ColumnTabs" Src="~/UI/Layout/Common/Controls/SearchDownloadColumnTabs.ascx" %>
<%@ Register TagPrefix="uc" TagName="DownloadSettings" Src="~/UI/Layout/Common/Controls/SearchDownloadSettings.ascx" %>
<%@ Register TagPrefix="uc" TagName="DownloadManager" Src="~/UI/Layout/Common/Controls/SearchDownloadManager.ascx" %>

<div class="row settings">

    <div class="col-md-4 col-search-download-columns">
        <h4 class="mb-0">Columns</h4>
        <div class="text-body-secondary fs-sm mb-2">Drag and drop to reorder</div>
        <uc:ColumnTabs runat="server" ID="ColumnTabs" />
    </div>

    <div class="col-md-4 col-search-download-settings">
        <h4>Settings</h4>
        <uc:DownloadSettings runat="server" ID="Settings" />

        <div runat="server" id="SettingsColumnConfig" class="form-group mb-3 d-none">
            <label class="form-label">
                <insite:Literal runat="server" Text="Column Configuration" />
            </label>
            <div>
                <insite:CheckBox runat="server" ID="IsBreakdownRegistrationCount" Text="Breakdown Registrations Count" />
            </div>
        </div>

    </div>

    <div class="col-md-4">
        <h4>Download Templates</h4>
        <uc:DownloadManager runat="server" ID="Manager" />

        <h4>Other</h4>

        <div class="mb-2">
            <insite:Button runat="server" ID="ExportByGroupStatus" ButtonStyle="Default" style="width:100%; text-align:left;" Text="Classes by Employer Status (*.pdf)" Icon="far fa-file-pdf" />
        </div>
        <div class="mb-2">
            <insite:Button runat="server" ID="ExportByAchievement" ButtonStyle="Default" style="width:100%; text-align:left;" Icon="far fa-file-pdf" Text="Classes by Achievement (*.pdf)" />
        </div>
        <div class="mb-2">
            <insite:Button runat="server" ID="ExportByAchievementXlsx" ButtonStyle="Default" style="width:100%; text-align:left;" Icon="far fa-file-excel" Text="Classes by Achievement (*.xlsx)" />
        </div>
        <div class="mb-2">
            <insite:Button runat="server" ID="ExportByTrade" ButtonStyle="Default" style="width:100%; text-align:left;" Icon="far fa-file-pdf" Text="Classes by Trade (*.pdf)" />
        </div>
        <div class="mb-2">
            <insite:Button runat="server" ID="ExportByTradeXlsx" ButtonStyle="Default" style="width:100%; text-align:left;" Icon="far fa-file-excel" Text="Classes by Trade (*.xlsx)" />
        </div>
        <div class="mb-2">
            <insite:Button runat="server" ID="ExportByCertificateaAndVenue" ButtonStyle="Default" style="width:100%; text-align:left;" Icon="far fa-file-pdf" Text="Classes by Certificates and Venue (*.pdf)" />
        </div>
        <div class="mb-2">
            <insite:Button runat="server" ID="ExportByCertificateaAndVenueXlsx" ButtonStyle="Default" style="width:100%; text-align:left;" Icon="far fa-file-excel" Text="Classes by Certificates and Venue (*.xlsx)" />
        </div>
        <div class="mb-2">
            <insite:Button runat="server" ID="ExportStudentsByTrade" ButtonStyle="Default" style="width:100%; text-align:left;" Icon="far fa-file-pdf" Text="Students by Trade (*.pdf)" />
        </div>
        <div class="mb-2">
            <insite:Button runat="server" ID="ExportStudentsByTradeXlsx" ButtonStyle="Default" style="width:100%; text-align:left;" Icon="far fa-file-excel" Text="Students by Trade (*.xlsx)" />
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

<script type="text/javascript">
    (function () {
        const columnConfig = document.getElementById('<%= SettingsColumnConfig.ClientID %>');

        const container = columnConfig.closest('div.col-search-download-settings');
        if (!container)
            return;

        const siblings = container.querySelectorAll('div.form-group');
        let lastSibling = null;

        for (let sibling of siblings) {
            if (sibling === columnConfig)
                break;

            lastSibling = sibling;
        }

        if (lastSibling) {
            if (lastSibling.nextSibling)
                lastSibling.parentNode.insertBefore(columnConfig, lastSibling.nextSibling);
            else
                lastSibling.parentNode.appendChild(columnConfig);
        }

        columnConfig.classList.remove('d-none');
    })();
</script>