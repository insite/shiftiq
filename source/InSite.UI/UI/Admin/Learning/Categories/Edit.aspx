<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" 
    Inherits="InSite.UI.Admin.Learning.Categories.Edit" 
    MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementListEditor.ascx" TagName="AchievementsListEditor" TagPrefix="uc" %>

<%@ Register Src="~/UI/Admin/Learning/Categories/Controls/CategoryDetail.ascx" TagName="CategoryDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="AlertStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Category" />

    <insite:Nav runat="server" ID="NavPanel">
        <insite:NavItem runat="server" ID="CategorySection" Title="Category" Icon="far fa-tag" IconPosition="BeforeText">

    <section class="mb-3">

        <div class="row">
            <div class="col-md-6">

                <div class="card border-0 shadow-lg mb-4">
                    <div class="card-body">

                        <h3>Category Details</h3>
                        <uc:CategoryDetail runat="server" ID="CategoryDetail" />

                    </div>
                </div>

                <div class="card border-0 shadow-lg mb-4">
                    <div class="card-body">

                        <h3>Courses</h3>
                        <ul>
                        <asp:Repeater runat="server" ID="CourseRepeater">
                            <ItemTemplate>
                                <li><%# Eval("CourseName") %></li>
                            </ItemTemplate>
                        </asp:Repeater>
                        </ul>

                    </div>
                </div>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <h3>Programs</h3>
                        <ul>
                        <asp:Repeater runat="server" ID="ProgramRepeater">
                            <ItemTemplate>
                                <li><%# Eval("ProgramName") %></li>
                            </ItemTemplate>
                        </asp:Repeater>
                        </ul>

                    </div>
                </div>

            </div>
            <div class="col-md-6">

                <asp:Literal runat="server" ID="DescriptionDisplay" />

            </div>
        </div>
    </section>

    </insite:NavItem>
    <insite:NavItem runat="server" ID="AchievementsSection" Title="Achievements" Icon="far fa-trophy" IconPosition="BeforeText">

        <section>
            <h2 class="h4 mt-4 mb-3">Achievements
            </h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">
                    <uc:AchievementsListEditor ID="AchievementsEditor" runat="server" />
                </div>
            </div>
        </section>

    </insite:NavItem>

    </insite:Nav>

    <div class="mt-4">
    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Category" />
    <insite:DeleteButton runat="server" ID="DeleteButton" />
    <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
