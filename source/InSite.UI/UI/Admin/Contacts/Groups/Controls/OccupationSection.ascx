<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OccupationSection.ascx.cs" Inherits="InSite.UI.Admin.Contacts.Groups.Controls.OccupationSection" %>

<div class="row">
    <div class="col-6">
        <asp:Literal runat="server" ID="ProfileCount" />

        <div class="float-end">
            <insite:IconLink Name="pencil" runat="server" ID="ChangeOccupations" ToolTip="Change Occupations" NavigateUrl="#" />
        </div>

        <table class="check-list">
        <asp:Repeater ID="AddedProfiles" runat="server">
            <ItemTemplate>
                <%# (Container.ItemIndex % 2) == 0 ? "<tr>": "" %>
                    <td style="white-space:nowrap;">
                        <a href='<%# "/ui/admin/standards/edit?id=" + Eval("StandardIdentifier") + "&group=" + DepartmentIdentifier %>'><%# Eval("Code") %></a>
                    </td>
                    <td style="padding-right:20px;">
                        <asp:Label runat="server" ID="Title" Text='<%# Eval("ContentTitle") %>' />
                    </td>
                <%# (Container.ItemIndex % 2) != 0 ? "</tr>": "" %>
            </ItemTemplate>
            <FooterTemplate>
                <%# (((Repeater)Container.Parent).Items.Count % 2) != 0 ? "</tr>": "" %>
            </FooterTemplate>
        </asp:Repeater>
        </table>
    </div>
</div>