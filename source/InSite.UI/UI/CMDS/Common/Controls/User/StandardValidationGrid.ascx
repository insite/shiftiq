<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StandardValidationGrid.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.Competencies.Controls.ValidationGrid" %>

<insite:DownloadButton runat="server" ID="DownloadButton" CssClass="mb-3" Visible="false" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:BoundField HeaderText="Name" DataField="UserFullName" />
        <asp:BoundField HeaderText="Department" DataField="DepartmentName" />
        <asp:BoundField HeaderText="Profile" DataField="ProfileStandardCode" />

        <asp:TemplateField HeaderText="Expired" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# GetLocalTime("Expired") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Validated" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# GetLocalTime("ValidationDate") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Validated By">
            <ItemTemplate>
                <%# Eval("ValidatorFullName") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Criticality">
            <ItemTemplate>
                <%# GetBoolean("IsCritical") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Time-Sensitivity">
            <ItemTemplate>
                <%# GetBoolean("IsTimeSensitive") %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
