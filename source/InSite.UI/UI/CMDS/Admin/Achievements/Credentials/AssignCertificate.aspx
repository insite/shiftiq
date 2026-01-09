<%@ Page Language="C#" CodeBehind="AssignCertificate.aspx.cs" Inherits="InSite.Cmds.Actions.BulkTool.Assign.AssignCertificate" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Education" />

    <asp:CustomValidator ID="EmployeeRequired" runat="server" Display="None" ValidationGroup="Education" ErrorMessage="There are no selected people." />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-trophy me-1"></i>
            Achievement Assignment
        </h2>

        <div class="row mb-3">
            <div class="col-md-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Achievement</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Type
                            </label>
                            <div>
                                <insite:ComboBox ID="SubType" runat="server" AllowBlank="false" />
                            </div>
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Category
                            </label>
                            <div>
                                <cmds:TrainingCategorySelector ID="Category" runat="server" />
                            </div>
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Achievement
                                <insite:RequiredValidator runat="server" ControlToValidate="AchievementIdentifier" FieldName="Resource" ValidationGroup="Education" />
                            </label>
                            <div>
                                <cmds:FindAchievement ID="AchievementIdentifier" runat="server" />
                            </div>
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Settings
                            </label>
                            <div>
                                <asp:CheckBox runat="server" ID="EnableSignOff" Text="Enable Sign Off" />
                                <asp:CheckBox runat="server" ID="IsRequired" Text="Required" />
                                <asp:CheckBox runat="server" ID="IsTimeSensitive" Text="Time-Sensitive" onclick="showHideDateExpired();" />
                            </div>
                            <div class="form-text"></div>
                        </div>

                        <div runat="server" ID="trValidFor" style="display:none;" class="form-group mb-3">
                            <label class="form-label">
                                Valid for Months
                            </label>
                            <div>
                                <insite:NumericBox ID="ValidForCount" runat="server"
                                    NumericMode="Integer"
                                    MinValue="0"
                                    MaxValue="999"
                                    ClientEvents-OnChange="ValidFor_Changed();"
                                    CssClass="w-25"
                                />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-md-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Department</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Department
                            </label>
                            <div>
                                <cmds:FindDepartment ID="Department" runat="server" />
                                <div style="height:35px; margin-top:5px; display: inline-block">
                                <asp:CheckBoxList ID="RoleTypeSelector" runat="server" RepeatDirection="Vertical" Visible="True" CssClass="check-list">
                                    <asp:ListItem Value="Organization" Text="Organization Employment" />
                                    <asp:ListItem Value="Department" Text="Department Employment" Selected="True" />
                                    <asp:ListItem Value="Administration" Text="Data Access" />
                                </asp:CheckBoxList>
                                </div>
                            </div>
                            <div class="form-text"></div>
                        </div>

                    </div>
                </div>
            </div>

        </div>

        <div runat="server" id="EmployeesPanel" visible="false" class="card border-0 shadow-lg h-100 mb-3">
            <div class="card-body">
                <h3 id="UserHeading" runat="server"></h3>

                <asp:HiddenField ID="UploadedPersonID" runat="server" />
                <asp:HiddenField ID="UploadedFilePath" runat="server" />
                <asp:HiddenField ID="UploadedName" runat="server" />
                <asp:HiddenField ID="UploadedDescription" runat="server" />
                <insite:SaveButton ID="SaveDownloadButton" runat="server" Style="display:none;" />

                <asp:Repeater ID="Employees" runat="server">
                    <HeaderTemplate>
                        <table class="table table-striped text-nowrap">
                            <tr>
                                <th class="text-center">Assigned</th>
                                <th>User</th>
                                <th style="width:450px;">Document #</th>
                                <th style="width:170px;">Status</th>
                                <th style="width:170px;">Date Completed</th>
                                <th>Expiry Date</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="text-center">
                                <asp:Literal ID="UserIdentifier" runat="server" Text='<%# Eval("UserIdentifier") %>' Visible="false" />
                                <asp:CheckBox ID="Assigned" runat="server" onclick="Assigned_onclick(this);" />
                            </td>
                            <td><%# Eval("FullName") %></td>
                            <td>

                                <asp:TextBox runat="server" ID="DocumentNumber" CssClass="insite-text form-control d-inline" MaxLength="32" />
                                        
                                <cmds:IconButton ID="AttachUploadButton" runat="server"
                                    IsFontIcon="true" CssClass="plus-circle"
                                    ToolTip="Attach new file"
                                    ImageAlign="AbsMiddle"
                                    OnClientClick="showFileUploader(); return false;"
                                />

                                <asp:Repeater ID="AttachmentRepeater" runat="server">
                                    <ItemTemplate>
                                        <div class="form-text">

                                            <asp:HiddenField runat="server" ID="UserIdentifier" Value='<%# Eval("PersonID") %>' />

                                            <asp:HyperLink runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%# GetUploadUrl((Upload)Container.DataItem) %>' Target="_blank" />

                                            <cmds:IconButton runat="server"
                                                IsFontIcon="true" CssClass="trash-alt"
                                                ImageAlign="AbsMiddle"
                                                ToolTip="Delete Attachment"
                                                ConfirmText="Are you sure you want to delete this attachment?"
                                                CommandName='<%# Eval("UploadID") == null ? "DeleteTempUpload" : "DeleteUpload" %>'
                                                CommandArgument='<%# Eval("UploadID") == null ? Eval("FilePath") : Eval("UploadID") %>' />

                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>

                            </td>
                            <td>
                                <cmds:EmployeeAchievementStatusSelector2 ID="Status" runat="server" AllowBlank="true" CssClass="d-inline" />
                                <insite:RequiredValidator ID="StatusRequired" runat="server" ControlToValidate="Status" FieldName="Status" ValidationGroup="Education" Enabled="false" />
                            </td>
                            <td>
                                        
                                <asp:TextBox runat="server" ID="DateCompleted" CssClass="insite-text form-control d-inline" MaxLength="10" placeholder="m/d/yyyy" onkeyup="DateCompleted_onkeyup(this, event);" />

                                <insite:CustomValidator ID="DateCompletedValidator" runat="server"
                                    ControlToValidate="DateCompleted"
                                    ValidationGroup="Education"
                                    ErrorMessage="Date Completed value should have valid date format"
                                    ClientValidationFunction="ValidateDate" />

                            </td>
                            <td>
                                <asp:Literal ID="ExpirationDate" runat="server" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            <tr>
                                <td colspan="6"></td>
                            </tr>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>

                <div class="mt-3 mb-3">
                    <strong>Note:</strong> Date Completed and Expiry Date must have this format: Month/Day/Year (e.g. 7/15/2024).
                </div>

                <div class="form-text">
                    Remember that this list of people includes only the individuals for whom you are the assigned supervisor or manager or validator.
                    In addition, this list includes only individuals who are assigned to the Workers role. 
                </div>

            </div>
        </div>

        <insite:SaveButton runat="server" ID="SaveButton" Text="Assign Certificates" ConfirmText="Are you sure you want to create credentials for selected people?" ValidationGroup="Education" />
        <insite:DeleteButton runat="server" ID="UnassignCertificates" Text="Unassign Certificates" />
        <insite:CancelButton runat="server" ID="CancelButton" NavigateUrl="/ui/admin/tools" />

    </section>    

    <script type="text/javascript">

        var _isValueChanging = false;

        function showHideDateExpired() {
            var chk = document.getElementById('<%= IsTimeSensitive.ClientID %>');
            var trValidFor = document.getElementById('<%= trValidFor.ClientID %>');

            trValidFor.style.display = chk.checked ? '' : 'none';
        }

        function Assigned_onclick(sender) {
            var statusRequiredValidator = getControl(sender.parentNode.parentNode, 'span', 'StatusRequired');

            ValidatorEnable(statusRequiredValidator, sender.checked);

            statusRequiredValidator.style.visibility = 'hidden';
        }

        function DateCompleted_onkeyup(sender, e) {
            if (_isValueChanging)
                return;

            var keyCode = window.event ? window.event.keyCode : e.which;

            if (keyCode == 9 || keyCode == 16 || keyCode == 17 || keyCode == 18)
                return;

            var value = strToDate(sender.value);

            if (value == null || !document.getElementById('<%= IsTimeSensitive.ClientID %>').checked)
                return;

            var validForCount = parseInt($('#<%= ValidForCount.ClientID %>').val());
            if (isNaN(validForCount))
                return;

            value.setMonth(value.getMonth() + validForCount);

            var expirationDateCtrl = getControl(sender.parentNode.parentNode, 'input', 'ExpirationDate');

            _isValueChanging = true;
            expirationDateCtrl.value = dateToStr(value);
            _isValueChanging = false;
        }

        function ValidFor_Changed() {
            if (_isValueChanging)
                return;

            var validForCount = parseInt($('#<%= ValidForCount.ClientID %>').val());
            if (isNaN(validForCount))
                return;

            var panel = document.getElementById('<%= EmployeesPanel.ClientID %>'); 
            if (!panel)
                return;

            var rows = panel.getElementsByTagName('tr');

            for (var i = 1; i < rows.length - 1; i++) {
                var dateCompletedCtrl = getControl(rows[i], 'input', 'DateCompleted');
                var value = strToDate(dateCompletedCtrl.value);

                if (value == null)
                    continue;

                value.setMonth(value.getMonth() + validForCount);

                var expirationDateCtrl = getControl(rows[i], 'input', 'ExpirationDate');

                _isValueChanging = true;
                expirationDateCtrl.value = dateToStr(value);
                _isValueChanging = false;
            }
        }

        function strToDate(str) {
            if (str == null)
                return null;

            var parts = str.split('/');

            if (parts.length != 3 || parts[2].length != 4)
                return null;

            return new Date(Number(parts[2]), Number(parts[0]) - 1, Number(parts[1]), 0, 0, 0, 0);
        }

        function dateToStr(date) {
            if (date == null)
                return null;

            return String(date.getMonth() + 1) + '/' + String(date.getDate()) + '/' + String(date.getFullYear());
        }

        function getControl(parentNode, tagName, id) {
            var regex = new RegExp(id + '$');
            var tags = parentNode.getElementsByTagName(tagName);

            for (var i = 0; i < tags.length; i++) {
                if (tags[i].id.search(regex) > 0) {
                    return tags[i];
                }
            }

            return null;
        }

        function ValidateDate(source, arguments) {
            var pattern = new RegExp(_dateValidationExpression);

            arguments.IsValid = pattern.test(arguments.Value);

            if (!arguments.IsValid)
                return;

            var parts = arguments.Value.split('/');
            var month = Number(parts[0]);
            var day = Number(parts[1]);
            var year = Number(parts[2]);

            arguments.IsValid = year >= 1753;

            if (!arguments.IsValid)
                return;

            var date = new Date(year, month - 1, day);

            arguments.IsValid = date.getDate() == day && date.getMonth() + 1 == month && date.getFullYear() == year;
        }

    </script>

