<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourseInfo.ascx.cs" Inherits="InSite.Admin.Courses.Courses.Controls.CourseInfo" %>

<div class="form-group mb-3">
    <label class="form-label">
        Course Name
    </label>
    <div>
        <a id="CourseLink" runat="server">
            <asp:Literal runat="server" ID="CourseName" />
        </a>
    </div>
</div>
            
<div class="form-group mb-3">
    <label class="form-label">
        Course Code
    </label>
    <div>
        <asp:Literal runat="server" ID="CourseCode" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Course Tag
    </label>
    <div>
        <asp:Literal runat="server" ID="CourseLabel" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Asset #
    </label>
    <div>
        <asp:Literal runat="server" ID="CourseAsset" />
    </div>
</div>
