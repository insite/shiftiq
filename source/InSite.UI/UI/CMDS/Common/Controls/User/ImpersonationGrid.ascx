<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImpersonationGrid.ascx.cs" Inherits="InSite.Cmds.Controls.User.ImpersonationGrid" %>

<p runat="server" id="NoDataMessage" class="alert alert-info">There are no impersonations.</p>

<insite:Grid runat="server" ID="Grid">
    <Columns>
            
        <asp:TemplateField HeaderText="Started">
            <ItemTemplate>
                <%# LocalizeTime(Eval("ImpersonationStarted")) %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Administrator">
            <ItemTemplate>
                <%# GetImpersonationName(Eval("ImpersonatorUserIdentifier") as Guid?, (string)Eval("ImpersonatorUserFullName")) %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Impersonated">
            <ItemTemplate>
                <%# GetImpersonationName(Eval("ImpersonatedUserIdentifier") as Guid?, (string)Eval("ImpersonatedUserFullName")) %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Stopped">
            <ItemTemplate>
                <%# LocalizeTime(Eval("ImpersonationStopped")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
