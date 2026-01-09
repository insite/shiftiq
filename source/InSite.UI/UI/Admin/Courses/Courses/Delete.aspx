<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Courses.Courses.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Courses/Courses/Controls/CourseInfo.ascx" TagName="CourseDetail" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">	

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />

    <div class="row settings">
        <div class="col-md-6">
                                    
            <div class="settings">

                <h3>Course</h3>

                <uc:CourseDetail ID="CourseDetail" runat="server" />

                <div class="alert alert-danger" role="alert">
                     <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this course and related data?
                </div>

                <p style="padding-bottom:10px;">	
                    <insite:DeleteButton runat="server" ID="DeleteButton" />
                    <insite:CancelButton runat="server" ID="CancelButton" />
                </p>
            </div>
        </div>

        <div class="col-lg-6">
            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The course will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                <tr>
                    <td>
                        Course
                    </td>
                    <td>
                        1
                    </td>
                </tr>
                <tr>
                    <td>
                        Modules
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="ModuleCount" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Activities
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="ActivityCount" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Web Pages
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="WebPageCount" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Gradebooks
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="GradebookCount" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>
</asp:Content>
