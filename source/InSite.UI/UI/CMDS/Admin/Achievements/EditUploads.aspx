<%@ Page Language="C#" CodeBehind="EditUploads.aspx.cs" Inherits="InSite.Cmds.Admin.Achievements.Forms.EditUploads" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:PageHeadContent runat="server">
        <style type="text/css">
            .attachment-container td {
                vertical-align: top;
            }
        </style>
    </insite:PageHeadContent>

    <insite:Alert runat="server" ID="ScreenStatus" />

    <section runat="server" ID="AttachmentSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-paperclip me-1"></i>
            Attachments
        </h2>

        <div class="row">
            <div class="col-lg-6">
                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
                        <insite:UpdatePanel runat="server" ID="UpdatePanel">
                            <ContentTemplate>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Upload Type
                                    </label>
                                    <div>
                                        <insite:RadioButton runat="server" ID="NewFile" Text="File" GroupName="UploadType" Checked="true" />
                                        <insite:RadioButton runat="server" ID="NewLink" Text="Link" GroupName="UploadType" />
                                    </div>
                                </div>

                                <div runat="server" id="FileField" class="mb-3 w-75">
                                    <insite:FileUploadV1 runat="server" ID="FileUpload" AllowMultiple="true" FileUploadType="Unlimited" />
                                </div>

                                <div runat="server" id="LinkField1" class="mb-3">
                                    <insite:TextBox ID="NavigationUrl" runat="server" EmptyMessage="URL" MaxLength="500" AllowHtml="true" CssClass="d-inline w-75" />
                                    <insite:RequiredValidator runat="server" ControlToValidate="NavigationUrl" FieldName="URL" ValidationGroup="Upload" />
                                </div>
                                <div runat="server" id="LinkField2" class="mb-3">
                                    <insite:TextBox ID="UrlTitle" runat="server" EmptyMessage="Title" MaxLength="256" CssClass="d-inline w-75" />
                                    <insite:RequiredValidator runat="server" ControlToValidate="UrlTitle" FieldName="Title" ValidationGroup="Upload" />
                                </div>

                            </ContentTemplate>
                        </insite:UpdatePanel>

                        <insite:Button runat="server" ID="UploadButton" Text="Upload" Icon="fas fa-upload"
                            ButtonStyle="Success" ValidationGroup="Upload" />
                    </div>
                </div>
            </div>
            <div class="col-lg-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Attachments <span class="form-text">(<asp:Literal runat="server" ID="AttachmentsCount"></asp:Literal>)</span></h3>

                        <asp:Repeater runat="server" ID="Attachments">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li>
                                    <a target="_blank" runat="server" href='<%# GetUploadUrl(Eval("UploadType"), Eval("ContainerIdentifier"), Eval("Name")) %>'><%# Eval("Title") %></a><br />
                                    Type: <%# (string)Eval("UploadType") == "CMDS File" ? "File" : (string)Eval("UploadType") %>,
                                    <asp:Label runat="server" Visible='<%# Eval("CompanyName") != DBNull.Value %>'>
                                        Organization: <%# Eval("DepartmentName") != DBNull.Value ? string.Format("{0} ({1})", Eval("CompanyName"), Eval("DepartmentName")) : Eval("CompanyName") %>,
                                    </asp:Label>
                                    Description: <%# (string)Eval("UploadType") == UploadType.CmdsFile ? ((int)Eval("ContentSize") / 1024) + " KB" : "" %>,
                                    Last Updated: <%# Eval("Uploaded", "{0:MMM d, yyyy}") %>
                                    <cmds:IconButton runat="server"
                                        IsFontIcon="true" CssClass="trash-alt"
                                        ToolTip="Delete File"
                                        ConfirmText="Are you sure you want to delete this file?"
                                        CommandName="DeleteFile"
                                        CommandArgument='<%# Eval("UploadIdentifier") %>'
                                        OnCommand="Attachment_Command"
                                    />
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
    </section>

</asp:Content>
