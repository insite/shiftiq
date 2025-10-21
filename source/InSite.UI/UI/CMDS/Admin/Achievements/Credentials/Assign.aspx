<%@ Page Language="C#" CodeBehind="Assign.aspx.cs" Inherits="InSite.Cmds.Actions.BulkTool.Assign.Education" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Education" />

    <asp:CustomValidator ID="EmployeeRequired" runat="server" Display="None" ValidationGroup="Education" ErrorMessage="There are no selected people." />

    <insite:UpdatePanel runat="server">
        <ContentTemplate>

            <section>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <h3>Achievements</h3>

                        <div class="row">

                            <div class="col-lg-4">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Achievement Type
                                        <insite:RequiredValidator runat="server" ControlToValidate="AchievementType" FieldName="Type" ValidationGroup="Education" />
                                    </label>
                                    <div>
                                        <cmds:AchievementTypeSelector ID="AchievementType" runat="server" AllowBlank="false" />
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Visibility
                                        <insite:RequiredValidator runat="server" ControlToValidate="AchievementVisibility" FieldName="Type" ValidationGroup="Education" />
                                    </label>
                                    <div>
                                        <insite:ComboBox ID="AchievementVisibility" runat="server">
                                            <Items>
                                                <insite:ComboBoxOption Value="Organization-Specific Achievements" Text="Organization-Specific Achievements" />
                                                <insite:ComboBoxOption Value="Global Achievements" Text="Global Achievements" />
                                                <insite:ComboBoxOption Value="All Achievements" Text="All Achievements" />
                                            </Items>
                                        </insite:ComboBox>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-5">
                                <div runat="server" id="Categories" class="form-group mb-3">
                                    <label class="form-label">
                                        Category
                                    </label>
                                    <div>
                                        <cmds:TrainingCategorySelector ID="Category" runat="server" />
                                        <cmds:AchievementCategorySelector ID="AchievementCategory" runat="server" />
                                    </div>
                                </div>
                            </div>

                        </div>
                        <div class="row">

                            <div class="col-md-12">
                                <div runat="server" id="Policies" visible="false" class="form-group mb-3">
                                    <label class="form-label">
                                        Achievements
                                    </label>
                                    <div>
                                        <cmds:IxCustomValidator ID="AchievementListRequired" runat="server" ValidationGroup="Education" ErrorMessage="At least one achievement should be selected" Display="None" />

                                        <asp:Panel ID="AchievementsPanel" runat="server" CssClass="mb-3">
                                            <insite:CheckBoxList runat="server" ID="AchievementList" RepeatColumns="2" CssClass="resource_list" />
                                        </asp:Panel>

                                        <div>
                                            <insite:Button ID="SelectAllAchievementsButton" runat="server" Icon="far fa-check-square" Text="Select All" ButtonStyle="OutlinePrimary" />
                                            <insite:Button ID="UnselectAllAchievementsButton" runat="server" Icon="far fa-square" Text="Deselect All" ButtonStyle="OutlinePrimary" />
                                        </div>
                                    </div>
                                    <div class="form-text"></div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

            </section>

            <section class="mt-5">

                <insite:UpdatePanel runat="server">
                    <ContentTemplate>

                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                        <h3>Learners</h3>

                        <div class="row">
                            <div class="col-lg-6">

                                        <div class="row">
                                            <div class="col-lg-12">

                                                <div class="form-group mb-3">
                                                    <label class="form-label">Department</label>
                                                    <div>
                                                        <div class="mb-3">
                                                            <cmds:FindDepartment ID="Department" runat="server" EmptyMessage="Department" />
                                                        </div>

                                                        <div runat="server" id="RolesDiv">
                                                            <insite:CheckBox runat="server" ID="OrganizationEmployment" Text="Organization Employment" />
                                                            <insite:CheckBox runat="server" ID="DepartmentEmployment" Text="Department Employment" />
                                                            <insite:CheckBox runat="server" ID="DataAccess" Text="Data Access" />
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="form-group mb-3">
                                                    <label class="form-label">Job Division</label>
                                                    <insite:JobDivisionComboBox runat="server" ID="JobDivision" />
                                                </div>

                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-6">
                                                <div class="form-group mb-3">
                                                    <label class="form-label">
                                                        Time-Sensitivity
                                                    </label>
                                                    <div>
                                                        <insite:CheckBox runat="server" ID="IsTimeSensitive" Text="Time-Sensitive" OnClientChange="showHideDateExpired();" />
                                                    </div>
                                                    <div id="TimeSensitiveHelp" class="form-text" style="display: none;">
                                                        Remember to enter a <strong>Valid For</strong> interval!
                                                    </div>
                                                </div>
                                                <div class="form-group mb-3">
                                                    <label class="form-label">
                                                        Options
                                                    </label>
                                                    <div>
                                                        <insite:CheckBox runat="server" ID="IsPlanned" Text="Training Plan" />
                                                        <insite:CheckBox runat="server" ID="IsMandatory" Text="Required" />
                                                    </div>
                                                    <div class="form-text"></div>
                                                </div>
                                            </div>
                                            <div class="col-lg-6">
                                                <div runat="server" id="trValidFor" style="display: none;" class="form-group mb-3">
                                                    <label class="form-label">
                                                        Valid For
                                                    </label>
                                                    <div>
                                                        <insite:NumericBox ID="ValidForCount" runat="server"
                                                            NumericMode="Integer"
                                                            MinValue="0"
                                                            MaxValue="999"
                                                            CssClass="w-50 d-inline" />
                                                        Months
                                                    </div>
                                                    <div class="form-text"></div>
                                                </div>
                                            </div>
                                        </div>

                            </div>
                            <div class="col-lg-6">

                                            <div id="NoCriteria" runat="server" class="form-text">
                                                Choose a department and select the learners to assign these achievements to.
                                            </div>

                                            <div>

                                                <asp:Panel ID="EmployeesTable" runat="server">
                                                    <asp:Repeater ID="Employees" runat="server">
                                                        <HeaderTemplate>
                                                            <table class="table table-striped table-bordered">
                                                                <tr>
                                                                    <th>Learner</th>
                                                                    <th>Completed</th>
                                                                </tr>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td>
                                                                    <insite:CheckBox runat="server" ID="UserIdentifier" Value='<%# Eval("UserIdentifier") %>' Text='<%# Eval("FullName") %>' />
                                                                </td>
                                                                <td>
                                                                    <%# Eval("DateCompleted") == DBNull.Value ? "" : string.Format("{0:MMM d, yyyy}", (DateTime)Eval("DateCompleted")) %>
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                        <FooterTemplate>
                                                            </table>
                                                        </FooterTemplate>
                                                    </asp:Repeater>
                                                    <div>
                                                        <insite:Button ID="SelectAllButton" runat="server"
                                                            Icon="far fa-check-square"
                                                            Text="Select All"
                                                            ButtonStyle="OutlinePrimary" />
                                                        <insite:Button ID="UnselectAllButton" runat="server"
                                                            Icon="far fa-square"
                                                            Text="Deselect All"
                                                            ButtonStyle="OutlinePrimary" />
                                                    </div>
                                                </asp:Panel>

                                            </div>

                            </div>
                        </div>

                            </div>
                        </div>

                    </ContentTemplate>
                </insite:UpdatePanel>

            </section>

        </ContentTemplate>
    </insite:UpdatePanel>

    <div class="mt-4">

        <insite:SaveButton runat="server" ID="SaveButton"
            Text="Save"
            ValidationGroup="Education"
            ConfirmText="Are you sure you want to assign achievements to the selected learners?"
            DisableAfterClick="true" />

        <insite:DeleteButton runat="server" ID="DeleteButton"
            ConfirmText="Are you sure you want to unassign achievements from the selected learners?"
            DisableAfterClick="true" />

        <insite:CancelButton runat="server" ID="CancelButton" />

    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">

            function showHideDateExpired() {
                const chk = document.getElementById("<%= IsTimeSensitive.ClientID %>");
                const trValidFor = document.getElementById("<%= trValidFor.ClientID %>");
                const tsHelp = document.getElementById("TimeSensitiveHelp");

                trValidFor.style.display = chk.checked ? "" : "none";
                tsHelp.style.display = chk.checked ? "" : "none";
            }

        </script>
    </insite:PageFooterContent>
</asp:Content>
