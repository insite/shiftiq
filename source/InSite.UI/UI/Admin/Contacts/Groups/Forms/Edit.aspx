<%@ Page CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Contacts.Groups.Forms.Edit" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/GroupOccupations.ascx" TagName="GroupOccupations" TagPrefix="uc" %>
<%@ Register Src="../Controls/PhotoSection.ascx" TagName="PhotoSection" TagPrefix="uc" %>
<%@ Register Src="../../Addresses/Controls/AddressList.ascx" TagName="AddressList" TagPrefix="uc" %>
<%@ Register Src="../Controls/RoleGrid.ascx" TagName="RoleGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Accounts/Permissions/Controls/PermissionGrid.ascx" TagName="PermissionGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementListEditor.ascx" TagName="AchievementListEditor" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <insite:Alert runat="server" ID="ScreenStatus" />
            <insite:ValidationSummary runat="server" ValidationGroup="Group" />
            <insite:ValidationSummary runat="server" ValidationGroup="GroupPhotosUpload" />
        </ContentTemplate>
    </insite:UpdatePanel>

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="GroupSection" Title="Group" Icon="far fa-users" IconPosition="BeforeText">
            <div class="row mt-3">
                <div class="col-lg-8 mb-3 mb-lg-0">

                    <div class="card border-0 shadow-lg">
                        <div class="card-body">
                            <div class="row">
                                <div class="col">
                                    <h4 class="card-title mb-3">Group</h4>
                                </div>
                                <div class="col text-end">
                                    <insite:Button runat="server" ID="HistoryButton"
                                        ButtonStyle="Default"
                                        Text="History"
                                        Icon="fas fa-history"
                                    />
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Group Name
                                            <insite:RequiredValidator runat="server" ControlToValidate="GroupName" FieldName="Group Name" ValidationGroup="Group" />
                                        </label>
                                        <insite:TextBox runat="server" ID="GroupName" MaxLength="90" />
                                        <div class="form-text">
                                            The name of the group should be unique and descriptive.
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Group Type
                                            <insite:RequiredValidator runat="server" ControlToValidate="GroupType" FieldName="Group Type" ValidationGroup="Group" />
                                        </label>
                                        <insite:GroupTypeComboBox runat="server" ID="GroupType" AllowBlank="false" />
                                        <div class="form-text">
                                            Group type determines the features available for using and managing it in the system.
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Group Tag
                                        </label>
                                        <insite:TextBox runat="server" ID="GroupLabel" MaxLength="100" />
                                        <div class="form-text">
                                            Optional: You can tag contact groups to make them easier to find.
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Industry
                                        </label>
                                        <insite:IndustriesComboBox runat="server" ID="GroupIndustry" DistinctValuesOnly="true"/>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Description
                                        </label>
                                        <insite:TextBox runat="server" ID="GroupDescription" TextMode="MultiLine" Rows="3" />
                                        <div class="form-text">
                                            Comment or note to describe the purpose of the group.
                                        </div>
                                    </div>

                                    <div runat="server" id="GroupCodeField" class="form-group mb-3">
                                        <label class="form-label">
                                            Group Code
                                        </label>
                                        <insite:TextBox runat="server" ID="GroupCode" MaxLength="32" />
                                        <div class="form-text">
                                            Alphanumeric code that uniquely identifies this contact group.
                                        </div>
                                    </div>

                                    <div runat="server" id="GroupCategoryField" class="form-group mb-3">
                                        <label class="form-label">
                                            Group Category
                                        </label>
                                        <insite:TextBox runat="server" ID="GroupCategory" MaxLength="120" />
                                        <div class="form-text">
                                            Classification or tag for this group.
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Group Status
                                        </label>
                                        <insite:MultiField runat="server">

                                            <insite:MultiFieldView runat="server" ID="GroupStatusItemSelectorView" Inputs="GroupStatusItemIdentifier">
                                                <span class="multi-field-input">
                                                    <insite:CollectionItemComboBox runat="server" ID="GroupStatusItemIdentifier" EnableSearch="true" />
                                                </span>
                                                <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                    ButtonStyle="Default" Icon="far fa-keyboard" ToolTip="Enter a value manually" />
                                            </insite:MultiFieldView>

                                            <insite:MultiFieldView runat="server" ID="GroupStatusItemTextView" Inputs="GroupStatusItemText">
                                                <span class="multi-field-input">
                                                    <insite:TextBox runat="server" ID="GroupStatusItemText" MaxLength="100" />
                                                </span>
                                                <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                    ButtonStyle="Default" Icon="far fa-list-ul" ToolTip="Select an option from the list" />
                                            </insite:MultiFieldView>

                                        </insite:MultiField>
                                        
                                        <div class="form-text">
                                            The current status of this group.
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Number of Employees
                                        </label>
                                        <insite:NumberOfEmployeesComboBox runat="server" ID="NumberOfEmployees" />
                                    </div>
                                </div>

                                <div class="col-md-6">
                                    <div runat="server" id="ParentField" class="form-group mb-3">
                                        <label class="form-label">
                                            Parent (Hierarchy)
                                            <insite:IconLink runat="server" ID="ParentLink" Name="pencil" ToolTip="Edit Parent" />
                                        </label>
                                        <insite:FindGroup ID="GroupParentID" runat="server" />
                                        <div class="form-text">
                                            You can select another group as the container for this contact group.
                                            This is optional, and it allows you to nest groups into subgroups.
                                        </div>
                                    </div>

                                    <div runat="server" id="ParentsField" class="form-group mb-3">
                                        <label class="form-label">
                                            Parents (Functional)
                                            <insite:IconLink runat="server" ID="OrganizeLink" Name="pencil" ToolTip="Organize Supergroups" />
                                        </label>
                                        <div>
                                            <asp:Repeater runat="server" ID="ParentContainmentRepeater">
                                                <HeaderTemplate>
                                                    <ul style="padding-left:23px; margin-bottom:0; margin-top:10px;">
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <li>
                                                        <asp:HyperLink runat="server" Text='<%# Eval("GroupName") %>'
                                                            NavigateUrl='<%# Eval("GroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>' />
                                                    </li>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    </ul>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                            <asp:Literal runat="server" ID="ParentContainmentMessage" Visible="false" />
                                        </div>
                                    </div>

                                    <div runat="server" ID="ChildrenField" class="form-group mb-3">
                                        <label class="form-label">
                                            Children (Hierarchy)
                                        </label>
                                        <div class="ms-1">
                                            <asp:Repeater runat="server" ID="ChildrenRepeater">
                                                <HeaderTemplate>
                                                    <ul style="padding-left:23px;">
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <li>
                                                        <asp:HyperLink runat="server" Text='<%# Eval("GroupName") %>'
                                                            NavigateUrl='<%# Eval("GroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>' />
                                                    </li>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    </ul>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>

                                    <div runat="server" ID="ChildrenContainmentField" class="form-group mb-3">
                                        <label class="form-label">
                                            Subgroups (Functional)
                                        </label>
                                        <div>
                                            <asp:Repeater runat="server" ID="ChildrenContainmentRepeater">
                                                <HeaderTemplate>
                                                    <ul style="padding-left:23px; margin-bottom:0; margin-top:10px;">
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <li>
                                                        <asp:HyperLink runat="server" Text='<%# Eval("GroupName") %>'
                                                            NavigateUrl='<%# Eval("GroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>' />
                                                    </li>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    </ul>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>

                                    <div runat="server" id="CapacityField" class="form-group mb-3">
                                        <label class="form-label">
                                            Capacity
                                        </label>
                                        <insite:NumericBox runat="server" ID="Capacity" NumericMode="Integer" MinValue="1" />
                                        <div class="form-text">
                                            The maximum number of people permitted in this group.
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Group Options
                                        </label>
                                        <div>
                                            <asp:CheckBox runat="server" ID="AddNewUsersAutomatically" Text="Automatically add new users to this group" />
                                        </div>
                                        <div>
                                            <asp:CheckBox runat="server" ID="AllowSelfSubscription" Text="Allow users to subscribe/unsubscribe themselves" />
                                        </div>
                                        <div>
                                            <asp:CheckBox runat="server" ID="AllowJoinGroupUsingLink" Text="Allow existing users to join a group using a link in content" />
                                        </div>
                                        <div runat="server" id="OnlyOperatorCanAddUserField">
                                            <asp:CheckBox runat="server" ID="OnlyOperatorCanAddUser" Text="Do not allow adding users to the group for non-operators" />
                                        </div>
                                    </div>

                                    <div runat="server" id="GroupTagsField" class="form-group mb-3">
                                        <label class="form-label">
                                            Group Tags
                                        </label>
                                        <asp:CheckBoxList runat="server" ID="GroupTagList" DataValueField="ItemIdentifier" DataTextField="ItemName" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Group Expiry</label>
                                        <div>
                                            <insite:DateTimeOffsetSelector runat="server" ID="GroupExpiry" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Group Membership Lifetime</label>
                                        <div class="row">
                                            <div class="col-sm-4">
                                                <insite:NumericBox runat="server" ID="LifetimeQuantity" MaxValue="999" NumericMode="Integer" />
                                            </div>
                                            <div class="col-sm-8">
                                                <insite:ComboBox runat="server" ID="LifetimeUnit">
                                                    <Items>
                                                        <insite:ComboBoxOption Text="Days" Value="Day" />
                                                        <insite:ComboBoxOption Text="Weeks" Value="Week" />
                                                        <insite:ComboBoxOption Text="Months" Value="Month" />
                                                    </Items>
                                                </insite:ComboBox>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>

                        </div>
                    </div>

                </div>
                <div class="col-lg-4">

                    <div runat="server" id="GroupContactCard" class="card border-0 shadow-lg">
                        <div class="card-body">
                            <h4 class="card-title mb-3">Contact</h4>

                            <div runat="server" id="GroupContactRegionField" class="form-group mb-3">
                                <label class="form-label">
                                    Region
                                </label>
                                <insite:ItemNameComboBox runat="server" ID="GroupRegion" />
                                <div class="form-text">
                                    Geographical area associated with the group.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Office
                                </label>
                                <insite:TextBox runat="server" ID="GroupOffice" MaxLength="30" />
                                <div class="form-text">
                                    Office name/location.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Phone
                                </label>
                                <insite:TextBox runat="server" ID="GroupPhone" MaxLength="30" />
                                <div class="form-text">
                                    Main office telephone number.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Email
                                </label>
                                <insite:TextBox runat="server" ID="GroupEmail" MaxLength="254" />
                                <div class="form-text">
                                    Main office email.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Web Site URL
                                </label>
                                <insite:TextBox runat="server" ID="GroupURL" MaxLength="500" />
                                <div class="form-text">
                                    Main office web site.
                                </div>
                            </div>

                        </div>
                    </div>
                
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="WorkflowSection" Title="Workflow" Icon="far fa-code-branch" IconPosition="BeforeText">
            <div class="row mt-3">
                <div class="col-lg-6 mb-3 mb-lg-0">
                    <div class="card border-0 shadow-lg">
                        <div class="card-body">
                            
                            <h4 class="card-title mb-3">Notifications</h4>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Group Membership Started - Notification to Administrator(s)
                                </label>
                                <insite:FindMessage runat="server" ID="MessageToAdminWhenMembershipStarted" Filter-Type="Notification" Filter-SenderType="Mailgun" />
                                <div class="form-text">
                                    Send an email message to administrators when someone joins this group.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Group Membership Ended - Notification to Administrator(s)
                                </label>
                                <insite:FindMessage runat="server" ID="MessageToAdminWhenMembershipEnded" Filter-Type="Notification" Filter-SenderType="Mailgun" />
                                <div class="form-text">
                                    Send an email message to administrators when someone leaves this group.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Group Membership Started - Notification to User
                                </label>
                                <insite:FindMessage runat="server" ID="MessageToUserWhenMembershipStarted" Filter-Type="Notification" Filter-SenderType="Mailgun" />
                                <div class="form-text">
                                    Send an email message to someone who joins this group.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Group Membership Ended - Notification to User
                                </label>
                                <insite:FindMessage runat="server" ID="MessageToUserWhenMembershipEnded" Filter-Type="Notification" Filter-SenderType="Mailgun" />
                                <div class="form-text">
                                    Send an email message to someone who leaves this group.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Event Venue Changed - Notification to Administrator(s)
                                </label>
                                <insite:FindMessage runat="server" ID="MessageToAdminWhenEventVenueChanged" />
                                <div class="form-text">
                                    Send an email message to administrators when this group is selected as the venue for an event.
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="card border-0 shadow-lg mb-3">
                        <div class="card-body">
                            <h4 class="card-title mb-3">Forms</h4>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Mandatory Form Submission
                                </label>
                                <insite:FindWorkflowForm runat="server" ID="MandatorySurveyFormIdentifier" />
                                <div class="form-text">
                                    Every user who joins this group must submit a submission to the selected form.
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card border-0 shadow-lg">
                        <div class="card-body">
                            <h4 class="card-title mb-3">Sales</h4>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Invoice Product
                                </label>
                                <insite:FindProduct runat="server" ID="MembershipProduct" />
                                <div class="form-text">
                                    Generate invoice when a new user is joined the group
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="OccupationSection" Title="Occupations" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <div class="mt-3">
                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="OccupationUpdatePanel" />

                <insite:UpdatePanel runat="server" ID="OccupationUpdatePanel">
                    <ContentTemplate>
                        <uc:GroupOccupations runat="server" ID="GroupOccupations" />
                    </ContentTemplate>
                </insite:UpdatePanel>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="PhotoSection" Title="Photos" Icon="far fa-images" IconPosition="BeforeText">
            <div class="mt-3">
                <uc:PhotoSection runat="server" ID="Photos" />
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="AddressSection" Title="Address" Icon="far fa-home" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">
                    <uc:AddressList runat="server" ID="AddressList" ContactType="Group" ValidationGroup="Group" />
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="RoleSection" Title="People" Icon="far fa-user" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">
                    <uc:RoleGrid runat="server" ID="RoleGrid" />
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="AchievementTab" Title="Achievements" Icon="far fa-trophy" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Achievements</h2>
                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:AchievementListEditor ID="AchievementEditor" runat="server" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="PermissionSection" Title="Permissions" Icon="far fa-key" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">
                    <uc:PermissionGrid runat="server" ID="PermissionGrid" />
                </div>
            </div>
        </insite:NavItem>

    </insite:Nav>

    <div class="row mt-3">
        <div class="col-lg-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Group" />
            <insite:DeleteButton runat="server" ID="DeleteButton" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>
</asp:Content>

