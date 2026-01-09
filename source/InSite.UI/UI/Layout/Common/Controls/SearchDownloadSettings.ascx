<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchDownloadSettings.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.SearchDownloadSettings" %>

<div class="form-group mb-3">
    <label class="form-label">
        <insite:Literal runat="server" Text="File Format" />
    </label>
    <div>
        <asp:RadioButtonList runat="server" ID="OutputFileFormat" RepeatDirection="Vertical" RepeatLayout="Flow">
            <asp:ListItem Text="Data Export (.csv)" Value="csv" Selected="True" />
            <asp:ListItem Text="Report Download (.xlsx)" Value="xlsx" />
        </asp:RadioButtonList>
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        <insite:Literal runat="server" Text="Hidden Columns" />
    </label>
    <div>
        <asp:RadioButtonList runat="server" ID="IsShowHidden" RepeatDirection="Vertical" RepeatLayout="Flow">
            <asp:ListItem Text="Show" Value="True" Selected="True" />
            <asp:ListItem Text="Hide" Value="False" />
        </asp:RadioButtonList>
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        <insite:Literal runat="server" Text="Column Headings" />
    </label>
    <div>
        <insite:CheckBox runat="server" ID="IsRemoveSpaces" Text="Remove Spaces" />
    </div>
</div>

<div class="alert alert-info fs-xs mt-3">
    <strong>Please Note:</strong>
    You can download a maximum of <asp:Literal runat="server" ID="PlatformSearchDownloadMaximumRows" /> rows. 
    If you need to download more than this, please request an export from <asp:Literal runat="server" ID="SupportEmailLink" />.
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            $('#<%= IsShowHidden.ClientID %> input[type="radio"]').on('change', onShowHiddenChanged).each(function () {
                if (this.checked)
                    onShowHiddenChanged.call(this);
            });

            function onShowHiddenChanged() {
                inSite.common.searchDownload.showHidden($(this).val());
            }
        })();
    </script>
</insite:PageFooterContent>