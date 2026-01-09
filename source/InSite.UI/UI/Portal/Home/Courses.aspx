<%@ Page Language="C#" CodeBehind="Courses.aspx.cs" Inherits="InSite.UI.Portal.Home.Courses" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div class="float-end">
        <a href="/ui/portal/learning/catalog" class="btn btn-primary"><i class="fas fa-books me-2"></i>Catalog</a>
    </div>

    <style type="text/css">
        .badge-success {
            color: #fff;
            background-color: #28a745
        }

        .badge {
            z-index: 1;
            right: 0% !important
        }
    </style>

    <!-- Content  -->
    <div class="row mb-4">
        <div class="col-lg-12">
            <insite:MyCoursesComboBox runat="server" ID="MyCoursesComboBox" AllowBlank="false" />
            <insite:Alert runat="server" ID="StatusAlert" CssClass="mt-2" />
        </div>
    </div>
    <div class="row">
        <div class="row row-cols-1 row-cols-md-3 g-4">

            <asp:Repeater ID="CoursesList" runat="server">
                <ItemTemplate>
                    <div class="col">

                        <a class="card card-hover card-tile border-0 shadow" href='<%# GetCourseUrl() %>'>
                            <asp:Literal runat="server" ID="CardStatus" />
                            <asp:Literal runat="server" ID="CardImage" />
                            <asp:Literal runat="server" ID="CardBadge" />
                            <div class="card-body text-center">
                                <asp:Literal runat="server" ID="CardIcon" />
                                <h3 class="h5 nav-heading mb-2"><%# Eval("CourseName") %></h3>
                            </div>
                        </a>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

        </div>
    </div>

</asp:Content>
