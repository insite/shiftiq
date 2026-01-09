<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Assessments.Banks.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="ConfigurationDetails" Src="../../Specifications/Controls/ConfigurationDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="CalculationDetails" Src="../../Specifications/Controls/ScoreCalculationDetails.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<insite:Alert runat="server" ID="CreatorStatus"/>
<insite:ValidationSummary runat="server" ValidationGroup="Assessment"/>

<section class="mb-3">

<h2 class="h4 mb-3">
    <i class="far fa-box-open"></i>
    Collection
</h2>
<div class="row mb-3">
    <div class="col-lg-6">
        <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Bank"/>
    </div>
</div>

<asp:MultiView runat="server" ID="MultiView">

<asp:View runat="server" ID="OneView">

    <div class="row">
        <div class="col-md-6">

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Bank Type
                        </label>
                        <div>
                            <insite:ComboBox runat="server" ID="BankTypeSelector">
                                <Items>
                                    <insite:ComboBoxOption Text="Basic" Value="Basic" Enabled="false"/>
                                    <insite:ComboBoxOption Text="Advanced" Value="Advanced" Selected="true"/>
                                </Items>
                            </insite:ComboBox>
                        </div>
                        <div class="form-text">
                            Select <i>Advanced</i> to enable advanced features on the new assessment bank.
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Bank Title
                            <insite:RequiredValidator runat="server" ControlToValidate="BankTitle" ValidationGroup="Assessment"/>
                        </label>
                        <div>
                            <insite:TextBox ID="BankTitle" runat="server"/>
                        </div>
                        <div class="form-text">
                            A descriptive user-friendly title for the bank.
                        </div>
                    </div>

                    <div runat="server" id="BankNameField" class="form-group mb-3">
                        <label class="form-label">
                            Bank Name
                            <insite:RequiredValidator runat="server" ControlToValidate="BankName" ValidationGroup="Assessment"/>
                        </label>
                        <div>
                            <insite:TextBox runat="server" ID="BankName" ValidationGroup="Assessment"/>
                        </div>
                        <div class="form-text">
                            A short name that identifies the bank internally for filing purposes.
                        </div>
                    </div>

                </div>
            </div>

        </div>


        <div class="col-md-6" runat="server" id="AdvancedSettingsColumn">

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Level Type and Number
                        </label>
                        <div class="row">
                            <div class="col-md-6">
                                <insite:ComboBox runat="server" ID="LevelType">
                                    <Items>
                                        <insite:ComboBoxOption/>
                                        <insite:ComboBoxOption Text="Certificate of Qualification" Value="CofQ"/>
                                        <insite:ComboBoxOption Text="Endorsement Exam" Value="EE"/>
                                        <insite:ComboBoxOption Text="Foundation Exam" Value="FE"/>
                                        <insite:ComboBoxOption Text="Interprovincial Standard Exam" Value="IPSE"/>
                                        <insite:ComboBoxOption Text="Standard Level Exam" Value="SLE"/>
                                    </Items>
                                </insite:ComboBox>
                            </div>
                            <div class="col-md-6">
                                <insite:ComboBox runat="server" ID="LevelNumber">
                                    <Items>
                                        <insite:ComboBoxOption/>
                                        <insite:ComboBoxOption Text="Level 1" Value="1"/>
                                        <insite:ComboBoxOption Text="Level 2" Value="2"/>
                                        <insite:ComboBoxOption Text="Level 3" Value="3"/>
                                        <insite:ComboBoxOption Text="Level 4" Value="4"/>
                                    </Items>
                                </insite:ComboBox>
                            </div>
                        </div>
                        <div class="form-text">
                            The type and rank for a discrete level of skill, which the questions in this bank are intended to evaluate.
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Edition
                            <insite:RequiredValidator runat="server" ControlToValidate="EditionMajor" FieldName="Major Version" ValidationGroup="Assessment"/>
                            <insite:RequiredValidator runat="server" ControlToValidate="EditionMinor" FieldName="Minor Version" ValidationGroup="Assessment"/>
                        </label>
                        <div class="row">
                            <div class="col-md-6">
                                <insite:TextBox runat="server" ID="EditionMajor" ValidationGroup="Assessment" Text="1"/>
                            </div>
                            <div class="col-md-6">
                                <insite:TextBox runat="server" ID="EditionMinor" ValidationGroup="Assessment" Text="0"/>
                            </div>
                        </div>
                        <div class="form-text">
                            The edition of this bank (e.g. Year and Month).
                        </div>
                    </div>

                </div>
            </div>

        </div>

    </div>


</asp:View>

