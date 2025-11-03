<%@ Page Language="C#" CodeBehind="OutlineEntry.aspx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.OutlineEntry" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="EntryDetail" Src="Controls/EntryDetail.ascx" %>
<%@ Register Src="Controls/CommentList.ascx" TagName="CommentList" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Accordion runat="server" IsFlush="false">
        <insite:AccordionPanel runat="server" ID="DetailPanel" Icon="far fa-pencil-ruler" Title="Entry">

            <uc:EntryDetail runat="server" ID="Detail" />

        </insite:AccordionPanel>
        <insite:AccordionPanel runat="server" ID="CompetenciesPanel" Icon="far fa-ruler-triangle" Title="Competencies">

            <div class="row">
                <div class="col-md-12">
                    <asp:Repeater runat="server" ID="AreaRepeater">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>

                                        </th>
                                        <th>
                                            <insite:Literal runat="server" Text="Competency" />
                                        </th>
                                        <th class="text-center">
                                            <insite:ContentTextLiteral runat="server" ContentLabel="Number of Hours"/>
                                        </th>
                                        <th style="text-align:center;">Number of Log Entries</th>
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
                                <td colspan="4">
                                    <b><%# Eval("Name") %></b>
                                </td>
                            </tr>

                            <asp:Repeater runat="server" ID="CompetencyRepeater">
                                <ItemTemplate>
                                    <tr>
                                        <td style="width:20px;">

                                        </td>
                                        <td class="align-middle">
                                            <%# Eval("Name") %>
                                            <%# GetSatisfactionLevelHtml((string)Eval("SatisfactionLevel")) %>
                                        </td>
                                        <td class="text-center align-middle">
                                            <%# Eval("Hours") %>
                                        </td>
                                        <td style="text-align:center;">
                                            <%# Eval("JournalItems") %>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

        </insite:AccordionPanel>
        <insite:AccordionPanel runat="server" ID="CommentsPanel" Icon="far fa-comments" Title="Comments">
            <uc:CommentList runat="server" ID="Comments" />
        </insite:AccordionPanel>
    </insite:Accordion>

</asp:Content>