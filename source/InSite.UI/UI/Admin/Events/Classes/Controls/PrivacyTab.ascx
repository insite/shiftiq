<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PrivacyTab.ascx.cs" Inherits="InSite.UI.Admin.Events.Classes.Controls.PrivacyTab" %>

<div class="card border-0 shadow-lg w-50">
	<div class="card-body">
        <div class="float-start">
            <h3>Groups</h3>
        </div>
        <div class="float-end">
            <insite:IconLink Name="pencil" runat="server" ID="ChangeLink" ToolTip="Change Privacy" />
        </div>

        <div class="clearfix">
            <asp:Literal runat="server" ID="FilterGroupList" />
        </div>

        <asp:Repeater runat="server" ID="GroupDataRepeater">
            <HeaderTemplate>
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Type</th>
                            <th>Name</th>
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
                        <%# Eval("Type") %>
                    </td>
                    <td>
                        <%# Eval("Name") %>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        <div runat="server" ID="GroupDataRepeaterFooter" visible="false">
            None
        </div>
	</div>
</div>