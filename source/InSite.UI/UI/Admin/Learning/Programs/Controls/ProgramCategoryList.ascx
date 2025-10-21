<%@ Control Language="C#" 
    CodeBehind="ProgramCategoryList.ascx.cs" 
    Inherits="InSite.UI.Admin.Learning.Programs.Controls.ProgramCategoryList" %>

<div class="form-group mb-3">
    <label class="form-label">
        Categories
    </label>
    <div class="ms-2">
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