<insite:Modal runat="server" ID="FileUploadWindow" Title="Upload File" Width="600px">
    <ContentTemplate>
        
        <div class="row">
            <div class="col-md-6">
                <div class="form-group mb-3">
                    <label class="form-label">
                        File
                    </label>
                    <div>
                        <asp:FileUpload ID="File" runat="server" />
                    </div>
                </div>
                <div class="form-group mb-3">
                    <label class="form-label">
                        Title
                    </label>
                    <div>
                        <insite:TextBox ID="TitleInput" runat="server" MaxLength="256" />
                    </div>
                </div>
                <div class="form-group mb-3">
                    <label class="form-label">
                        Description
                    </label>
                    <div>
                        <insite:TextBox ID="Description" runat="server" TextMode="MultiLine" />
                    </div>
                </div>

                <insite:SaveButton runat="server" ID="AttachFileButton" />
                <insite:CloseButton runat="server" ButtonStyle="Default" OnClientClick="closeFileUploader(); return false;" />
            </div>
        </div>

    </ContentTemplate>
</insite:Modal>

<script type="text/javascript">

    function showFileUploader(personID) {
        document.getElementById('<%= UploadedPersonID.ClientID %>').value = personID;
        modalManager.show($('#<%= FileUploadWindow.ClientID %>'));
    }

    function closeFileUploader(json) {
        const arg = JSON.parse(json);
        if (arg != null) {
            document.getElementById('<%= UploadedFilePath.ClientID %>').value = arg.filePath;
            document.getElementById('<%= UploadedName.ClientID %>').value = arg.name;
            document.getElementById('<%= UploadedDescription.ClientID %>').value = arg.description;

            __doPostBack('<%= SaveDownloadButton.UniqueID %>', '');
        }
        modalManager.close($('#<%= FileUploadWindow.ClientID %>'));
    }

</script>
</asp:Content>
