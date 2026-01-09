<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Sites.Pages.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="../../Pages/Controls/PagePopupSelector.ascx" TagName="PagePopupSelector" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Page" />

    <section class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    <i class="far fa-calendar-alt me-1"></i>
                    New Page
                </h4>
                <div class="row mb-3">
                    <div class="col-lg-4">
                        <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Page" />
                    </div>
                </div>

                <asp:MultiView runat="server" ID="MultiView">

                    <asp:View runat="server" ID="OneView">

                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Page Type
                                        <insite:RequiredValidator runat="server" ControlToValidate="SinglePageType" FieldName="Page Type" ValidationGroup="Page" />
                                    </label>
                                    <div>
                                        <insite:WebPageTypeComboBox ID="SinglePageType" runat="server" Width="100%" />
                                    </div>
                                    <div class="form-text">Pages are organized by type.</div>
                                </div>

                                <div runat="server" id="SingleBlockControlField" class="form-group mb-3" visible="false">
                                    <label class="form-label">
                                        Block Control
                                        <insite:RequiredValidator runat="server" ControlToValidate="SingleBlockControl" FieldName="Block Control" ValidationGroup="Page" />
                                    </label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="SingleBlockControl" Width="100%" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Title
                                        <insite:RequiredValidator runat="server" ControlToValidate="SingleTitle" FieldName="Title" ValidationGroup="Page" />
                                    </label>
                                    <div>
                                        <insite:TextBox ID="SingleTitle" runat="server" MaxLength="128" Width="100%" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Page Slug (URL Segment) 
                                    </label>
                                    <div>
                                        <insite:TextBox ID="SingleSlug" runat="server" MaxLength="100" Width="100%" />
                                    </div>
                                    <div class="form-text">
                                        The part of the URL that specifically refers to this page. If this field is left blank, the segment will default to match the Title above.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Visible Tabs</label>
                                    <div>
                                        <insite:TextBox runat="server" ID="ContentLabels" MaxLength="100" Text="PageBlocks, Body, Title, Summary, ImageURL" />
                                    </div>
                                    <div class="form-text">
                                        Controls which tabs appear on the Content Editor screen. 
                                    </div>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Portal or Web Site
                                        <insite:RequiredValidator runat="server" ControlToValidate="SingleWebSiteSelector" FieldName="Portal or Web Site" ValidationGroup="Page" />
                                    </label>
                                    <insite:QSiteComboBox runat="server" ID="SingleWebSiteSelector" AllowBlank="true" />
                                    <div class="form-text">
                                        Which portal or web site should contain this page.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Parent/Container</label>
                                    <uc:PagePopupSelector runat="server" ID="SingleParentPageId" />
                                    <div class="form-text">
                                        Select the container for this page to place it in a hierarchy.
                                    </div>
                                </div>
                            </div>

                        </div>

                    </asp:View>

                    <asp:View runat="server" ID="UploadView">

                        <div class="row">
                            <div class="col-lg-4">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        File Type
                                    </label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="UploadFileType" Width="100%">
                                            <Items>
                                                <insite:ComboBoxOption Text="Markdown" Value="MD" />
                                                <insite:ComboBoxOption Text="JSON" Value="JSON" Selected="true" />
                                            </Items>
                                        </insite:ComboBox>
                                    </div>
                                </div>

                                <insite:FileUploadV1 runat="server" 
                                    ID="CreateUploadFile" 
                                    LabelText="Select and Upload JSON File" 
                                    FileUploadType="Unlimited"
                                    OnClientFileUploaded="pageCreate.onFileUploaded" />
                                <asp:Button runat="server" ID="UploadFileUploaded" CssClass="d-none" />
                            </div>

                            <div class="col-lg-8">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Uploaded JSON
                                        <insite:RequiredValidator runat="server" ControlToValidate="UploadJsonInput" FieldName="Uploaded JSON" Display="Dynamic" ValidationGroup="Page" />
                                    </label>
                                    <insite:TextBox runat="server" ID="UploadJsonInput" TextMode="MultiLine" Rows="15" />
                                    <div class="form-text">
                                    </div>
                                </div>

                            </div>
                        </div>

                    </asp:View>

                    <asp:View runat="server" ID="MultipleView">
                        <div class="row">
                            <div class="col-md-6">
                                
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Portal or Web Site
                                        <insite:RequiredValidator runat="server" ControlToValidate="MultipleWebSiteSelector" FieldName="Portal or Web Site" ValidationGroup="Page" />
                                    </label>
                                    <insite:WebSiteComboBox runat="server" ID="MultipleWebSiteSelector" AllowBlank="true" />
                                    <div class="form-text">
                                        Which portal or web site should contain this page.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Parent/Container</label>
                                    <uc:PagePopupSelector runat="server" ID="MultipleParentPageId" />
                                    <div class="form-text">
                                        Select the container for this page to place it in a hierarchy.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Outline
                                        <insite:RequiredValidator runat="server" ControlToValidate="Outline" Display="Dynamic" FieldName="Outline" ValidationGroup="Page" />
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

                            </div>
                        </div>
                    </asp:View>

                    <asp:View runat="server" ID="CopyView">

                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Portal or Web Site
                                        <insite:RequiredValidator runat="server" ControlToValidate="CopyWebSiteSelector" FieldName="Portal or Web Site" ValidationGroup="Page" />
                                    </label>
                                    <insite:WebSiteComboBox runat="server" ID="CopyWebSiteSelector" AllowBlank="true" />
                                    <div class="form-text">
                                        Which portal or web site should contain this page.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Web Page
                                        <insite:CustomValidator runat="server" ID="PageDuplicateValidator" ValidationGroup="Page" Display="None" />
                                    </label>
                                    <uc:PagePopupSelector runat="server" ID="CopyWebPageId" />
                                    <div class="form-text">
                                        Select an existing web page that you want to use as a template for the new web page.
                                    </div>
                                </div>
                            </div>
                        </div>

                    </asp:View>

                </asp:MultiView>
            </div>
        </div>
    </section>

    <section class="mb-3" runat="server" visible="false" id="PreviewSection">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    <i class="far fa-calendar-alt me-1"></i>
                    Review
                </h4>

                <asp:Repeater runat="server" ID="TreeViewRepeater">
                    <ItemTemplate>
                        <%# Eval("HtmlPrefix") %>
                        <div>
                            <div>
                                <div class="node-title">
                                    <span class='text'>
                                        <%# Eval("Icon") == null ? string.Empty : Eval("Icon", "<i class='align-middle {0}'></i>") %>
                                        <span><%# Eval("Title") %></span>
                                    </span>
                                </div>
                                <div class="node-inputs node-inputs-sm">
                                    <insite:WebPageTypeComboBox runat="server" ID="TypeSelector" Width="150px" AllowBlank="false" ButtonSize="Small" />
                                </div>
                            </div>
                        </div>
                        <%# Eval("HtmlPostfix") %>
                    </ItemTemplate>
                </asp:Repeater>

                <div class="form-group mb-3" runat="server" id="PreviewContentLabelsField" visible="false">
                    <label class="form-label">Content Tags</label>
                    <div>
                        <insite:TextBox runat="server" ID="PreviewContentLabels" MaxLength="100" Text="PageBlocks, Body, Title, Summary" />
                    </div>
                </div>

            </div>
        </div>
    </section>

    <section>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Page" />
        <insite:NextButton runat="server" ID="NextButton" ValidationGroup="Page" Visible="false" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </section>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        (function () {
            var pageCreate = window.pageCreate = window.pageCreate || {};

            pageCreate.onFileUploaded = function () {
                __doPostBack('<%= UploadFileUploaded.UniqueID %>', '')
            };
        })();

    </script>
</insite:PageFooterContent>
</asp:Content>
