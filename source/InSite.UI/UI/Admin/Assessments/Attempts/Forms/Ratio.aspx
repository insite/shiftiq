<%@ Page Language="C#" CodeBehind="Ratio.aspx.cs" Inherits="InSite.Admin.Assessments.Attempts.Forms.Ratio" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="QuestionCompetencyRatioRepeater" Src="../Controls/QuestionCompetencyRatioRepeater.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Exam Report" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="CriteriaTab" Title="Criteria" Icon="far fa-search" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <div class="row">
                        <div class="col-lg-3">
                            <div class="form-group mb-3">
                                <label class="form-label">Exam Bank Occupation</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaExamBankOccupation" /></div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Exam Bank Framework</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaExamBankFramework" /></div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Exam Bank</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaExamBank" /></div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Exam Form</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaExamForm" /></div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Exam Candidate</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaExamCandidate" /></div>
                            </div>
                        </div>
                        <div class="col-lg-3">
                            <div class="form-group mb-3">
                                <label class="form-label">Exam Event</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaExamEvent" /></div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Event Format</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaEventFormat" /></div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Attempt Tag</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaAttemptTag" /></div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Attempt Inclusion</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaIncludeAttemptMode" /></div>
                            </div>
                        </div>
                        <div class="col-lg-3">
                            <div class="form-group mb-3">
                                <label class="form-label">Started &ge;</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaAttemptStartedSince" /></div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Started &lt;</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaAttemptStartedBefore" /></div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Completed &ge;</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaAttemptCompletedSince" /></div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Completed &lt;</label>
                                <div><asp:Literal runat="server" ID="SearchCriteriaAttemptCompletedBefore" /></div>
                            </div>
                        </div>
                        <div class="col-lg-3">
                            <div class="form-group mb-3">
                                <label class="form-label">Group By</label>
                                <insite:MultiComboBox runat="server" ID="GroupBySelector" Multiple-ActionsBox="true">
                                    <Items>
                                        <insite:ComboBoxOption Text="Occupation" Value="Occupation" />
                                        <insite:ComboBoxOption Text="Framework" Value="Framework" />
                                        <insite:ComboBoxOption Text="Tag" Value="Tag" />
                                        <insite:ComboBoxOption Text="Area" Value="Gac" />
                                        <insite:ComboBoxOption Text="Form" Value="Form" />
                                        <insite:ComboBoxOption Text="Format" Value="Format" />
                                    </Items>
                                </insite:MultiComboBox>
                            </div>

                            <insite:SearchButton runat="server" ID="RefreshButton" Text="Apply" Icon="far fa-check" />
                        </div>
                    </div>
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="RatioTab" Title="Ratio" Icon="far fa-list-alt" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <uc:QuestionCompetencyRatioRepeater runat="server" ID="CompetencyRatio" />

                </div>
            </div>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>
