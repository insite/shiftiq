<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.Outline" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="Controls/CompetencyProgressGrid.ascx" TagName="CompetencyProgressGrid" TagPrefix="uc" %>
<%@ Register Src="Controls/CommentList.ascx" TagName="CommentList" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:Accordion runat="server" ID="MainAccordion" IsFlush="false">
        <insite:AccordionPanel runat="server" ID="FieldsPanel" Icon="far fa-pencil-ruler" Title="Logbook Entries">

            <div class="row">
                <div class="col-md-12">

                    <asp:Repeater runat="server" ID="ExperienceRepeater">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>#</th>
                                        <th>Created</th>
                                        <th>Status</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <%# Eval("Sequence") %>
                                </td>
                                <td>
                                    <%# GetLocalTime(Eval("Created")) %>
                                </td>
                                <td>
                                    <%# Eval("Status") %>
                                </td>
                                <td class="text-start" style="width:180px;">
                                    <insite:Button runat="server" ButtonStyle="Primary" ToolTip="View Entry" Icon="far fa-search"
                                        NavigateUrl='<%# Eval("ExperienceIdentifier", "/ui/portal/records/logbooks/outline-entry?experience={0}") %>' />
                                    <insite:Button runat="server" ButtonStyle="Primary" ToolTip="Edit Entry" Icon="far fa-pencil"
                                        Visible='<%# !(bool)Eval("IsValidated") && !(bool)Eval("IsJournalSetupLocked") %>'
                                        NavigateUrl='<%# Eval("ExperienceIdentifier", "/ui/portal/records/logbooks/change-entry?experience={0}") %>' />
                                    <insite:Button runat="server" ButtonStyle="Danger" ToolTip="Delete Entry" Icon="far fa-trash-alt"
                                        Visible='<%# !(bool)Eval("IsValidated") && !(bool)Eval("IsJournalSetupLocked") %>'
                                        NavigateUrl='<%# Eval("ExperienceIdentifier", "/ui/portal/records/logbooks/delete-entry?experience={0}") %>' />

                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

            <div class="row">
                <div class="col-md-12">
                    <insite:Button runat="server" ID="AddEntryLink" ButtonStyle="Default" Icon="fa fa-plus-circle" Text="Add Entry"/>
                    <insite:DownloadButton runat="server" ID="DownloadPDFButton" Text="Download PDF"/>
                </div>
            </div>
        </insite:AccordionPanel>
        <insite:AccordionPanel runat="server" ID="ProgressPanel" Icon="far fa-check" Title="Progress">
            <uc:CompetencyProgressGrid runat="server" ID="ProgressGrid" />
        </insite:AccordionPanel>
        <insite:AccordionPanel runat="server" ID="CommentsPanel" Icon="far fa-comments" Title="Comments">
            <uc:CommentList runat="server" ID="Comments" />
        </insite:AccordionPanel>
    </insite:Accordion>

</asp:Content>