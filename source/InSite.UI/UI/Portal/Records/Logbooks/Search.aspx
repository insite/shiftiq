<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.Search" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:Accordion runat="server" ID="MainAccordion">
        <insite:AccordionPanel runat="server" Title="Logbooks" Icon="fas fa-pencil-ruler">
            <asp:Repeater runat="server" ID="JournalRepeater">
                <HeaderTemplate>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                                <th><insite:Literal runat="server" Text="Logbook" /></th>
                                <th class="text-center"><insite:Literal runat="server" Text="# of Entries" /></th>
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
                            <a class="me-2" href='<%# Eval("JournalSetupIdentifier", "/ui/portal/records/logbooks/outline?journalsetup={0}") %>'><%# Eval("Title") %></a>
                        </td>
                        <td class="text-center">
                            <%# Eval("ExperienceCount", "{0:n0}") %>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </insite:AccordionPanel>
    </insite:Accordion>

</asp:Content>
