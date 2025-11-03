<%@ Page Language="C#" CodeBehind="Change.aspx.cs" Inherits="InSite.Admin.Assessments.Questions.Forms.Change" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Import Namespace="InSite.Common" %>

<%@ Register TagPrefix="uc" TagName="TextEditor" Src="../Controls/QuestionTextEditor.ascx" %>
<%@ Register TagPrefix="uc" TagName="MatchingDetails" Src="../Controls/QuestionMatchingDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="PrerequisiteRepeater" Src="../Controls/QuestionPrerequisiteRepeater.ascx" %>
<%@ Register TagPrefix="uc" TagName="RandomizationInput" Src="../Controls/RandomizationInput.ascx" %>
<%@ Register TagPrefix="uc" TagName="CommentRepeater" Src="../Controls/CommentRepeater.ascx" %>
<%@ Register TagPrefix="uc" TagName="OrganizationTags" Src="~/UI/Admin/Assets/Contents/Controls/OrganizationTags.ascx" %>
<%@ Register TagPrefix="uc" TagName="QuestionAttachmentRepeater" Src="../../Attachments/Controls/QuestionAttachmentRepeater.ascx" %>
<%@ Register TagPrefix="uc" TagName="LayoutInput" Src="../Controls/QuestionLayoutInput.ascx" %>
<%@ Register TagPrefix="uc" TagName="GlossaryTermGrid" Src="~/UI/Admin/Assets/Glossaries/Terms/Controls/TermGrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="RubricDetail" Src="../Controls/RubricDetail.ascx" %>
<%@ Register TagPrefix="uc" TagName="LikertDetails" Src="../Controls/QuestionLikertDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="HotspotDetails" Src="../Controls/QuestionHotspotDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="OrderingDetails" Src="../Controls/QuestionOrderingDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="GradeItemList" Src="../Controls/GradeItemList.ascx" %>
<%@ Register TagPrefix="uc" TagName="ExemplarEditor" Src="../Controls/QuestionExemplarEditor.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <insite:Alert runat="server" ID="EditorStatus" />
            <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />
        </ContentTemplate>
    </insite:UpdatePanel>

    <insite:Nav runat="server" ID="Accordion">

        <insite:NavItem runat="server" ID="QuestionTab" Title="Question" Icon="far fa-question" IconPosition="BeforeText">

            <div class="card border-0 shadow-lg mb-3">
                <div class="card-body">
                    <h3>Question</h3>

                    <div class="row">
                        <div class="col-lg-10">
                            <uc:TextEditor runat="server" ID="QuestionText" />
                        </div>

                        <div class="col-lg-2">

                            <div class="form-group mb-3">
                                <label class="form-label">Question Type</label>
                                <div>
                                    <asp:Literal runat="server" ID="QuestionType" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Publication Status</label>
                                <div>
                                    <asp:Literal runat="server" ID="PublicationStatusName" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Current Version</label>
                                <div>
                                    <div class="float-end">
                                        <span runat="server" id="AssetVersionIncremented" visible="false" class="badge bg-danger">New Version!</span>
                                    </div>
                                    v<asp:Literal runat="server" ID="AssetVersion" />
                                    <insite:IconButton runat="server" ID="AssetVersionIncrement" Name="arrow-alt-up" />
                                </div>
                                <div class="form-text">
                                    <asp:Repeater runat="server" ID="AssetVersionRepeater">
                                        <ItemTemplate>
                                            <a href='<%# Eval("NavigateUrl") %>'>v<%# Eval("AssetVersion") %></a>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

            </div>

            <div runat="server" id="ExemplarCard" visible="false" class="card border-0 shadow-lg h-100 mb-3">
                <div class="card-body">
                    <h3>Exemplar</h3>
                    <uc:ExemplarEditor runat="server" ID="ExemplarText" />
                </div>
            </div>

            <div runat="server" id="AttachmentCard" class="card border-0 shadow-lg mb-3">
                <div class="card-body">
                    <h3>Attachments</h3>
                    <uc:QuestionAttachmentRepeater runat="server" ID="AttachmentRepeater" />
                </div>
            </div>

            <asp:MultiView runat="server" ID="QuestionItemsMultiView">
                <asp:View runat="server" ID="QuestionOptionsView">
                    <div class="card border-0 shadow-lg mb-3">
                        <div class="card-body">
                            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="OptionListUpdatePanel" />
                            <insite:UpdatePanel runat="server" ID="OptionListUpdatePanel" CssClass="mb-3">
                                <ContentTemplate>
                                    <insite:DynamicControl runat="server" ID="Options" />
                                </ContentTemplate>
                            </insite:UpdatePanel>
                        </div>
                    </div>
                </asp:View>
                <asp:View runat="server" ID="QuestionMatchingView">
                    <div class="card border-0 shadow-lg mb-3">
                        <div class="card-body">
                            <uc:MatchingDetails runat="server" ID="MatchingDetails" ValidationGroup="Assessment" />
                        </div>
                    </div>
                </asp:View>
                <asp:View runat="server" ID="QuestionRubricView">
                    <div class="card border-0 shadow-lg mb-3">
                        <div class="card-body">
                            <uc:RubricDetail runat="server" ID="RubricDetail" />
                        </div>
                    </div>
                </asp:View>
                <asp:View runat="server" ID="QuestionLikertView">
                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="LikertUpdatePanel" />
                    <insite:UpdatePanel runat="server" ID="LikertUpdatePanel" Cssclass="form-group mb-3">
                        <ContentTemplate>
                            <uc:LikertDetails runat="server" ID="LikertDetails" ValidationGroup="Assessment" />
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </asp:View>
                <asp:View runat="server" ID="QuestionHotspotView">
                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="HotspotUpdatePanel" />
                    <insite:UpdatePanel runat="server" ID="HotspotUpdatePanel" Cssclass="form-group mb-3">
                        <ContentTemplate>
                            <uc:HotspotDetails runat="server" ID="HotspotDetails" ValidationGroup="Assessment" />
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </asp:View>
                <asp:View runat="server" ID="QuestionOrderingView">
                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="OrderingUpdatePanel" />
                    <insite:UpdatePanel runat="server" ID="OrderingUpdatePanel" Cssclass="form-group mb-3">
                        <ContentTemplate>
                            <uc:OrderingDetails runat="server" ID="OrderingDetails" ValidationGroup="Assessment" />
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </asp:View>
            </asp:MultiView>

        </insite:NavItem>

        <insite:NavItem runat="server" ID="SettingsTab" Title="Settings" Icon="far fa-sliders-h" IconPosition="BeforeText">
            <div class="row">
                <div class="col-lg-4">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">
                            <h3>Classification</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">Code</label>
                                <div>
                                    <insite:TextBox ID="QuestionCode" runat="server" MaxLength="40" />
                                </div>
                                <div class="form-text">This is a unique code assigned for internal filing and/or reference.</div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Tag</label>
                                <div>
                                    <insite:TextBox ID="QuestionTag" runat="server" MaxLength="100" />
                                </div>
                                <div class="form-text">Assign any number of tags to this question for searching and filtering.</div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Taxonomy</label>
                                <div>
                                    <insite:TaxonomyComboBox runat="server" ID="TaxonomySelector" />
                                </div>
                                <div class="form-text">Refer to the <a href="/UI/Admin/assessments/questions/content/tradestaxonomy.pdf" target="_blank">Trades Taxonomy</a> for details.</div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Difficulty</label>
                                <div>
                                    <insite:DifficultyComboBox runat="server" ID="DifficultySelector" />
                                </div>
                                <div class="form-text">The estimated difficulty for this question.</div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Like Item Group</label>
                                <div>
                                    <insite:TextBox runat="server" ID="LikeItemGroup" MaxLength="64" />
                                </div>
                                <div class="form-text">Use the Like Item Group (LIG) to assign this question to a collection of similar questions, where mutual exclusivity in an exam is needed.</div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Reference</label>
                                <div>
                                    <insite:TextBox runat="server" ID="SourceDescriptor" TextMode="MultiLine" MaxLength="500" Rows="3" />
                                </div>
                                <div class="form-text">The source material for this question (e.g. industry regulation or code book reference).</div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-8">
                    <div class="row">
                        <div class="col-lg-6">

                            <div class="card border-0 shadow-lg mb-3">
                                <div class="card-body">
                                    <h3>Administration</h3>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Condition</label>
                                        <div>
                                            <insite:QuestionConditionComboBox runat="server" ID="QuestionCondition" CssClass="w-50" />
                                            <insite:RequiredValidator runat="server" ControlToValidate="QuestionCondition" ValidationGroup="Assessment" Enabled="false" />
                                        </div>
                                        <div class="form-text">The current condition of this item in your question bank.</div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Current Flag</label>
                                        <div>
                                            <insite:ColorComboBox runat="server" ID="QuestionFlag" CssClass="w-50" />
                                            <asp:Literal runat="server" ID="QuestionFlagIcon" />
                                        </div>
                                        <div class="form-text">Assign a flags to this question for special attention.</div>
                                    </div>
                                </div>
                            </div>

                            <div runat="server" id="ComposedVoiceSection" visible="false" class="card border-0 shadow-lg mb-3">
                                <div class="card-body">
                                    <h3>Composed Voice</h3>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Time Limit (seconds)</label>
                                        <div>
                                            <insite:NumericBox runat="server" ID="ComposedVoiceTimeLimit" NumericMode="Integer" CssClass="w-50" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Attempt Limit</label>
                                        <div>
                                            <insite:NumericBox runat="server" ID="ComposedVoiceAttemptLimit" NumericMode="Integer" CssClass="w-50" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div runat="server" id="HotspotCustomSection" visible="false" class="card border-0 shadow-lg mb-3">
                                <div class="card-body">
                                    <h3>Hotspot</h3>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Pin Limit</label>
                                        <div>
                                            <insite:NumericBox runat="server" ID="HotspotPinLimit" NumericMode="Integer" MinValue="1" CssClass="w-50" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Shapes</label>
                                        <div>
                                            <insite:BooleanComboBox runat="server" ID="HotspotShowShapes" TrueText="Show" FalseText="Hide" AllowBlank="false" CssClass="w-50" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="card border-0 shadow-lg">
                                <div class="card-body">

                                    <h3>Calculation and Scoring</h3>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Maximum Possible Points</label>
                                        <div>
                                            <insite:NumericBox runat="server" ID="MaximumPoints" DecimalPlaces="2" MinValue="0.00" MaxValue="999.99" ReadOnly="true" CssClass="w-50" />
                                        </div>
                                        <div class="form-text">The maximum number of possible points awarded for an answer to this question.</div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Cut Score (%)</label>
                                        <div>
                                            <insite:NumericBox runat="server" ID="CutScore" ValueAsDecimal="1" DecimalPlaces="2" MinValue="0.00" MaxValue="1.00" CssClass="w-50" />
                                        </div>
                                        <div class="form-text">The average of the cut scores for correct answers. Incorrect answers have a cut score of zero.</div>
                                    </div>

                                    <div runat="server" id="CalculationMethodField" class="form-group mb-3" visible="false">
                                        <label class="form-label">
                                            Calculation Method
                                            <insite:RequiredValidator runat="server" ControlToValidate="CalculationMethod" ValidationGroup="Assessment" />
                                        </label>
                                        <div>
                                            <insite:ExamQuestionCalculationMethodComboBox runat="server" ID="CalculationMethod" Width="250px" />
                                        </div>
                                    </div>

                                </div>
                            </div>

                        </div>

                        <div class="col-lg-6">
                            <div class="card border-0 shadow-lg mb-3">
                                <div class="card-body">
                                    <h3>Question Set</h3>

                                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="SetUpdatePanel" />
                                    <insite:UpdatePanel runat="server" ID="SetUpdatePanel">
                                        <ContentTemplate>

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Question Set
                                                    <insite:RequiredValidator runat="server" ControlToValidate="SetID" ValidationGroup="Assessment" />
                                                </label>
                                                <div>
                                                    <insite:ComboBox runat="server" ID="SetID" AllowBlank="false" />
                                                </div>
                                                <div class="form-text">The Set to which the question is assigned.</div>
                                            </div>

                                            <div runat="server" id="CompetencyPanel" class="form-group mb-3">
                                                <label class="form-label">Competency</label>
                                                <div class="ms-1 mt-1">
                                                    <insite:Container runat="server" ID="LooseCompetencyContainer">
                                                        <insite:FindStandard runat="server" ID="LooseCompetencyIdentifier" />

                                                        <asp:Repeater runat="server" ID="LooseSubCompetencyRepeater" Visible="false">
                                                            <HeaderTemplate>
                                                                <table class="ms-4 mt-3"><tbody>
                                                            </HeaderTemplate>
                                                            <FooterTemplate>
                                                                </tbody></table>
                                                            </FooterTemplate>
                                                            <ItemTemplate>
                                                                <tr><td>
                                                                    <insite:CheckBox runat="server" ID="IsSelected"
                                                                        Text='<%# Eval("ItemText") %>'
                                                                        Value='<%# Eval("StandardIdentifier") %>' />
                                                                </td></tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </insite:Container>

                                                    <asp:Repeater runat="server" ID="StrictCompetencyRepeater" Visible="false">
                                                        <HeaderTemplate>
                                                            <table id='<%= StrictCompetencyRepeater.ClientID %>'><tbody>
                                                        </HeaderTemplate>
                                                        <FooterTemplate>
                                                            </tbody></table>
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                            <tr><td>
                                                                <insite:RadioButton runat="server" ID="IsSelected"
                                                                    GroupName='<%# StrictCompetencyRepeater.ClientID %>'
                                                                    Text='<%# Eval("ItemText") %>'
                                                                    Value='<%# Eval("StandardIdentifier") %>' />

                                                                <asp:Repeater runat="server" ID="SubCompetencyRepeater" Visible="false">
                                                                    <HeaderTemplate>
                                                                        <table class="ms-4"><tbody>
                                                                    </HeaderTemplate>
                                                                    <FooterTemplate>
                                                                        </tbody></table>
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <tr><td>
                                                                            <insite:CheckBox runat="server" ID="IsSelected"
                                                                                Text='<%# Eval("ItemText") %>'
                                                                                Value='<%# Eval("StandardIdentifier") %>' />
                                                                        </td></tr>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </td></tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>

                                                </div>
                                                <div class="form-text">The competency evaluated by this question.</div>
                                            </div>

                                            <asp:Panel runat="server" ID="NoCompetencyPanel" CssClass="alert alert-info" Style="margin-bottom: 0;" Visible="false">
                                                <i class="far fa-info-circle me-1"></i><strong>Competency Evaluation</strong>
                                                The selected question set is not aligned to a competency framework.
                                            </asp:Panel>

                                        </ContentTemplate>
                                    </insite:UpdatePanel>
                                </div>
                            </div>

                            <div class="card border-0 shadow-lg">
                                <div class="card-body">
                                    <h3>Tags</h3>
                                    <p runat="server" id="NoOrganizationTags">There are no organization tags to assign.</p>
                                    <uc:OrganizationTags runat="server" ID="QuestionTags" />
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <insite:Container runat="server" ID="RandomizationContainer">
                        <div class="card border-0 shadow-lg mt-3">
                            <div class="card-body">
                                <h3>Randomization and Display</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">List or Shuffle</label>
                                    <div>
                                        <uc:RandomizationInput runat="server" ID="RandomizationInput" />
                                    </div>
                                    <div class="form-text">List options in the sequence they are input or in random order.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Option Layout
                                    </label>
                                    <div>
                                        <uc:LayoutInput runat="server" ID="LayoutInput" />
                                    </div>
                                    <div class="form-text">List options in a single column or in a multi-column table.</div>
                                </div>
                            </div>
                        </div>
                    </insite:Container>

                </div>

            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="RationaleTab" Title="Rationale" Icon="far fa-books" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mb-3">
                <div class="card-body">
                    <div class="d-md-flex">
                        <div class="text-nowrap pe-md-4">

                            <insite:Nav runat="server" ID="RationaleNavigation" ItemType="Pills" ItemAlignment="Vertical" ContentRendererID="RationaleRenderer">

                                <insite:NavItem runat="server" Title="On All Answers">
                                    <div class="form-text mb-2">Feedback to a candidate regardless of the answer submitted.</div>
                                    <uc:TextEditor runat="server" ID="Rationale" />
                                </insite:NavItem>

                                <insite:NavItem runat="server" Title="On Correct Answers">
                                    <div class="form-text mb-2">Feedback to a candidate who answers the question <strong class='text-success'>correctly</strong>.</div>
                                    <uc:TextEditor runat="server" ID="RationaleOnCorrectAnswer" />
                                </insite:NavItem>

                                <insite:NavItem runat="server" Title="On Incorrect Answers">
                                    <div class="form-text mb-2">Feedback to a candidate who answers the question <strong class='text-danger'>incorrectly</strong>.</div>
                                    <uc:TextEditor runat="server" ID="RationaleOnIncorrectAnswer" />
                                </insite:NavItem>

                                <insite:NavItem runat="server" Title="Description">
                                    <div class="form-text mb-2">What is the purpose or key feature of this question?</div>
                                    <uc:TextEditor runat="server" ID="ContentDescription" />
                                </insite:NavItem>

                            </insite:Nav>

                        </div>
                        <div class="content-inputs w-100 mt-4 mt-md-0">
                            <insite:NavContent runat="server" ID="RationaleRenderer" />
                        </div>
                    </div>
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="PrerequisitesTab" Title="Prerequisites" Icon="far fa-bolt" IconPosition="BeforeText">
            <uc:PrerequisiteRepeater runat="server" ID="Prerequisites" ValidationGroup="Assessment" />
        </insite:NavItem>

        <insite:NavItem runat="server" ID="CommentsTab" Title="Comments" Icon="far fa-comments" IconPosition="BeforeText">
            <div class="row">
                <div class="col-md-12">

                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">
                            <h3>Posted Comments</h3>

                            <uc:CommentRepeater runat="server" ID="CommentRepeater" AllowHide="false" />

                        </div>
                    </div>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ConfirmRegradeTab" Title="Regrade Attempts" Icon="far fa-clipboard-list" IconPosition="BeforeText" Visible="false">
            <div class="alert alert-warning" role="alert">
                Existing attempts contain this question.  Select the regrade option for these attempts:
            </div>

            <asp:RadioButtonList runat="server" ID="RegradeOptionSelector">
                <asp:ListItem Value="AwardPointsForCorrectedAndPrevious" Text="Award points for corrected and previously correct answers (no scores reduced)" Selected="true" />
                <asp:ListItem Value="AwardPointsForCorrectedOnly" Text="Only award points for correct (some users may have their scores reduced)" />
                <asp:ListItem Value="FullCreditForEveryone" Text="Give everyone full credit for this question" />
                <asp:ListItem Value="None" Text="Update question without regrading" />
            </asp:RadioButtonList>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="GlossaryTermTab" Title="Glossary Terms" Icon="far fa-scroll" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg">
                <div class="card-body">
                    <uc:GlossaryTermGrid runat="server" ID="GlossaryTermGrid" />
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="GradebookTab" Title="Gradebook" Icon="far fa-spell-check" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg">
                <div class="card-body">
                    <uc:GradeItemList runat="server" ID="GradeItems" />
                </div>
            </div>
        </insite:NavItem>

    </insite:Nav>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" CausesValidation="true" GroupName="QuestionCommands" 
            OnClientClick="if (!questionChange.allowSave()) return false;" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

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

                $('a.scroll-send').on('click', function () {
                    var scroll = $(window).scrollTop();

                    if (scroll > 0)
                        scroll = inSite.common.base64.fromInt(Math.floor(scroll));
                    else
                        scroll = null;

                    var url = $(this).attr('href');

                    $(this).attr('href', inSite.common.updateQueryString('scroll', scroll, url));
                });
            })();

            (function () {
                let $subComnpetencyInputs;
                let isComnpetencyLocked = false;
                let isSubComnpetencyLocked = false;

                Sys.Application.add_load(function () {
                    const $table = $('table#<%= StrictCompetencyRepeater.ClientID %>');
                    if ($table.data('inited') === true)
                        return;

                    $table.find('input[type="radio"]').on('change', onCompetencyChange);

                    $subComnpetencyInputs = $table.find('input[type="checkbox"]').on('change', onSubCompetencyChange);

                    $table.data('inited', true);
                });

                function changeCompetency(fn) {
                    isComnpetencyLocked = true;

                    fn();

                    isComnpetencyLocked = false;
                }

                function changeSubCompetency(fn) {
                    isSubComnpetencyLocked = true;

                    fn();

                    isSubComnpetencyLocked = false;
                }

                function onCompetencyChange() {
                    if (isComnpetencyLocked)
                        return;

                    changeSubCompetency(() => $subComnpetencyInputs.filter(':checked').prop('checked', false));
                }

                function onSubCompetencyChange() {
                    if (isSubComnpetencyLocked)
                        return;

                    var $check = $(this);
                    var $radio = $check.closest('table').closest('td').find('input[type="radio"]');
                    if ($radio.prop('checked') === true)
                        return;

                    changeSubCompetency(() => $subComnpetencyInputs.filter(function () { return !$check.is(this); }).prop('checked', false));
                    changeCompetency(() => $radio.prop('checked', true));
                }
            })();

            (function () {
                if (window.questionChange)
                    return;

                const instance = window.questionChange = {};
                
                instance.allowSave = function () {
                    const rubricDetail = window.<%= RubricDetail.ClientID %>;
                    if (!rubricDetail || !rubricDetail.allowSave)
                        return true;

                    return rubricDetail.allowSave();
                };
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>
