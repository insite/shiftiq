<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BankCommentaryCriteria.ascx.cs" Inherits="InSite.Admin.Assessments.Reports.Controls.BankCommentaryCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="SelectorsUpdatePanel" ID="SelectorsLoadingPanel" />
                    <insite:UpdatePanel runat="server" ID="SelectorsUpdatePanel">
                        <ContentTemplate>
                            <div class="mb-2">
                                <insite:FindBank runat="server" ID="BankIdentifier" EmptyMessage="Exam Bank" />
                            </div>

                            <div class="mb-2">
                                <insite:FindBankForm runat="server" ID="FormIdentifier" EmptyMessage="Exam Form" />
                            </div>

                            <div class="mb-2">
                                <insite:FindEntity runat="server" ID="QuestionIdentifier" EmptyMessage="Exam Question" Enabled="false" />
                            </div>
                        </ContentTemplate>
                    </insite:UpdatePanel>                    

                    <div class="mb-2">
                        <insite:FlagComboBox runat="server" ID="QuestionFlag" EmptyMessage="Question Flag" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="CommentPostedSince" EmptyMessage="Posted &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="CommentPostedBefore" EmptyMessage="Posted &lt;" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="EventFormat" EmptyMessage="Format">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Text="Online" Value="Online" />
                                <insite:ComboBoxOption Text="Paper" Value="Paper" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <insite:FilterButton runat="server" ID="SearchButton" />
                    <insite:ClearButton runat="server" ID="ClearButton" />
                </div>                  
            
                <div class="col-6">
                    <div class="mb-2">
                        <insite:FlagComboBox runat="server" ID="CommentFlag" EmptyMessage="Comment Flag" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CommentText" EmptyMessage="Comment Text" />
                    </div>

                    <div class="mb-2">
                        <insite:CommentAuthorTypeComboBox runat="server" ID="AuthorRole" EmptyMessage="Comment Author Role" />
                    </div>

                    <div class="mb-2">
                        <insite:FeedbackCategoryMultiComboBox runat="server" ID="CommentCategory" EmptyMessage="Feedback Category" 
                            Multiple-ActionsBox="true" Settings-IncludeNoCategory="true" />
                    </div>

                    <div class="mb-2">
                        <insite:ExamAttemptTagMultiComboBox runat="server" ID="AttemptTag" Multiple-ActionsBox="true" EmptyMessage="Attempt Tag" />
                    </div>

                    <div class="mb-2">
                        <insite:FindEvent runat="server" ID="AttemptEventIdentifier" EmptyMessage="Exam Event" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="col-3">
        <div class="mb-2">
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>

        <div class="mb-2">
            <insite:ComboBox runat="server" ID="IsShowAuthor">
                <Items>
                    <insite:ComboBoxOption Text="Show Author Name" Value="True" />
                    <insite:ComboBoxOption Text="Hide Author Name" Value="False" />
                </Items>
            </insite:ComboBox>
        </div>
    </div>
    <div class="col-3">
        <h4>Saved Filters</h4>
        <uc:FilterManager runat="server" ID="FilterManager" />
    </div>
</div>



