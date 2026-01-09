<%@ Page Language="C#" CodeBehind="AssignFollowers.aspx.cs" Inherits="InSite.Cmds.Admin.Workflows.Followers.Assign" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="SubscriberGrid" Src="FollowerSubscriberGrid.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <insite:Alert runat="server" ID="ScreenStatus" />
            <insite:ValidationSummary runat="server" ValidationGroup="Assignment" />
        </ContentTemplate>
    </insite:UpdatePanel>

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-users me-1"></i>
            Followers
        </h2>

        <div class="row">
            <div class="col-lg-6">
                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="CriteriaUpdatePanel" />
                        <insite:UpdatePanel runat="server" ID="CriteriaUpdatePanel">
                            <ContentTemplate>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Organization
                                        <insite:RequiredValidator runat="server" ControlToValidate="OrganizationIdentifier" FieldName="Organization" ValidationGroup="Assignment" />
                                    </label>
                                    <insite:AuthorizedOrganizationComboBox runat="server" ID="OrganizationIdentifier" AllowBlank="false" Enabled="false" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Department
                                        <insite:RequiredValidator runat="server" ControlToValidate="DepartmentIdentifier" FieldName="Department" ValidationGroup="Assignment" />
                                    </label>
                                    <insite:FindDepartment runat="server" ID="DepartmentIdentifier" />
                                </div>

                                <div runat="server" id="EmploymentTypeField" visible="false" class="form-group mb-3">
                                    <label class="form-label">
                                        Employment Type
                                    </label>
                                    <div>
                                        <asp:CheckBoxList runat="server" ID="EmploymentType" RepeatDirection="Horizontal" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Notification
                                        <insite:RequiredValidator runat="server" ControlToValidate="MessageIdentifier" FieldName="Notification" ValidationGroup="Assignment" />
                                    </label>
                                    <insite:ComboBox ID="MessageIdentifier" runat="server">
                                        <Items>
                                            <insite:ComboBoxOption Value="9e0b9a5c-5e10-4161-b9dd-b94c3c86a61e" Text="Achievements Expired" Selected="True" />
                                            <insite:ComboBoxOption Value="90899182-5d9c-4997-972f-832fdf0cfeeb" Text="Achievements Expiring in 1 Month" />
                                            <insite:ComboBoxOption Value="c42afaad-fc11-421b-900d-7fd6e04d83f5" Text="Achievements Expiring in 2 Months" />
                                            <insite:ComboBoxOption Value="d1ddd7d7-2c9e-44d4-b174-f08f3ca3bdda" Text="Achievements Expiring in 3 Months" />
                                            <insite:ComboBoxOption Value="E6EE3474-ADEB-4655-BBC6-A898002C04C9" Text="Competencies Expired" />
                                        </Items>
                                    </insite:ComboBox>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Follower
                                        <insite:RequiredValidator runat="server" ControlToValidate="FollowerID" FieldName="Follower" ValidationGroup="Assignment" />
                                    </label>
                                    <insite:FindPerson runat="server" ID="FollowerID" />
                                </div>

                                <div class="mt-3">
                                    <insite:FilterButton runat="server" ID="SearchButton" CausesValidation="true" ValidationGroup="Assignment" />
                                </div>

                            </ContentTemplate>
                        </insite:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>

        <div class="position-relative">
            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ResultUpdatePanel" />
            <insite:UpdatePanel runat="server" ID="ResultUpdatePanel">
                <ContentTemplate>
                    <div runat="server" id="ResultCard" class="card border-0 shadow-lg mt-3" visible="false">
                        <div class="card-body">

                            <uc:SubscriberGrid runat="server" ID="SubscriberGrid" />

                        </div>
                    </div>
                </ContentTemplate>
            </insite:UpdatePanel>
        </div>

    </section>

    <div class="mt-3">
        <insite:CloseButton runat="server" NavigateUrl="/ui/admin/tools" />
    </div>

</asp:Content>