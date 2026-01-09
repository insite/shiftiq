<%@ Page Language="C#" CodeBehind="Add.aspx.cs" Inherits="InSite.Admin.Assessments.Forms.Forms.Add" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="NameField" Src="../Controls/FormNameField.ascx" %>
<%@ Register TagPrefix="uc" TagName="CodeField" Src="../Controls/FormCodeField.ascx" %>
<%@ Register TagPrefix="uc" TagName="QuestionList" Src="../../Sections/Controls/QuestionList.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="CreatorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-4">
        <h2 class="h4 mb-3">
            <i class="far fa-window"></i>
            Add New Assessment Form
        </h2>

        <div class="row">

            <div class="col-lg-6 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Assessment Form</h3>

                        <div runat="server" id="SpecificationOutputField" class="form-group mb-3" visible="false">
                            <div class="float-end">
                                <span class="badge bg-custom-default">
                                    <asp:Literal runat="server" ID="SpecificationOutputType" /></span>
                            </div>
                            <label class="form-label">Specification</label>
                            <div>
                                <asp:Literal runat="server" ID="SpecificationOutputName" />
                            </div>
                            <div class="form-text">
                                Advanced assessment banks use a specification to define requirements that must be satisfied
                            by a form.
                            </div>
                        </div>

                        <uc:NameField runat="server" ID="FormName" ValidationGroup="Assessment" />

                        <uc:CodeField runat="server" ID="CodeField" ValidationGroup="Assessment" />

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Time Limit (Minutes)
                            </label>
                            <div>
                                <insite:NumericBox runat="server" ID="TimeLimit" Width="100%" MinValue="1" MaxValue="262800" NumericMode="Integer" />
                            </div>
                            <div class="form-text">
                                This is the number of minutes allowed for each attempt on the exam.
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="col-lg-6 mb-3 mb-lg-0">

                <asp:PlaceHolder runat="server" ID="CriteriaPanel" Visible="false">

                    <div runat="server" id="CriteriaColumn" visible="false">

                        <div class="col-lg-12 mb-3">

                            <div class="card border-0 shadow-lg h-100">
                                <div class="card-body">
                                    <h3>Criteria</h3>
                                    <asp:Repeater runat="server" ID="CriterionRepeater">
                                        <HeaderTemplate>
                                            <table class="table table-condensed table-hover table-criteria">
                                                <thead>
                                                    <tr>
                                                        <th style="width: 35px;">
                                                            <input type="checkbox" class="input-select-all"></th>
                                                        <th>Question Set</th>
                                                        <th class="text-end">Set Weight</th>
                                                        <th class="text-end">Question Limit</th>
                                                        <th class="text-end">Available</th>
                                                        <th class="text-end">Required</th>
                                                        <th>Filter Type</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                        </HeaderTemplate>
                                        <FooterTemplate>
                                            </tbody>
                                    <tfoot>
                                        <tr>
                                            <th></th>
                                            <th></th>
                                            <th class="text-end">
                                                <asp:Literal runat="server" ID="SetWeightSum" /></th>
                                            <th class="text-end">
                                                <asp:Literal runat="server" ID="QuestionLimitSum" /></th>
                                            <th class="text-end">
                                                <asp:Literal runat="server" ID="AvailableSum" /></th>
                                            <th class="text-end">
                                                <asp:Literal runat="server" ID="RequiredSum" /></th>
                                            <th></th>
                                        </tr>
                                    </tfoot>
                                            </table>
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <tr data-input='<%# Container.FindControl("IsSelected").ClientID %>'>
                                                <td>
                                                    <asp:CheckBox runat="server" ID="IsSelected" /></td>
                                                <td><%# Eval("Title") %></td>
                                                <td class="text-end"><%# Eval("SetWeight", "{0:p0}") %></td>
                                                <td class="text-end"><%# Eval("QuestionLimit", "{0:n0}") %></td>
                                                <td class="text-end"><%# Eval("QuestionCount", "{0:n0}") %></td>
                                                <td class="text-end"><%# Eval("RequiredCount", "{0:n0}") %></td>
                                                <td><%# Eval("FilterType") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>

                            </div>

                        </div>

                        <div class="col-lg-12">
                            <div class="card border-0 shadow-lg h-100">
                                <div class="card-body">
                                    <h3>Form Settings</h3>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Number of Forms
                                <insite:RequiredValidator runat="server" ControlToValidate="FormsCount" ValidationGroup="Assessment" />
                                        </label>
                                        <div>
                                            <insite:NumericBox runat="server" ID="FormsCount" Width="100%" MinValue="1" NumericMode="Integer" />
                                        </div>
                                        <div class="form-text">
                                            The number of forms to be created.
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Question Status Filter
                                <insite:CustomValidator runat="server" ID="QuestionStatusValidator" Enabled="false" ErrorMessage="Question Status Filter is required field." ClientValidationFunction="formAdd.onQuestionStatusValidation" ValidationGroup="Assessment" />
                                        </label>
                                        <div class="status-list">
                                            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="QuestionStatusUpdatePanel" />
                                            <insite:UpdatePanel runat="server" ID="QuestionStatusUpdatePanel">
                                                <ContentTemplate>
                                                    <asp:CheckBoxList runat="server" ID="QuestionStatusList" />
                                                    <div runat="server" id="NoQuestionsMessage" style="margin-top: 8px; padding: 8px 16px; background-color: #f5f5f5; border: 1px solid #ccc; border-radius: 4px;" visible="false">
                                                        No questions found for the selected criteria.
                                                    </div>
                                                </ContentTemplate>
                                            </insite:UpdatePanel>
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Like Item Groups</label>
                                        <div>
                                            <asp:RadioButtonList runat="server" ID="LikeItemGroups">
                                                <asp:ListItem Value="MutuallyExclusive" Text="Mutually Exclusive" Selected="true" />
                                                <asp:ListItem Value="NotMutuallyExclusive" Text="Not Mutually Exclusive" />
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>

                                </div>

                            </div>
                        </div>

                    </div>

                </asp:PlaceHolder>

                <asp:Panel runat="server" ID="NoCriteriaMessage" Visible="false">

                    <div class="col-lg-12 mb-3">

                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <h3>Criteria</h3>
                                <div class="alert alert-info">There are no question sets you can assign to the form.</div>
                            </div>

                        </div>

                    </div>

                </asp:Panel>


            </div>
        </div>

    </section>

    <section runat="server" id="PreviewQuestionsSection" class="mb-3" visible="false">
        <h2 class="h4 mb-3">
            <i class="far fa-question"></i>
            <asp:Literal ID="PreviewQuestionsSectionHeader" runat="server"></asp:Literal>
        </h2>

        <div class="row">

            <div class="col-lg-12">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <insite:Alert runat="server" ID="PreviewQuestionsAlert" />
                        <uc:QuestionList runat="server" ID="PreviewQuestionsList" />
                    </div>
                </div>
            </div>


        </div>

    </section>

    <section runat="server" ID="PreviewFormsSection" Visible="false"  class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-question"></i>
            <asp:Literal ID="PreviewFormsSectionHeader" runat="server"></asp:Literal>
        </h2>

        <div class="row">

            <div class="col-lg-12">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <div class="col-md-2 col-sm-4 col-xs-4">
                            <insite:Nav runat="server" ID="PreviewFormsNav" ItemType="Pills" ItemAlignment="Vertical" ContentRendererID="PreviewFormsNavContent">
                            </insite:Nav>
                        </div>
                        <div class="col-md-10 col-sm-8 col-xs-8">
                            <insite:NavContent runat="server" ID="PreviewFormsNavContent" />
                        </div>

                    </div>
                </div>
            </div>


        </div>

    </section>

    <div class="row" runat="server">
        <div class="col-lg-12">
            <insite:NextButton runat="server" ID="NextButton" ValidationGroup="Assessment" CausesValidation="true" />
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>


