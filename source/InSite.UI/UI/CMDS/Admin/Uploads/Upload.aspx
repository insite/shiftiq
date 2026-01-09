<%@ Page Language="C#" CodeBehind="Upload.aspx.cs" Inherits="InSite.Cmds.Actions.Contact.Company.Achievement.Upload" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/CompanyAchievementProblems.ascx" TagName="CompanyAchievementProblems" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/UploadCompetencyChooser.ascx" TagName="UploadCompetencyChooser" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />

    <uc:CompanyAchievementProblems ID="CompanyAchievementProblems" runat="server" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" Title="Upload a File" Icon="far fa-upload" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Upload a File
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <div class="row">

                            <div class="col-lg-6">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Select and Upload File(s)
                                        <insite:RequiredValidator runat="server" ID="FileUploadRequired" ControlToValidate="FileUpload" FieldName="File" ValidationGroup="FileValidation" />
                                    </label>
                                    <div class="w-75">
                                        <insite:FileUploadV1 ID="FileUpload" runat="server" LabelText="" FileUploadType="Unlimited" />
                                    </div>
                                </div>

                                <insite:UpdatePanel runat="server">
                                    <ContentTemplate>

                                        <div class="form-group mb-3">
                                            <div>
                                                <asp:CheckBox ID="OverwriteFile" runat="server" Text="Overwrite this file if it already exists" Checked="true" />
                                            </div>
                                        </div>
                                        <div class="form-group mb-3">
                                            <div>
                                                <asp:RadioButtonList ID="FileOption" runat="server" RepeatDirection="Vertical" CssClass="check-list">
                                                    <asp:ListItem Value="AchievementOnly" Text="Attach this file to the achievement only" Selected="True" />
                                                    <asp:ListItem Value="Both" Text="Attach this file to the achievement and to the competency" />
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Type
                                                <insite:RequiredValidator runat="server" ControlToValidate="FileAchievementType" FieldName="Type" ValidationGroup="FileValidation" />
                                            </label>
                                            <div class="w-75">
                                                <cmds:AchievementTypeSelector ID="FileAchievementType" runat="server" ClientEvents-OnChange="enableUploadAchievementFile" NullText="" />
                                            </div>
                                        </div>
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Category
                                            </label>
                                            <insite:MultiField runat="server" ID="FileAchievementCategoryField">

                                                <insite:MultiFieldView runat="server" ID="FileAchievementCategorySelectorView" Inputs="FileAchievementCategorySelector">
                                                    <span class="multi-field-input w-75">
                                                        <cmds:TrainingCategorySelector ID="FileAchievementCategorySelector" runat="server" />
                                                    </span>
                                                    <insite:Button runat="server"
                                                        OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                        ButtonStyle="Default"
                                                        Icon="far fa-keyboard"
                                                        ToolTip="Enter a value manually"
                                                    />
                                                </insite:MultiFieldView>

                                                <insite:MultiFieldView runat="server" ID="FileAchievementCategoryTextView" Inputs="FileAchievementCategoryText">
                                                    <span class="multi-field-input w-75">
                                                        <insite:TextBox runat="server" ID="FileAchievementCategoryText" />
                                                    </span>
                                                    <insite:Button runat="server"
                                                        OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                        ButtonStyle="Default"
                                                        Icon="far fa-list-ul"
                                                        ToolTip="Select an option from the list"
                                                    />
                                                </insite:MultiFieldView>

                                            </insite:MultiField>
                                        </div>
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Achievement
                                                <insite:RequiredValidator ID="FileAchievementSelectorRequired" runat="server" ControlToValidate="FileAchievementSelector" FieldName="Achievement" ValidationGroup="FileValidation" />
                                                <insite:RequiredValidator ID="FileAchievementTextRequired" runat="server" ControlToValidate="FileAchievementText" FieldName="Achievement" ValidationGroup="FileValidation" />
                                            </label>
                                            <insite:MultiField runat="server" ID="FileAchievementField">

                                                <insite:MultiFieldView runat="server" ID="FileAchievementSelectorView" Inputs="FileAchievementSelector">
                                                    <span class="multi-field-input w-75">
                                                        <cmds:FindAchievement ID="FileAchievementSelector" runat="server" PageSize="10" />
                                                    </span>
                                                    <insite:Button runat="server"
                                                        OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                        ButtonStyle="Default"
                                                        Icon="far fa-keyboard"
                                                        ToolTip="Enter a value manually"
                                                    />
                                                </insite:MultiFieldView>

                                                <insite:MultiFieldView runat="server" ID="FileAchievementTextView" Inputs="FileAchievementText">
                                                    <span class="multi-field-input w-75">
                                                        <insite:TextBox runat="server" ID="FileAchievementText" MaxLength="100" />
                                                    </span>
                                                    <insite:Button runat="server"
                                                        OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                        ButtonStyle="Default"
                                                        Icon="far fa-list-ul"
                                                        ToolTip="Select an option from the list"
                                                    />
                                                </insite:MultiFieldView>

                                            </insite:MultiField>
                                        </div>
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Competency
                                            </label>
                                            <div runat="server" id="FileCompetencyCell">
                                                <uc:UploadCompetencyChooser ID="FileCompetency" runat="server" />
                                            </div>
                                        </div>
                                        <div class="form-group mb-3">
                                            <div class="mb-3">
                                                <insite:SaveButton ID="SubmitFileButton" runat="server"
                                                    Icon="fas fa-upload"
                                                    Text="Submit"
                                                    ButtonStyle="Success"
                                                    ValidationGroup="FileValidation"
                                                    OnClientClick="return !this.getAttribute('disabled');"
                                                />
                                                <insite:CancelButton runat="server" ID="HomeButton1" />
                                            </div>
                                            <insite:ValidationSummary runat="server" ValidationGroup="FileValidation" />
                                        </div>

                                    </ContentTemplate>
                                </insite:UpdatePanel>

                            </div>

                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" Title="Uploaded Files" Icon="far fa-paperclip" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Uploaded Files
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="GridUpdatePanel" />

                        <insite:UpdatePanel runat="server" ID="GridUpdatePanel">
                            <ContentTemplate>
                                <p runat="server" id="NoUploadedFilesPanel">No Uploaded Files</p>

                                <asp:PlaceHolder ID="UploadedFilesPanel" runat="server">

                                    <insite:Grid runat="server" ID="FileDownloads" DataKeyNames="UploadIdentifier">
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <a href='/ui/cmds/design/uploads/edit?id=<%# Eval("UploadIdentifier") %>&achievement-uploader=yes'>
                                                        <i class="fas fa-pencil"></i>
                                                    </a>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Name">
                                                <ItemTemplate>
                                                    <a target="_blank" runat="server" href='<%# GetFileUrl((Guid)Eval("ContainerIdentifier"), (string)Eval("Name")) %>'><%# Eval("Title") %></a>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Achievements">
                                                <ItemTemplate>
                                                    <asp:Repeater ID="Achievements" runat="server">
                                                        <SeparatorTemplate>,<br /></SeparatorTemplate>
                                                        <ItemTemplate><%# Eval("AchievementTitle") %></ItemTemplate>
                                                    </asp:Repeater>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Competencies">
                                                <ItemTemplate>
                                                    <asp:Repeater ID="Competencies" runat="server">
                                                        <SeparatorTemplate>, </SeparatorTemplate>
                                                        <ItemTemplate>
                                                            <asp:Literal ID="Competency" runat="server" /></ItemTemplate>
                                                    </asp:Repeater>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <cmds:IconButton runat="server" IsFontIcon="true" CssClass="trash-alt" ToolTip="Delete File" ConfirmText="Are you sure you want to delete this file?" CommandName="DeleteFile" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </insite:Grid>

                                </asp:PlaceHolder>
                            </ContentTemplate>
                        </insite:UpdatePanel>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" Title="Upload a Link" Icon="far fa-upload" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Upload a Link
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="row">
                            <div class="col-lg-6">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        URL
                                        <insite:RequiredValidator runat="server" ID="NavigationUrlRequired" ControlToValidate="NavigationUrl" FieldName="URL" ValidationGroup="LinkValidation" />
                                        <insite:UrlValidator runat="server" ID="NavigationUrlValid"
                                            ControlToValidate="NavigationUrl"
                                            ErrorMessage="Specified URL is not valid"
                                            ValidationGroup="LinkValidation"
                                            Enabled="false"
                                        />
                                    </label>
                                    <div class="w-75">
                                        <insite:TextBox ID="NavigationUrl" runat="server" MaxLength="500" AllowHtml="true" />
                                    </div>
                                </div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Title
                                    <insite:RequiredValidator ID="UrlTitleRequired" runat="server" ControlToValidate="UrlTitle" FieldName="Title" ValidationGroup="LinkValidation" />
                                    </label>
                                    <div class="w-75">
                                        <insite:TextBox ID="UrlTitle" runat="server" MaxLength="256" />
                                    </div>
                                </div>
                                <div class="form-group mb-3">
                                    <div>
                                        <asp:RadioButtonList ID="UrlOption" runat="server" RepeatDirection="Vertical" CssClass="check-list" Width="">
                                            <asp:ListItem Value="AchievementOnly" Text="Attach URL to the achievement only" Selected="True" />
                                            <asp:ListItem Value="Both" Text="Attach URL to the achievement and to the competency" />
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Type
                                        <insite:RequiredValidator runat="server" ControlToValidate="UrlAchievementType" FieldName="Type" ValidationGroup="LinkValidation" />
                                    </label>
                                    <div class="w-75">
                                        <cmds:AchievementTypeSelector ID="UrlAchievementType" runat="server" ClientEvents-OnChange="enableUploadAchievementLink" NullText="" />
                                    </div>
                                    <div class="form-text"></div>
                                </div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Category
                                    </label>
                                    <insite:MultiField runat="server" ID="UrlAchievementCategoryField">

                                        <insite:MultiFieldView runat="server" ID="UrlAchievementCategorySelectorView" Inputs="UrlAchievementCategorySelector">
                                            <span class="multi-field-input w-75">
                                                <cmds:TrainingCategorySelector ID="UrlAchievementCategorySelector" runat="server" />
                                            </span>
                                            <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                ButtonStyle="Default" Icon="far fa-keyboard" ToolTip="Enter a value manually" />
                                        </insite:MultiFieldView>

                                        <insite:MultiFieldView runat="server" ID="UrlAchievementCategoryTextView" Inputs="UrlAchievementCategoryText">
                                            <span class="multi-field-input w-75">
                                                <insite:TextBox runat="server" ID="UrlAchievementCategoryText" />
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
                                        <insite:RequiredValidator ID="UrlAchievementSelectorRequired" runat="server" ControlToValidate="UrlAchievementSelector" FieldName="Achievement" ValidationGroup="LinkValidation" />
                                        <insite:RequiredValidator ID="UrlAchievementTextRequired" runat="server" ControlToValidate="UrlAchievementText" FieldName="Achievement" ValidationGroup="LinkValidation" />
                                    </label>
                                    <insite:MultiField runat="server" ID="UrlAchievementField">

                                        <insite:MultiFieldView runat="server" ID="UrlAchievementSelectorView" Inputs="UrlAchievementSelector">
                                            <span class="multi-field-input w-75">
                                                <cmds:FindAchievement ID="UrlAchievementSelector" runat="server" />
                                            </span>
                                            <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                ButtonStyle="Default" Icon="far fa-keyboard" ToolTip="Enter a value manually" />
                                        </insite:MultiFieldView>

                                        <insite:MultiFieldView runat="server" ID="UrlAchievementTextView" Inputs="UrlAchievementText">
                                            <span class="multi-field-input w-75">
                                                <insite:TextBox runat="server" ID="UrlAchievementText" MaxLength="100" />
                                            </span>
                                            <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                ButtonStyle="Default" Icon="far fa-list-ul" ToolTip="Select an option from the list" />
                                        </insite:MultiFieldView>

                                    </insite:MultiField>
                                    <div class="form-text"></div>
                                </div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Competency
                                    </label>
                                    <div runat="server" id="UrlCompetencyCell">
                                        <uc:UploadCompetencyChooser ID="UrlCompetency" runat="server" />
                                    </div>
                                    <div class="form-text"></div>
                                </div>
                                <div class="form-group mb-3">
                                    <div>
                                        <div class="mb-3">
                                            <insite:SaveButton ID="SubmitLinkButton" runat="server" Text="Submit" ValidationGroup="LinkValidation" OnClientClick="return !this.getAttribute('disabled');" />
                                            <insite:CancelButton runat="server" ID="HomeButton2" />
                                        </div>
                                        <insite:ValidationSummary runat="server" ValidationGroup="LinkValidation" />
                                        <insite:ValidationSummary runat="server" ValidationGroup="FileValidation" />
                                    </div>
                                    <div class="form-text"></div>
                                </div>
                            </div>
                        </div>
                
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" Title="Uploaded Links" Icon="far fa-link" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Uploaded Links
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <p runat="server" id="NoUploadedUrlsPanel">No Uploaded Links</p>

                        <asp:Panel ID="UploadedUrlsPanel" runat="server">

                            <insite:Grid runat="server" ID="UrlDownloads" DataKeyNames="UploadIdentifier">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <a href='/ui/cmds/design/uploads/edit?id=<%# Eval("UploadIdentifier") %>&achievement-uploader=yes'>
                                                <i class="fas fa-pencil"></i>
                                            </a>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Name">
                                        <ItemTemplate>
                                            <a target="_blank" runat="server" href='<%# Eval("Name") %>'><%# Eval("Title") %></a>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Achievements">
                                        <ItemTemplate>
                                            <asp:Repeater ID="Achievements" runat="server">
                                                <SeparatorTemplate>,<br />
                                                </SeparatorTemplate>
                                                <ItemTemplate><%# Eval("AchievementTitle") %></ItemTemplate>
                                            </asp:Repeater>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Competencies">
                                        <ItemTemplate>
                                            <asp:Repeater ID="Competencies" runat="server">
                                                <SeparatorTemplate>, </SeparatorTemplate>
                                                <ItemTemplate>
                                                    <asp:Literal ID="Competency" runat="server" /></ItemTemplate>
                                            </asp:Repeater>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <cmds:IconButton runat="server" IsFontIcon="true" CssClass="trash-alt" ToolTip="Delete File" ConfirmText="Are you sure you want to delete this file?" CommandName="DeleteFile" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </insite:Grid>

                        </asp:Panel>
                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            function disableUploadAchievementFile() {
                $get("<%= OverwriteFile.ClientID %>").disabled = true;

            disableMultiField($('#<%= FileAchievementCategoryField.ClientID %>'), true);
            disableFileAchievementField(true);
            disableCompetencyChooser($get("<%= FileCompetencyCell.ClientID %>"));

            $get("<%= SubmitFileButton.ClientID %>").setAttribute('disabled', 'disabled');

            ValidatorEnable($get("<%= FileUploadRequired.ClientID %>"), false);
            }

            function enableUploadAchievementFile() {
                $get("<%= OverwriteFile.ClientID %>").disabled = false;

            disableMultiField($('#<%= FileAchievementCategoryField.ClientID %>'), false);
            disableFileAchievementField(false);
            enableCompetencyChooser($get("<%= FileCompetencyCell.ClientID %>"));

            $get("<%= SubmitFileButton.ClientID %>").removeAttribute('disabled');

            ValidatorEnable($get("<%= FileUploadRequired.ClientID %>"), true);
            $get("<%= FileUploadRequired.ClientID %>").style.visibility = "hidden";
            }

            function disableUploadAchievementLink() {
                $get("<%= NavigationUrl.ClientID %>").setAttribute('disabled', 'disabled');
            $get("<%= UrlTitle.ClientID %>").setAttribute('disabled', 'disabled');

            disableMultiField($('#<%= UrlAchievementCategoryField.ClientID %>'), true);
            disableUrlAchievementField(true);
            disableCompetencyChooser($get("<%= UrlCompetencyCell.ClientID %>"));

            $get("<%= SubmitLinkButton.ClientID %>").setAttribute('disabled', 'disabled');

            ValidatorEnable($get("<%= NavigationUrlRequired.ClientID %>"), false);
            ValidatorEnable($get("<%= NavigationUrlValid.ClientID %>"), false);
            ValidatorEnable($get("<%= UrlTitleRequired.ClientID %>"), false);
            }

            function enableUploadAchievementLink() {
                $get("<%= NavigationUrl.ClientID %>").removeAttribute('disabled');
            $get("<%= UrlTitle.ClientID %>").removeAttribute('disabled');

            disableMultiField($('#<%= UrlAchievementCategoryField.ClientID %>'), false);
            disableUrlAchievementField(false);
            enableCompetencyChooser($get("<%= UrlCompetencyCell.ClientID %>"));

            $get("<%= SubmitLinkButton.ClientID %>").removeAttribute('disabled');

            ValidatorEnable($get("<%= NavigationUrlRequired.ClientID %>"), true);
            $get("<%= NavigationUrlRequired.ClientID %>").style.visibility = "hidden";

            ValidatorEnable($get("<%= NavigationUrlValid.ClientID %>"), true);
            $get("<%= NavigationUrlValid.ClientID %>").style.visibility = "hidden";

            ValidatorEnable($get("<%= UrlTitleRequired.ClientID %>"), true);
            $get("<%= UrlTitleRequired.ClientID %>").style.visibility = "hidden";
            }

            function disableMultiField($field, disable) {
                $field.find('select').prop('disabled', disable).selectpicker('refresh');
                $field.find('input[type="text"]').prop('disabled', disable);
                var $anchors = $field.find('a').prop('disabled', disable);

                if (disable)
                    $anchors.addClass('disabled');
                else
                    $anchors.removeClass('disabled');
            }

            function disableUrlAchievementField(disable) {
                disableAchievementField(
                '<%= UrlAchievementField.ClientID %>',
                '<%= UrlAchievementSelector.ClientID %>',
                '<%= UrlAchievementSelector.ClientID %>',
                ['<%= UrlAchievementSelectorRequired.ClientID %>', '<%= UrlAchievementTextRequired.ClientID %>'],
                disable);
            }

            function disableFileAchievementField(disable) {
                disableAchievementField(
                '<%= FileAchievementField.ClientID %>',
                '<%= FileAchievementSelector.ClientID %>',
                '<%= FileAchievementSelector.ClientID %>',
                ['<%= FileAchievementSelectorRequired.ClientID %>', '<%= FileAchievementTextRequired.ClientID %>'],
                disable);
            }

            function disableAchievementField(fieldId, selectorId, textId, validators, disable) {
                for (var i = 0; i < validators.length; i++) {
                    var validator = document.getElementById(validators[i]);
                    ValidatorEnable(validator, !disable);
                    validator.style.visibility = "hidden";
                }

                var $field = $(document.getElementById(fieldId));
                var $anchors = $field.find('a').prop('disabled', disable);
                var selector = document.getElementById(selectorId);
                var text = document.getElementById(textId);

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
        </script>
    </insite:PageFooterContent>
</asp:Content>