<asp:View runat="server" ID="UploadView">
    <div class="row settings">

        <div class="col-md-6">

            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">

                    <div class="form-group mb-3">
                        <label class="form-label">
                            File Type
                        </label>
                        <div>
                            <insite:ComboBox runat="server" ID="UploadFileFormat" CssClass="w-25" AllowBlank="false"/>
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            <asp:Literal ID="JsonFileUploadLabel" runat="server"></asp:Literal>
                        </label>
                        <div>
                            <insite:FileUploadV1 runat="server" ID="JsonFileUpload" LabelText="" AllowedExtensions=".json" FileUploadType="Unlimited"/>
                        </div>
                    </div>

                </div>
            </div>
        </div>

        <div class="col-md-6">
            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">

                    <div runat="server" id="UploadSaveJsonPanel" class="form-group mb-3">
                        <label class="form-label">
                            Uploaded JSON
                            <insite:RequiredValidator runat="server" ControlToValidate="JsonInput" FieldName="Uploaded JSON" Display="Dynamic" ValidationGroup="Assessment"/>
                        </label>
                        <div>
                            <insite:TextBox runat="server" ID="JsonInput" TextMode="MultiLine" Rows="15"/>
                        </div>
                    </div>

                    <div runat="server" id="Div2" class="form-group mb-3">
                        <label class="form-label">
                            Standard Hooks
                        </label>
                        <div>
                            <insite:TextBox runat="server" ID="StandardHooks" TextMode="MultiLine" Rows="5" />
                        </div>
                    </div>

                    <div runat="server" ID="UploadSaveQtiPanel" Visible="false">
                        <h3>Identification</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                External Name
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="UploadSaveQtiExternalName"/>
                            </div>
                            <div class="form-text">The descriptive title for this question bank.</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Internal Name
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="UploadSaveQtiInternalName"/>
                            </div>
                            <div class="form-text">The name that uniquely identifies this bank for internal filing purposes.</div>
                        </div>

                        <h3>Measurements</h3>

                        <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                            <tbody>
                            <tr>
                                <td>
                                    Bank Size
                                </td>
                                <td>
                                    ~
                                    <asp:Literal runat="server" ID="UploadSaveQtiBankSize"/>
                                    <insite:IconButton Name="download" runat="server" ID="DownloadQtiBankDefinition" Style="padding: 8px" ToolTip="Download JSON"/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Question Sets
                                </td>
                                <td>
                                    <asp:Literal runat="server" ID="UploadSaveQtiSetCount"/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Question Items
                                </td>
                                <td>
                                    <asp:Literal runat="server" ID="UploadSaveQtiQuestionCount"/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Question Options
                                </td>
                                <td>
                                    <asp:Literal runat="server" ID="UploadSaveQtiOptionCount"/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Specifications
                                </td>
                                <td>
                                    <asp:Literal runat="server" ID="UploadSaveQtiSpecificationCount"/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Forms
                                </td>
                                <td>
                                    <asp:Literal runat="server" ID="UploadSaveQtiFormCount"/>
                                </td>
                            </tr>
                            </tbody>
                        </table>
                        <div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:View>

