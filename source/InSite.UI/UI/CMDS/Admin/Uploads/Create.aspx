<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Cmds.Admin.Uploads.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/GlobalAchievementExporter.ascx" TagName="GlobalAchievementExporter" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/UploadCompetencyChooser.ascx" TagName="UploadCompetencyChooser" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Upload" />

    <section runat="server" ID="FileSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-upload me-1"></i>
            Upload a Achievement
        </h2>

        <div class="card border-0 shadow-lg mb-3">
            <div class="card-body">
                <div class="row">
                    <div class="col-lg-6">
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Uploaded Type
                            </label>
                            <div class="w-75">
                                <insite:ComboBox runat="server" ID="UploadType">
                                    <Items>
                                        <insite:ComboBoxOption Value="File" Text="File" />
                                        <insite:ComboBoxOption Value="Link" Text="Link" />
                                    </Items>
                                </insite:ComboBox>
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div runat="server" id="FileUploadField" class="form-group mb-3">
                            <label class="form-label">
                                File
                                <insite:RequiredValidator runat="server" ID="FileUploadRequired" ControlToValidate="FileUpload" FieldName="File" ValidationGroup="Upload" />
                            </label>
                            <div class="w-75">
                                <insite:FileUploadV1 ID="FileUpload" runat="server" LabelText="" FileUploadType="Unlimited" />
                            </div>
                        </div>
                        <div runat="server" id="UrlField" class="form-group mb-3">
                            <label class="form-label">
                                URL
                                <insite:RequiredValidator runat="server" ID="NavigationUrlRequired" ControlToValidate="NavigationUrl" FieldName="URL" ValidationGroup="Upload" />
                                <insite:UrlValidator runat="server" ID="NavigationUrlValid"
                                    ControlToValidate="NavigationUrl"
                                    ErrorMessage="Specified URL is not valid"
                                    ValidationGroup="Upload"
                                />
                            </label>
                            <div class="w-75">
                                <insite:TextBox ID="NavigationUrl" runat="server" MaxLength="500" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div runat="server" id="UrlTitleField" class="form-group mb-3">
                            <label class="form-label">
                                Title
                                <insite:RequiredValidator runat="server" ID="UrlTitleRequired" ControlToValidate="UrlTitle" FieldName="Title" ValidationGroup="Upload" />
                            </label>
                            <div class="w-75">
                                <insite:TextBox ID="UrlTitle" runat="server" MaxLength="256" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div runat="server" id="OverwriteFileField" class="form-group mb-3">
                            <div>
                                <asp:CheckBox ID="OverwriteFile" runat="server" Text="Overwrite this file if it already exists" Checked="true" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div class="form-group mb-3">
                            <div>
                                <asp:RadioButtonList ID="AchievementOption" runat="server" RepeatDirection="Vertical" CssClass="check-list">
                                    <asp:ListItem Value="AchievementOnly" Text="Attach this file to the achievement only" Selected="True" />
                                    <asp:ListItem Value="Both" Text="Attach this file to the achievement and to the competency" />
                                </asp:RadioButtonList>
                            </div>
                            <div class="form-text"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

        <insite:UpdatePanel runat="server" ID="UpdatePanel">
            <ContentTemplate>
                <div class="row">

                    <div class="col-lg-6">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Type
                                        <insite:RequiredValidator runat="server" ControlToValidate="AchievementType" FieldName="Type" ValidationGroup="Upload" />
                                    </label>
                                    <div>
                                        <cmds:AchievementTypeSelector ID="AchievementType" runat="server" Width="450" ClientEvents-OnChange="enableUploadAchievement" NullText="" />
                                    </div>
                                    <div class="form-text"></div>
                                </div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Category
                                    </label>
                                    <insite:MultiField runat="server" ID="AchievementCategoryField">

                                        <insite:MultiFieldView runat="server" ID="AchievementCategorySelectorView" Inputs="AchievementCategorySelector">
                                            <span class="multi-field-input w-75">
                                                <cmds:TrainingCategorySelector ID="AchievementCategorySelector" runat="server" />
                                            </span>
                                            <insite:Button runat="server"
                                                OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                ButtonStyle="Default" Icon="far fa-keyboard"
                                                ToolTip="Enter a value manually" />
                                        </insite:MultiFieldView>

                                        <insite:MultiFieldView runat="server" ID="AchievementCategoryTextView" Inputs="AchievementCategoryText">
                                            <span class="multi-field-input w-75">
                                                <insite:TextBox runat="server" ID="AchievementCategoryText" />
                                            </span>
                                            <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                ButtonStyle="Default" Icon="far fa-list-ul" ToolTip="Select an option from the list" />
                                        </insite:MultiFieldView>

                                    </insite:MultiField>
                                    <div class="form-text"></div>
                                </div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Achievement
                                        <insite:RequiredValidator ID="AchievementSelectorRequired" runat="server" ControlToValidate="AchievementSelector" FieldName="Achievement" ValidationGroup="Upload" />
                                        <insite:RequiredValidator ID="AchievementTextRequired" runat="server" ControlToValidate="AchievementText" FieldName="Achievement" ValidationGroup="Upload" />
                                    </label>
                                    <insite:MultiField runat="server" ID="AchievementField">

                                        <insite:MultiFieldView runat="server" ID="AchievementSelectorView" Inputs="AchievementSelector">
                                            <span class="multi-field-input w-75">
                                                <cmds:FindAchievement ID="AchievementSelector" runat="server" />
                                            </span>
                                            <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                ButtonStyle="Default" Icon="far fa-keyboard" ToolTip="Enter a value manually" />
                                        </insite:MultiFieldView>

                                        <insite:MultiFieldView runat="server" ID="AchievementTextView" Inputs="AchievementText">
                                            <span class="multi-field-input w-75">
                                                <insite:TextBox runat="server" ID="AchievementText" MaxLength="100" />
                                            </span>
                                            <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                ButtonStyle="Default" Icon="far fa-list-ul" ToolTip="Select an option from the list" />
                                        </insite:MultiFieldView>

                                    </insite:MultiField>
                                    <div class="form-text"></div>
                                </div>
                                <div runat="server" id="CompetencyCell" class="form-group mb-3">
                                    <label class="form-label">
                                        Competency
                                    </label>
                                    <div>
                                        <uc:UploadCompetencyChooser ID="Competency" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-6">
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <uc:GlobalAchievementExporter ID="GlobalAchievementExporter" runat="server" />
                            </div>
                        </div>
                    </div>

                </div>
            </ContentTemplate>
        </insite:UpdatePanel>
    </section>

    <div class="mb-3">
        <insite:SaveButton runat="server" ID="SubmitButton" Text="Submit" ValidationGroup="Upload" OnClientClick="return !this.getAttribute('disabled');" />
        <insite:CancelButton runat="server" NavigateUrl="/ui/cmds/admin/uploads/search" />
    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            function disableUploadAchievement() {
                if ($get("<%= NavigationUrl.ClientID %>") == null) {
                    $get("<%= OverwriteFile.ClientID %>").disabled = true;

                    ValidatorEnable($get("<%= FileUploadRequired.ClientID %>"), false);
                } else {
                    $get("<%= NavigationUrl.ClientID %>").setAttribute('disabled', 'disabled');
                    $get("<%= UrlTitle.ClientID %>").setAttribute('disabled', 'disabled');

                    ValidatorEnable($get("<%= NavigationUrlRequired.ClientID %>"), false);
                    ValidatorEnable($get("<%= NavigationUrlValid.ClientID %>"), false);
                    ValidatorEnable($get("<%= UrlTitleRequired.ClientID %>"), false);
                }

                disableAchievementCategoryField(true);
                disableAchievementField(true);
                disableCompetencyChooser($get("<%= CompetencyCell.ClientID %>"));

                $get("<%= SubmitButton.ClientID %>").setAttribute('disabled', 'disabled');
                $get("<%= SubmitButton.ClientID %>").classList.add("disabled")
            }

            function enableUploadAchievement() {
                if ($get("<%= NavigationUrl.ClientID %>") == null) {
                    $get("<%= OverwriteFile.ClientID %>").disabled = false;

                    ValidatorEnable($get("<%= FileUploadRequired.ClientID %>"), true);
                    $get("<%= FileUploadRequired.ClientID %>").style.visibility = "hidden";
                } else {
                    $get("<%= NavigationUrl.ClientID %>").removeAttribute('disabled');
                    $get("<%= UrlTitle.ClientID %>").removeAttribute('disabled');

                    ValidatorEnable($get("<%= NavigationUrlRequired.ClientID %>"), true);
                    $get("<%= NavigationUrlRequired.ClientID %>").style.visibility = "hidden";

                    ValidatorEnable($get("<%= NavigationUrlValid.ClientID %>"), true);
                    $get("<%= NavigationUrlValid.ClientID %>").style.visibility = "hidden";

                    ValidatorEnable($get("<%= UrlTitleRequired.ClientID %>"), true);
                    $get("<%= UrlTitleRequired.ClientID %>").style.visibility = "hidden";
                }

                disableAchievementCategoryField(false);
                disableAchievementField(false);

                enableCompetencyChooser($get("<%= CompetencyCell.ClientID %>"));

                $get("<%= SubmitButton.ClientID %>").removeAttribute('disabled');
                $get("<%= SubmitButton.ClientID %>").classList.remove("disabled");

            }

            function disableAchievementField(disable) {
                var selectorValidator = document.getElementById('<%= AchievementSelectorRequired.ClientID %>');
                ValidatorEnable(selectorValidator, !disable);
                selectorValidator.style.visibility = "hidden";

                var textValidator = document.getElementById('<%= AchievementTextRequired.ClientID %>');
                ValidatorEnable(textValidator, !disable);
                textValidator.style.visibility = "hidden";

                var $field = $('#<%= AchievementField.ClientID %>');
                var $anchors = $field.find('a').prop('disabled', disable);
                var selector = document.getElementById('<%= AchievementSelector.ClientID %>');
                var text = document.getElementById('<%= AchievementSelector.ClientID %>');

                if (disable) {
                    $anchors.addClass('disabled');
                    selector.disable();
                    text.setAttribute('disabled', 'disabled');
                } else {
                    $anchors.removeClass('disabled');
                    selector.enable();
                    text.removeAttribute('disabled');
                }
            }

            function disableAchievementCategoryField(disable) {
                var $field = $('#<%= AchievementCategoryField.ClientID %>');
                $field.find('select').prop('disabled', disable).selectpicker('refresh');
                $field.find('input[type="text"]').prop('disabled', disable);
                var $anchors = $field.find('a').prop('disabled', disable);

                if (disable)
                    $anchors.addClass('disabled');
                else
                    $anchors.removeClass('disabled');
            }
        </script>
    </insite:PageFooterContent>
</asp:Content>
