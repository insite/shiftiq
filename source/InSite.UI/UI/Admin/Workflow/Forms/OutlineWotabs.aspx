<%@ Page Language="C#" CodeBehind="OutlineWotabs.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.OutlineWotabs" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/QuestionsManager.ascx" TagName="QuestionsManager" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/MessageDetails.ascx" TagName="MessageDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        
        .conditions-list {

        }

        .conditions-list dl.dl-horizontal + dl.dl-horizontal {
            padding-top: 20px;
            border-top: dotted 1px #ccc;
        }

        .conditions-list dl.dl-horizontal dd ul {
            padding-left: 30px;
        }

        .conditions-list dl.dl-horizontal dd ul li {
            margin-top: 15px !important;
        }

        .branches-list {
        }

        .branches-list > div {
            margin-bottom: 20px;
        }

        .branches-list > div + div {
            padding-top: 20px;
            border-top: dotted 1px #ccc;
        }

        .branches-list > div > h3 {
            margin: 0;
        }

        .branches-list > div > h4,
        .branches-list > div > h5 {
            margin: 10px 0 0 0;
        }

        .command-buttons .btn,
        .command-buttons .btn-group {
            display: block;
            float: right;
        }

        .command-buttons .btn + .btn,
        .command-buttons .btn-group + .btn {
            margin-right: 5px;
        }

        .command-buttons .seprator {
            margin: 1px 8px;
            border-left: 1px dotted #ccc;
            float: right;
            height: 31px;
        }

        .command-buttons .seprator:first-child {
            display: none;
        }

        .command-buttons .seprator + .seprator {
            display: none;
        }

    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="HelpContent">
    <div class="mb-3">
        <insite:DropDownButton runat="server" Text="Jump To" ButtonStyle="OutlinePrimary">
            <Items>
                <insite:DropDownButtonLinkItem NavigateUrl="#Top" Text="Form" />
                <insite:DropDownButtonLinkItem NavigateUrl="#QuestionsAnchor" Text="Questions" />
                <insite:DropDownButtonLinkItem NavigateUrl="#ContentAnchor" Text="Content" />
                <insite:DropDownButtonLinkItem NavigateUrl="#MessagesAnchor" Text="Messages" />
            </Items>
        </insite:DropDownButton>
    </div>
    <div class="mb-5">
        <div class="btn-group" role="group">
            <a href="#Top" class="btn btn-outline-primary">First</a>
            <a href="#" class="btn btn-outline-primary disabled">Prev</a>
            <a href="#" class="btn btn-outline-primary disabled">Next</a>
            <a href="#MessagesAnchor" class="btn btn-outline-primary">Last</a>
        </div>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />
    <insite:Alert runat="server" ID="DuplicateStatus" />
    <insite:Alert runat="server" ID="ValidationStatus" />
    <insite:Alert runat="server" ID="LockStatus" />
    <insite:Alert runat="server" ID="RemoveStatus" />
    <insite:Alert runat="server" ID="BranchValidationStatus" />
        
    <section id="SurveyAnchor" class="pb-5 mb-md-2">        
        <h2 class="h4 mb-1">Form</h2>
        <div class="row">
            <div class="col-lg-12">
                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <div class="row button-group mb-4">
                            <div class="col-lg-12">                   
                                <insite:Button runat="server" ID="NewSurveyLink" Text="New" Icon="fas fa-file" CssClass="btn" ButtonStyle="Default" />

                                <insite:ButtonSpacer runat="server" />

                                <insite:Button runat="server" ID="DuplicateLink" Text="Duplicate" Icon="fas fa-copy" CssClass="btn" ButtonStyle="Default" />
                                <insite:Button runat="server" ID="TranslateSurveyLink" Text="Translate" Icon="fas fa-globe" CssClass="btn" ButtonStyle="Default" />

                                <insite:ButtonSpacer runat="server" />

                                <insite:Button runat="server" ID="ViewHistoryLink" Text="History" Icon="fas fa-history" CssClass="btn" ButtonStyle="Default" />
                                <insite:DownloadButton runat="server" ID="DownloadSurveyLink" CssClass="btn" />
                                <insite:DeleteButton runat="server" ID="VoidSurveyLink" CssClass="btn" />
                            </div>
                            <div class="col-lg-12 mt-2">
                                <insite:Button runat="server" ID="SurveyReportLink" Text="Reports" Icon="fas fa-chart-bar" CssClass="btn" ButtonStyle="Primary" />
                                <insite:Button runat="server" ID="ResponseSearchLink" Text="Submissions" Icon="fas fa-search" CssClass="btn" ButtonStyle="Default" />
                                <insite:Button runat="server" ID="DeleteResponseLink" Text="Reset" Icon="fas fa-power-off" CssClass="btn" ButtonStyle="Default" />
                                <span class="form-text ms-2"><asp:Literal runat="server" ID="ResponseCount" /></span>
                            </div>
                        </div>                                                       

                        <insite:Nav runat="server">               
                            <insite:NavItem runat="server" ID="DetailsTab" Title="Details">
                                <div class="row settings">
                                    <div class="col-lg-6">                            
                                        <h3>Identification</h3>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink runat="server" id="ChangeInternalName" style="padding:8px" ToolTip="Change Form Name" Name="pencil" />
                                            </div>
                                            <asp:label runat="server" ID="InternalNameLabel" AssociatedControlID="InternalName" Text="Internal Name" CssClass="form-label" />
				                            <div>
                                                <asp:Literal runat="server" ID="InternalName" />
                                            </div>
                                            <div class="form-text">
                                                The internal name is used as a reference for filing purposes. 
                                                It is required, but it is not visible to the form respondent.
                                            </div>
                                        </div>

				                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="ChangeSurveyTitle" style="padding:8px" ToolTip="Change Form Title" />
                                            </div>
                                            <asp:label runat="server" ID="SurveyTitleLabel" AssociatedControlID="SurveyTitle" Text="External Title" CssClass="form-label" />
				                            <div>
                                                <asp:Literal runat="server" ID="SurveyTitle" />
                                            </div>
                                            <div class="form-text">
                                                This is the form title displayed to the form respondent.
                                            </div>
				                        </div>
                                
				                        <div class="form-group mb-3">
                                            <asp:label runat="server" ID="TestSurveyLinkLabel" AssociatedControlID="TestSurveyLink" Text="Test Link" CssClass="form-label" />
				                            <div>
				                                <asp:HyperLink runat="server" ID="TestSurveyLink" Target="_blank" CssClass="form-text" />
                                            </div>
                                            <div class="form-text text-danger">
                                                This URL is intended for use by <strong>administrators only</strong>, 
                                                to review and test the form. 
                                                It should <strong>NOT</strong> be shared with actual form respondents.
                                            </div>
				                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink runat="server" ID="LockLink" ToolTip="Lock this form" Name="lock" />
                                                <insite:IconLink runat="server" ID="UnlockLink" ToolTip="Unlock this form" Name="lock-open" />
                                            </div>
                                            <asp:label runat="server" ID="SurveyLockStatusLabel" AssociatedControlID="SurveyLockStatus" Text="Lock Status" CssClass="form-label" />
                                            <div>
                                                <asp:Literal runat="server" ID="SurveyLockStatus" />
                                            </div>
                                            <div class="form-text">Changes to locked form are not permitted.</div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="ChangeHook" style="padding:8px" ToolTip="Change Form Hook" />
                                            </div>
                                            <asp:label runat="server" ID="SurveyHookLabel" AssociatedControlID="SurveyHook" Text="Hook / Integration Code" CssClass="form-label" />
				                            <div>
                                                <asp:Literal runat="server" ID="SurveyHook" />
                                            </div>
                                            <div class="form-text">
                                                Unique code for integration with internal toolkits and external systems.
                                            </div>
				                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="ChangeDepartments" style="padding:8px" ToolTip="Change Departments" />
                                            </div>
                                            <asp:label runat="server" ID="DepartmentNamesLabel" AssociatedControlID="DepartmentNames" Text="Departments" CssClass="form-label" />
				                            <div>
                                                <asp:Literal runat="server" ID="DepartmentNames" Text="None" />
                                            </div>
                                            <div class="form-text">
                                                The departments to which this form is assigned.
                                            </div>
				                        </div>

                                    </div>

                                    <div class="col-lg-6">

                                        <h3>Configuration</h3>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="ChangeSurveyStatus" style="padding:8px" ToolTip="Change Status" />
                                            </div>
                                            <asp:label runat="server" ID="CurrentStatusLabel" AssociatedControlID="CurrentStatus" Text="Publication Status" CssClass="form-label" />
                                            <div>
                                                <asp:Literal runat="server" ID="CurrentStatus" />
                                            </div>
                                            <div class="form-text">
                                                A Drafted form is open to submissions from administrators only. An Opened form is open to all respondents.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <asp:label runat="server" ID="OpenedLabel" AssociatedControlID="Opened" Text="Opened Date" CssClass="form-label" />
				                            <div>
                                                <asp:Literal runat="server" ID="Opened" />
                                            </div>
                                            <div class="form-text">
                                                The date and time when submissions to the form are opened to all respondents.
                                            </div>
				                        </div>

                                        <div class="form-group mb-3">
                                            <asp:label runat="server" ID="ClosedLabel" AssociatedControlID="Closed" Text="Close Date" CssClass="form-label" />
				                            <div>
                                                <asp:Literal runat="server" ID="Closed" />
                                            </div>
                                            <div class="form-text">
                                                New submissions are not permitted after the form is closed.
                                            </div>
				                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="ChangeConfiguration4" style="padding:8px" ToolTip="Change Configuration" />
                                            </div>
                                            <asp:label runat="server" ID="DurationMinutesLabel" AssociatedControlID="DurationMinutes" Text="Expected Duration (in Minutes)" CssClass="form-label" />
				                            <div>
                                                <asp:Literal runat="server" ID="DurationMinutes" />
                                            </div>
                                            <div class="form-text">
                                                The number of minutes that a user is expected to need in order to complete this form.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="ChangeConfiguration1" style="padding:8px" ToolTip="Change Configuration" />
                                            </div>
                                            <asp:label runat="server" ID="UserFeedbackLabel" AssociatedControlID="UserFeedback" Text="Feedback for Respondents" CssClass="form-label" />
				                            <div>
                                                <asp:Literal runat="server" ID="UserFeedback" />
                                            </div>
                                            <div class="form-text">
                                                Allow respondents to review feedback from the form administrator about answers they submitted to questions on the form.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="ChangeConfiguration2" style="padding:8px" ToolTip="Change Configuration" />
                                            </div>
                                            <asp:label runat="server" ID="IsLimitResponsesLabel" AssociatedControlID="IsLimitResponses" Text="Limit Submissions per Respondent" CssClass="form-label" />
				                            <div>
                                                <asp:Literal runat="server" ID="IsLimitResponses" />
                                            </div>
                                            <div class="form-text">
                                                If you want to ensure each respondent answers the form no more than once then select Limited.
                                            </div>
				                        </div>

                                        <div runat="server" id="AllowAnonymousResponsesField" class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="ChangeConfiguration3" style="padding:8px" ToolTip="Change Configuration" />
                                            </div>
                                            <asp:label runat="server" ID="AllowAnonymousResponsesLabel" AssociatedControlID="AllowAnonymousResponses" Text="Allow Anonymous Submissions" CssClass="form-label" />
				                            <div>
                                                <asp:Literal runat="server" ID="AllowAnonymousResponses" />
                                            </div>
                                            <div class="form-text">
                                                Allow users to answer the form without identifying themselves and without signing in.
                                            </div>
				                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="ChangeConfiguration5" style="padding:8px" ToolTip="Change Configuration" />
                                            </div>
                                            <asp:label runat="server" ID="EnableUserConfidentialityLabel" AssociatedControlID="EnableUserConfidentiality" Text="Confidentiality for Respondents" CssClass="form-label" />
				                            <div>
                                                <asp:Literal runat="server" ID="EnableUserConfidentiality" />
                                            </div>
                                            <div class="form-text">
                                                Enable this setting if you do not want to disclose confidential/personal information 
                                                about form respondents to form administrators.
                                            </div>
				                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="ChangeConfiguration6" style="padding:8px" ToolTip="Change Configuration" />
                                            </div>
                                            <asp:label runat="server" ID="DisplaySummaryChartLabel" AssociatedControlID="DisplaySummaryChart" Text="Display Summary Chart" CssClass="form-label" />
				                            <div>
                                                <asp:Literal runat="server" ID="DisplaySummaryChart" />
                                            </div>
				                        </div>
                                    </div>
                                </div>
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="TranslationsTab" Title="Translations">

                                <div class="row settings">

                                    <div class="col-md-6">

                                        <div class="form-group mb-3">
                                            <asp:label runat="server" ID="SurveyLanguageLabel" AssociatedControlID="SurveyLanguage" Text="Language" CssClass="form-label" />
				                            <div>
                                                <asp:Literal runat="server" ID="SurveyLanguage" />
                                            </div>
				                        </div>

                                        <div runat="server" id="SurveyTranslationLanguagesField" class="form-group mb-3">
                                            <asp:label runat="server" ID="SurveyTranslationLanguagesLabel" AssociatedControlID="SurveyTranslationLanguages" Text="Translate To" CssClass="form-label" />
				                            <div>
                                                <asp:Literal runat="server" ID="SurveyTranslationLanguages" />
                                            </div>
				                        </div>

                                    </div>

                                    <div class="col-md-6"></div>

                                </div>

                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="ConditionsTab" Title="Conditions">
                                <div class="row">
                                    <div class="col-lg-12 conditions-list">

                                        <div class="button-group">
                                            <insite:Button runat="server" ID="AddConditions" Text="Add Conditions" Icon="fas fa-plus-circle" ButtonStyle="Default" />
                                        </div>

                                        <p runat="server" id="ConditionsNoItemsMessage" visible="false">
                                            There are no conditions in this form.
                                        </p>

                                        <asp:Repeater runat="server" ID="ConditionsRepeater" Visible="false">
                                            <ItemTemplate>
                                                <dl class="dl-horizontal">
                                                    <dt>
                                                        Question:
                                                    </dt>
                                                    <dd>
                                                        <%# Eval("MaskingQuestionCode") %>.
                                                        <%# GetText(Eval("MaskingQuestionTitle"), "(Untitled)") %>
                                                    </dd>

                                                    <dt runat="server" visible='<%# Eval("ShowList") %>'>
                                                        List:
                                                    </dt>
                                                    <dd runat="server" visible='<%# Eval("ShowList") %>'>
                                                        <%# GetText(Eval("MaskingListTitle"), "(Untitled)") %>
                                                    </dd>

                                                    <dt>
                                                        Option:
                                                    </dt>
                                                    <dd>
                                                        <%# GetText(Eval("MaskingOptionTitle"), "(Untitled)") %>
                                                        <div class="float-end">
                                                            <insite:Button runat="server" visible='<%# CanEdit %>'
                                                                NavigateUrl ='<%# string.Format("/ui/admin/workflow/forms/conditions/change?form={0}&option={1}&returnpanel=form&returntab=Conditions", SurveyID, Eval("MaskingOptionIdentifier")) %>'
                                                                ToolTip="Edit condition"
                                                                style="padding: 8px;" ButtonStyle="Default" Text="Edit Condition" Icon="far fa-pencil" />
                                                        </div>
                                                    </dd>

                                                    <dt>
                                                        Hides:
                                                    </dt>
                                                    <dd>
                                                        <ul>
                                                            <asp:Repeater runat="server" ID="MaskedQuestionsRepeater">
                                                                <ItemTemplate>
                                                                    <li style="margin-bottom:8px;padding-bottom:8px;">
                                                                        <%# Eval("MaskedQuestionCode") %>.
                                                                        <%# GetText(Eval("MaskedQuestionTitle"), "(Untitled)") %>
                                                                        <insite:IconLink runat="server" visible='<%# CanEdit %>'
                                                                            NavigateUrl='<%# string.Format("/ui/admin/workflow/forms/conditions/delete?option={0}&question={1}&returnpanel=form&returntab=Conditions", Eval("MaskingOptionIdentifier"), Eval("MaskedQuestionIdentifier")) %>'
                                                                            ToolTip="Delete option"
                                                                            style="padding: 8px; "
                                                                            Name="trash-alt" />
                                                                    </li>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </ul>
                                                    </dd>
                                                </dl>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                    </div>
                                </div>
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="BranchesTab" Title="Branches">
                                <div class="row">
                                    <div class="col-md-12 branches-list">
                                        <p runat="server" id="BranchesNoItemsMessage" visible="false">
                                            There are no branches in this form.
                                        </p>

                                        <asp:Repeater runat="server" ID="BranchesRepeater" Visible="false">
                                            <ItemTemplate>
                                                <div>
                                                    <h3>
                                                        Question <%# Eval("QuestionCode") %>
                                                        <asp:Literal runat="server" Visible='<%# !(bool)Eval("SingleList") %>' Text='<%# Eval("OptionListSequence", ", Option List {0}") %>' />
                                                    </h3>
                                                    <h4>
                                                        <%# Eval("QuestionTitle") %>
                                                        <div class="float-end">
                                                            <insite:Button runat="server"
                                                                visible='<%# (bool)Eval("SingleList") && CanEdit %>'
                                                                NavigateUrl='<%# string.Format("/ui/admin/workflow/forms/change-branch?list={0}&returnpanel=form&returntab=Branches", Eval("OptionListIdentifier")) %>'
                                                                ToolTip="Edit branch"
                                                                style="padding: 8px;" ButtonStyle="Default"
                                                                Text="Edit Branch" Icon="far fa-pencil" />
                                                        </div>
                                                    </h4>
                                                    <h5>
                                                        <%# (bool)Eval("SingleList") ? "" : Eval("OptionListTitle") ?? "Option List " + Eval("OptionListSequence") %>
                                                        <insite:IconLink runat="server"
                                                            visible='<%# !(bool)Eval("SingleList") && CanEdit %>'
                                                            NavigateUrl='<%# string.Format("/ui/admin/workflow/forms/change-branch?list={0}&returnpanel=form&returntab=Branches", Eval("OptionListIdentifier")) %>'
                                                            ToolTip="Change branch"
                                                            style="padding: 8px;"
                                                            Name="pencil" />
                                                    </h5>
                                        
                                                    <table class="table table-striped">
                                                        <thead>
                                                        <tr>
                                                            <th colspan="2">If a respondent selects...</th>
                                                            <th>... then branch to this question</th>
                                                        </tr>
                                                        </thead>
                                                        <asp:Repeater runat="server" ID="OptionsRepeater">
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td style="width:30px;"><b><%# Eval("Letter") %>.</b></td>
                                                                    <td><b><%# Eval("Title") %></b></td>
                                                                    <td>
                                                                        <%# Eval("BranchToQuestionCode") != null ? Eval("BranchToQuestionCode", "{0}.") : null %>
                                                                        <%# Eval("BranchToQuestionTitle") %>

                                                                        <div runat="server" class="form-text" visible='<%# (bool)Eval("IsNotFirstQuestion") && !(bool)Eval("IsNotFollowingPage") %>' style="color:#8a6d3b;">
                                                                            <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                                                                            Question <%# Eval("BranchToQuestionCode") %> is not the first question on page <%# Eval("BranchToPageNumber") %>
                                                                        </div>

                                                                        <div runat="server" class="form-text" visible='<%# Eval("IsNotFollowingPage") %>' style="color:#8a6d3b;">
                                                                            <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                                                                            Question <%# Eval("BranchToQuestionCode") %> is not on the following page
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </table>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="SummariesTab" Title="Summaries">

                                <div class="row settings">
                                    <div class="col-md-6">
                            
                                        <h3>Feedback Scales on Likert Tables</h3>

                                        <table class="table table-striped">
                                            <tr>
                                                <th>Category</th>
                                                <th>Question</th>
                                                <th>Likert Table</th>
                                                <th>Feedback Scale</th>
                                            </tr>
                                        <asp:Repeater runat="server" ID="LikertScaleRepeater">
                                            <ItemTemplate>

                                                <tr>
                                                    <td><%# Eval("Category") %></td>
                                                    <td><%# Eval("Question") %></td>
                                                    <td><%# Eval("Rows") %></td>
                                                    <td><%# Eval("Size") %></td>
                                                </tr>
                                    
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        </table>

                                    </div>
                                </div>

                            </insite:NavItem>

                        </insite:Nav>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section id="QuestionsAnchor" class="pb-5 mb-md-2">
        <h2 class="h4 mb-1">Questions</h2>
        <div class="row">
            <div class="col-lg-12">
                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:QuestionsManager runat="server" ID="QuestionsManager" />
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section id="ContentAnchor" class="pb-5 mb-md-2">
        <h2 class="h4 mb-1">Content</h2>
        <div class="row">
            <div class="col-lg-12">        
                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <insite:Nav runat="server" ID="ContentTabs"></insite:Nav>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section id="MessagesAnchor" class="pb-5 mb-md-2">
        <h2 class="h4 mb-1">Messages</h2>
        <div class="row">
            <div class="col-lg-12">        
                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <insite:Nav runat="server">
                            <insite:NavItem runat="server" ID="InvitationTab" Title="Invitation">
                                <uc:MessageDetails runat="server" ID="InvitationMessageDetails" />
                            </insite:NavItem>
                            <insite:NavItem runat="server" ID="ResponseStartedAdministratorTab" Title="Submission Started (Administrator)">
                                <uc:MessageDetails runat="server" ID="ResponseStartedMessageDetails" />
                            </insite:NavItem>
                            <insite:NavItem runat="server" ID="ResponseCompletedAdministratorTab" Title="Submission Completed (Administrator)">
                                <uc:MessageDetails runat="server" ID="ResponseCompletedMessageDetails" />
                            </insite:NavItem>
                            <insite:NavItem runat="server" ID="ResponseCompletedRespondentTab" Title="Submission Completed (Respondent)">
                                <uc:MessageDetails runat="server" ID="ResponseCompletedRespondentMessageDetails" />
                            </insite:NavItem>
                        </insite:Nav>
                    </div>
                </div>
            </div>
        </div>
    </section> 

<insite:PageFooterContent runat="server" ID="FooterScript">
    <script type="text/javascript">

        (function () {
            var instance = window.outline = window.outline || {};

            // event handlers

            instance.onJumpToChanged = function () {
                var $this = $(this);
                var value = $this.selectpicker('val');

                window.location.href = `#${value}`;
            };
        })();

    </script>
</insite:PageFooterContent>

</asp:Content>
