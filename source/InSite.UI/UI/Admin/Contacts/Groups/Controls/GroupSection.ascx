<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupSection.ascx.cs" Inherits="InSite.UI.Admin.Contacts.Groups.Controls.GroupSection" %>

<div class="row outline">

    <div class="col-5">

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeGroupName" ToolTip="Change Group Name" NavigateUrl="#" />
            </div>
            <label class="form-label">
                Group Name
            </label>
            <div>
                <asp:Literal runat="server" ID="GroupName" />
            </div>
            <div class="form-text">The name of the group should be unique and descriptive.</div>
        </div>

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeGroupType" ToolTip="Change Group Type" NavigateUrl="#" />
            </div>
            <label class="form-label">
                Group Type
            </label>
            <div>
                <asp:Literal runat="server" ID="GroupType" />
            </div>
            <div class="form-text">
                Group type determines the features available for using and managing it in the system.
            </div>
        </div>

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeGroupLabel" ToolTip="Change Group Label" NavigateUrl="#" />
            </div>
            <label class="form-label">
                Group Label
            </label>
            <div>
                <asp:Literal runat="server" ID="GroupLabel" />
            </div>
            <div class="form-text">
                Optional: You can tag contact groups to make them easier to find.
            </div>
        </div>


        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeGroupDescription" ToolTip="Change Group Description" NavigateUrl="#" />
            </div>
            <label class="form-label">Description</label>
            <div>
                <asp:Literal runat="server" ID="GroupDescription" />
            </div>
            <div class="form-text">Comment or note to describe the purpose of the group.</div>
        </div>

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeGroupCode" ToolTip="Change Group Code" NavigateUrl="#" />
            </div>
            <label class="form-label">Group Code</label>
            <div>
                <asp:Literal runat="server" ID="GroupCode" />
            </div>
            <div class="form-text">Alphanumeric code that uniquely identifies this contact group.</div>
        </div>

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeGroupCategory" ToolTip="Change Group Category" NavigateUrl="#" />
            </div>
            <label class="form-label">
                Group Category
            </label>
            <div>
                <asp:Literal runat="server" ID="GroupCategory" />
            </div>
            <div class="form-text">Classification or tag for this group</div>
        </div>

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeGroupStatus" ToolTip="Change Group Status" NavigateUrl="#" />
            </div>
            <label class="form-label">
                Group Status
            </label>
            <div>
                <asp:Literal runat="server" ID="GroupStatus" />
            </div>
            <div class="form-text">The current status of this group.</div>
        </div>

    </div>

    <div class="col-5">

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeParent" ToolTip="Change Parent" NavigateUrl="#" />
            </div>
            <label class="form-label">
                Parent (Hierarchy)
            </label>
            <div>
                <asp:HyperLink runat="server" ID="ParentLink" />
                <asp:Literal ID="NoParent" runat="server" />
            </div>
            <div class="form-text">
                You can select another group as the container for this contact group. 
                This is optional, and it allows you to nest groups into subgroups.
            </div>
        </div>

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink runat="server" ID="OrganizeLink" Name="pencil" ToolTip="Organize Supergroups" />
            </div>
            <label class="form-label">
                Parents (Functional)
            </label>
            <div>
                <asp:Repeater runat="server" ID="ParentContainmentRepeater">
                    <HeaderTemplate>
                        <ul style="padding-left:23px; margin-bottom:0; margin-top:10px;">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <asp:HyperLink runat="server" NavigateUrl='<%# Eval("GroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>' Text='<%# Eval("GroupName") %>' />
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Literal runat="server" ID="ParentContainmentMessage" Visible="false" />
            </div>
        </div>

        <div class="form-group mb-3" runat="server" ID="ChildrenField">
            <label class="form-label">Children (Hierarchy)</label>
            <div style="margin-left:5px;">
                <asp:Repeater runat="server" ID="ChildrenRepeater">
                    <HeaderTemplate>
                        <ul style="padding-left:23px;">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <asp:HyperLink runat="server" NavigateUrl='<%# Eval("GroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>' Text='<%# Eval("GroupName") %>' />
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>

        <div class="form-group mb-3" runat="server" ID="ChildrenContainmentField">
            <label class="form-label">Subgroups (Functional)</label>
            <div>
                <asp:Repeater runat="server" ID="ChildrenContainmentRepeater">
                    <HeaderTemplate>
                        <ul style="padding-left:23px; margin-bottom:0; margin-top:10px;">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <asp:HyperLink runat="server" NavigateUrl='<%# Eval("GroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>' Text='<%# Eval("GroupName") %>' />
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeCapacity" ToolTip="Change Capacity" NavigateUrl="#" />
            </div>
            <label class="form-label">Capacity</label>
            <div>
                <asp:Literal runat="server" ID="Capacity" />
            </div>
            <div class="form-text">The maximum number of people permitted in this group.</div>
        </div>

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeGroupOptions" ToolTip="Change Group Options" NavigateUrl="#" />
            </div>
            <label class="form-label">Group Options</label>
            <div>
                <asp:CheckBox runat="server" ID="AddNewUsersAutomatically" Text="Automatically add new users to this group" Enabled="false" />
                <div>
                    <asp:CheckBox runat="server" ID="AllowSelfSubscription" Text="Allow users to subscribe/unsubscribe themselves" Enabled="false" />
                </div>
            </div>
        </div>

    </div>

    <div runat="server" id="OfficePhonePanel" class="col-2">

        <div runat="server" id="GroupRegionField" class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeGroupRegion" ToolTip="Change Region" NavigateUrl="#" />
            </div>
            <label class="form-label">Region</label>
            <div>
                <asp:Literal runat="server" ID="GroupRegion" />
            </div>
            <div class="form-text">Geographical area associated with the group.</div>
        </div>

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeGroupOffice" ToolTip="Change Office" NavigateUrl="#" />
            </div>
            <label class="form-label">
                Office
            </label>
            <div>
                <asp:Literal runat="server" ID="GroupOffice" />
            </div>
            <div class="form-text">Office name/location.</div>
        </div>

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeGroupPhone" ToolTip="Change Phone" NavigateUrl="#" />
            </div>
            <label class="form-label">
                Phone
            </label>
            <div>
                <asp:Literal runat="server" ID="GroupPhone" />
            </div>
            <div class="form-text">Main office telephone number.</div>
        </div>

    </div>

</div>
