<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionsSection.ascx.cs" Inherits="InSite.Admin.Assessments.Outlines.Controls.QuestionsSection" %>

<insite:Alert runat="server" ID="ControlStatus" />

<div runat="server" id="NoQuestionsAlert" class="alert alert-warning">
    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
    The bank does not yet contain any questions. Click "Add Set" to start a new set of questions.
</div>

<div class="row mb-3">
    <asp:Panel runat="server" DefaultButton="FilterQuestionsButton" CssClass="col-lg-4 mb-3 mb-lg-0">
        <insite:TextBox runat="server" ID="FilterQuestionsTextBox" EmptyMessage="Filter Questions" CssClass="d-inline-block" style="width:calc(100% - 25px);" />
        <insite:IconButton runat="server" ID="FilterQuestionsButton" Name="filter" ToolTip="Apply Filter" />
    </asp:Panel>

    <div class="col-lg-8 text-end">
        <insite:Button runat="server" ID="PreviewSetButton" ButtonStyle="Default" ToolTip="Preview the questions in this set" Text="Preview" Icon="fas fa-external-link" />

        <insite:DropDownButton runat="server" ID="AddButton" IconName="plus-circle" Text="Add" CssClass="d-inline-block">
            <Items>
                <insite:DropDownButtonItem Name="AddQuestion" ToolTip="Add a new question to this set" IconName="question" Text="Question" />
                <insite:DropDownButtonItem Name="AddSet" ToolTip="Add a new set to the bank" IconName="th-list" Text="Set"  />
            </Items>
        </insite:DropDownButton>

        <insite:DropDownButton runat="server" ID="SortButton" IconName="sort" Text="Reorder" CssClass="d-inline-block">
            <Items>
                <insite:DropDownButtonItem Name="SortQuestions" ToolTip="Reorder the questions in this set" IconName="question" Text="Questions" />
                <insite:DropDownButtonItem Name="SortSets" ToolTip="Reorder the sets in this bank" IconName="th-list" Text="Sets"  />
            </Items>
        </insite:DropDownButton>

        <insite:DropDownButton runat="server" ID="DownloadButton" IconName="download" Text="Download" CssClass="d-inline-block" MenuCssClass="dropdown-menu-end">
            <Items>
                <insite:DropDownButtonItem Name="SetMarkdown" ToolTip="Add a new question to this set" IconName="hashtag" Text="Shift iQ Markdown Outline (*.md)" />
            </Items>
        </insite:DropDownButton>
    </div>
</div>

<div class="row">
    <div class="col-lg-2 col-4">
        <insite:Nav runat="server" ID="SetsNav" ItemType="Pills" ItemAlignment="Vertical" ContentRendererID="SetsNavContent">

        </insite:Nav>
    </div>
    <div id="sets-nav-content" class="col-lg-10 col-8">
        <insite:NavContent runat="server" ID="SetsNavContent" />
    </div>
</div>
