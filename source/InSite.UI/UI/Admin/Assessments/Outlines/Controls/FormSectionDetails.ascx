<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormSectionDetails.ascx.cs" Inherits="InSite.Admin.Assessments.Outlines.Controls.FormSectionDetails" %>

<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<%@ Register TagPrefix="uc" TagName="QuestionRepeater" Src="../../Questions/Controls/QuestionRepeater.ascx" %>

<div class="float-end">
    <insite:DownloadButton runat="server" ID="DownloadSummariesButton" Text="Download Summaries" />
</div>
<div runat="server" id="Section">
    <insite:Nav runat="server">

        <insite:NavItem runat="server" ID="QuestionsTab">
            <insite:Alert runat="server" ID="QuestionsAlert" />

            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="QuestionsUpdatePanel" />
            <insite:UpdatePanel runat="server" ID="QuestionsUpdatePanel">
                <ContentTemplate>
                    <uc:QuestionRepeater runat="server" ID="Questions" />

                    <insite:Button runat="server" ID="LoadQuestionsButton" Visible="false" ButtonStyle="Success" CssClass="w-100 mt-3" Text="Load All Questions" Icon="fas fa-spinner" />
                </ContentTemplate>
            </insite:UpdatePanel>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="SectionTab" Title="Section">
            <div class="row">
                <div class="col-lg-6">

                    <h3>Identification</h3>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Section Number
                            <insite:IconLink Name="trash-alt" runat="server" ID="DeleteSectionLink" ToolTip="Delete Section" CssClass="ms-1" />
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="SectionNumber" />
                        </div>
                        <div class="form-text">
                            This section of the form takes items from the question set identified below.
                        </div>
                    </div>

                    <h3>Content</h3>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Title
                            <insite:IconLink Name="pencil" runat="server" ID="EditSectionContentTitle" ToolTip="Revise External Title" CssClass="ms-1" />
                        </label>
                        <div>
                            <span runat="server" id="SectionContentTitle" style="white-space: pre-wrap;"></span>
                        </div>
                        <div class="form-text">
                            The external title for this section.
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Summary
                            <insite:IconLink Name="pencil" runat="server" ID="EditSectionContentSummary" ToolTip="Revise Summary" CssClass="ms-1" />
                        </label>
                        <div>
                            <span runat="server" id="SectionContentSummary" style="white-space: pre-wrap;"></span>
                        </div>
                        <div class="form-text">
                            The purpose of this section.
                        </div>
                    </div>

                    <insite:Container runat="server" ID="ConfigurationSection">

                        <h3>Configuration</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Warning on Next Tab
                                <insite:IconLink Name="pencil" runat="server" ID="ReconfigureLink" ToolTip="Reconfigure Section" CssClass="ms-1" />
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="WarningOnNextTabEnabled" />
                            </div>
                        </div>

                        <div runat="server" id="BreakTimerEnabledField" class="form-group mb-3">
                            <label class="form-label">
                                Break Timer
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="BreakTimerEnabled" />
                            </div>
                        </div>

                        <div runat="server" id="TimeLimitField" class="form-group mb-3">
                            <label class="form-label">
                                Time Limit
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="TimeLimit" />
                            </div>
                        </div>

                        <div runat="server" id="TimerTypeField" class="form-group mb-3">
                            <label class="form-label">
                                Timer Type
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="TimerType" />
                            </div>
                        </div>

                    </insite:Container>

                </div>
                <div class="col-lg-6">

                    <h3>Specification Criteria</h3>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Set Names
                        </label>
                        <div>
                            <asp:Repeater runat="server" ID="SetRepeater">
                                <HeaderTemplate>
                                    <ul>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <li>
                                        <%# Eval("Name") %>
                                    </li>
                                </ItemTemplate>
                                <FooterTemplate></ul></FooterTemplate>
                            </asp:Repeater>
                        </div>
                        <div class="form-text">
                            The sets from which questions are selected and filtered for this criterion
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Set Weight
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="SetWeight" />
                        </div>
                        <div class="form-text">
                            The desired weighting for the question set to which this criterion applies, within the overall specification.
                            The sum of all question set weights for the criteria in a specification must equal 1 (i.e. 100 percent).
                        </div>
                    </div>

                    <div runat="server" id="QuestionLimitField" class="form-group mb-3">
                        <label class="form-label">
                            Question Item Limit
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="QuestionLimit" />
                        </div>
                        <div class="form-text">
                            The maximum number of items allowed on an exam form from this set.
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Question Item Filter
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="FilterType" />
                        </div>
                        <div class="form-text">
                            The type of filter applied to the question items in this set.
                        </div>
                    </div>

                    <div class="form-group mb-3" runat="server" id="TagFilterField">
                        <label class="form-label">
                            Question Tag Filter
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="TagFilter" />
                        </div>
                        <div class="form-text">
                            The type of filter applied to the question items in this set.
                        </div>
                    </div>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="FormTab" Title="Form">
            <assessments:ContentRepeater runat="server" ID="FormDetails" ControlPath="~/UI/Admin/Assessments/Forms/Controls/FormDetails.ascx" />
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ContentTab" Title="Content">
            <assessments:ContentRepeater runat="server" ID="FormContent" ControlPath="~/UI/Admin/Assessments/Forms/Controls/FormContent.ascx" />
        </insite:NavItem>

        <insite:NavItem runat="server" ID="AddendumTab" Title="Addendum">
            <assessments:ContentRepeater runat="server" ID="AddendumDetails" ControlPath="~/UI/Admin/Assessments/Forms/Controls/FormAddendumDetails.ascx" />
        </insite:NavItem>

        <insite:NavItem runat="server" ID="StatisticsTab" Title="Summaries">
            <assessments:ContentRepeater runat="server" ID="StatisticsPanel" ControlPath="~/UI/Admin/Assessments/Questions/Controls/QuestionStatisticsPanel.ascx" />
        </insite:NavItem>

    </insite:Nav>
</div>
