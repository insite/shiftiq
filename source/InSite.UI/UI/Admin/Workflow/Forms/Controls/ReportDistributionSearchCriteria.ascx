<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportDistributionSearchCriteria.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.ReportDistributionSearchCriteria" %>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .form-check-label {
            margin-left:0.5rem;
        }

        .toggle-off.btn {
            margin-bottom: 0;
        }
    </style>
</insite:PageHeadContent>

<div class="form-group mb3">
	<label class="form-label">
		Display Options
	</label>
	<div>
        <insite:CheckSwitch runat="server" ID="ShowSelections" Text="Display quantitative results (selections)" />
        <insite:CheckSwitch runat="server" ID="ShowNumbers" Text="Display quantitative results (numeric)" />
        <insite:CheckSwitch runat="server" ID="ShowText" Text="Display qualitative results (text and comments)" />
        <insite:CheckSwitch runat="server" ID="EnableQuestionFilter" Text="Filter by answer field" />

        <div style="overflow:hidden;">

            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AnswerFilterUpdatePanel" />

            <insite:UpdatePanel runat="server" ID="AnswerFilterUpdatePanel" CssClass="toggle-panel">
                <ContentTemplate>
                    <asp:Repeater runat="server" ID="AnswerFilterRepeater">
                        <HeaderTemplate>
                            <table class="table">
                                <thead>
                                    <tr>
                                        <th>Question</th>
                                        <th>Comparison</th>
                                        <th>Option</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("QuestionOutputText") %></td>
                                <td><%# Eval("ComparisonName") %></td>
                                <td><%# Eval("AnswerOutputText") %></td>
                                <td>
                                    <insite:IconButton runat="server" Name="trash-alt" CommandName="Delete" ConfirmText="Are you want to delete this item?" />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>

                    <div runat="server" id="AnswerFilterNoItemMessage" style="margin:0 0 30px 0;" visible="false">
                        No filtering conditions to display.
                    </div>

                    <div class="float-end pt-1">
                        <asp:RadioButtonList runat="server" ID="FilterOperator" RepeatLayout="Flow" RepeatDirection="Horizontal">
                            <Items>
                                <asp:ListItem Value="and" Text="AND" Selected="True" />
                                <asp:ListItem Value="or" Text="OR" />
                            </Items>
                        </asp:RadioButtonList>
                    </div>

                    <asp:Panel runat="server" ID="AnswerFieldFilterButtons" Visible="false" CssClass="toggle-buttons">
                        <insite:Button runat="server" ID="AddAnswerConditionButton" Icon="fas fa-plus-circle" Text="Add Filter" ButtonStyle="Default"
                            OnClientClick="reportSearchCriteria.showEditor('open'); return false;" />
                        <insite:ClearButton runat="server" ID="ClearAnswersFilterButton" CausesValidation="false" />
                    </asp:Panel>
                </ContentTemplate>
            </insite:UpdatePanel>

        </div>

    </div>
</div>

<div class="form-group mb3">
	<label class="form-label">&nbsp;</label>
	<div class="text-end">
        <insite:Button runat="server" ID="SearchButton" Icon="fas fa-check" Text="Submit"
            Width="97px" ValidationGroup="DistributionReport" />
        <insite:ClearButton runat="server" ID="ClearButton" Width="97px" CausesValidation="false" />
    </div>
</div>

<insite:Modal runat="server" ID="AnswerFilterWindow" Title="Add Filter">
    <ContentTemplate>
        <div class="row">
            <div class="col-md-12 col-lg-12">
                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AnswerFilterWindowUpdatePanel" />
                <insite:UpdatePanel runat="server" ID="AnswerFilterWindowUpdatePanel" ClientEvents-OnResponseEnd="reportSearchCriteria.onEditorAjaxResponseEnd">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <div class="form-group mb3">
                                <label class="col-sm-3 form-label">Question</label>
                                <div class="col-sm-8">
                                    <insite:FormQuestionComboBox runat="server" ID="AnswerFilterQuestionID" Width="100%" MaxTextLength="40" ZIndex="8501" MaxHeight="500px" HasResponseAnswer="true" ExcludeSpecialQuestions="true" />
                                    <div>
                                        <asp:Literal runat="server" ID="AnswerFilterQuestionControlName" />
                                    </div>
                                </div>
                                <div class="col-sm-1">
                                    <insite:RequiredValidator runat="server" ControlToValidate="AnswerFilterQuestionID" FieldName="Question" ValidationGroup="AnswerFilter" />
                                </div>
                            </div>

                            <div class="form-group mb3">
                                <label class="col-sm-3 form-label">Comparison</label>
                                <div class="col-sm-8">
                                    <asp:RadioButtonList runat="server" ID="AnswerFilterComparison" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="Equals" Value="Equals" Selected="True" />
                                        <asp:ListItem Text="Not Equals" Value="NotEquals" />
                                    </asp:RadioButtonList>
                                </div>
                                <div class="col-sm-1">
                                </div>
                            </div>

                            <div class="form-group mb3">
                                <label class="col-sm-3 form-label">Answer</label>
                                <div class="col-sm-8">
                                    <insite:FormSubmissionAnswerComboBox runat="server" ID="AnswerFilterAnswerText" AllowBlank="true" Width="100%" ZIndex="8501" MaxTextLength="100" MaxHeight="500px" />
                                </div>
                                <div class="col-sm-1">
                                    <insite:RequiredValidator runat="server" ControlToValidate="AnswerFilterAnswerText" FieldName="Answer" ValidationGroup="AnswerFilter" />
                                </div>
                            </div>

                            <div class="form-group mb3" style="margin-top:15px;">
                                <div class="col-sm-12">
                                    <insite:Button runat="server" ID="AddConditionButton" Text="Add" Icon="fas fa-plus-circle" ButtonStyle="Default" ValidationGroup="AnswerFilter" />
                                    <insite:CancelButton runat="server" CausesValidation="false" OnClientClick="reportSearchCriteria.closeEditor(); return false;" />
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>
            </div>
        </div>
    </ContentTemplate>
</insite:Modal>

<insite:PageFooterContent runat="server">

<script type="text/javascript">
    var reportSearchCriteria = {
        showEditor: function (cmd) {
            modalManager.show('<%= AnswerFilterWindow.ClientID %>');
            document.getElementById('<%= AnswerFilterWindowUpdatePanel.ClientID %>').ajaxRequest(cmd);
        },
        closeEditor: function () {
            modalManager.close('<%= AnswerFilterWindow.ClientID %>');
        },

        onEditorAjaxResponseEnd: function (s, e) {
            if (e.asyncTarget == '<%= AddConditionButton.UniqueID %>') {
                reportSearchCriteria.closeEditor();

                __doPostBack('<%= AddAnswerConditionButton.UniqueID %>', '');
            }
        }
    };

    $(document).ready(function () {
        var $EnableQuestionFilter = $('#<%= EnableQuestionFilter.ClientID %>').change(function () {
            var $panel = $('#<%= AnswerFilterUpdatePanel.ClientID %>');
            if ($(this).prop('checked') === true)
                $panel.show('blind', { duration: 250, easing: 'swing' });
            else
                $panel.hide('blind', { duration: 250, easing: 'swing' });
        });

        $('#<%= AnswerFilterUpdatePanel.ClientID %>').css('display', $EnableQuestionFilter.prop('checked') === true ? '' : 'none');
    });
</script>

</insite:PageFooterContent>