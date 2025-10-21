<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BankSection.ascx.cs" Inherits="InSite.Admin.Assessments.Outlines.Controls.BankSection" %>

<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<%@ Register TagPrefix="uc" TagName="QuestionStatisticsPanel" Src="../../Questions/Controls/QuestionStatisticsPanel.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProblemRepeater" Src="./ProblemRepeater.ascx" %>
<%@ Register TagPrefix="uc" TagName="BankTranslation" Src="./BankTranslation.ascx" %>

<insite:Nav runat="server">

    <insite:NavItem runat="server" Title="Details">

        <div class="row">
            <div class="col-lg-6">

                <h3>Identification</h3>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Asset Number
                        <insite:IconLink runat="server" ID="DeleteLink" ToolTip="Delete assessment bank" Name="trash-alt" CssClass="ms-1" />
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="AssetNumber" />
                    </div>
                    <div class="form-text">
                        The unique inventory asset number assigned to the bank.
                    </div>
                </div>

                <div runat="server" id="BankStandardField" class="form-group mb-3">
                    <label class="form-label">
                        Standard
                        <insite:IconLink Name="pencil" runat="server" id="ChangeBankStandard" ToolTip="Change Standard" CssClass="ms-1" />
                    </label>
                    <div>
                        <div class="float-end">
                            <asp:Literal runat="server" ID="BankStandardCalculationMethod" />
                        </div>
                        <asp:Literal runat="server" ID="BankStandard" />
                    </div>
                    <div class="form-text">
                        The formal standard evaluated by questions in the bank.
                    </div>
                </div>

                <div runat="server" id="BankNameField" class="form-group mb-3">
                    <label class="form-label">
                        Bank Name
                        <insite:IconLink Name="pencil" runat="server" id="RenameBankLink2" ToolTip="Rename Bank" CssClass="ms-1" />
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="BankName" />
                    </div>
                    <div class="form-text">
                        The name that uniquely identifies the bank for internal filing purposes.
                    </div>
                </div>

                <div runat="server" id="BankLevelField" class="form-group mb-3">
                    <label class="form-label">
                        Level
                        <insite:IconLink Name="pencil" runat="server" id="ChangeBankLevel" ToolTip="Change Bank Level" CssClass="ms-1" />
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="BankLevel" />
                    </div>
                    <div class="form-text">
                        The type and number for a discrete skill level.
                    </div>
                </div>

                <div runat="server" id="BankEditionField" class="form-group mb-3">
                    <label class="form-label">
                        Edition
                        <insite:IconLink Name="pencil" runat="server" id="ChangeBankEdition" ToolTip="Change Edition" CssClass="ms-1" />
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="BankEdition" />
                    </div>
                    <div class="form-text">
                        The edition of this bank (e.g. Year and Month).
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Lock Status on Published Questions
                        <insite:IconLink Name="lock-open" runat="server" id="UnlockQuestionBank" ToolTip="Unlock this question bank" CssClass="ms-1" />
                        <insite:IconLink Name="lock" runat="server" id="LockQuestionBank" ToolTip="Lock this question bank" CssClass="ms-1" />
                    </label>
                    <div>
                        <div runat="server" id="LockedLabel"><i class="text-danger fas fa-lock-open me-2"></i>Locked</div>
                        <div runat="server" id="UnlockedLabel"><i class="text-success fas fa-lock-open me-2"></i>Unlocked</div>
                    </div>
                    <div class="form-text">
                        Changes to questions contained in a locked bank are not permitted.
                    </div>
                </div>

                <insite:UpdatePanel runat="server">
                    <ContentTemplate>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Bank Status
                                <insite:IconButton runat="server" ID="DisableBankButton" Name="toggle-on" />
                                <insite:IconButton runat="server" ID="EnableBankButton" Name="toggle-off" />
                            </label>
                            <div><asp:Literal runat="server" ID="BankStatus" /></div>
                            <div runat="server" ID="BankStatusHelp" class="form-text">
                
                            </div>
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>

            </div>
            <div class="col-lg-6">

                <h3>Content</h3>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Bank Title
                        <insite:IconLink Name="pencil" runat="server" id="EditBankTitleLink" ToolTip="Revise Bank Title" CssClass="ms-1" />
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="BankTitle" />
                    </div>
                    <div class="form-text">
                        The descriptive title for the bank.
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Summary
                        <insite:IconLink Name="pencil" runat="server" id="EditBankSummaryLink" ToolTip="Revise Summary" CssClass="ms-1" />
                    </label>
                    <div>
                        <span style="white-space:pre-wrap;"><asp:Literal runat="server" ID="Summary" /></span>
                    </div>
                    <div class="form-text">
                        The purpose or executive summary for the bank.
                    </div>
                </div>

                <h3>Measurements</h3>

                <table class="table table-striped table-bordered table-metrics" style="width: 300px;">
                    <tbody>
                        <tr>
                            <td class="text-nowrap">
                                Bank Size
                            </td>
                            <td class="text-nowrap">
                                ~ <asp:Literal runat="server" ID="BankSize" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Question Sets
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="SetCount" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Question Items
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="QuestionCount" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Question Points
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="QuestionPointsSum" />
                            </td>
                        </tr>
                        <tr>
                            <td class="text-nowrap">
                                Question Options
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="OptionCount" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Specifications
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="SpecificationCount" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Forms
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="FormCount" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Attempts
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="AttemptCount" />
                            </td>
                        </tr>
                    </tbody>
                </table>
                                
                <h3>
                    Materials for Distribution
                    <insite:IconLink Name="pencil" runat="server" id="EditExamMaterialsLink1" ToolTip="Revise Exam Materials for Distribution" CssClass="fs-6 ms-1" />
                </h3>

                <div class="form-group mb-3">
                    <div class="form-text">
                        Physical materials required in the distribution package for paper exam forms generated from the bank.
                    </div>
                    <div>
                        <asp:Literal runat="server" ID="ExamMaterialsForDistribution" />
                    </div>
                </div>

                <h3>
                    Materials for Candidates/Participation
                    <insite:IconLink Name="pencil" runat="server" id="EditExamMaterialsLink2" ToolTip="Revise Exam Materials for Participation/Candidates" CssClass="fs-6 ms-1" />
                </h3>

                <div class="form-group mb-3">
                    <div class="form-text">
                        Physical materials permitted to candidates writing an exam form in the bank.
                    </div>
                    <div>
                        <asp:Literal runat="server" ID="ExamMaterialsForParticipation" />
                    </div>
                </div>

            </div>

        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" ID="TranslationsTab" Title="Translations" Visible="false">
        <uc:BankTranslation runat="server" ID="BankTranslationPanel" />
    </insite:NavItem>

    <insite:NavItem runat="server" ID="StatisticsTab" Title="Summaries" Visible="false">
        <uc:QuestionStatisticsPanel runat="server" ID="StatisticsPanel" />
    </insite:NavItem>

    <insite:NavItem runat="server" ID="ProblemsTab" Title="Problems" Visible="false">
        <uc:ProblemRepeater runat="server" ID="ProblemRepeater" />
    </insite:NavItem>

</insite:Nav>