<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HistorySnapshotGrid.ascx.cs" Inherits="InSite.Cmds.Controls.Reports.Compliances.HistorySnapshotGrid" %>

<insite:Grid runat="server" ID="Grid" AllowPaging="false">
    <Columns>
        <asp:TemplateField HeaderText="Snapshot Date">
            <ItemTemplate>
                <%# Eval("AsAt", "{0:MMM d, yyyy}") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Average Compliance %" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Literal ID="AvgCompliance" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="# of Companies" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# Eval("CompanyCount", "{0:n0}") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="# of Users" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# Eval("EmployeeCount", "{0:n0}") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="# of Competencies" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# Eval("CompetencyCountRequired", "{0:n0}") %>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>
