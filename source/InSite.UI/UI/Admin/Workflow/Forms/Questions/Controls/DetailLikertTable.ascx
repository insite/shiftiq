<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailLikertTable.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Questions.Controls.DetailLikertTable" %>

<%@ Register Src="~/UI/Admin/Workflow/Forms/Questions/Controls/LikertTableColumnGrid.ascx" TagName="LikertTableColumnGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Workflow/Forms/Questions/Controls/LikertTableRowGrid.ascx" TagName="LikertTableRowGrid" TagPrefix="uc" %>

<style type="text/css">
    .likert-analysis input {
        margin-left: 12px;
    }
</style>

<asp:CustomValidator runat="server" ID="RowRequiredValidator"
    ValidationGroup="SurveyQuestion"
    Display="None"
    ErrorMessage="Column(s) cannot be saved without any row. Please add at least one row." />

<div>
    <span runat="server" id="LanguageOutput" class="badge bg-custom-default text-uppercase" style="position: absolute; right: 16px;"></span>

    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="ColumnTab" Title="Columns">
            <uc:LikertTableColumnGrid runat="server" ID="LikertTableColumnGrid" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="RowTab" Title="Rows">
            <uc:LikertTableRowGrid runat="server" ID="LikertTableRowGrid" />
        </insite:NavItem>
    </insite:Nav>
</div>

<div>
    <div clss="mt-2">
        <div>
            <asp:CheckBox runat="server" ID="ListEnableBranch" Text="Enable Branches" />
            <asp:CheckBox runat="server" ID="ListDisableColumnHeadingWrap" Text="Disable Column Heading Wrap" />
        </div>
        <div class="mt-1">
            Likert Table Analysis:
            <asp:RadioButtonList runat="server" ID="LikertTableReporting" RepeatLayout="Flow" RepeatDirection="Horizontal" CssClass="likert-analysis">
                <asp:ListItem Value="Current Question Only" Selected="True" />
                <asp:ListItem Value="Preceding Questions, All Scales" />
                <asp:ListItem Value="Preceding Questions, Highest-Point Scale Only" />
            </asp:RadioButtonList>
        </div>
    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var instance = window.detailLikertTable = window.detailLikertTable || {};

            instance.setLanguage = function (lang) {
                if (typeof likertTableRowGrid != 'undefined')
                    likertTableRowGrid.setLanguage(lang);

                if (typeof likertTableColumnGrid != 'undefined')
                    likertTableColumnGrid.setLanguage(lang);

                $('#<%= LanguageOutput.ClientID %>').text(lang);
            };
        })();
    </script>
</insite:PageFooterContent>
