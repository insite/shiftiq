<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Content.aspx.cs" Inherits="InSite.UI.Admin.Messages.Messages.Forms.Content" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="ValidPlaceHolderNames" Src="../../Outlines/Controls/ValidPlaceHolderNames.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <insite:ResourceLink runat="server" Type="Css" Url="/UI/Admin/messages/messages/content/styles/content.css" />
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <div id="autosave-indicator"><i class="far fa-save"></i></div>

    <insite:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <insite:Alert runat="server" ID="Status" />
        </ContentTemplate>
    </insite:UpdatePanel>
    <insite:ValidationSummary runat="server" ValidationGroup="Document" />

    <insite:Alert runat="server" ID="ConfirmAlert" Indicator="Warning" Visible="false">
        This message has one or more scheduled mailouts that are still pending. 
        Any changes made now will not be applied to these scheduled mailouts. To include your 
        edits, please cancel all pending mailouts and reschedule them after completing your changes.
    </insite:Alert>

    <section class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row">
                    <div class="col">
                        <h4 class="card-title mb-3">
                            <i class="far fa-edit me-1"></i>
                            Content
                        </h4>
                    </div>
                    <div class="col text-end">
                        <insite:Button runat="server" ID="DuplicateButton" ButtonStyle="Default" Text="Duplicate to New Message" Icon="fas fa-copy" CssClass="mb-3" />
                    </div>
                </div>

                <div class="row d-none">
                    <div class="col-md-3">
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Document Style
                                <insite:RequiredValidator runat="server" ControlToValidate="DocumentStyle" Display="Dynamic" FieldName="Document Style" ValidationGroup="Document" />
                            </label>
                            <insite:ComboBox runat="server" ID="DocumentStyle" />
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Document Type
                                <insite:RequiredValidator runat="server" ControlToValidate="DocumentType" Display="Dynamic" FieldName="Document Type" ValidationGroup="Document" />
                            </label>
                            <insite:ComboBox runat="server" ID="DocumentType">
                                <Items>
                                    <insite:ComboBoxOption Value="Email" Text="Email Message" />
                                    <insite:ComboBoxOption Value="Web" Text="Web Page" />
                                </Items>
                            </insite:ComboBox>
                        </div>
                    </div>
                </div>

                <div>
                    <insite:MarkdownEditor runat="server" ID="ContentInput"
                        UploadControl="ContentUpload" 
                        TranslationControl="ContentTranslation"
                        ClientEvents-OnSetup="messageContent.onMdeSetup"
                        ClientEvents-OnInited="messageContent.onMdeInited"
                        ClientEvents-OnPreview="messageContent.onMdePreview" />
                    <div class="mt-1">
                        <insite:EditorTranslation runat="server" ID="ContentTranslation" TableContainerID="TranslationContainer" EnableMarkdownConverter="false" />
                    </div>
                    <insite:EditorUpload runat="server" ID="ContentUpload" />
                    <div runat="server" id="TranslationContainer"></div>
                </div>

                <uc:ValidPlaceHolderNames runat="server" ID="ValidPlaceHolderNames" />

                <div class="form-text">
                    <a runat="server" id="MessageEditorHelpUrl" href="#"><i class="far fa-arrow-up-right-from-square me-1"></i>Click here</a> for more information on how to use the markdown editor toolbar and additional features.
                </div>

            </div>
        </div>
    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Document" />
        <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
        <insite:DropDownButton runat="server" ID="DownloadButton" IconName="download" Text="Download" CssClass="d-inline-block">
            <Items>
                <insite:DropDownButtonItem Name="html" IconName="file-code" Text="HTML (*.html)" />
                <insite:DropDownButtonItem Name="md" IconType="Brand" IconName="markdown" Text="Markdown (*.md)" />
            </Items>
        </insite:DropDownButton>
    </div>

    <insite:UpdatePanel runat="server" ID="AutosavePanel"
        ClientEvents-OnRequestStart="messageContent.onAutoSaveStart"
        ClientEvents-OnResponseEnd="messageContent.onAutoSaveEnd" />

    <insite:ResourceBundle runat="server" Type="JavaScript">
        <Items>
            <insite:ResourceBundleFile Url="/UI/Layout/common/parts/plugins/showdown/showdown.js" />
            <insite:ResourceBundleFile Url="/UI/Admin/messages/messages/content/scripts/markdown.js" />
            <insite:ResourceBundleMethod Type="InSite.UI.Admin.Messages.Messages.Forms.Content, InSite.UI" Name="SetupTemplates" />
            <insite:ResourceBundleFile Url="/UI/Admin/messages/messages/content/scripts/content.js" />
        </Items>
    </insite:ResourceBundle>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                messageContent.init({
                    inputId: '<%= ContentInput.ClientID %>',
                    autoSaveId: '<%= AutosavePanel.ClientID %>',
                    docStyleId: '<%= DocumentStyle.ClientID %>',
                    docTypeId: '<%= DocumentType.ClientID %>'
                });
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>