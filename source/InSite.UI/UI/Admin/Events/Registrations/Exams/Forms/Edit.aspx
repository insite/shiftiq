<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Events.Candidates.Forms.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <insite:Alert runat="server" ID="ScreenStatus" />
            <insite:ValidationSummary runat="server" ValidationGroup="Registration" />
            <insite:ValidationSummary runat="server" ValidationGroup="Accommodation" />
        </ContentTemplate>
    </insite:UpdatePanel>

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="RegistrationTab" Title="Registration" Icon="far fa-id-card" IconPosition="BeforeText">
            <div class="mt-3">
                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="row">

                            <div class="col-lg-4 mb-3">
                                <div class="card h-100">
                                    <div class="card-body">

                                        <h4 class="card-title mb-3">
                                            Exam Event
                                        </h4>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Exam Title
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="EventTitle" /> 
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Scheduled Date and Time
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="EventStartTime" /> 
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Venue/Location
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="EventVenue" /> 
                                                <span class="form-text" runat="server" id="EventVenueRoom"></span>
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Exam Number
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="EventNumber" /> 
                                                <span class="form-text">
                                                    <asp:Literal runat="server" ID="EventFormat" />
                                                </span>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-4 mb-3">
                                <div class="card h-100">
                                    <div class="card-body">

                                        <h4 class="card-title mb-3">
                                            Exam Form
                                        </h4>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Form Title
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="FormTitle" /> 
                                            </div>
                                        </div>

                                        <div runat="server" id="FormNameField" class="form-group mb-3">
                                            <label class="form-label">
                                                Form Name
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="FormName" /> 
                                            </div>
                                        </div>

                                        <div runat="server" id="FormCodeField" class="form-group mb-3">
                                            <label class="form-label">
                                                Form Code
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="FormCode" /> 
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-4 mb-3">
                                <div class="card h-100">
                                    <div class="card-body">

                                        <h4 class="card-title mb-3">
                                            Exam Candidate
                                        </h4>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Candidate Type
                                            </label>
                                            <insite:ItemNameComboBox runat="server" ID="CandidateType" Settings-CollectionName="Registrations/Exams/Candidate/Type" AllowBlank="true" />
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Exam Password
                                                <insite:RequiredValidator runat="server" ControlToValidate="RegistrationPassword" FieldName="Registration Password" ValidationGroup="Registration" />
                                            </label>
                                            <insite:TextBox runat="server" ID="RegistrationPassword" MaxLength="14"/>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Attendance Status
                                            </label>
                                            <div>
                                                <insite:ItemNameComboBox runat="server" ID="SettingsAttendanceStatus">
                                                    <Settings UseCurrentOrganization="true" CollectionName="Registrations/Attendance/Status" />
                                                </insite:ItemNameComboBox>
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Time Limit
                                            </label>
                                            <insite:UpdatePanel runat="server" ID="CandidateTimeLimitUpdatePanel">
                                                <ContentTemplate>
                                                    <asp:Literal runat="server" ID="CandidateTimeLimit" /> 
                                                </ContentTemplate>
                                            </insite:UpdatePanel>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Grading Status
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="GradingStatus" /> 
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-4 mb-3">
                                <div class="card h-100">
                                    <div class="card-body">

                                        <h4 class="card-title mb-3">
                                            Eligibility (Prerequisite Checks)
                                        </h4>

                                        <p runat="server" id="EligibilityStatus" class="form-text"></p>

                                        <asp:Repeater runat="server" ID="EligibilityErrors">
                                            <ItemTemplate>
                                                <p>
                                                    <span class="badge bg-danger">Error</span>
                                                    <%# Container.DataItem %>
                                                </p>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                        <asp:Repeater runat="server" ID="EligibilityWarnings">
                                            <ItemTemplate>
                                                <p>
                                                    <span class="badge bg-warning">Warning</span>
                                                    <%# Container.DataItem %>
                                                </p>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-4 mb-3">
                                <div class="card h-100">
                                    <div class="card-body">

                                        <h4 class="card-title mb-3">
                                            Approval
                                        </h4>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Approval Status
                                            </label>
                                            <div>
                                                <asp:RadioButtonList runat="server" ID="ApprovalStatus" />
                                                <asp:Literal runat="server" ID="ApprovalStatusText" />
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <insite:ComboBox runat="server" ID="ApprovalReasonCombo" EmptyMessage="Predefined Reasons List" Width="100%" style="display:none;">
                                                <Items>
                                                    <insite:ComboBoxOption />
                                                    <insite:ComboBoxOption Text="Candidate is not registered into the appropriate apprenticeship program. Candidate will not be permitted to write the certification exam(s) for this trade if they are not registered into the program." />
                                                    <insite:ComboBoxOption Text="Candidate is missing a previous level of technical training. Candidate will not be permitted to write the certification exam(s) for this trade if they have not completed all levels of technical training." />
                                                    <insite:ComboBoxOption Text="Candidate has not passed a previous CofQ exam. Candidate is approved to write the scheduled certification exam once. They must pass the previous CofQ exam before they can rewrite the CofQ exam for this level or proceed to the next level." />
                                                    <insite:ComboBoxOption Text="Candidate is not certified at a previous level. Candidate will not be permitted to write the certification exam for this level until they obtain certification at the previous level." />
                                                    <insite:ComboBoxOption Text="Candidate already has credit for this level." />
                                                    <insite:ComboBoxOption Text="Candidate eligibility currently under review. Candidate will be contacted by SkilledTradesBC." />
                                                    <insite:ComboBoxOption Text="Name mismatch between exam request and Direct Access account. The student's name on their government issued photo ID must match the name on the exam. Please contact the appropriate institution (SkilledTradesBC or the training provider) to correct your name." />
                                                </Items>
                                            </insite:ComboBox>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Reason
                                            </label>
                                            <insite:TextBox runat="server" ID="ApprovalReason" TextMode="MultiLine" Rows="4" />
                                        </div>

                                        <asp:Panel runat="server" ID="ApprovalPanel">
                                            <asp:Literal runat="server" ID="ApprovalPanelText" />
                                            <asp:LinkButton runat="server" ID="ApprovalPanelButton" Text="Republish" Icon="fas fa-cloud-upload" />
                                        </asp:Panel>

                                    </div>
                                </div>
                            </div>

                        </div>

                    </div>
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="AccommodationTab" Title="Accommodations" Icon="far fa-assistive-listening-systems" IconPosition="BeforeText">
            <div class="row mt-3">
                <div class="col-lg-8">
                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AccommodationUpdatePanel" />

                            <insite:UpdatePanel runat="server" ID="AccommodationUpdatePanel">
                                <ContentTemplate>

                                    <div class="form-group mb-3">
                                        <table><tr><td>

                                            <insite:MultiField runat="server" style="display:inline-block; width:280px;">
                                                <insite:MultiFieldView runat="server" ID="AccommodationTypeSelectorView" Inputs="AccommodationTypeSelector">
                                                    <span class="multi-field-input">
                                                        <insite:AccommodationTypeComboBox runat="server" ID="AccommodationTypeSelector" AllowBlank="true" EnableSearch="true" DropDown-Width="268px" />
                                                    </span>
                                                    <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                        ButtonStyle="OutlineSecondary" Icon="far fa-keyboard" ToolTip="Enter a value manually" />
                                                </insite:MultiFieldView>

                                                <insite:MultiFieldView runat="server" ID="AccommodationTypeTextView" Inputs="AccommodationTypeText">
                                                    <span class="multi-field-input">
                                                        <insite:TextBox runat="server" ID="AccommodationTypeText" />
                                                    </span>
                                                    <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                        ButtonStyle="OutlineSecondary" Icon="far fa-list-ul" ToolTip="Select an option from the list" />
                                                </insite:MultiFieldView>
                                            </insite:MultiField>

                                            <insite:RequiredValidator runat="server" ControlToValidate="AccommodationTypeSelector" FieldName="Accommodation Type" ValidationGroup="Accommodation" RenderMode="Dot" Display="None" />
                                            <insite:RequiredValidator runat="server" ControlToValidate="AccommodationTypeText" FieldName="Accommodation Type" ValidationGroup="Accommodation" RenderMode="Dot" Display="None" />
                                            <insite:TextBox runat="server" ID="AccommodationName" MaxLength="100" Width="250" EmptyMessage="Name" CssClass="d-inline-block" />

                                        </td><td style="padding-left:5px;">

                                            <insite:Button runat="server" ID="AddAccommodationButton" ValidationGroup="Accommodation"
                                                ButtonStyle="OutlinePrimary" Icon="far fa-plus-circle" ToolTip="Add Accommodation" />

                                        </td></tr></table>
                                    </div>

                                    <div runat="server" id="AccommodationField" class="form-group mb-3">
                                        <div>
                                            <table class="table table-stripped">
                                                <thead>
                                                    <tr>
                                                        <th>Accommodation Type</th>
                                                        <th>Name</th>
                                                        <th style="text-align:right;">Time Extension</th>
                                                        <th></th>
                                                    </tr>
                                                </thead>
                                                <asp:Repeater runat="server" ID="AccommodationsRepeater">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td><%# Eval("AccommodationType") %></td>
                                                            <td><%# Eval("AccommodationName") %></td>
                                                            <td style="text-align:right;"><%# Eval("TimeExtension") %> minutes</td>
                                                            <td style="width:50px;">
                                                                <insite:IconButton runat="server"
                                                                    CommandName="Delete"
                                                                    CommandArgument='<%# Eval("AccommodationType") %>'
                                                                    Name="trash-alt"
                                                                    ToolTip="Remove Accommodation"
                                                                    ConfirmText="Are you sure you to remove this accommodation?"
                                                                />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </table>
                                        </div>
                                    </div>

                                </ContentTemplate>
                            </insite:UpdatePanel>

                        </div>
                    </div>
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="SchoolTab" Title="School/Training Provider" Icon="far fa-graduation-cap" IconPosition="BeforeText">
            <div class="row mt-3">
                <div class="col-lg-6 mb-3">
                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="SchoolUpdatePanel" />

                            <insite:UpdatePanel runat="server" ID="SchoolUpdatePanel">
                                <ContentTemplate>
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            School
                                        </label>
                                        <insite:FindGroup runat="server" ID="SchoolID" />
                                    </div>

                                    <div runat="server" id="InstructorsField" class="form-group mb-3">
                                        <label class="form-label">
                                            School Contacts
                                        </label>
                                        <div>
                                            <asp:CheckBoxList runat="server" ID="Instructors" />
                                        </div>
                                    </div>

                                    <div class="mt-3">
                                        <insite:Button runat="server" ID="NewContactsLink" Text="New Contact Person(s)" Icon="fas fa-plus-circle" ButtonStyle="OutlinePrimary" />
                                    </div>
                                </ContentTemplate>
                            </insite:UpdatePanel>

                        </div>
                    </div>
                </div>
            </div>
        </insite:NavItem>

    </insite:Nav>
        
    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Registration" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">

            (function () {
                var $approvalReasonCombo = $('#<%= ApprovalReasonCombo.ClientID %>').on('change', onRegistrationReasonSelected);
                var $approvalStatus = $('#<%= ApprovalStatus.ClientID %>').on('click', onRegistrationStatusChanged);

                function onRegistrationStatusChanged() {
                    if ($approvalStatus.find('input:checked').val() !== 'Eligible') {
                        $approvalReasonCombo.show();
                    } else {
                        $approvalReasonCombo.hide();
                    }
                }

                function onRegistrationReasonSelected(s, e) {
                    var text = $('#<%= ApprovalReasonCombo.ClientID %>').find(":selected").text().trim();
                    if (text)
                        $('#<%= ApprovalReason.ClientID %>').val(text);
                }
            })();

        </script>
    </insite:PageFooterContent>

</asp:Content>