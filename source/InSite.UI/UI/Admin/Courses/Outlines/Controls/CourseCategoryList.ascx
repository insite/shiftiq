<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourseCategoryList.ascx.cs" Inherits="InSite.UI.Admin.Courses.Outlines.Controls.CourseCategoryList" %>

<div runat="server" ID="RowCompany" class="form-group mb-3">
    <label class="form-label">
        Organization
    </label>
    <div>
        <asp:Literal runat="server" ID="CompanyName" />
    </div>
</div>
<div runat="server" ID="RowCategory" class="form-group mb-3">
    <label class="form-label">
        Organization-Specific Categories
    </label>
    <div class="ms-2" style="max-height:400px; overflow-y:auto;">
        <asp:Repeater runat="server" ID="FolderRepeater">
            <ItemTemplate>
                <div class="mt-1 mb-2 fs-sm">
                    <%# Eval("FolderName") %>
                </div>
                <div class="ms-2"><insite:CheckBoxList runat="server" ID="CategoryList" /></div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Literal runat="server" ID="NoneLiteral" Text="None" />
    </div>
</div>