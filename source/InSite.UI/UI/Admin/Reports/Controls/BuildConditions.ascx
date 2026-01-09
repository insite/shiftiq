<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BuildConditions.ascx.cs" Inherits="InSite.UI.Admin.Reports.Controls.BuildConditions" %>

<%@ Register Src="ConditionControl.ascx" TagName="ConditionControl" TagPrefix="uc" %>

<div class="row settings">
    <div class="col-lg-12">

<asp:MultiView runat="server" ID="MultiView">
    <asp:View runat="server" ID="TableView">
        <insite:Alert runat="server" ID="TableStatus" />

        <asp:Repeater runat="server" ID="ConditionRepeater">
            <HeaderTemplate>
                <table class="table">
                    <thead>
                        <tr>
                            <th><insite:Literal runat="server" Text="Name" /></th>
                            <th><insite:Literal runat="server" Text="Condition" /></th>
                            <th style="width:120px;"></th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="text-nowrap">
                        <asp:Literal runat="server" ID="ColumnName" Text='<%# Eval("Name") %>' />
                    </td>
                    <td>
                        <div style="white-space:pre-wrap"><%# Eval("Sql") %></div>
                    </td>
                    <td class="text-nowrap text-end">
                        <insite:Button runat="server" Icon="fas fa-edit" ToolTip='<%# Translate("Edit Condition") %>'
                            CommandName="EditCondition" />
                        <insite:Button runat="server" Icon="far fa-trash-alt" ToolTip='<%# Translate("Delete Condition") %>'
                            CommandName="DeleteCondition" />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </asp:View>
    <asp:View runat="server" ID="EditorView">
        <insite:RequiredValidator runat="server" ID="ConditionNameRequiredValidator" ControlToValidate="ConditionName" RenderMode="Exclamation" Display="None" />
        <insite:CustomValidator runat="server" ID="ConditionNameUniqueValidator" ErrorMessage="Condition Name must be unique" RenderMode="Exclamation" Display="None" />

        <div class="mb-3">
            <insite:TextBox runat="server" ID="ConditionName" EmptyMessage="Condition Name" MaxLength="50" />
        </div>

        <insite:Nav runat="server">
            <insite:NavItem runat="server" ID="ConditionWhereTab" Title="WHERE">
                <uc:ConditionControl runat="server" ID="ConditionWhere" />
            </insite:NavItem>

            <insite:NavItem runat="server" ID="ConditionAndTab" Title="AND">
                <uc:ConditionControl runat="server" ID="ConditionAnd" />
            </insite:NavItem>

            <insite:NavItem runat="server" ID="ConditionOrTab" Title="OR">
                <uc:ConditionControl runat="server" ID="ConditionOr" />
            </insite:NavItem>
        </insite:Nav>
    </asp:View>
</asp:MultiView>

    </div>
</div>
