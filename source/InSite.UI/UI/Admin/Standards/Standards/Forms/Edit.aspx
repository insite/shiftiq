<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Standards.Standards.Forms.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Standards/Standards/Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:PageHeadContent runat="server">

        <style type="text/css">

            .shortcuts-menu + .dropdown-menu i.fa,
            .shortcuts-menu + .dropdown-menu i.fas,
            .shortcuts-menu + .dropdown-menu i.far,
            .shortcuts-menu + .dropdown-menu i.fab,
            .shortcuts-menu + .dropdown-menu i.fal{
                width: 17px !important;
            }

                .shortcuts-menu + .dropdown-menu i.fa-caret-up,
                .shortcuts-menu + .dropdown-menu i.fa-caret-down {
                    padding-left: 3px;
                }

                .shortcuts-menu + .dropdown-menu i.fa-caret-right {
                    padding-left: 5px;
                }

        </style>

    </insite:PageHeadContent>

    <insite:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <insite:Alert runat="server" ID="EditorStatus" />
        </ContentTemplate>
    </insite:UpdatePanel>
    <insite:ValidationSummary runat="server" ValidationGroup="Asset" />

    <uc:Detail runat="server" ID="Detail" />

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Asset" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

    <insite:Modal runat="server" ID="ViewEmailListWindow" Title="Email Address List">
        <ContentTemplate>

            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ViewEmailListUpdatePanel" />

            <insite:UpdatePanel runat="server" ID="ViewEmailListUpdatePanel">
                <ContentTemplate>
                    <asp:Panel runat="server">

                        <div>
                            <p><asp:Literal runat="server" ID="EmailsLiteral" /></p>
                            <br />
                            <insite:CloseButton runat="server" OnClientClick="closeEmailList(); return false;" />
                        </div>

                    </asp:Panel>
                </ContentTemplate>
            </insite:UpdatePanel>

        </ContentTemplate>
    </insite:Modal>

    <asp:Button ID="RefreshButton" runat="server" CssClass="d-none" />

    <insite:Modal runat="server" ID="HistoryViewerWindow" Title="Change History" Width="710px" MinHeight="520px" />

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">

            function showEmailList() {
                modalManager.show('<%= ViewEmailListWindow.ClientID %>');
                document.getElementById('<%= ViewEmailListUpdatePanel.ClientID %>').ajaxRequest();
            }

            function closeEmailList() {
                modalManager.close('<%= ViewEmailListWindow.ClientID %>');
            }

            function reloadEditor() {
                __doPostBack('<%= RefreshButton.UniqueID %>', '');
            }
        </script>
</insite:PageFooterContent>
</asp:Content>
