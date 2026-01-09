<%@ Page Language="C#" CodeBehind="Build.aspx.cs" Inherits="InSite.Cmds.Actions.Contact.Company.Competency.Manage" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Import Namespace="InSite.Cmds.Controls.Contacts.Companies.Competencies" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/DepartmentCompetencyField.ascx" TagName="DepartmentCompetencyField" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Admin/Departments/ProfileCompetencies/Select.ascx" TagName="FilterWindow" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="DepartmentInfo" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="CriteriaTab" Title="Settings" Icon="far fa-filter" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Settings
                </h2>

                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Profile
                                        <span runat="server" id="ProfileDetails" visible="false"> - <asp:Literal ID="CompetencyCount" runat="server" /></span>
                                    </label>
                                    <div>
                                        <cmds:FindProfile ID="ProfileIdentifier" runat="server" />
                                    </div>
                                </div>

                                <uc:FilterWindow runat="server" ID="FilterWindow" Visible="false" />

                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ResultTab" Title="Competencies" Icon="far fa-ruler-triangle" IconPosition="BeforeText" Visible="false">

            <section>
                <h2 class="h4 mt-4 mb-3">
                    Competencies
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <insite:Grid runat="server" ID="Grid" AutoBinding="false">
                            <Columns>

                                <asp:HyperLinkField HeaderText="#" Target="_blank" DataTextField="CompetencyNumber" DataNavigateUrlFields="CompetencyStandardIdentifier" DataNavigateUrlFormatString="/ui/cmds/admin/standards/competencies/edit?id={0}" HeaderStyle-Width="80px"/>

                                <asp:BoundField HeaderText="Competency Summary" DataField="CompetencySummary" HeaderStyle-Width="250px" ItemStyle-CssClass="border-end" />

                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <uc:DepartmentCompetencyField ID="Department1" runat="server" DepartmentCompetency='<%# (DepartmentCompetency)Eval("Department1") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <uc:DepartmentCompetencyField ID="Department2" runat="server" DepartmentCompetency='<%# (DepartmentCompetency)Eval("Department2") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <uc:DepartmentCompetencyField ID="Department3" runat="server" DepartmentCompetency='<%# (DepartmentCompetency)Eval("Department3") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <uc:DepartmentCompetencyField ID="Department4" runat="server" DepartmentCompetency='<%# (DepartmentCompetency)Eval("Department4") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <uc:DepartmentCompetencyField ID="Department5" runat="server" DepartmentCompetency='<%# (DepartmentCompetency)Eval("Department5") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </insite:Grid>
    
                        <div class="mt-3">
                            <insite:SaveButton ID="SaveButton" runat="server" />
                            <insite:SaveButton ID="SaveAndCloseButton" runat="server" Text="Save & Close" />
                        </div>

                        <div class="d-none">
                            <asp:HiddenField ID="SelectedDepartmentID" runat="server" />
                            <asp:Button ID="SelectAllCompetenciesButton" runat="server" />
                            <asp:Button ID="UnselectAllCompetenciesButton" runat="server" />

                            <asp:HiddenField ID="DepartmentFilter" runat="server" />
                            <asp:HiddenField ID="SaveOptionField" runat="server" Value="All" />
                            <asp:Button ID="ApplyFilterButton" runat="server" />
                        </div>

                    </div>
                </div>

                <div class="card border-0 shadow-lg mt-3">
                    <div class="card-body">

                        <h3>Remove Competencies</h3>

                        <p>
                            Enter the list of competency numbers that you would like to remove from the selected departments. (Numbers should be separated by commas or by new lines.)
                        </p>

                        <insite:TextBox runat="server" ID="CompetenciesToRemove" TextMode="MultiLine" Rows="7" />

                        <div class="mt-3">
                            <insite:DeleteButton runat="server" ID="RemoveCompetenciesButton" Text="Delete Competencies" 
                                ConfirmText="Are you sure you want to remove these competencies?" />
                            <asp:Literal ID="RemovedLabel" runat="server" />
                        </div>

                    </div>
                </div>

            </section>

        </insite:NavItem>

    </insite:Nav>

    <div class="mt-3">
        <insite:CloseButton runat="server" ID="CloseButton" />
    </div>

    <div class="d-none">
        <asp:HiddenField ID="AffectedCompetencyStandardIdentifier" runat="server" />
        <asp:HiddenField ID="OperationOptions" runat="server" />
        <asp:Button ID="DeleteCompetencyButton" runat="server" />
        <asp:Button ID="CopyCompetencyButton" runat="server" />
        <asp:Button ID="SetLevelButton" runat="server" />
    </div>

    <insite:Modal runat="server" ID="SettingWindow" Title="Competency Settings" Width="700px" />

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">

            var savedChk;

            function showSettingsEditor(settingsButtonId, chkId) {

                var settingsButton = $get(settingsButtonId);

                savedChk = $get(chkId);

                var competencyId = settingsButton.getAttribute("competencyId");
                var departmentId = settingsButton.getAttribute("departmentId");
                var profileId = settingsButton.getAttribute("profileId");
                var saveOption = $get("<%= SaveOptionField.ClientID %>").value;

                var url = "/ui/cmds/admin/departments/profile-competencies/prioritize?departmentID="
                    + departmentId + "&competencyID=" + competencyId + "&profileID=" + profileId + "&saveOption=" + saveOption;
                modalManager.load('<%= SettingWindow.ClientID %>', url);
            }

            function setCheckboxes(checkboxId, checked) {
                var checkboxes = document.getElementsByTagName("input");

                for (var i = 0; i < checkboxes.length; i++) {
                    var chk = checkboxes[i];

                    if (chk.type.toLowerCase() == "checkbox" && chk.id.indexOf(checkboxId) >= 0) {
                        chk.checked = checked;

                        selectedChanged(chk.id)
                    }
                }

                return false;
            }

            function selectedChanged(chkId) {
                var chk = $get(chkId);
                var checked = chk.checked;

                var settingsButtonId = chk.parentNode.getAttribute("settingsButtonId");
                var alarmImageId = chk.parentNode.getAttribute("alarmImageId");
                var criticalImageId = chk.parentNode.getAttribute("criticalImageId");

                var isTimeSensitive = chk.parentNode.getAttribute("isTimeSensitive") == "true";
                var isCritical = chk.parentNode.getAttribute("isCritical") == "true";

                $get(settingsButtonId).style.display = checked ? "" : "none";
                $get(alarmImageId).style.display = checked && isTimeSensitive ? "" : "none";
                $get(criticalImageId).style.display = checked && isCritical ? "" : "none";
            }

            function getParam(name) {
                name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");

                var regexS = "[\\?&]" + name + "=([^&#]*)";
                var regex = new RegExp(regexS);
                var results = regex.exec(window.location.href);
                return results == null ? "" : results[1];
            }

            function onSettingsEditorClose(arg) {

                if (arg == null)
                    return;

                if (arg.isSetLevel) {
                    $get('<%= AffectedCompetencyStandardIdentifier.ClientID %>').value = arg.competencyID;
                    $get('<%= OperationOptions.ClientID %>').value = arg.allProfiles + "," + arg.departmentID;
                    __doPostBack('<%= SetLevelButton.UniqueID %>', '');
                    return;
                }
                else if (arg.isDeleted) {
                    $get('<%= AffectedCompetencyStandardIdentifier.ClientID %>').value = arg.competencyID;
                    __doPostBack('<%= DeleteCompetencyButton.UniqueID %>', '');
                    return;
                }
                else if (arg.isCopied) {
                    $get('<%= AffectedCompetencyStandardIdentifier.ClientID %>').value = arg.competencyID;
                    $get('<%= OperationOptions.ClientID %>').value = arg.departmentID;
                    __doPostBack('<%= CopyCompetencyButton.UniqueID %>', '');
                    return;
                }

                var alarmImageId = savedChk.parentNode.getAttribute("alarmImageId");
                var alarmImage = $get(alarmImageId);

                alarmImage.title = arg.timeSensitiveText;

                savedChk.parentNode.setAttribute("isTimeSensitive", arg.isTimeSensitive);
                savedChk.parentNode.setAttribute("isCritical", arg.isCritical);

                var selectedOldId = savedChk.parentNode.getAttribute("selectedOldId");
                var selectedOld = $get(selectedOldId);

                selectedOld.value = "true";

                selectedChanged(savedChk.id);
            }

            (function () {
                var $modal = $(document.getElementById('<%= SettingWindow.ClientID %>'));
                $modal.on('closing.modal.insite', function (e, s, a) {
                    console.log(JSON.stringify(a));
                    onSettingsEditorClose(a);
                })
            })();

        </script>
    </insite:PageFooterContent>

</asp:Content>