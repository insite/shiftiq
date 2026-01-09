<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailList.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Questions.Controls.DetailList" %>

<%@ Register Src="~/UI/Admin/Workflow/Forms/Questions/Controls/OptionGrid.ascx" TagName="OptionGrid" TagPrefix="uc" %>

<div>
    <uc:OptionGrid runat="server" ID="OptionGrid" />
</div>

<div class="mt-3">

    <div runat="server" id="ListEnableRandomizationField">
        <insite:CheckBox runat="server" ID="ListEnableRandomization" Text="Randomize Options" />
    </div>
    <div runat="server" id="ListEnableOtherTextField">
        <insite:CheckBox runat="server" ID="ListEnableOtherText" Text="Display <strong>Other</strong> Text Box" />
    </div>
    <div runat="server" id="ListEnableBranchField">
        <insite:CheckBox runat="server" ID="ListEnableBranch" Text="Enable Branches" />
    </div>
    <div runat="server" id="ListEnableMembershipField">
        <insite:CheckBox runat="server" ID="ListEnableGroupMembership" Text="Enable Group Memberships" />
    </div>
    <insite:UpdatePanel runat="server" ID="SelectionRangeField">
        <ContentTemplate>
            <insite:CheckBox runat="server" ID="SelectionRangeEnabled" Text="Set Min/Max Selection" />

            <asp:Panel runat="server" ID="SelectionRangeFieldsPanel" CssClass="ms-3 mt-2 row" style="max-width:250px;" Visible="false">
                <div class="col">
                    <div class="form-group mb-3">
                        <label class="form-label">
                            Min
                            <insite:CustomValidator runat="server" ID="SelectionRangeRequiredValidation"
                                ErrorMessage="Required field: Min/Max Selection" Display="None"
                                ClientValidationFunction="detailList.onSelectionMinMaxValidation"
                                ControlToValidate="SelectionRangeMin" ValidateEmptyText="true"
                                ValidationGroup="SurveyQuestion"
                            />
                            <insite:CompareValidator runat="server" Display="None" Type="Integer"
                                ControlToValidate="SelectionRangeMin" ControlToCompare="SelectionRangeMax" Operator="LessThan" 
                                ErrorMessage="<strong>Min Selection</strong> field value must be less than <strong>Max Selection</strong> field value"
                                ValidationGroup="SurveyQuestion" 
                            />
                        </label>
                        <div>
                            <insite:NumericBox runat="server" ID="SelectionRangeMin" MinValue="0" NumericMode="Integer" />
                        </div>
                    </div>
                </div>
                <div class="col">
                    <div class="form-group mb-3">
                        <label class="form-label">
                            Max
                        </label>
                        <div>
                            <insite:NumericBox runat="server" ID="SelectionRangeMax" MinValue="0" NumericMode="Integer" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </insite:UpdatePanel>

</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            if (window.detailList)
                return;

            const instance = window.detailList = {
                onSelectionMinMaxValidation: function (s, a) {
                    const min = parseInt(document.getElementById('<%= SelectionRangeMin.ClientID %>').value);
                    const max = parseInt(document.getElementById('<%= SelectionRangeMax.ClientID %>').value);

                    a.IsValid = typeof min == 'number' && !isNaN(min) || typeof max == 'number' && !isNaN(max);
                }
            };
        })();
    </script>
</insite:PageFooterContent>