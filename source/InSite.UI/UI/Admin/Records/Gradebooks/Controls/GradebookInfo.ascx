<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GradebookInfo.ascx.cs" Inherits="InSite.Admin.Records.Gradebooks.Controls.GradebookInfo" %>

<div class="form-group mb-3">
    <label class="form-label">
        Title
    </label>
    <div>
        <a runat="server" Id="GradebookLink">
            <asp:Literal runat="server" ID="GradebookTitle" />
        </a>
    </div>
    <div class="form-text">
        Created: <asp:Literal runat="server" ID="GradebookCreated" />
    </div>
    <div runat="server" id="LockedField" style="color:red;">
        Locked
    </div>
</div>
            
<div class="form-group mb-3">
    <label class="form-label">
        Class
    </label>
    <div>
        <asp:Literal runat="server" ID="ClassTitle" />
    </div>
    <div class="form-text">
        <asp:Literal runat="server" ID="ClassScheduled" />
    </div>
</div>

<div runat="server" id="ClassInstructorsField" class="form-group mb-3" visible="false">
    <label class="form-label">
        Class Instructors
    </label>
    <div>
        <asp:Repeater runat="server" ID="ClassInstructors">
            <ItemTemplate>
                <div>
                    <a href='<%# Eval("UserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'>
                        <%# Eval("UserFullName") %>
                    </a>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Achievement
    </label>
    <div>
        <asp:Literal runat="server" ID="AchievementTitle" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Include:
    </label>
    <div>
        <asp:CheckBox runat="server" ID="Scores" Enabled="false" Text="Scores" />
        <asp:CheckBox runat="server" ID="Standards" Enabled="false" Text="Standards" />
    </div>
</div>

<div runat="server" id="StandardField" class="form-group mb-3" visible="false">
    <label class="form-label">
        Framework
    </label>
    <div>
        <asp:Literal runat="server" ID="FrameworkTitle" />
    </div>
</div>

