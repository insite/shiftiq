<%@ Page Language="C#" CodeBehind="ChangeSettings.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.ChangeSettings" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Workflow/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Survey" />

    <section runat="server" id="NameSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-check-square me-1"></i>
            Change Settings
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6">
                        <div>
                            <uc:FormDetails runat="server" ID="SurveyDetail" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div>

                            <div class="form-group mb-3">
                                <label class="form-label">Expected Duration (in Minutes)</label>
                                <div>
                                    <insite:NumericBox runat="server" ID="DurationMinutes" MinValue="0" NumericMode="Integer" CssClass="w-25" />
                                </div>
                                <div class="form-text">
                                    The number of minutes that a user is expected to need in order to complete this form.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Feedback for Respondents</label>
                                <div>
                                    <insite:ComboBox runat="server" ID="UserFeedback" CssClass="w-75" AllowBlank="false" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Limit Submissions per Respondent</label>
                                <div>
                                    <insite:ComboBox runat="server" ID="IsSubmissionsLimitedSelector" CssClass="w-75">
                                        <Items>
                                            <insite:ComboBoxOption Value="True" Text="Limited" />
                                            <insite:ComboBoxOption Value="False" Text="Not Limited" />
                                        </Items>
                                    </insite:ComboBox>
                                </div>
                                <div class="form-text">
                                    If you want to ensure each respondent answers the form no more than once then select Limited.
                                </div>
                            </div>

                            <div runat="server" id="EnableAnonymousSaveGroup" class="form-group mb-3" style="display: none;">
                                <label class="form-label">Allow Anonymous Submissions</label>
                                <div>
                                    <insite:BooleanComboBox runat="server" ID="EnableAnonymousSave" TrueText="Permitted" FalseText="Not Permitted" AllowBlank="false" CssClass="w-75" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Confidentiality for Respondents</label>
                                <div>
                                    <insite:BooleanComboBox runat="server" ID="EnableUserConfidentiality" TrueText="Enabled" FalseText="Disabled" AllowBlank="false" CssClass="w-75" />
                                </div>
                                <div class="form-text">
                                    Enable this setting if you do not want to disclose confidential/personal information 
                                    about form respondents to form administrators.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Display Summary Chart</label>
                                <div>
                                    <insite:BooleanComboBox runat="server" ID="DisplaySummaryChart" TrueText="Display" FalseText="Not Display" AllowBlank="false" CssClass="w-75" />
                                </div>
                                <div class="form-text">
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

            </div>
        </div>
    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Survey" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
        
            (function () {
                Sys.Application.add_load(function () {
                    $('#<%= IsSubmissionsLimitedSelector.ClientID %>')
                        .off('change', onIsSubmissionsLimitedChanged)
                        .on('change', onIsSubmissionsLimitedChanged);
        
                    onIsSubmissionsLimitedChanged();
                });
        
                function onIsSubmissionsLimitedChanged() {
                    var $combo = $('#<%= IsSubmissionsLimitedSelector.ClientID %>');
                    var $field = $('#<%= EnableAnonymousSaveGroup.ClientID %>');
                    if ($combo.selectpicker('val') === 'True') {
                        $field.hide();
                    } else {
                        $field.show();
                    }
                }
            })();
        
        </script>
    </insite:PageFooterContent>

</asp:Content>
