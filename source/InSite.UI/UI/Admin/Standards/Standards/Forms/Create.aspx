<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Standards.Standards.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/StandardPopupSelector.ascx" TagName="StandardPopupSelector" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="CreatorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Outline" />
    <insite:ValidationSummary runat="server" ValidationGroup="Review" />
    <insite:ValidationSummary runat="server" ValidationGroup="UploadMarkdown" />
    <insite:ValidationSummary runat="server" ValidationGroup="UploadJson" />
    <insite:ValidationSummary runat="server" ValidationGroup="Upload2ValidationGroup" />

    <asp:MultiView runat="server" ID="MainMultiView">

        <asp:View runat="server" ID="ViewStandard">
            <section class="card border-0 shadow-lg">
                <div class="card-body">
                    <h2 class="h4 mb-3">Standard</h2>

                    <div class="row pb-3">
                        <div class="col-md-6">
                            <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Standard" />
                        </div>
                    </div>

                    <asp:MultiView runat="server" ID="StandardMultiView">

                        <asp:View runat="server" ID="ViewStandardOne">

                            <div class="row">
                                <div class="col-md-4">
                                    <div>
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Type
                                                <insite:RequiredValidator runat="server" ControlToValidate="StandardType" FieldName="Type" ValidationGroup="Asset" />
                                            </label>
                                            <div>
                                                <insite:StandardTypeComboBox ID="StandardType" runat="server" Width="100%" />
                                            </div>
                                            <div class="form-text">What type of standard is this?</div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Name
                                                <insite:RequiredValidator runat="server" ControlToValidate="ContentName" FieldName="Name" ValidationGroup="Asset" />
                                            </label>
                                            <div>
                                                <insite:TextBox ID="ContentName" runat="server" MaxLength="256" Width="100%" />
                                            </div>
                                            <div class="form-text">
                                                The internal name used to refer to this particular standard.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">Tag</label>
                                            <div>
                                                <insite:TextBox runat="server" ID="AssetLabel" MaxLength="30" />
                                            </div>
                                            <div class="form-text">
                                                A label allows you to use whatever term is used in your organization to refer to this type of standard.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">Tier</label>
                                            <div>
                                                <insite:TextBox runat="server" ID="AssetTier" MaxLength="30" />
                                            </div>
                                            <div class="form-text">
                                                The tier identifies the hierarchical position above (+) or below (-) the standard that is used for assessments (0).
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Code
                                                <insite:IconButton runat="server" ID="AutoCode" Name="sort-numeric-down" ToolTip="Automatically Code Asset Structure" Visible="false" />
                                            </label>
                                            <div>
                                                <insite:TextBox runat="server" ID="Code" MaxLength="40" />
                                            </div>
                                            <div class="form-text">
                                                An alphanumeric code used to identify the standard in a list.
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                <div class="col-md-4">
                                    <div>

                                        <div class="form-group mb-3" runat="server" id="SingleParentField">
                                            <label class="form-label">
                                                Parent
                                                <insite:IconButton runat="server" ID="ShowSingleParentInfoButton" CssClass="edit ml-1" Visible="false" Name="bars" ToolTip="View Details" />
                                            </label>
                                            <uc:StandardPopupSelector runat="server" ID="SingleParentAssetID" />
                                            <div class="form-text">
                                                Select the container for this standard to place it in a hierarchy.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">Source/Reference</label>
                                            <div>
                                                <insite:TextBox runat="server" ID="SourceDescriptor" TextMode="MultiLine" Rows="6" MaxLength="3400"/>
                                            </div>
                                            <div class="form-text">
                                                The background source or external reference for this standard. 
                                                For example, this might be an industry code for a business activity, or a regulation identification number.
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-md-4">
                                    <div>
                                        <div class="form-group mb-3">
                                            <label class="form-label">Author Name</label>
                                            <div>
                                                <insite:TextBox runat="server" ID="AuthorName" MaxLength="100" />
                                            </div>
                                            <div class="form-text">
                                                Name of the individual who authored this standard.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">Date Authored</label>
                                            <div>
                                                <insite:DateTimeOffsetSelector runat="server" ID="AuthorDate" />
                                            </div>
                                            <div class="form-text">
                                                Date the standard was initially written.
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="mt-3">
                                <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Asset" />
                                <insite:CancelButton runat="server" ID="CancelNewButton" NavigateUrl="/ui/admin/standards/standards/search" CausesValidation="false" />
                            </div>

                        </asp:View>

                        <asp:View runat="server" ID="ViewStandardOutline">

                            <div class="row">
                                <div class="col-md-6">
                                    <div class="mb-0 pb-0" >

                                        <div class="form-group mb-3" runat="server" id="MultipleParentField">
                                            <label class="form-label">
                                                Parent
		                                        <insite:IconButton runat="server" ID="ShowMultipleParentInfoButton" CssClass="edit ml-1" Visible="false" Name="bars" ToolTip="View Details" />
                                            </label>
                                            <uc:StandardPopupSelector runat="server" ID="MultipleParentAssetID" />
                                            <div class="form-text">
                                                Select the container for this standard to place it in a hierarchy.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Tiers
                                                <insite:RequiredValidator runat="server" ControlToValidate="Tiers" Display="Dynamic" FieldName="Tiers" ValidationGroup="Outline" />
                                                <insite:CustomValidator runat="server" ID="TiersValidator"
                                                    ControlToValidate="Tiers" Display="Dynamic" ValidateEmptyText="false" ValidationGroup="Outline"
                                                    ErrorMessage="Tiers field contains invalid standard type"
                                                    ClientValidationFunction="standardCreator.ValidateTiers" />
                                            </label>
                                            <div>
                                                <insite:TextBox runat="server" ID="Tiers" />
                                            </div>
                                            <div class="form-text">
                                                Each level of indentation in your outline is a <b>tier</b>. 
                                                Indicate the type of item at each tier in your outline.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Outline
                                                <insite:RequiredValidator runat="server" ControlToValidate="Outline" Display="Dynamic" FieldName="Outline" ValidationGroup="Outline" />
                                            </label>
                                            <div>
                                                <insite:TextBox runat="server" ID="Outline" TextMode="MultiLine" Rows="20" />
                                            </div>
                                            <div class="form-text">
                                                Enter your outline with one item per line. 
                                                Use the hash tag character for indentation, indicating the tier for each item.
                                                An example is provided for reference.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <insite:NextButton runat="server" ID="OutlineNextButton" ValidationGroup="Outline" />
                                            <insite:CancelButton runat="server" ID="CancelOutlineButton" NavigateUrl="/ui/admin/standards/standards/search" />
                                        </div>

                                    </div>
                                </div>

                            </div>

                        </asp:View>

                        <asp:View runat="server" ID="ViewStandardUpload">
                            <div class="row">
                                <div class="col-md-6">
                                    <div class="mb-0 pb-0">

                                        <div class="card border-0 shadow-lg h-100">
                                            <div class="card-body">

                                                <h3>Outline Template</h3>

                                                <div class="form-group mb-3">
                                                    <label class="form-label">
                                                        File Type
                                                    </label>
                                                    <div>
                                                        <insite:ComboBox runat="server" ID="UploadFileType">
                                                            <Items>
                                                                <insite:ComboBoxOption Text="Markdown" Value="MD" Selected="true" />
                                                                <insite:ComboBoxOption Text="JSON" Value="JSON" />
                                                            </Items>
                                                        </insite:ComboBox>
                                                    </div>
                                                </div>

                                                <insite:Container runat="server" ID="MarkdownUploadContainer" Visible="false">
                                                    <div class="form-group mb-3">
                                                        <label class="form-label">
                                                            Upload Markdown
                                                            <insite:RequiredValidator runat="server" ControlToValidate="MarkdownFileUpload" Display="Dynamic" ValidationGroup="UploadMarkdown" />
                                                            <insite:CustomValidator runat="server" ID="MarkdownFileUploadExtensionValidator"
                                                                ControlToValidate="MarkdownFileUpload" Display="Dynamic" ValidateEmptyText="false" ValidationGroup="UploadMarkdown"
                                                                ErrorMessage="Invalid file type. File types supported: .txt .zip"
                                                                ClientValidationFunction="standardCreator.ValidateMarkdownFileUpload" />
                                                            <insite:CustomValidator runat="server" ID="MarkdownZipFileValidator"
                                                                ControlToValidate="MarkdownFileUpload" Display="Dynamic" ValidateEmptyText="false" ValidationGroup="UploadMarkdown" />
                                                        </label>
                                                        <div>
                                                            <div class="input-group">
                                                                <insite:TextBox runat="server" ReadOnly="true" style="background-color: #fff;" />
                                                                <button class="btn btn-icon btn-outline-secondary" data-upload="#<%= MarkdownFileUpload.ClientID %>" title="Browse" type="button"><i class="far fa-folder-open"></i></button>
                                                                <insite:Button runat="server" ID="MarkdownFileUploadButton" Size="Default"
                                                                    ToolTip="Upload" ButtonStyle="OutlineSecondary" Icon="far fa-upload"
                                                                    CausesValidation="true" ValidationGroup="UploadMarkdown" />
                                                            </div>
                                                            <div class="d-none">
                                                                <asp:FileUpload runat="server" ID="MarkdownFileUpload" />
                                                            </div>
                                                        </div>
                                                        <div class="form-text">
                                                        </div>
                                                    </div>

                                                    <div class="form-group mb-3">
                                                        <label class="form-label">
                                                            Select Example Outline
                                                        </label>
                                                        <div class="pl-1">
                                                            <asp:Repeater runat="server" ID="DefaultOutlineRepeater">
                                                                <ItemTemplate>
                                                                    <div class="mt-1">
                                                                        <asp:Button runat="server" ID="LoadOutline" CssClass="d-none"
                                                                            CommandName="LoadOutline" CommandArgument="<%# Container.ItemIndex %>"
                                                                            OnClientClick="return confirm('Are you sure you want to load the outline example?');" />
                                                                        <asp:Label runat="server" AssociatedControlID="LoadOutline" Style="cursor: pointer;">
                                                                            <span class="btn btn-sm btn-default mr-1"><i class='far fa-folder-open'></i></span>
                                                                            <%# Eval("Name") %>
                                                                        </asp:Label>
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </div>
                                                        <div class="form-text">
                                                        </div>
                                                    </div>
                                                </insite:Container>

                                                <insite:Container runat="server" ID="JsonUploadContainer" Visible="false">
                                                    <div class="form-group mb-3">
                                                        <label class="form-label">
                                                            Upload JSON
                                                            <insite:CustomValidator runat="server" ID="JsonFileUploadExtensionValidator"
                                                                ControlToValidate="JsonFileUpload" Display="Dynamic" ValidateEmptyText="false" ValidationGroup="UploadJson"
                                                                ErrorMessage="Invalid file type. File types supported: .json .zip"
                                                                ClientValidationFunction="standardCreator.ValidateJsonFileUpload" />
                                                            <insite:CustomValidator runat="server" ID="JsonZipFileValidator"
                                                                ControlToValidate="JsonFileUpload" Display="Dynamic" ValidateEmptyText="false" ValidationGroup="UploadJson" />
                                                        </label>
                                                        <div>
                                                            <div class="input-group">
                                                                <insite:TextBox runat="server" ReadOnly="true" style="background-color: #fff;" />
                                                                <button class="btn btn-icon btn-outline-secondary" data-upload="#<%= JsonFileUpload.ClientID %>" title="Browse" type="button"><i class="far fa-folder-open"></i></button>
                                                                <insite:Button runat="server" ID="JsonFileUploadButton" Size="Default"
                                                                    ToolTip="Upload" ButtonStyle="OutlineSecondary" Icon="far fa-upload"
                                                                    CausesValidation="true" ValidationGroup="UploadJson" />
                                                            </div>
                                                            <div class="d-none">
                                                                <asp:FileUpload runat="server" ID="JsonFileUpload" />
                                                            </div>
                                                        </div>
                                                        <div class="form-text">
                                                        </div>
                                                    </div>
                                                </insite:Container>
                                
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                <div class="col-md-6">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Uploaded File Content
                                            <insite:RequiredValidator runat="server" ControlToValidate="JsonInput" FieldName="Uploaded File Content" Display="Dynamic" ValidationGroup="Upload2ValidationGroup" />
                                        </label>
                                        <div>
                                            <insite:TextBox runat="server" ID="JsonInput" TextMode="MultiLine" Rows="15" />
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div class="mt-3">
                                <insite:NextButton runat="server" ID="UploadMarkdownSaveButton" ValidationGroup="Upload2ValidationGroup" />
                                <insite:NextButton runat="server" ID="UploadJSONSaveButton" ValidationGroup="Upload2ValidationGroup" />
                                <insite:CancelButton runat="server" ID="UploadCancelButton" NavigateUrl="/ui/admin/standards/standards/search" />
                            </div>
                        </asp:View>

                    </asp:MultiView>
                </div>
            </section>
        </asp:View>

        <asp:View runat="server" ID="ViewReview">
            <section class="card border-0 shadow-lg h-100">
                <div class="card-body">
                    <h2 class="h4 mb-3">Review</h2>

                    <asp:Repeater runat="server" ID="TreeViewRepeater">
                        <ItemTemplate>
                            <asp:Literal runat="server" ID="HtmlPrefix" />
                            <div>
                                <div>
                                    <div class="node-title">
                                        <div class="pre-text">
                                            <%# Eval("Icon") == null ? string.Empty : Eval("Icon", "<i class='{0}'></i>") %>
                                            <span runat="server" visible='<%# !string.IsNullOrEmpty((string)Eval("Code")) %>' title='<%# Eval("Code") %>' class="fw-bold"><%# Eval("Code") %>.</span>
                                        </div>
                                        <div class="text">
                                            <insite:TextBox runat="server" ID="TitleInput" Text='<%# Eval("Title") %>' CssClass="form-control-sm" />
                                        </div>
                                        <div runat="server" visible='<%# !string.IsNullOrEmpty((string)Eval("Summary")) %>' class="post-text form-text">
                                            <%# Shift.Common.Markdown.ToHtml((string)Eval("Summary")) %>
                                        </div>
                                    </div>

                                    <div class="node-inputs node-inputs-sm">
                                        <insite:StandardTypeComboBox runat="server" ID="TypeSelector" Width="150px" AllowBlank="false" ButtonSize="Small" />
                                    </div>
                                </div>
                            </div>
                            <asp:Literal runat="server" ID="HtmlPostfix" />
                        </ItemTemplate>
                    </asp:Repeater>

                    <div class="mt-3">
                        <insite:SaveButton runat="server" ID="ReviewSaveButton" ValidationGroup="Review" />
                        <insite:Button runat="server" ID="ReviewPrevButton" Text="Prev" Icon="fas fa-arrow-alt-left" ButtonStyle="Default" />
                        <insite:CancelButton runat="server" ID="CancelReviewButton" NavigateUrl="/ui/admin/standards/standards/search" CausesValidation="false" />
                    </div>
                </div>
            </section>
        </asp:View>
    </asp:MultiView>

    <insite:PageHeadContent runat="server">
        <style type="text/css">
            ul.tree-view > li > div > div .node-title > div.pre-text {
                display: inline-block;
                width: 75px;
                position: absolute;
                line-height: 38px;
                white-space: nowrap;
                overflow: hidden;
                text-overflow: ellipsis;
            }

            ul.tree-view > li > div > div .node-title > div.text {
                padding-left: 75px;
                margin: -6px 0;
                width: calc(100% - 160px);
            }

            ul.tree-view li > div > div > span.toggle-button {
                line-height: 26px;
            }
        </style>
    </insite:PageHeadContent>

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
                var standardCreator = window.standardCreator = window.standardCreator || {};
                var validSubtypes = <%= Shift.Common.JsonHelper.SerializeJsObject(ValidSubtypes.Select(x => x.ToUpper())) %>;

                standardCreator.ValidateTiers = function (s, e) {
                    e.IsValid = true;

                    var tiers = e.Value.split(',');
                    for (var i = 0; i < tiers.length; i++) {
                        var isValid = false;
                        var subtype = tiers[i].trim().toUpperCase();

                        for (var j = 0; j < validSubtypes.length; j++) {
                            if (validSubtypes[j] === subtype) {
                                isValid = true;
                                break;
                            }
                        }

                        if (!isValid) {
                            e.IsValid = false;
                            break;
                        }
                    }
                };

                standardCreator.ValidateMarkdownFileUpload = function (s, e) {
                    if (!e.Value)
                        return;

                    var ext = '';
                    var index = e.Value.lastIndexOf('.');
                    if (index > 0)
                        ext = e.Value.substring(index).toLowerCase();

                    e.IsValid = ext === '.txt' || ext === '.zip';
                };

                standardCreator.ValidateJsonFileUpload = function (s, e) {
                    if (!e.Value)
                        return;

                    var ext = '';
                    var index = e.Value.lastIndexOf('.');
                    if (index > 0)
                        ext = e.Value.substring(index).toLowerCase();

                    e.IsValid = ext === '.json' || ext === '.zip';
                };
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>
