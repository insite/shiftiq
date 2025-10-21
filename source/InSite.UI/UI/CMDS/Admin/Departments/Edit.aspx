<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Cmds.Admin.Departments.Forms.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/PersonGrid.ascx" TagName="PersonGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/ContactProfileEditor.ascx" TagName="ContactProfileEditor" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementListEditor.ascx" TagName="AchievementListEditor" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/DepartmentDetails.ascx" TagName="DepartmentDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <asp:HiddenField ID="NeedMoveTop" runat="server" />

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="DepartmentInfo" />

    <asp:Button ID="RemoveReferencesButton" runat="server" style="display:none;" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="DepartmentTab" Title="Department" Icon="far fa-building" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Department
                </h2>

                <div class="row">
                    <div class="col-lg-6">

                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <uc:DepartmentDetails ID="Details" runat="server" />

                            </div>
                        </div>

                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="PeopleTab" Title="People" Icon="far fa-users" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    People
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="PeopleUpdatePanel" />

                        <insite:UpdatePanel runat="server" ID="PeopleUpdatePanel">
                            <ContentTemplate>
                                <uc:PersonGrid ID="PersonGrid" runat="server" />
                            </ContentTemplate>
                        </insite:UpdatePanel>

                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ProfileTab" Title="Profiles" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Profiles
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <uc:ContactProfileEditor ID="ProfileEditor" runat="server" />

                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="AchievementTab" Title="Achievements" Icon="far fa-trophy" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Achievements
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <uc:AchievementListEditor ID="AchievementEditor" runat="server" />

                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="CompetencyTab" Title="Competencies" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Competencies
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <insite:Button ButtonStyle="Default" runat="server" ID="DepartmentSkillEditorLink" Icon="far fa-pencil" Text="Edit competency priorities for this department" />

                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="DepartmentInfo" />
        <insite:DeleteButton runat="server" ID="DeleteButton" CausesValidation="false" ConfirmText="Are you sure you want to delete this department?" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

    <insite:PageFooterContent runat="server">

        <script type="text/javascript">

            function DepartmentEditor_endRequest(sender, args) {
                var needMoveTop = $get("<%= NeedMoveTop.ClientID %>");

                if (needMoveTop.value == "true")
                {
                    needMoveTop.value = null;
                    scroll(0, 0);
                }
            }

        </script>

    </insite:PageFooterContent>

</asp:Content>