<insite:PageHeadContent runat="server">
    <style type="text/css">

        .table-criteria > tbody > tr {
            cursor: pointer;
        }

        .status-list {
            padding-left: 5px;
        }

            .status-list table > tbody > tr > td > input {
                position: absolute;
                margin-top: 5px;
            }

            .status-list table > tbody > tr > td > label {
                padding-left: 25px;
            }

    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var instance = window.formAdd = window.formAdd || {};

            instance.onQuestionStatusValidation = function (sender, args) {
                args.IsValid = false;

                $('table#<%= QuestionStatusList.ClientID %> > tbody > tr > td > input[type="checkbox"]').each(function () {
                    if ($(this).prop('checked') == true) {
                        args.IsValid = true;
                        return false;
                    }
                });
            };

            var $selectAllInput = $('.table-criteria > thead input.input-select-all').on('change', onSelectAll);
			var questionStatusRefreshTimeout = null;

            var $rows = $('.table-criteria > tbody > tr[data-input]').on('click', onRowClick).each(function () {
                var $input = getRowInput(this);
                if ($input !== null)
                    $input.on('change', onRowToggle);
            });

            onRowToggle(true);

            function onRowClick(e) {
                var $input = getRowInput(this, e.target);
                if ($input === null)
                    return;

                $input.prop('checked', !$input.prop('checked'));

                onRowToggle();
            }

            function onRowToggle(initialization) {
                var isAllChecked = $rows.length > 0;

                $rows.each(function () {
                    var $input = getRowInput(this);
                    if ($input === null)
                        return;

                    if (!$input.prop('checked')) {
                        isAllChecked = false;
                        return false;
                    }
                });

                $selectAllInput.prop('checked', isAllChecked);

                if (initialization !== true)
                    refreshQuestionStatus();
            }

            function onSelectAll() {
                var isChecked = $selectAllInput.prop('checked');

                $rows.each(function () {
                    var $input = getRowInput(this);
                    if ($input !== null)
                        $input.prop('checked', isChecked);
                });

                refreshQuestionStatus();
            }

            function getRowInput(row, eventTarget) {
                var inputId = $(row).data('input');
                if (!inputId || eventTarget && eventTarget.id === inputId)
                    return null;

                var $input = $('#' + inputId);
                if ($input.length)
                    return $input.eq(0);

                return null;
            }

            function refreshQuestionStatus(isTimeout) {
                if (isTimeout === true || questionStatusRefreshTimeout) {
                    clearTimeout(questionStatusRefreshTimeout);
                    questionStatusRefreshTimeout = null;
                }

                if (isTimeout !== true || $.active > 0) {
                    questionStatusRefreshTimeout = setTimeout(refreshQuestionStatus, 750, true);
                    return;
                }

                document.getElementById('<%= QuestionStatusUpdatePanel.ClientID %>').ajaxRequest('refresh');
            }
        })();
    </script>
</insite:PageFooterContent>
</asp:Content>
