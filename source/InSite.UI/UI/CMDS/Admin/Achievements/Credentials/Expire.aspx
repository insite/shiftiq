<%@ Page Language="C#" CodeBehind="Expire.aspx.cs" Inherits="InSite.Cmds.Actions.BulkTool.Expiry.Achievement" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">



</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />

    <section class="mb-3">
        
        <div class="card border-0 shadow-lg">

            <div class="card-body">

                <div class="row">

                    <div class="col-lg-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Achievement
                                <insite:RequiredValidator runat="server" ControlToValidate="AchievementIdentifier" ValidationGroup="Bulk" />
                            </label>
                            <div>
                                <cmds:FindAchievement ID="AchievementIdentifier" runat="server" />
                            </div>
                            <div class="form-text">
                                Only achievements assigned to your organization are available for bulk-expiration.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Scope
                            </label>
                            <div>
                                <insite:ComboBox ID="OperationScope" runat="server">
                                    <Items>
                                        <insite:ComboBoxOption Value="Person" Text="Person" />
                                        <insite:ComboBoxOption Value="Department" Text="Department" />
                                        <insite:ComboBoxOption Value="Organization" Text="Organization" />
                                    </Items>
                                </insite:ComboBox>
                            </div>
                            <div class="form-text">
                                Do you want to expire this achievement for a person, department, or organization?
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                <asp:Literal ID="Subject" runat="server" />
                                <insite:RequiredValidator ID="PersonRequired" runat="server" ControlToValidate="PersonFinder" ValidationGroup="Bulk" />
                                <insite:RequiredValidator ID="DepartmentRequired" runat="server" ControlToValidate="DepartmentFinder" ValidationGroup="Bulk" />
                                <insite:RequiredValidator ID="OrganizationRequired" runat="server" ControlToValidate="OrganizationFinder" ValidationGroup="Bulk" />
                            </label>
                            <div>
                                <cmds:FindPerson ID="PersonFinder" runat="server" Enabled="False" />
                                <cmds:FindDepartment ID="DepartmentFinder" runat="server" Enabled="False" />
                                <cmds:FindCompany ID="OrganizationFinder" runat="server" Enabled="False" />
                            </div>
                            <div class="form-text">
                                
                            </div>
                        </div>

                        <div runat="server" id="CredentialStatus" visible="false" class="form-group mb-3">
                            <label class="form-label">
                                Credential Status
                            </label>
                            <div>
                                <insite:CheckBox runat="server" ID="CredentialStatusPending" Text="Pending" />
                                <insite:CheckBox runat="server" ID="CredentialStatusValid" Text="Valid" />
                                <insite:CheckBox runat="server" ID="CredentialStatusExpired" Text="Expired" />
                            </div>
                        </div>

                        <div runat="server" id="GradebookStatus" visible="false" class="form-group mb-3">
                            <label class="form-label">
                                Gradebook Status
                            </label>
                            <div>
                                <insite:CheckBox runat="server" ID="GradebookStatusEnrollment" Text="Course Enrollments" />
                            </div>
                            <div class="form-text">
                                Do you want to restart course enrollments for expired achievements? 
                                This will reset learners' progress in the gradebook for the course.
                            </div>
                        </div>

                    </div>

                </div>

            </div>

        </div>

    </section>

    <insite:Button ID="ExpireButton" runat="server"
        Icon="fas fa-alarm-clock"
        Text="Expire"
        ButtonStyle="Primary"
        ValidationGroup="Bulk"
        ConfirmText="Are you sure you want to bulk-expire the selected achievement?"
        DisableAfterClick="true"
    />

    <insite:CancelButton runat="server" NavigateUrl="/ui/admin/tools" />

</asp:Content>
