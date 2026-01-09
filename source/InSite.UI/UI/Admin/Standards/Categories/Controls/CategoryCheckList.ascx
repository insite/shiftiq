<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryCheckList.ascx.cs" Inherits="InSite.Admin.Standards.Categories.Controls.CategoryCheckList" %>

<div class="form-text"><asp:Literal id="Instructions" runat="server" /></div>

<div id='<%= ClientID %>'>
    <asp:Repeater runat="server" ID="Repeater">
        <ItemTemplate>
            <div>
                <asp:CheckBox runat="server" ID="IsSelected" Text='<%# Eval("CategoryName") %>' Checked='<%# Eval("Selected") %>' data-id='<%# Eval("CategoryIdentifier") %>'
                    AutoPostBack="true" OnCheckedChanged="IsSelected_CheckedChanged" />
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <insite:LoadingPanel runat="server" />
</div>
