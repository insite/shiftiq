<%@ Page Language="C#" CodeBehind="Workshop.aspx.cs" Inherits="InSite.Admin.Assessments.Specifications.Forms.Workshop" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<%@ Register TagPrefix="uc" TagName="WorkshopQuestionRepeater" Src="~/UI/Admin/Assessments/Questions/Controls/WorkshopQuestionRepeater.ascx" %>
<%@ Register TagPrefix="uc" TagName="WorkshopQuestionScript" Src="~/UI/Admin/Assessments/Questions/Controls/WorkshopQuestionScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="WorkshopCommentRepeater" Src="~/UI/Admin/Assessments/Comments/Controls/WorkshopCommentRepeater.ascx" %>
<%@ Register TagPrefix="uc" TagName="AttachmentsTabsNav" Src="~/UI/Admin/Assessments/Attachments/Controls/AttachmentsTabsNav.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProblemRepeater" Src="~/UI/Admin/Assessments/Outlines/Controls/ProblemRepeater.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:PageHeadContent runat="server">
        <style type="text/css">

            table.table-bankview > thead > tr > th,
            table.table-criterion > thead > tr > th,
            table.table-competency > thead > tr > th {
                background-color: white;
            }

            table.table-bankview > tbody > tr[data-competency] > td.cell-t1-actual,
            table.table-bankview > tbody > tr[data-competency] > td.cell-t2-actual,
            table.table-bankview > tbody > tr[data-competency] > td.cell-t3-actual {
                background-color: #f9f9f9;
            }

            .table-danger {
                background-color: #ecdede !important;
            }
            .table-success {
                background-color: #92cf77 !important;
                color: #fff !important;
            }

        </style>
    </insite:PageHeadContent>

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Specification" />

    <insite:Nav runat="server">

        <insite:NavItem runat="server" ID="SpecificationTab" Title="Specification" Icon="far fa-clipboard-list" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <div class="row">

                        <div class="col-lg-4">
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Framework
                                </label>
                                <div>
                                    <assessments:AssetTitleDisplay runat="server" ID="BankStandard" ShowLink="false" ShowParent="false" />
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Required Number of Forms
                                    <insite:RequiredValidator runat="server" ControlToValidate="SpecificationFormLimit" ValidationGroup="Specification" />
                                </label>
                                <insite:NumericBox runat="server" ID="SpecificationFormLimit" NumericMode="Integer" MinValue="0" />
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Required Number of Questions per Form
                                    <insite:RequiredValidator runat="server" ControlToValidate="SpecificationQuestionLimit" ValidationGroup="Specification" />
                                </label>
                                <insite:NumericBox runat="server" ID="SpecificationQuestionLimit" NumericMode="Integer" MinValue="0" />
                            </div>
                        </div>

                    </div>

                    <div class="row">

                        <div class="col-xxl-6 mb-3 mb-xxl-0">

                            <asp:Repeater runat="server" ID="CriterionTableRepeater">
                                <HeaderTemplate>
                            
                                    <table class="table table-sm table-criterion">
                                        <thead>
                                            <tr>
                                                <th>Criterion</th>
                                                <th>GAC</th>
                                                <th class="text-end text-nowrap" title="Set Weight">GAC %</th>
                                                <th class="text-end text-nowrap" title="Required Number of Questions">GAC #</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                        
                                </HeaderTemplate>
                                <FooterTemplate>
                                        
                                        </tbody>
                                        <tfoot>
                                            <tr>
                                                <th colspan="2"></th>
                                                <th class="text-end cell-weight"></th>
                                                <th class="text-end cell-criterion"></th>
                                            </tr>
                                        </tfoot>
                                    </table>
                                    
                                </FooterTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("CriterionTitle") %></td>
                                        <td>
                                            <asp:Repeater runat="server" ID="SetStandardRepeater">
                                                <ItemTemplate>
                                                    <div>
                                                        <assessments:AssetTitleDisplay runat="server" ID="SetStandard" AssetID='<%# (Guid)Container.DataItem %>' ShowLink="false" ShowParent="false" />
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </td>
                                        <td class="text-end align-middle cell-weight">
                                            <insite:NumericBox runat="server" ID="SetWeight" 
                                                NumericMode="Integer" MinValue="0" MaxValue="100" Width="45px"
                                                ValueAsDecimal='<%# Eval("SetWeight") %>'
                                                CssClass="text-end form-control-sm px-2" />
                                        </td>
                                        <td class="text-end align-middle cell-criterion" data-criterion='<%# Eval("CriterionID") %>'></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            
                        </div>
                        <div class="col-xxl-6">

                            <asp:Repeater runat="server" ID="CompetencyTableRepeater">
                                <HeaderTemplate>
                            
                                    <table class="table table-sm table-competency">
                                        <thead>
                                            <tr>
                                                <th>Competency</th>
                                                <th class="text-end text-nowrap">GAC #</th>
                                                <th class="text-end text-nowrap">Competency</th>
                                                <th class="text-end text-nowrap">T1</th>
                                                <th class="text-end text-nowrap">T2</th>
                                                <th class="text-end text-nowrap">T3</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                        
                                </HeaderTemplate>
                                <FooterTemplate>
                                        
                                        </tbody>
                                        <tfoot>
                                            <tr>
                                                <th></th>
                                                <th class="text-end cell-criterion"></th>
                                                <th class="text-end cell-competency"></th>
                                                <th class="text-end cell-t1"></th>
                                                <th class="text-end cell-t2"></th>
                                                <th class="text-end cell-t3"></th>
                                            </tr>
                                        </tfoot>
                                    </table>
                                    
                                </FooterTemplate>
                                <ItemTemplate>
                                    <tr class="fw-bold" data-criterion='<%# Eval("CriterionID") %>'>
                                        <td><%# Eval("CriterionTitle") %></td>
                                        <td class="text-end cell-criterion-total"></td>
                                        <td class="text-end cell-competency-total"></td>
                                        <td colspan="3"></td>
                                    </tr>
                                    <asp:Repeater runat="server" ID="CompetencyRepeater">
                                        <ItemTemplate>
                                            <tr data-competency='<%# Eval("CompetencyStandardIdentifier") %>'>
                                                <td><%# Eval("CompetencyName") %></td>
                                                <td class="text-end align-middle cell-criterion"></td>
                                                <td class="text-end align-middle cell-competency"></td>
                                                <td class="text-end align-middle cell-t1">
                                                    <insite:NumericBox runat="server" ID="Tax1Count"
                                                        NumericMode="Integer" Width="45px"
                                                        ValueAsInt='<%# (int?)Eval("Tax1Count") %>'
                                                        CssClass="text-end form-control-sm px-2" />
                                                </td>
                                                <td class="text-end align-middle cell-t2">
                                                    <insite:NumericBox runat="server" ID="Tax2Count"
                                                        NumericMode="Integer" Width="45px"
                                                        ValueAsInt='<%# (int?)Eval("Tax2Count") %>'
                                                        CssClass="text-end form-control-sm px-2" />
                                                </td>
                                                <td class="text-end align-middle cell-t3">
                                                    <insite:NumericBox runat="server" ID="Tax3Count"
                                                        NumericMode="Integer" Width="45px"
                                                        ValueAsInt='<%# (int?)Eval("Tax3Count") %>'
                                                        CssClass="text-end form-control-sm px-2" />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ItemTemplate>
                            </asp:Repeater>
                            
                            
                        </div>
                    </div>
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="BankViewTab" Title="Bank View" Icon="far fa-balance-scale">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <asp:Repeater runat="server" ID="BankViewRepeater">
                        <HeaderTemplate>
                    
                            <table class="table table-sm table-bankview">
                                <thead>
                                    <tr>
                                        <th class="align-bottom">Competency</th>
                                        <th class="text-center text-nowrap align-bottom">Planned<br />GAC</th>
                                        <th class="text-center text-nowrap align-bottom">Planned<br />Competency</th>
                                        <th class="text-center text-nowrap align-bottom">Total<br />Actual</th>
                                        <th class="text-center text-nowrap align-bottom">Variance</th>
                                        <th class="text-center text-nowrap align-bottom">T1<br />Planned</th>
                                        <th class="text-center text-nowrap align-bottom">T1<br />Actual</th>
                                        <th class="text-center text-nowrap align-bottom">T2<br />Planned</th>
                                        <th class="text-center text-nowrap align-bottom">T2<br />Actual</th>
                                        <th class="text-center text-nowrap align-bottom">T3<br />Planned</th>
                                        <th class="text-center text-nowrap align-bottom">T3<br />Actual</th>
                                        <th class="text-center text-nowrap align-bottom">Unassigned</th>
                                    </tr>
                                </thead>
                                <tbody>
                                
                        </HeaderTemplate>
                        <FooterTemplate>
                                
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <th></th>
                                        <th class="text-end cell-planned-criterion"></th>
                                        <th class="text-end cell-planned-competency"></th>
                                        <th class="text-end cell-total-actual"></th>
                                        <th class="text-end cell-variance"></th>
                                        <th class="text-end cell-t1-planned"></th>
                                        <th class="text-end cell-t1-actual"></th>
                                        <th class="text-end cell-t2-planned"></th>
                                        <th class="text-end cell-t2-actual"></th>
                                        <th class="text-end cell-t3-planned"></th>
                                        <th class="text-end cell-t3-actual"></th>
                                        <th class="text-end cell-unassigned"></th>
                                    </tr>
                                    <tr>
                                        <th colspan="3"></th>
                                        <th class="text-end cell-completed"></th>
                                        <th colspan="8"></th>
                                    </tr>
                                </tfoot>
                            </table>
                            
                        </FooterTemplate>
                        <ItemTemplate>
                            <tr class="fw-bold" data-criterion='<%# Eval("CriterionID") %>'>
                                <td><%# Eval("CriterionTitle") %></td>
                                <td class="text-end cell-planned-criterion"></td>
                                <td class="text-end cell-planned-competency"></td>
                                <td class="text-end cell-total-actual"></td>
                                <td class="text-end cell-variance"></td>
                                <td class="text-end cell-t1-planned"></td>
                                <td class="text-end cell-t1-actual"></td>
                                <td class="text-end cell-t2-planned"></td>
                                <td class="text-end cell-t2-actual"></td>
                                <td class="text-end cell-t3-planned"></td>
                                <td class="text-end cell-t3-actual"></td>
                                <td class="text-end cell-unassigned"></td>
                            </tr>
                            <asp:Repeater runat="server" ID="CompetencyRepeater">
                                <ItemTemplate>
                                    <tr data-competency='<%# Eval("CompetencyStandardIdentifier") %>'>
                                        <td><%# Eval("CompetencyName") %></td>
                                        <td class="text-end cell-planned-criterion"></td>
                                        <td class="text-end cell-planned-competency"></td>
                                        <td class="text-end cell-total-actual"><%# Eval("QuestionsCount", "{0:n0}") %></td>
                                        <td class="text-end cell-variance"></td>
                                        <td class="text-end cell-t1-planned"></td>
                                        <td class="text-end cell-t1-actual"><%# Eval("Tax1Count", "{0:n0}") %></td>
                                        <td class="text-end cell-t2-planned"></td>
                                        <td class="text-end cell-t2-actual"><%# Eval("Tax2Count", "{0:n0}") %></td>
                                        <td class="text-end cell-t3-planned"></td>
                                        <td class="text-end cell-t3-actual"><%# Eval("Tax3Count", "{0:n0}") %></td>
                                        <td class="text-end cell-unassigned"><%# Eval("UnassignedCount", "{0:n0}") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ItemTemplate>
                    </asp:Repeater>
                    
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="QuestionsTab" Title="Questions" Icon="far fa-question" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="QuestionsUpdatePanel" />

                    <insite:UpdatePanel runat="server" ID="QuestionsUpdatePanel">
                        <ContentTemplate>

                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Set
                                        </label>
                                        <insite:ComboBox runat="server" ID="SetSelector" AllowBlank="false" />
                                    </div>
                                </div>

                                <div class="col-md-6">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Competency
                                        </label>
                                        <insite:StandardComboBox runat="server" ID="CompetencySelector" TextType="CodeTitle" />
                                    </div>
                                </div>
                            </div>

                            <div class="mb-3">
                                <insite:Button runat="server" ID="MoveQuestionsButton" ButtonStyle="Default"
                                    ToolTip="Move the questions to another bank" 
                                    Text="Move" Icon="far fa-file-export" />
                                <insite:DropDownButton runat="server" ID="AddQuestionButton1" DefaultAction="PostBack"
                                    IconType="Regular" IconName="plus-circle"
                                    Text="Add" ToolTip="Add a new multiple choice question to selected set">
                                    <Items>
                                        <insite:DropDownButtonItem Name="QuickMultipleChoice" IconType="Regular" IconName="check-circle" Text="Multiple Choice" ToolTip="Add a new multiple choice question to selected set" />
                                        <insite:DropDownButtonItem Name="QuickMultipleCorrect" IconType="Regular" IconName="check-square" Text="Multiple Correct" ToolTip="Add a new multiple correct question to selected set" />
                                        <insite:DropDownButtonItem Name="QuickComposedEssay" IconType="Regular" IconName="file-lines" Text="Composed Essay" ToolTip="Add a new composed essay response question to selected set" />
                                        <insite:DropDownButtonItem Name="QuickComposedVoice" IconType="Regular" IconName="microphone" Text="Composed Voice" ToolTip="Add a new composed voice response question to selected set" />
                                        <insite:DropDownButtonItem Name="QuickBooleanTable" IconType="Regular" IconName="th" Text="Multiple True/False List" ToolTip="Add a new multiple true/false list question to selected set" />
                                        <insite:DropDownButtonItem Name="QuickMatching" IconType="Regular" IconName="exchange" Text="Matching" ToolTip="Add a new matching question to selected set" />
                                        <insite:DropDownButtonSeparatorItem />
                                        <insite:DropDownButtonItem Name="Creator" IconType="Solid" IconName="plus-circle" Text="Go To Creator" ToolTip="Add a new question to selected set using Question Creator" />
                                    </Items>
                                </insite:DropDownButton>
                            </div>

                            <insite:Container runat="server" ID="QuestionFilter">
                                <div class="row">
                                    <div class="col-sm-6">
                                        <div class="row">

                                            <div class="col-md-6">
                                                <div class="form-group mb-3">
                                                    <label class="form-label">
                                                        Question Flag
                                                    </label>
                                                    <div>
                                                        <insite:FlagMultiComboBox runat="server" ID="QuestionFlag" AllowBlank="false" Multiple-ActionsBox="true" />
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-md-6">
                                                <div class="form-group mb-3">
                                                    <label class="form-label">
                                                        Condition
                                                    </label>
                                                    <div>
                                                        <insite:QuestionConditionMultiComboBox runat="server" ID="QuestionCondition" Multiple-ActionsBox="true" />
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="row">

                                            <div class="col-lg-4">
                                                <div class="form-group mb-3">
                                                    <label class="form-label">
                                                        Taxonomy
                                                    </label>
                                                    <insite:TaxonomyComboBox runat="server" ID="QuestionTaxonomy" />
                                                </div>
                                            </div>

                                            <div class="col-lg-4">
                                                <div class="form-group mb-3">
                                                    <label class="form-label">
                                                        LIG
                                                    </label>
                                                    <insite:BooleanComboBox runat="server" ID="IsQuestionHasLig" TrueText="LIG" FalseText="No LIG" AllowBlank="true" />
                                                </div>
                                            </div>

                                            <div class="col-lg-4">
                                                <div class="form-group mb-3">
                                                    <label class="form-label">
                                                        Reference
                                                    </label>
                                                    <insite:BooleanComboBox runat="server" ID="IsQuestionHasReference" TrueText="Reference" FalseText="No Reference" AllowBlank="true" />
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                                <div class="row">

                                    <div class="col-lg-3">
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Question Changed On
                                            </label>
                                            <div>
                                                <insite:ComboBox runat="server" ID="QuestionDateRangeSelector" />
                                            </div>
                                        </div>
                                    </div>

                                    <div runat="server" ID="QuestionDateRangeInputs" class="col-lg-5 d-none">
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                &nbsp;
                                            </label>
                                            <div>
                                                <div class="d-inline-block" style="width:calc(50% - 20px);">
                                                    <insite:DateSelector runat="server" ID="QuestionDateRangeSince" />
                                                </div>
                                                <div class="d-inline-block text-center" style="width:30px;">
                                                    to
                                                </div>
                                                <div class="d-inline-block" style="width:calc(50% - 20px);">
                                                    <insite:DateSelector runat="server" ID="QuestionDateRangeBefore" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </insite:Container>

                            <div>
                                <insite:Button runat="server" ID="ApplyFilterButton" ButtonStyle="Default" 
                                    Text="Apply" Icon="fas fa-filter" />
                                <insite:Button runat="server" ID="ClearFilterButton" ButtonStyle="Default" 
                                    Text="Clear" Icon="fas fa-times" />
                            </div>

                            <hr class="mt-4" />

                            <div class="mt-3">
                                <h3 runat="server" id="QuestionsHeader" >
                                    Questions
                                    <span class="form-text">(<asp:Literal runat="server" ID="QuestionCount" />)</span>
                                </h3>

                                <uc:WorkshopQuestionRepeater runat="server" ID="QuestionRepeater" />
                            </div>

                            <div class="mt-3">
                                <insite:DropDownButton runat="server" ID="AddQuestionButton2" />
                            </div>
                        </ContentTemplate>
                    </insite:UpdatePanel>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="CommentsTab" Title="Comments" Icon="far fa-comments" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <div class="mb-3">
                        <insite:AddButton runat="server" id="AddCommentLink" Text="New Comment" />
                    </div>

                    <uc:WorkshopCommentRepeater runat="server" ID="CommentRepeater" />

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="AttachmentsTab" Title="Attachments" Icon="far fa-paperclip" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <div class="row mb-3">
                        <div class="col-lg-6">
                            <insite:TextBox runat="server" ID="FilterAttachmentsTextBox" EmptyMessage="Filter Attachments" />
                        </div>
                    </div>

                    <uc:AttachmentsTabsNav runat="server" ID="AttachmentsNav" KeywordInput="FilterAttachmentsTextBox" />

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ProblemTab" Title="Problems" Icon="far fa-exclamation-triangle" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <uc:ProblemRepeater runat="server" ID="ProblemRepeater" />

                </div>
            </div>
        </insite:NavItem>

    </insite:Nav>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Specification" />
        <insite:CancelButton runat="server" ID="CloseButton" />
    </div>

    <insite:Modal runat="server" ID="CommentWindow" Title="New Comment" Width="700px">
        <ContentTemplate>

            <div class="p-2">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Author Type
                    </label>
                    <insite:CommentAuthorTypeComboBox runat="server" ID="CommentAuthorType" AllowBlank="false" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Flag
                    </label>
                    <insite:FlagComboBox runat="server" ID="CommentFlag" AllowBlank="false" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Text
                    </label>
                    <insite:TextBox runat="server" ID="CommentText" TextMode="MultiLine" Rows="4" />
                </div>

                <div class="mt-3">
                    <insite:SaveButton runat="server" ID="PostCommentButton" Text="Post" OnClientClick="saveCommentWindow(); return false;" />
                    <insite:CancelButton runat="server" OnClientClick="closeCommentWindow(); return false;" />
                </div>

            </div>

            <insite:LoadingPanel runat="server" />

        </ContentTemplate>
    </insite:Modal>

    <uc:WorkshopQuestionScript runat="server" ID="WorkshopScript" />

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            var _questionId;

            function showCommentWindow(questionId) {
                _questionId = questionId;

                modalManager.show($('#<%= CommentWindow.ClientID %>'));
            }

            function closeCommentWindow() {
                modalManager.close($('#<%= CommentWindow.ClientID %>'));
            }

            function saveCommentWindow() {
                var $loadingPanel = $('#<%= CommentWindow.ClientID %> .loading-panel').show();

                $.ajax({
                    type: 'POST',
                    data:
                    {
                        isPageAjax: true,
                        questionId: _questionId,
                        authorType: $('#<%= CommentAuthorType.ClientID %>').selectpicker('val'),
                        flag: $('#<%= CommentFlag.ClientID %>').selectpicker('val'),
                        text: $('#<%= CommentText.ClientID %>').val()
                    },
                    error: function () {
                        alert('An error occurred during operation.');
                    },
                    success: function (data) {
                        var $section = $('tr[data-question="' + _questionId + '"] div.posted-comments-section');

                        $section.show();
                        $section.html(data);

                        {
                            var $combo = $('#<%= CommentAuthorType.ClientID %>');
                            $combo.selectpicker('val', $combo.find('option:first').prop('value'));
                        }

                        {
                            var $combo = $('#<%= CommentFlag.ClientID %>');
                            $combo.selectpicker('val', $combo.find('option:first').prop('value'));
                        }

                        $('#<%= CommentText.ClientID %>').val('');

                        bindScrollParameter();

                        modalManager.close($('#<%= CommentWindow.ClientID %>'));
                    },
                    complete: function () {
                        $loadingPanel.hide();
                    }
                });
            }

            (function () {
                Sys.Application.add_load(function () {
                    $('#<%= QuestionDateRangeSelector.ClientID %>')
                        .off('change', onDateRangeSelected)
                        .on('change', onDateRangeSelected);

                    onDateRangeSelected();
                });

                function onDateRangeSelected() {
                    var $dateRange = $('#<%= QuestionDateRangeSelector.ClientID %>');
                    var $panel = $('#<%= QuestionDateRangeInputs.ClientID %>');

                    if ($dateRange.selectpicker('val') == 'Custom')
                        $panel.removeClass('d-none');
                    else
                        $panel.addClass('d-none');
                }
            })();

            (function () {
                var $criterionTable = $('table.table-criterion');
                var $competencyTable = $('table.table-competency');
                var $bankViewTable = $('table.table-bankview');

                Sys.Application.add_load(onLoad);

                function onLoad() {
                    $('#<%= SpecificationFormLimit.ClientID %>,#<%= SpecificationQuestionLimit.ClientID %>')
                        .off('change', updateCriterionTable)
                        .on('change', updateCriterionTable);

                    $criterionTable.find('> tbody > tr > td.cell-weight input[type="text"].insite-numeric')
                        .off('change', updateCriterionTable)
                        .on('change', updateCriterionTable);

                    $competencyTable.find('> tbody > tr > td.cell-t1 input[type="text"].insite-numeric,> tbody > tr > td.cell-t2 input[type="text"].insite-numeric,> tbody > tr > td.cell-t3 input[type="text"].insite-numeric')
                        .off('change', updateCompetencyTable)
                        .on('change', updateCompetencyTable);

                    updateCriterionTable();
                }

                function updateCriterionTable() {
                    var questionLimit = parseNumber($('#<%= SpecificationQuestionLimit.ClientID %>').val());

                    var totalWeight = 0;
                    var totalQuestions = 0;

                    $criterionTable.find('> tbody > tr').each(function () {
                        var $row = $(this);
                        var $weightInput = $row.find('> td.cell-weight input[type="text"].insite-numeric');
                        var weightValue = zeroIfNaN(parseNumber($weightInput.val()));
                        var questionsValue = Math.round(questionLimit * weightValue / 100);

                        totalWeight += weightValue;
                        totalQuestions += questionsValue;

                        var criterionId = $row.find('> td.cell-criterion').text(formatNumber(questionsValue)).data('criterion');

                        $competencyTable.find('> tbody > tr[data-criterion="' + String(criterionId) + '"] > td.cell-criterion-total').text(formatNumber(questionsValue));
                    });

                    var $cellWeight = $criterionTable.find('> tfoot > tr > .cell-weight').text(formatNumber(totalWeight));

                    if (totalWeight === 100)
                        $cellWeight.removeClass('table-danger');
                    else
                        $cellWeight.addClass('table-danger');

                    $criterionTable.find('> tfoot > tr > .cell-criterion').text(formatNumber(totalQuestions));

                    updateCompetencyTable();
                }

                function updateCompetencyTable() {
                    var formLimit = parseNumber($('#<%= SpecificationFormLimit.ClientID %>').val());

                    var totalCriterion = 0;
                    var totalCompetencies = 0;
                    var subtotalCompetencies = 0;

                    var $lastCriterionRow = null;

                    $competencyTable.find('> tbody > tr').each(function () {
                        var $row = $(this);

                        if (this.hasAttribute('data-criterion')) {
                            if ($lastCriterionRow != null)
                                setCriterionSubtotal();

                            $lastCriterionRow = $row;
                        } else if (this.hasAttribute('data-competency')) {
                            var competencyId = $row.data('competency');

                            var $tax1Input = $row.find('> td.cell-t1 input[type="text"].insite-numeric');
                            var tax1Value = zeroIfNaN(parseNumber($tax1Input.val()));

                            var $tax2Input = $row.find('> td.cell-t2 input[type="text"].insite-numeric');
                            var tax2Value = zeroIfNaN(parseNumber($tax2Input.val()));

                            var $tax3Input = $row.find('> td.cell-t3 input[type="text"].insite-numeric');
                            var tax3Value = zeroIfNaN(parseNumber($tax3Input.val()));

                            var competencyValue = tax1Value + tax2Value + tax3Value;
                            subtotalCompetencies += competencyValue;

                            $row.find('> td.cell-competency').text(formatNumber(competencyValue));

                            $bankViewTable.find('> tbody > tr[data-competency="' + String(competencyId) + '"]').each(function () {
                                var $row = $(this);

                                var competencyActual = zeroIfNaN(parseNumber($row.find('> td.cell-total-actual').text()));

                                $row.find('> td.cell-planned-competency').text(formatNumber(competencyValue * formLimit));
                                $row.find('> td.cell-variance').each(function () {
                                    var $cell = $(this).removeClass('table-danger table-success');

                                    var competencyVariance = competencyActual - competencyValue * formLimit;
                                    if (competencyVariance < 0)
                                        $cell.addClass('table-danger');
                                    else if (competencyVariance > 0)
                                        $cell.addClass('table-success');

                                    $cell.text(formatNumber(competencyVariance));
                                });

                                $row.find('> td.cell-t1-planned').text(formatNumber(tax1Value * formLimit));
                                $row.find('> td.cell-t2-planned').text(formatNumber(tax2Value * formLimit));
                                $row.find('> td.cell-t3-planned').text(formatNumber(tax3Value * formLimit));
                            });
                        }
                    });

                    if ($lastCriterionRow != null)
                        setCriterionSubtotal();

                    $competencyTable.find('> tfoot > tr > .cell-criterion').text(formatNumber(totalCriterion));
                    $competencyTable.find('> tfoot > tr > .cell-competency').text(formatNumber(totalCompetencies));

                    updateBankViewTable();

                    function setCriterionSubtotal() {
                        var criterionId = $lastCriterionRow.data('criterion');
                        var $criterionTotal = $lastCriterionRow.find('> td.cell-criterion-total');
                        var $competencyTotal = $lastCriterionRow.find('> td.cell-competency-total');

                        var criterionCount = zeroIfNaN(parseNumber($criterionTotal.text()));

                        totalCriterion += criterionCount;

                        $competencyTotal.text(formatNumber(subtotalCompetencies));

                        if (criterionCount !== subtotalCompetencies)
                            $competencyTotal.addClass('table-danger');
                        else
                            $competencyTotal.removeClass('table-danger');

                        $bankViewTable
                            .find('> tbody > tr[data-criterion="' + String(criterionId) + '"] > td.cell-planned-criterion')
                            .text(formatNumber(formLimit * criterionCount));

                        totalCompetencies += subtotalCompetencies;
                        subtotalCompetencies = 0;
                    }
                }

                function updateBankViewTable() {
                    var plannedCriterion = 0;
                    var plannedCompetency = 0;
                    var actualCompetency = 0;
                    var variance = 0;
                    var t1Planned = 0;
                    var t1Actual = 0;
                    var t2Planned = 0;
                    var t2Actual = 0;
                    var t3Planned = 0;
                    var t3Actual = 0;
                    var unassigned = 0;

                    $bankViewTable.find('> tbody > tr').each(function () {
                        var $row = $(this);

                        if (this.hasAttribute('data-criterion')) {
                            plannedCriterion += zeroIfNaN(parseNumber($row.find('> td.cell-planned-criterion').text()));
                        } else if (this.hasAttribute('data-competency')) {
                            var $rowT1Planned = $row.find('> td.cell-t1-planned');
                            var $rowT1Actual = $row.find('> td.cell-t1-actual');
                            var $rowT2Planned = $row.find('> td.cell-t2-planned');
                            var $rowT2Actual = $row.find('> td.cell-t2-actual');
                            var $rowT3Planned = $row.find('> td.cell-t3-planned');
                            var $rowT3Actual = $row.find('> td.cell-t3-actual');

                            var rowT1PlannedValue = zeroIfNaN(parseNumber($rowT1Planned.text()));
                            var rowT1ActualValue = zeroIfNaN(parseNumber($rowT1Actual.text()));
                            var rowT2PlannedValue = zeroIfNaN(parseNumber($rowT2Planned.text()));
                            var rowT2ActualValue = zeroIfNaN(parseNumber($rowT2Actual.text()));
                            var rowT3PlannedValue = zeroIfNaN(parseNumber($rowT3Planned.text()));
                            var rowT3ActualValue = zeroIfNaN(parseNumber($rowT3Actual.text()));

                            if (rowT1PlannedValue > rowT1ActualValue)
                                $rowT1Actual.addClass('text-danger');
                            else
                                $rowT1Actual.removeClass('text-danger');

                            if (rowT2PlannedValue > rowT2ActualValue)
                                $rowT2Actual.addClass('text-danger');
                            else
                                $rowT2Actual.removeClass('text-danger');

                            if (rowT3PlannedValue > rowT3ActualValue)
                                $rowT3Actual.addClass('text-danger');
                            else
                                $rowT3Actual.removeClass('text-danger');

                            plannedCompetency += zeroIfNaN(parseNumber($row.find('> td.cell-planned-competency').text()));
                            actualCompetency += zeroIfNaN(parseNumber($row.find('> td.cell-total-actual').text()));
                            variance += zeroIfNaN(parseNumber($row.find('> td.cell-variance').text()));
                            t1Planned += rowT1PlannedValue;
                            t1Actual += rowT1ActualValue;
                            t2Planned += rowT2PlannedValue;
                            t2Actual += rowT2ActualValue;
                            t3Planned += rowT3PlannedValue;
                            t3Actual += rowT3ActualValue;
                            unassigned += zeroIfNaN(parseNumber($row.find('> td.cell-unassigned').text()));
                        }
                    });

                    $bankViewTable.find('> tfoot > tr > th.cell-planned-criterion').text(formatNumber(plannedCriterion));
                    $bankViewTable.find('> tfoot > tr > th.cell-planned-competency').text(formatNumber(plannedCompetency));
                    $bankViewTable.find('> tfoot > tr > th.cell-total-actual').text(formatNumber(actualCompetency));
                    $bankViewTable.find('> tfoot > tr > th.cell-completed').text(formatNumber(Math.round(actualCompetency / plannedCompetency * 100), '%'));
                    $bankViewTable.find('> tfoot > tr > th.cell-variance').each(function () {
                        var $cell = $(this).removeClass('table-danger table-success');

                        if (variance < 0)
                            $cell.addClass('table-danger');
                        else if (variance > 0)
                            $cell.addClass('table-success');

                        $cell.text(formatNumber(variance));
                    });
                    $bankViewTable.find('> tfoot > tr > th.cell-t1-planned').text(formatNumber(t1Planned));
                    $bankViewTable.find('> tfoot > tr > th.cell-t1-actual').text(formatNumber(t1Actual));
                    $bankViewTable.find('> tfoot > tr > th.cell-t2-planned').text(formatNumber(t2Planned));
                    $bankViewTable.find('> tfoot > tr > th.cell-t2-actual').text(formatNumber(t2Actual));
                    $bankViewTable.find('> tfoot > tr > th.cell-t3-planned').text(formatNumber(t3Planned));
                    $bankViewTable.find('> tfoot > tr > th.cell-t3-actual').text(formatNumber(t3Actual));
                    $bankViewTable.find('> tfoot > tr > th.cell-unassigned').text(formatNumber(unassigned));
                }

                function formatNumber(value, postfix) {
                    if (!isFinite(value) || isNaN(value))
                        return 'N/A';

                    if (value == null)
                        return '';

                    var result = value.toLocaleString('en-CA');

                    if (typeof postfix === 'string')
                        result = result + postfix;

                    return result;
                }

                function parseNumber(value) {
                    if (value == null || value == '')
                        return NaN;

                    value = String(value).replace(/,/, '');

                    return parseInt(value);
                }

                function zeroIfNaN(value) {
                    if (isNaN(value))
                        return 0;

                    return value;
                }
            })();

            (function () {
                var $window = $(window);

                var data = [];

                addTable('table.table-criterion');
                addTable('table.table-competency');
                addTable('table.table-bankview');

                $window.on('scroll click resize', function (e) {
                    var scrollTop = $window.scrollTop();

                    var headerHeight = $('header.navbar:first').outerHeight() - 1;

                    for (var i = 0; i < data.length; i++) {
                        var item = data[i];
                        if (!item.$table.is(':visible'))
                            continue;

                        if (item.prevScrollTop === scrollTop)
                            continue;

                        item.prevScrollTop = scrollTop;

                        var top = scrollTop - item.$table.offset().top + headerHeight;

                        if (top > 0) {
                            var bottom = scrollTop - item.$lastRow.offset().top + headerHeight + item.$headers.first().outerHeight();
                            if (bottom > 0)
                                top = top - bottom;
                        }

                        if (top <= 0) {
                            if (item.isFixed) {
                                item.$headers.css('transform', '');
                                item.isFixed = false;
                            }

                            continue;
                        }

                        item.$headers.css('transform', 'translateY(' + String(top) + 'px)');
                        item.isFixed = true;
                    }
                });

                function addTable(selector) {
                    var $table = $(selector);

                    data.push({
                        $table: $table,
                        $headers: $table.find('> thead > tr > th'),
                        $lastRow: $table.find('> tbody > tr:last'),
                        prevScrollTop: -1,
                        isFixed: false
                    });
                }
            })();

        </script>
    </insite:PageFooterContent>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                if (typeof URLSearchParams !== 'undefined') {
                    var location = window.location;
                    var queryParams = new URLSearchParams(location.search);
                    var scroll = queryParams.get('scroll');

                    if (scroll) {
                        try {
                            var scrollTop = inSite.common.base64.toInt(scroll);

                            $(document).ready(function () {
                                $(window).scrollTop(scrollTop);
                            });

                            queryParams.delete('scroll');

                            var path = location.pathname + '?' + queryParams.toString();
                            var url = location.origin + path;

                            window.history.replaceState({}, '', url);

                            $('form#aspnetForm').attr('action', path);
                        } catch (e) {

                        }
                    }
                }

                bindScrollParameter();
            })();

            function bindScrollParameter() {
                $('a.scroll-send').on('click', function () {
                    var scroll = $(window).scrollTop();

                    if (scroll > 0)
                        scroll = inSite.common.base64.fromInt(Math.floor(scroll));
                    else
                        scroll = null;

                    var url = $(this).attr('href');

                    $(this).attr('href', inSite.common.updateQueryString('scroll', scroll, url));
                });
            }

        </script>
    </insite:PageFooterContent>
</asp:Content>
