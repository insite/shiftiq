<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportDuplicateGrid.ascx.cs" Inherits="InSite.Admin.Surveys.Forms.Controls.ReportDuplicateGrid" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier" ShowHeader="false">
    <Columns>
        <asp:TemplateField HeaderText="Submission">
            <ItemTemplate>
                <strong><%# Eval("FullName") %></strong>
                submitted
                        <%# Eval("ResponseCount") %> responses:
                        <asp:Literal runat="server" ID="Dates" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>
