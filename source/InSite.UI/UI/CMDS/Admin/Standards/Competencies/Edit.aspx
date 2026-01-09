<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Cmds.Admin.Competencies.Forms.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementListEditor.ascx" TagName="AchievementListEditor" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/StandardValidationGrid.ascx" TagName="ValidationGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <div runat="server" id="DestroyPanel" visible="false" class="d-flex alert alert-danger mb-4" role="alert">
        <i class="fa-solid fa-circle-stop fs-xl pe-1 me-2"></i>
        <div>
            <strong>This is a destructive action</strong>
            <div>
                This action <strong>cannot</strong> be undone. 
                This will permanently delete the competency 
                <strong><asp:Literal runat="server" ID="DestroyLabel1" /></strong> 
                and all its content, including references to it from other parts of the system.
            </div>
            <div class="mt-3 mb-2">
                To confirm deletion, type the competency number: <asp:Literal runat="server" ID="DestroyLabel2" />
            </div>
            <div class="input-group">
                <input runat="server" id="DestroyInput" type="text" class="form-control" placeholder="Competency number">
                <button runat="server" id="DestroyButton" type="button" class="btn btn-danger">Delete</button>
            </div>
        </div>
    </div>

    <insite:ValidationSummary runat="server" ValidationGroup="Competency" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="CompetencyTab" Title="Competency" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <section>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="row">
                            <div class="col-lg-6">

                                <h3>Details</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Competency Category
                                        <insite:RequiredValidator runat="server" ControlToValidate="CategoryIdentifier" FieldName="Category" ValidationGroup="Competency" />
                                    </label>
                                    <cmds:CompetencyCategorySelector ID="CategoryIdentifier" runat="server" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Competency Number
                                        <asp:CustomValidator ID="UniqueNumber" runat="server" ControlToValidate="Number" ErrorMessage="Competency with this number already exist." ValidationGroup="Competency" Display="Dynamic" />
                                        <insite:RequiredValidator runat="server" ControlToValidate="Number" FieldName="Number" ValidationGroup="Competency" />
                                    </label>
                                    <insite:TextBox ID="Number" runat="server" MaxLength="16" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Competency Summary
                                        <insite:RequiredValidator runat="server" ControlToValidate="Summary" FieldName="Summary" ValidationGroup="Competency" />
                                    </label>
                                    <insite:TextBox ID="Summary" runat="server" TextMode="MultiLine" Rows="3" MaxLength="256" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Program Hours
                                    </label>
                                    <insite:NumericBox ID="ProgramHours" runat="server" DecimalPlaces="2" />
                                </div>

                                <insite:Container runat="server" ID="NotUsedPanel">

                                    <h3>Not Used</h3>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Old Number(s)
                                        </label>
                                        <insite:TextBox ID="NumberOld" runat="server" MaxLength="3400" />
                                    </div>

                                </insite:Container>

                            </div>
                            <div class="col-lg-6">

                                <h3>Knowledge</h3>

                                <div class="form-group mb-3">
                                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="KnowledgeUpdatePanel" />
                                    <insite:UpdatePanel runat="server" ID="KnowledgeUpdatePanel">
                                        <ContentTemplate>
                                            <div runat="server" id="KnowledgeOutput">
                                                <div class="mb-3">
                                                    <insite:Button ID="EditKnowledgeButton" runat="server" ButtonStyle="Default" Icon="far fa-pencil" Text="Edit" OnClientClick="competencyEditor.knowledge.edit(); return false;" />
                                                </div>
                                                <asp:Literal ID="KnowledgeHtml" runat="server" />
                                            </div>

                                            <div runat="server" id="KnowledgeInput" class="d-none">
                                                <insite:TextBox runat="server" TextMode="MultiLine" Rows="10" />
                                                <div class="my-3">
                                                    <insite:Button runat="server" ID="KnowledgeApplyButton" ButtonStyle="Primary" ToolTip="Apply"
                                                        Icon="fas fa-check" Text="OK" OnClientClick="competencyEditor.knowledge.apply(); return false;" />
                                                    <insite:CancelButton runat="server" OnClientClick="competencyEditor.knowledge.cancel(); return false;" />
                                                </div>
                                            </div>

                                            <asp:HiddenField runat="server" ID="KnowledgeValue" />
                                        </ContentTemplate>
                                    </insite:UpdatePanel>
                                </div>

                                <h3>Skills</h3>

                                <div class="form-group mb-3">
                                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="SkillsUpdatePanel" />
                                    <insite:UpdatePanel runat="server" ID="SkillsUpdatePanel">
                                        <ContentTemplate>
                                            <div runat="server" id="SkillsOutput">
                                                <div class="mb-3">
                                                    <insite:Button ID="EditSkillsButton" runat="server" ButtonStyle="Default" Icon="far fa-pencil" Text="Edit" OnClientClick="competencyEditor.skills.edit(); return false;" />
                                                </div>
                                                <asp:Literal ID="SkillsHtml" runat="server" />
                                            </div>

                                            <div runat="server" id="SkillsInput" class="d-none">
                                                <insite:TextBox runat="server" TextMode="MultiLine" Rows="10" />
                                                <div class="my-3">
                                                    <insite:Button runat="server" ID="SkillsApplyButton" ButtonStyle="Primary" ToolTip="Apply"
                                                        Icon="fas fa-check" Text="OK" OnClientClick="competencyEditor.skills.apply(); return false;" />
                                                    <insite:CancelButton runat="server" OnClientClick="competencyEditor.skills.cancel(); return false;" />
                                                </div>
                                            </div>

                                            <asp:HiddenField runat="server" ID="SkillsValue" />
                                        </ContentTemplate>
                                    </insite:UpdatePanel>
                                </div>

                            </div>
                        </div>

                    </div>
                </div>

            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="AchievementTab" Title="Achievements" Icon="far fa-trophy" IconPosition="BeforeText">
            <section>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AchievementsUpdatePanel" />
                        <insite:UpdatePanel runat="server" ID="AchievementsUpdatePanel">
                            <ContentTemplate>
                                <uc:AchievementListEditor ID="AchievementEditor" runat="server" />
                            </ContentTemplate>
                        </insite:UpdatePanel>

                    </div>
                </div>

            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="CompanyTab" Title="Organizations" Icon="far fa-city" IconPosition="BeforeText">
            <section>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div id="CompanyCompetencyPanel" runat="server">
                            Competency is used by <%= Identity.Organization.CompanyName %>
                        </div>
                        <div id="NoCompanyCompetencyPanel" runat="server">
                            <asp:Literal ID="NoCompanyCompetencyLabel" runat="server" />
                        </div>

                    </div>
                </div>

            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="DepartmentTab" Title="Departments" Icon="far fa-building" IconPosition="BeforeText">
            <section>
                
                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div id="NoDepartmentsPanel" runat="server">
                            <asp:Literal ID="NoDepartmentsLabel" runat="server" />
                        </div>

                        <insite:Container ID="DepartmentsPanel" runat="server">

                            <div class="mb-3">
                                <strong>
                                    <%= Identity.Organization.CompanyName %> has
                                    <asp:Literal ID="DepartmentCount" runat="server" />
                                    using this competency.
                                </strong>
                            </div>

                            <insite:Grid runat="server" ID="DepartmentGrid" EnablePaging="false">
                                <Columns>

                                    <asp:HyperLinkField HeaderText="Department Name" DataTextField="DepartmentName" DataNavigateUrlFields="DepartmentIdentifier" DataNavigateUrlFormatString="/ui/cmds/admin/departments/edit?id={0}" />
                                    <asp:BoundField HeaderText="Priority" DataField="PriorityName" />

                                    <asp:TemplateField HeaderText="Time-Sensitive">
                                        <ItemTemplate>
                                            <%# (Boolean)Eval("IsTimeSensitive") ? "Yes": "No" %>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Valid For">
                                        <ItemTemplate>
                                            <%# (Boolean)Eval("IsTimeSensitive") && Eval("ValidForCount") != DBNull.Value && Eval("ValidForUnit") != DBNull.Value ? string.Format("{0} {1}", Eval("ValidForCount"), Eval("ValidForUnit")) : null %>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>
                            </insite:Grid>

                        </insite:Container>

                    </div>
                </div>

            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ProfileTab" Title="Profiles" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <section>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ProfilesUpdatePanel" />

                        <insite:UpdatePanel runat="server" ID="ProfilesUpdatePanel">
                            <ContentTemplate>
                                <p>
                                    <asp:CheckBox ID="OnlyActiveCompanyProfiles" runat="server" Text="Show active organization profiles only" Checked="true" />
                                </p>

                                <p id="NoProfilePanel" runat="server">
                                    <%= Identity.Organization.CompanyName %> has no profiles that include this competency.
                                </p>

                                <insite:Container ID="ProfilePanel" runat="server">
                                    <p>
                                        <strong>
                                            <%= Identity.Organization.CompanyName %> has
                                            <asp:Literal ID="ProfileCount" runat="server" />
                                            that include this competency.
                                        </strong>
                                    </p>

                                    <insite:Grid runat="server" ID="ProfileGrid" EnablePaging="false">
                                        <Columns>
                                            <asp:HyperLinkField HeaderText="Profile #" DataTextField="ProfileNumber" DataNavigateUrlFields="ProfileStandardIdentifier" DataNavigateUrlFormatString="/ui/cmds/admin/standards/profiles/edit?id={0}" />
                                            <asp:BoundField HeaderText="Profile Title" DataField="ProfileTitle" />
                                        </Columns>
                                    </insite:Grid>

                                </insite:Container>
                            </ContentTemplate>
                        </insite:UpdatePanel>

                    </div>
                </div>

            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ValidationTab" Title="Validations" Icon="far fa-balance-scale" IconPosition="BeforeText">
            <section>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ValidationUpdatePanel" />
                        <insite:UpdatePanel runat="server" ID="ValidationUpdatePanel">
                            <ContentTemplate>
                                <uc:ValidationGrid runat="server" ID="ValidationGrid" />
                            </ContentTemplate>
                        </insite:UpdatePanel>

                    </div>
                </div>

            </section>
        </insite:NavItem>

    </insite:Nav>

    <div class="mt-4">

        <!-- Save | Copy | Delete | Cancel
                         Material Design Guidelines state:
                           - affirmative/primary actions go on the left (e.g., Save)
                           - dismissive actions go on the right (e.g., Cancel)
                           - destructive actions go in the middle (e.g., Copy and Delete)
                      -->

        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Competency" />

        <insite:Button runat="server" ID="CopyButton" ButtonStyle="Default" Text="Copy" Icon="far fa-copy"
            ConfirmText="Are you sure you want to copy this competency?" />

        <insite:DeleteButton runat="server" ID="DeleteButton" CausesValidation="false"
            ConfirmText="Are you sure you want to delete this competency?" />

        <insite:Button runat="server" ID="UndeleteButton" ButtonStyle="Default" Text="Undelete" Icon="far fa-trash-undo-alt"
            ConfirmText="Are you sure you want to undelete this competency?" />

        <insite:CancelButton runat="server" ID="CancelButton" />

    </div>

    <insite:Modal runat="server" ID="HistoryViewerWindow" Title="Change History" Width="710px" MinHeight="520px" />

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">

            (function () {
                var instance = window.competencyEditor = window.competencyEditor || {};

                instance.skills = (function () {
                    var instance = {};

                    function $getOutputPanel() { return $('#<%= SkillsOutput.ClientID %>'); }
                    function $getInputPanel() { return $('#<%= SkillsInput.ClientID %>'); }
                    function $getInputText() { return $('#<%= SkillsInput.ClientID %> textarea:first'); }
                    function $getInputValue() { return $('#<%= SkillsValue.ClientID %>'); }

                    instance.edit = function () {
                        $getOutputPanel().addClass('d-none');
                        $getInputPanel().removeClass('d-none');

                        $getInputText().val($getInputValue().val());
                    };

                    instance.apply = function () {
                        $getInputValue().val($getInputText().val());

                        $getOutputPanel().removeClass('d-none');
                        $getInputPanel().addClass('d-none');

                        __doPostBack('<%= SkillsApplyButton.UniqueID %>', '');
                    };

                    instance.cancel = function () {
                        $getOutputPanel().removeClass('d-none');
                        $getInputPanel().addClass('d-none');
                    };

                    return instance;
                })();

                instance.knowledge = (function () {
                    var instance = {};

                    function $getOutputPanel() { return $('#<%= KnowledgeOutput.ClientID %>'); }
                    function $getInputPanel() { return $('#<%= KnowledgeInput.ClientID %>'); }
                    function $getInputText() { return $('#<%= KnowledgeInput.ClientID %> textarea:first'); }
                    function $getInputValue() { return $('#<%= KnowledgeValue.ClientID %>'); }

                    instance.edit = function () {
                        $getOutputPanel().addClass('d-none');
                        $getInputPanel().removeClass('d-none');

                        $getInputText().val($getInputValue().val());
                    };

                    instance.apply = function () {
                        $getInputValue().val($getInputText().val());

                        $getOutputPanel().removeClass('d-none');
                        $getInputPanel().addClass('d-none');

                        __doPostBack('<%= KnowledgeApplyButton.UniqueID %>', '');
                    };

                    instance.cancel = function () {
                        $getOutputPanel().removeClass('d-none');
                        $getInputPanel().addClass('d-none');
                    };

                    return instance;
                })();
            })();

        </script>
    </insite:PageFooterContent>


</asp:Content>