<asp:View runat="server" ID="CopyView">
    <div class="row">

        <div class="col-md-6">

            <div class="card border-0 shadow-lg">
                <div class="card-body">


                    <div class="form-group mb-3">
                        <label class="form-label">
                            Bank
                        </label>
                        <div>
                            <insite:FindBank runat="server" ID="BankSelector"/>
                        </div>
                    </div>
                    <div class="form-group mb-3">
                        <label class="form-label">
                            Bank Type
                        </label>
                        <div>
                            <insite:ComboBox runat="server" ID="DuplicateBankTypeSelector">
                                <Items>
                                    <insite:ComboBoxOption Text="Basic" Value="Basic" Enabled="false"/>
                                    <insite:ComboBoxOption Text="Advanced" Value="Advanced" Selected="true"/>
                                </Items>
                            </insite:ComboBox>
                        </div>
                        <div class="form-text">
                            Select <i>Advanced</i> to enable advanced features on the new assessment bank.
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Bank Title
                            <insite:RequiredValidator runat="server" ControlToValidate="DuplicateBankTitle" ValidationGroup="Assessment"/>
                        </label>
                        <div>
                            <insite:TextBox ID="DuplicateBankTitle" runat="server"/>
                        </div>
                        <div class="form-text">
                            A descriptive user-friendly title for the bank.
                        </div>
                    </div>

                    <div runat="server" id="Div1" class="form-group mb-3">
                        <label class="form-label">
                            Bank Name
                            <insite:RequiredValidator runat="server" ControlToValidate="DuplicateBankName" ValidationGroup="Assessment"/>
                        </label>
                        <div>
                            <insite:TextBox runat="server" ID="DuplicateBankName" ValidationGroup="Assessment"/>
                        </div>
                        <div class="form-text">
                            A short name that identifies the bank internally for filing purposes.
                        </div>
                    </div>


                </div>

            </div>

        </div>

        <div runat="server" id="DuplicateAdvancedSettingsColumn" class="col-md-6">

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Level Type and Number
                        </label>
                        <div class="row">
                            <div class="col-md-6">
                                <insite:ComboBox runat="server" ID="DuplicateLevelType">
                                    <Items>
                                        <insite:ComboBoxOption/>
                                        <insite:ComboBoxOption Text="Certificate of Qualification" Value="CofQ"/>
                                        <insite:ComboBoxOption Text="Endorsement Exam" Value="EE"/>
                                        <insite:ComboBoxOption Text="Foundation Exam" Value="FE"/>
                                        <insite:ComboBoxOption Text="Interprovincial Standard Exam" Value="IPSE"/>
                                        <insite:ComboBoxOption Text="Standard Level Exam" Value="SLE"/>
                                    </Items>
                                </insite:ComboBox>
                            </div>
                            <div class="col-md-6">
                                <insite:ComboBox runat="server" ID="DuplicateLevelNumber">
                                    <Items>
                                        <insite:ComboBoxOption/>
                                        <insite:ComboBoxOption Text="Level 1" Value="1"/>
                                        <insite:ComboBoxOption Text="Level 2" Value="2"/>
                                        <insite:ComboBoxOption Text="Level 3" Value="3"/>
                                        <insite:ComboBoxOption Text="Level 4" Value="4"/>
                                    </Items>
                                </insite:ComboBox>
                            </div>
                        </div>
                        <div class="form-text">
                            The type and rank for a discrete level of skill, which the questions in this bank are intended to evaluate.
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Edition
                            <insite:RequiredValidator runat="server" ControlToValidate="DuplicateMajorVersion" FieldName="Major Version" ValidationGroup="Assessment"/>
                            <insite:RequiredValidator runat="server" ControlToValidate="DuplicateMinorVersion" FieldName="Minor Version" ValidationGroup="Assessment"/>
                        </label>
                        <div class="row">
                            <div class="col-md-6">
                                <insite:TextBox runat="server" ID="DuplicateMajorVersion" ValidationGroup="Assessment" Text="1"/>
                            </div>
                            <div class="col-md-6">
                                <insite:TextBox runat="server" ID="DuplicateMinorVersion" ValidationGroup="Assessment" Text="0"/>
                            </div>
                        </div>
                        <div class="form-text">
                            A major revision indicates relatively high-impact changes that are not backward-compatible with the preceding version.
                            A minor revision indicates relatively low-impact changes that are typically backward-compatible with the preceding version.
                            Refer to <a href="https://semver.org" target="_blank">semver.org</a> for details on semantic versioning.
                        </div>
                    </div>


                </div>

            </div>

        </div>

    </div>
</asp:View>

</asp:MultiView>

</section>

<div class="row">
    <div class="col-lg-12">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment"/>
        <insite:CancelButton runat="server" ID="CancelButton"/>
    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

            (function () {
                $('[data-upload]').each(function () {
                    var $btn = $(this);
                    var uploadSelector = $btn.data('upload');
                    $(uploadSelector).on('change', function () {
                        var fileName = '';

                        if (this.files) {
                            if (this.files.length > 0) {
                                fileName = this.files[0].name;
                            }
                        } else if (this.value) {
                            fileName = this.value.split(/(\\|\/)/g).pop();
                        }

                        $btn.closest('.input-group').find('input[type="text"]').val(fileName);
                    });
                }).on('click', function () {
                    var uploadSelector = $(this).data('upload');
                    $(uploadSelector).click();
                });
            })();

            (function () {
                var bankCreator = window.bankCreator = window.bankCreator || {};

                bankCreator.ValidateJsonFileUpload = function (s, e) {
                    if (!e.Value)
                        return;

                    var ext = '';
                    var index = e.Value.lastIndexOf('.');
                    if (index > 0)
                        ext = e.Value.substring(index).toLowerCase();

                    e.IsValid = ext === '.json';
                };
                bankCreator.ValidateQtiFileUpload = function (s, e) {
                    if (!e.Value)
                        return;

                    var ext = '';
                    var index = e.Value.lastIndexOf('.');
                    if (index > 0)
                        ext = e.Value.substring(index).toLowerCase();

                    e.IsValid = ext === '.xml';
                };
            })();

        </script>
</insite:PageFooterContent>
</asp:Content>