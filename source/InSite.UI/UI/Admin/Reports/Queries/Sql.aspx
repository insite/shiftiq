<%@ Page Language="C#" CodeBehind="Sql.aspx.cs" Inherits="InSite.Admin.Reports.Queries.Sql" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="./Controls/SqlSearchCriteria.ascx" TagName="SearchCriteria" TagPrefix="uc" %>
<%@ Register Src="./Controls/SqlSearchResults.ascx" TagName="SearchResults" TagPrefix="uc" %>
<%@ Register Src="~/UI/Layout/Common/Controls/SearchDownload.ascx" TagName="SearchDownload" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="SearchResultsTab" Title="Results" Icon="far fa-database" IconPosition="BeforeText">
            <h2 class="h4 mt-4 float-start">Results</h2>

            <uc:SearchResults runat="server" ID="SearchResults" />
        </insite:NavItem>

        <insite:NavItem runat="server" ID="SearchCriteriaTab" Title="Query" Icon="far fa-file-alt" IconPosition="BeforeText">
            <h2 class="h4 mt-4">Query</h2>
            <uc:SearchCriteria runat="server" ID="SearchCriteria" />
        </insite:NavItem>

    </insite:Nav>

    <insite:PageHeadContent runat="server">
        <style type="text/css">
            div.sql-editor {
                height: 300px;
            }

                div.sql-editor > div {
                    padding: 6px 12px;
                    border: 1px solid #ccc;
                    border-radius: 4px;
                    width: 100%;
                    height: 100%;
                }
        </style>
    </insite:PageHeadContent>

    <insite:PageFooterContent runat="server">
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
    </insite:PageFooterContent>
</asp:Content>
