<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Contacts.People.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-3">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>

            <div class="mb-2">
                <insite:TextBox runat="server" ID="FullName" EmptyMessage="Contact Full Name" MaxLength="256" />
            </div>

            <div class="mb-2">
                <insite:TextBox runat="server" ID="FirstName" EmptyMessage="Contact First Name" MaxLength="40" />
            </div>

            <div class="mb-2">
                <insite:TextBox runat="server" ID="LastName" EmptyMessage="Contact Last Name" MaxLength="40" />
            </div>

            <div class="mb-2">
                <insite:ComboBox ID="NameFilterType" runat="server">
                    <Items>
                        <insite:ComboBoxOption Value="Exact" Text="Match Exact Spelling" />
                        <insite:ComboBoxOption Value="Similar" Text="Similar Pronunciation" />
                    </Items>
                </insite:ComboBox>
            </div>

            <div class="mb-2">
                <insite:TextBox runat="server" ID="Code" EmptyMessage="Person Code" MaxLength="256" />
            </div>

            <div class="mb-2">
                <insite:TextBox ID="CommentKeyword" runat="server" EmptyMessage="Comment" MaxLength="256" />
            </div>

            <div class="mb-2">
                <insite:GenderComboBox runat="server" ID="Gender" AllowBlank="true" EmptyMessage="Gender" />
            </div>

            <div class="mb-2">
                <insite:ComboBox ID="Issues" runat="server" EmptyMessage="Cases">
                    <Items>
                        <insite:ComboBoxOption Selected="true" />
                        <insite:ComboBoxOption Value="OpenIssueAssignee" Text="Open Case Assignee" />
                        <insite:ComboBoxOption Value="ClosedIssueAssignee" Text="Closed Case Assignee" />
                        <insite:ComboBoxOption Value="OpenIssueAdministrator" Text="Open Case Administrator" />
                        <insite:ComboBoxOption Value="ClosedIssueAdministrator" Text="Closed Case Administrator" />
                    </Items>
                </insite:ComboBox>
            </div>

            <div class="mb-2" runat="server">
                <insite:MultiComboBox runat="server" ID="CommentFlags" EmptyMessage="Comments Flag">
                    <Items>
                        <insite:ComboBoxOption Value="1" Text="Posted" />
                        <insite:ComboBoxOption Value="2" Text="Flagged" />
                        <insite:ComboBoxOption Value="3" Text="Submitted" />
                        <insite:ComboBoxOption Value="4" Text="Resolved" />
                    </Items>
                </insite:MultiComboBox>
            </div>

            <h4 class="mt-3">Email</h4>

            <div class="mb-2">
                <insite:TextBox runat="server" ID="Email" EmptyMessage="Email" MaxLength="256" />
            </div>

            <div class="mb-2">
                <insite:TextBox runat="server" ID="EmailAlternate" EmptyMessage="Email Alternate" MaxLength="256" />
            </div>

            <div class="mb-2" id="EmailEnabledField" runat="server">
                <insite:ComboBox ID="EmailEnabled" runat="server" EmptyMessage="Email Enabled">
                    <Items>
                        <insite:ComboBoxOption />
                        <insite:ComboBoxOption Value="True" Text="Email Enabled" />
                        <insite:ComboBoxOption Value="False" Text="Email Disabled" />
                    </Items>
                </insite:ComboBox>
            </div>

            <div class="mb-2" id="EmailStatusField" runat="server">
                <insite:ComboBox ID="EmailStatus" runat="server" EmptyMessage="Email Validity">
                    <Items>
                        <insite:ComboBoxOption />
                        <insite:ComboBoxOption Value="Valid Email" Text="Valid Email" />
                        <insite:ComboBoxOption Value="Invalid Email" Text="Invalid Email" />
                    </Items>
                </insite:ComboBox>
            </div>

            <div class="mb-2">
                <insite:ComboBox ID="EmailVerified" runat="server" EmptyMessage="Email Verification">
                    <Items>
                        <insite:ComboBoxOption />
                        <insite:ComboBoxOption Value="True" Text="Email Verified" />
                        <insite:ComboBoxOption Value="False" Text="Email Not Verified" />
                    </Items>
                </insite:ComboBox>
            </div>
        </div>
            <div class="mb-2">
            <insite:FilterButton runat="server" ID="SearchButton" />
            <insite:ClearButton runat="server" ID="ClearButton" />
        </div>
    </div>

    <div class="col-3">
        <h4>Employment</h4>

        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="EmployerUpdatePanel" />

        <insite:UpdatePanel runat="server" ID="EmployerUpdatePanel">
            <ContentTemplate>
                <div class="mb-2">
                    <insite:FindGroup ID="EmployerParentGroupID" runat="server" EmptyMessage="Employer Group Parent" CurrentOrganizationOnly="true" />
                </div>
                <div class="mb-2">
                    <insite:FindGroup ID="EmployerGroupID" runat="server" EmptyMessage="Employer Group" CurrentOrganizationOnly="true" />
                </div>
            </ContentTemplate>
        </insite:UpdatePanel>

        <div class="mb-2">
            <insite:TextBox ID="JobTitle" runat="server" EmptyMessage="Job Title" MaxLength="256" />
        </div>

        <h4 class="mt-3">Status</h4>

        <div class="mb-2" runat="server">
            <insite:ItemIdMultiComboBox runat="server" ID="AccountStatusId" EmptyMessage="Account Status" />
        </div>

        <div class="mb-2">
            <insite:ComboBox ID="IsApproved" runat="server" EmptyMessage="User Status">
                <Items>
                    <insite:ComboBoxOption />
                    <insite:ComboBoxOption Value="True" Text="Access Granted" />
                    <insite:ComboBoxOption Value="False" Text="Access Not Granted" />
                </Items>
            </insite:ComboBox>
        </div>

        <div class="mb-2">
            <insite:ComboBox ID="IsArchived" runat="server" EmptyMessage="Archive Status">
                <Items>
                    <insite:ComboBoxOption />
                    <insite:ComboBoxOption Value="True" Text="Archived" />
                    <insite:ComboBoxOption Value="False" Text="Not Archived" />
                </Items>
            </insite:ComboBox>
        </div>

        <div class="mb-2">
            <insite:ComboBox ID="IsMultiOrganization" runat="server" EmptyMessage="Multi-Organization">
                <Items>
                    <insite:ComboBoxOption />
                    <insite:ComboBoxOption Value="True" Text="Multi-Organization Users Only" />
                </Items>
            </insite:ComboBox>
        </div>

        <div class="mb-2">
            <insite:OrganizationRoleMultiComboBox ID="OrganizationRole" runat="server" EmptyMessage="Organization Role" />
        </div>

        <h4 class="mt-3">Achievements</h4>

        <div class="mb-2" runat="server">
            <insite:FindAchievement runat="server" ID="ValidAchievementIdentifier" />
        </div>
    </div>

    <div class="col-3">
        <h4>Address</h4>

        <div class="mb-2">
            <insite:PersonAddressTypeMultiComboBox runat="server" ID="AddressType" EmptyMessage="Address Type" />
        </div>

        <div class="mb-2">
            <insite:OrganizationPersonCountryComboBox runat="server" ID="Country" EmptyMessage="Country" />
        </div>

        <div class="mb-2">
            <insite:OrganizationPersonProvinceMultiComboBox runat="server" ID="Province" EmptyMessage="Province" />
        </div>

        <div class="mb-2">
            <insite:OrganizationPersonCityMultiComboBox runat="server" ID="City" EmptyMessage="City" />
        </div>

        <div class="mb-2">
            <insite:TextBox runat="server" ID="Phone" EmptyMessage="Phone" MaxLength="32" />
        </div>

        <div class="mb-2" runat="server" id="RegionField">
            <insite:ItemNameComboBox runat="server" ID="Region" EmptyMessage="Region" />
        </div>

        <h4 class="mt-3">Groups</h4>

        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="GroupsUpdatePanel" />

        <insite:UpdatePanel runat="server" ID="GroupsUpdatePanel">
            <ContentTemplate>

                <div class="mb-2">
                    <insite:FindGroup ID="MembershipGroupID" runat="server" EmptyMessage="Group" />
                </div>

                <div class="mb-2" id="GroupMembershipDateField" runat="server">
                    <insite:DateTimeOffsetSelector ID="GroupMembershipDate" runat="server" EmptyMessage="Group Assigned After" />
                </div>

                <div class="mb-2">
                    <insite:FindGroup ID="ExcludeMembershipGroupID" runat="server" EmptyMessage="Exclude Group" />
                </div>

            </ContentTemplate>
        </insite:UpdatePanel>
    </div>

    <div class="col-3">
        <div class="mb-2">
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>

        <div class="mb-2">
            <insite:ComboBox ID="SortColumns" runat="server">
                <Items>
                    <insite:ComboBoxOption Text="Sort by Full Name" Value="FullName" />
                    <insite:ComboBoxOption Text="Sort by Last Name, First Name" Value="LastName, FirstName" />
                    <insite:ComboBoxOption Text="Sort by Date Created" Value="Created DESC" />
                    <insite:ComboBoxOption Text="Sort by Membership Status, Full Name" Value="StatusText, FullName" />
                    <insite:ComboBoxOption Text="Sort by District, Last Name, First Name" Value="EmployerDistrictName, LastName, FirstName" />
                    <insite:ComboBoxOption Text="Sort by District, Employer, Full Name" Value="EmployerDistrictName, EmployerGroupName, FullName" />
                    <insite:ComboBoxOption Text="Sort by District, Membership Status, Full Name" Value="EmployerDistrictName, StatusText, FullName" />
                    <insite:ComboBoxOption Text="Sort by Region, Employer" Value="EmployerGroupRegion, EmployerGroupName" />
                </Items>
            </insite:ComboBox>
        </div>

        <div>
            <h4 class="mt-3">Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
         <h4 class="mt-3">Timestamps</h4>
        <div class="mb-2">
            <insite:DateTimeOffsetSelector ID="CreatedSince" runat="server" EmptyMessage="Created Since" />
        </div>
        <div class="mb-2">
            <insite:DateTimeOffsetSelector ID="CreatedBefore" runat="server" EmptyMessage="Created Before" />
        </div>
           <div class="mb-2">
            <insite:DateTimeOffsetSelector ID="ModifiedSince" runat="server" EmptyMessage="Modified Since" />
        </div>
          <div class="mb-2">
            <insite:DateTimeOffsetSelector ID="ModifiedBefore" runat="server" EmptyMessage="Modified Before" />
        </div>
        <div class="mb-2">
            <insite:DateTimeOffsetSelector runat="server" ID="LastAuthenticatedSince" EmptyMessage="Last Authenticated Since" />
        </div>
         <div class="mb-2">
            <insite:DateTimeOffsetSelector runat="server" ID="LastAuthenticatedBefore" EmptyMessage="Last Authenticated Before" />
        </div>
    </div>
</div>
