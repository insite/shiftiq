<%@ Page Language="C#" CodeBehind="Reconfigure.aspx.cs" Inherits="InSite.UI.Admin.Assessments.Sections.Forms.Reconfigure" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assessments/Criteria/Controls/Detail.ascx" TagName="CriterionDetails" TagPrefix="uc" %>


<%--
<%@ Register TagPrefix="uc" TagName="ConfigurationDetails" Src="../Controls/ConfigurationDetails.ascx" %>--%>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-clipboard-list"></i>
            Reconfigure Specification
        </h2>

        <div class="row">

            <div class="col-lg-4 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Section</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Number
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="SectionNumber" />
                            </div>
                            <div class="form-text">
                                This section of the form takes items from the question set identified below.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Title
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
                            </label>
                            <div>
                                <span runat="server" id="SectionContentSummary" style="white-space: pre-wrap;"></span>
                            </div>
                            <div class="form-text">
                                The purpose of this section.
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="col-lg-4 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Configuration</h3>

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ConfigurationUpdatePanel" />

                        <insite:UpdatePanel runat="server" ID="ConfigurationUpdatePanel">
                            <ContentTemplate>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Warning on Next Tab
                                    </label>
                                    <div>
                                        <insite:BooleanComboBox runat="server" ID="WarningOnNextTab" TrueText="Show" FalseText="Disabled" AllowBlank="false" />
                                    </div>
                                </div>

                                <div runat="server" id="BreakTimerField" class="form-group mb-3">
                                    <label class="form-label">
                                        Break Timer
                                    </label>
                                    <div>
                                        <insite:BooleanComboBox runat="server" ID="BreakTimer" TrueText="Enabled" FalseText="Disabled" AllowBlank="false" />
                                    </div>
                                </div>

                                <div runat="server" id="TimeLimitField" class="form-group mb-3">
                                    <label class="form-label">
                                        Time Limit (minutes)
                                    </label>
                                    <div>
                                        <insite:NumericBox runat="server" ID="TimeLimit" Width="100%" MinValue="0" MaxValue="1440" NumericMode="Integer" />
                                    </div>
                                </div>

                                <div runat="server" id="TimerTypeField" class="form-group mb-3">
                                    <label class="form-label">
                                        Timer Type
                                    </label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="TimerType" AllowBlank="false" />
                                    </div>
                                </div>

                            </ContentTemplate>
                        </insite:UpdatePanel>
                    </div>
                </div>

            </div>

            <div class="col-lg-4 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Criterion</h3>

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

            </div>
        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" />
            <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
        </div>
    </div>

</asp:Content>
