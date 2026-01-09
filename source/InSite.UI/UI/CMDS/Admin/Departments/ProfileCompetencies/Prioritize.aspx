<%@ Page Language="C#" CodeBehind="Prioritize.aspx.cs" Inherits="InSite.Cmds.Actions.Contact.Company.Competency.Popup.Setting" MasterPageFile="~/UI/Layout/Admin/AdminModal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Container runat="server" ID="ContentContainer">
        <div class="card border-0 shadow-lg mb-3 mt-2">
            <div class="card-body">
                <h3>Competency Settings</h3>

                <p><asp:Literal ID="HelpText" runat="server" /></p>

                <div class="form-group mb-3">
                    <label class="form-label">Time Sensitive:</label>
                    <div>
                        <insite:CheckBox ID="IsTimeSensitive" runat="server" OnClientChange="showHideDateExpired();" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Valid for:</label>
                    <div class="row w-75">
                        <div class="col-sm-6">
                            <insite:NumericBox ID="ValidForCount" runat="server" NumericMode="Integer" Enabled="false" />
                        </div>
                        <div class="col-sm-6">
                            <cmds:ValidForUnitSelector ID="ValidForUnit" runat="server" Enabled="false" />
                        </div>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Priority:</label>
                    <div>
                        <cmds:CompetencyPrioritySelector ID="Priority" runat="server" AllowBlank="False" Width="200px" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <insite:RadioButton runat="server" ID="OnlySaveOption" GroupName="SaveOption" Text="Apply these settings to the selected profile in the selected department" Checked="True" />
                    <insite:RadioButton runat="server" ID="AllSaveOption" GroupName="SaveOption" Text="Apply these settings to all profiles in the selected department" />
                </div>

                <insite:SaveButton runat="server" ID="SaveButton" />
                <insite:CancelButton runat="server" ID="CancelButton" OnClientClick="modalManager.closeModal(); return false;" />

            </div>
        </div>

        <div class="card border-0 shadow-lg mb-3">
            <div class="card-body">
                <h3>Priority Assignment</h3>

                <p>Apply these settings to ALL competencies in this profile</p>

                <insite:SaveButton runat="server"
                    ID="SetLevelButton"
                    Text="Apply"
                    OnClientClick="return confirm('Are you sure you want apply these settings to ALL competencies in this profile?');"
                />
            </div>
        </div>

        <div class="card border-0 shadow-lg mb-3">
            <div class="card-body">
                <h3>Copy Settings to All Departments</h3>

                <p><asp:Literal ID="CopySettingsInstructions" runat="server" /></p>

                <insite:Button runat="server" ID="CopyButton" Icon="fas fa-copy" Text="Copy" ButtonStyle="Default" />

            </div>
        </div>

        <div class="card border-0 shadow-lg mb-3">
            <div class="card-body">
                <h3>Remove Competency</h3>

                <p><asp:Literal ID="RemoveCompetencyInstructions" runat="server" /></p>
        
                <insite:DeleteButton runat="server" ID="DeleteButton" />

            </div>
        </div>
    </insite:Container>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        function showHideDateExpired() {
            const chk = document.getElementById("<%= IsTimeSensitive.ClientID %>");

            const $validForUnit = $(document.getElementById('<%= ValidForUnit.ClientID %>'));
            $validForUnit.prop("disabled", !chk.checked);
            $validForUnit.selectpicker("refresh");

            const validForCount = document.getElementById('<%= ValidForCount.ClientID %>');
            validForCount.disabled = !chk.checked;
        }

        function closeWithoutDelete(isTimeSensitive, isCritical, timeSensitiveText) {
            var arg = new Object();

            arg.isDeleted = false;
            arg.isTimeSensitive = isTimeSensitive;
            arg.isCritical = isCritical;
            arg.timeSensitiveText = timeSensitiveText;

            modalManager.closeModal(arg);
        }

        function closeWithDelete(competencyID) { 
            var arg = new Object();

            arg.isDeleted = true;
            arg.competencyID = competencyID;

            modalManager.closeModal(arg);
        }

        function closeWithCopy() {
            var arg = new Object();

            arg.isCopied = true;
            arg.competencyID = getParam("competencyID");
            arg.departmentID = getParam("departmentID");

            modalManager.closeModal(arg);
        }

        function closeWithSetLevel() {
            const allSaveOption = document.getElementById('<%= AllSaveOption.ClientID %>');

            const arg = {
                isSetLevel: true,
                allProfiles: allSaveOption.checked,
                competencyID: getParam("competencyID"),
                departmentID: getParam("departmentID")
            };

            modalManager.closeModal(arg);
        }

        function getParam(name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");

            var regexS = "[\\?&]" + name + "=([^&#]*)";
            var regex = new RegExp(regexS);
            var results = regex.exec(window.location.href);
            return results == null ? "" : results[1];
        }

    </script>
</insite:PageFooterContent>
</asp:Content>
