<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FilePermissionList.ascx.cs" Inherits="InSite.UI.Admin.Assets.Files.Controls.FilePermissionList" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="PermissionPanel" />

<insite:UpdatePanel runat="server" ID="PermissionPanel">
    <ContentTemplate>

        <div class="mb-3">
            <insite:RadioButton runat="server" ID="PublicRadioButton" Text="Public" GroupName="PermissionType" Checked="true" />
            <insite:RadioButton runat="server" ID="PrivateRadioButton" Text="Private" GroupName="PermissionType" />
        </div>

        <div runat="server" id="ClaimObjectPanel" class="mb-3 hstack">
            <insite:ComboBox runat="server" ID="ClaimObjectType" CssClass="me-2">
                <Items>
                    <insite:ComboBoxOption Value="User" Text="User" Selected="true" />
                    <insite:ComboBoxOption Value="Group" Text="Group" />
                </Items>
            </insite:ComboBox>

            <insite:FindGroup runat="server" ID="GroupID" CurrentOrganizationOnly="true" EmptyMessage="Select" />
            <insite:FindPerson runat="server" ID="PersonID" CurrentOrganizationOnly="true" EmptyMessage="Select" />
        </div>

        <asp:Repeater runat="server" ID="PermissionList">
            <HeaderTemplate>
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Type</th>
                            <th>Name</th>
                            <th class="text-end"></th>
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
                        <%# Eval("ObjectType") %>
                    </td>
                    <td>
                        <%# Eval("ObjectName") %>
                    </td>
                    <td class="text-end">
                        <insite:IconButton runat="server"
                            ToolTip="Delete"
                            CommandName="Delete"
                            CommandArgument='<%# Eval("ObjectIdentifier") %>'
                            Name="trash-alt"
                            ConfirmText="Are you sure you want to delete this object?"
                        />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>

    </ContentTemplate>
</insite:UpdatePanel>