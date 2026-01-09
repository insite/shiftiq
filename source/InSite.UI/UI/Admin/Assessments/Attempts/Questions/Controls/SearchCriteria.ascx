<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Attempts.Questions.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">

        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>

            <div class="row">

                <div class="col-6">
                    <asp:CustomValidator runat="server" ID="CriteriaValidator" Display="None" ValidationGroup="SearchCriteria" ErrorMessage="Please select an assessment form or a question or a candidate in your search criteria." />
                    <insite:RequiredValidator runat="server" Display="None" RenderMode="Dot" ControlToValidate="BankID" FieldName="Assessment Bank" ValidationGroup="SearchCriteria" />

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="LeftUpdatePanel" />
                    <insite:UpdatePanel runat="server" ID="LeftUpdatePanel">
                        <ContentTemplate>
                            <div class="mb-2">
                                <insite:FindBank runat="server" ID="BankID" EmptyMessage="Assessment Bank" />
                            </div>

                            <div runat="server" id="FormField" class="mb-2">
                                <insite:BankFormComboBox runat="server" ID="FormID" EmptyMessage="Assessment Form" IsShortList="true" />
                            </div>
                        </ContentTemplate>
                    </insite:UpdatePanel>

                    <div class="mb-2">
                        <insite:Button runat="server" ID="SearchButton" Text="Build" Icon="far fa-chart-bar" ValidationGroup="SearchCriteria" />
                        <insite:ClearButton runat="server" ID="ClearButton" CausesValidation="false" />
                    </div>
                </div>

                <div class="col-6">
                    <insite:UpdatePanel runat="server" ID="RightUpdatePanel">
                        <ContentTemplate>
                            <div runat="server" id="QuestionField" class="mb-2">
                                <insite:FindBankQuestion runat="server" ID="BankQuestionID" EmptyMessage="Bank Question" />
                            </div>

                            <div runat="server" id="CandidateField" class="mb-2">
                                <insite:FindUser runat="server" ID="CandidateID" EmptyMessage="Candidate" />
                            </div>
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </div>

            </div> 
        </div>

    </div>

    <div class="col-3">
        <div class="mb-2">
            <h4>Settings</h4>
            <div class="mb-2">
                <insite:MultiComboBox ID="ShowColumns" runat="server" />
            </div>
            <div class="mb-4">
                <insite:ComboBox ID="SortColumns" runat="server">
                    <Items>
                        <insite:ComboBoxOption Text="Sort by Exam Form" Value="Form.FormTitle,AttemptStarted" />
                        <insite:ComboBoxOption Text="Sort by Exam Candidate" Value="LearnerPerson.UserFullName,AttemptStarted" />
                        <insite:ComboBoxOption Text="Sort by Submission Start Time" Value="AttemptStarted" />
                        <insite:ComboBoxOption Text="Sort by Submission Score" Value="AttemptScore,AttemptStarted" />
                    </Items>
                </insite:ComboBox>
            </div>
        </div>
    </div>
    <div class="col-3">
        <div class="mb-2">
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>