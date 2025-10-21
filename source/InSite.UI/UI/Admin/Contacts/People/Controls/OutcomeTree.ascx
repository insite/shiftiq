<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OutcomeTree.ascx.cs" Inherits="InSite.UI.Admin.Contacts.People.Controls.OutcomeTree" %>

<div runat="server" id="NoOutcomes" class="alert alert-warning" role="alert">
    There are no scores
</div>

<asp:Repeater runat="server" ID="GradebookRepeater">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Gradebook / Competency</th>
                    <th>Achievement</th>
                    <th>Points</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td style="width:65%;">
                <a href='/ui/admin/records/gradebooks/outline?<%# Eval("GradebookIdentifier", "id={0}") %>'>
                    <%# Eval("GradebookName") %>
                </a>
            </td>
            <td style="width:35%;">
                <a runat="server" visible='<%# Eval("AchievementIdentifier") != null %>' href='<%# Eval("AchievementIdentifier", "/ui/admin/records/achievements/outline?id={0}") %>'>
                    <%# Eval("AchievementName") %>
                </a>
            </td>
            <td></td>
            <td style="width:40px;"></td>
        </tr>

        <asp:Repeater runat="server" ID="StandardRepeater">
            <ItemTemplate>
                <tr>
                    <td style="width:65%;">
                        <div style='<%# "padding-left:" + (10 * (int)Eval("Level")) + "px" %>'>
                            <%# Eval("StandardName") %>
                        </div>
                    </td>
                    <td style="width:35%;"></td>
                    <td style="white-space:nowrap;text-align:right;">
                        <%# Eval("Points", "{0:n2}") %>
                    </td>
                    <td style="width:40px;text-align:right;">
                        <insite:IconLink runat="server" ID="EditLink"
                            ToolTip="Edit" Name="pencil"
                            Visible='<%# Eval("Assigned") %>'
                            NavigateUrl='<%# GetEditUrl(Container.DataItem) %>'
                        />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </ItemTemplate>
</asp:Repeater>