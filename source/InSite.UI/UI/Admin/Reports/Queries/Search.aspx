<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Admin.Reports.Queries.Search" %>
<%@ Register Src="./Controls/QuerySearchCriteria.ascx" TagName="SearchCriteria" TagPrefix="uc" %>
<%@ Register Src="./Controls/QuerySearchResults.ascx" TagName="SearchResults" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">  
    <style type="text/css">
        div.sql-editor {
            height: 500px;
        }

        div.sql-editor > div {
            padding: 6px 12px;
            border: 1px solid #ccc;
            border-radius: 4px;
            width: 100%;
            height: 100%;
        }
    </style>

    <insite:ResourceLink runat="server" Type="JavaScript" Url="/UI/Layout/common/parts/plugins/ace.cloud9/ace.js" />

    <script type="text/javascript">
        (function () {
            $('textarea.sql-editor').each(function () {
                var $input = $(this).hide();
                var $wrapper = $('<div class="sql-editor">');
                var $editor = $('<div>');

                $wrapper.insertAfter($input);
                $wrapper.append($editor);

                var editor = ace.edit($editor[0], {
                    minLines: 15
                });

                editor.$blockScrolling = Infinity;
                editor.setFontSize(15);
                editor.setShowPrintMargin(false);
                editor.session.setMode('ace/mode/sqlserver');

                editor.session.setValue($input.val());
                editor.session.on('change', function () {
                    $input.val(editor.session.getValue());
                });

                //editor.session.setUseWrapMode(true);
                // 
                // editor.session.on('changeScrollTop', onEditorScroll);
            });
        })();
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <insite:Alert runat="server" ID="SearchAlert" />
    <insite:Alert runat="server" ID="ScreenStatus" />   

    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="SearchResultsTab" Icon="fas fa-database" Title="Results">
            <asp:Panel runat="server" ID="DownloadButtonSection" Visible="false">
                <div class="pull-left" style="margin-bottom: 15px;">
                    <insite:DownloadButton runat="server" ID="DownloadButton" />
                    <span runat="server" id="MoreThanTwenty" style="padding-left: 10px;">Only the first <strong>20 Results</strong> are displayed. Click <strong>Download</strong> for the full data set.</span>
                </div>
                <div style="clear:both;"></div>
            </asp:Panel>
            <uc:SearchResults runat="server" ID="SearchResults" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="SearchCriteriaTab" Icon="fas fa-filter" Title="Criteria">
            <uc:SearchCriteria runat="server" ID="SearchCriteria" />
        </insite:NavItem>
    </insite:Nav>

</asp:Content>