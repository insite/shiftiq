<%@ Page CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Assets.Contents.Forms.Edit" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ID="ValidationSummary" ValidationGroup="Content" />

    <section class="mb-3">
        <h2 class="h4 mt-4 mb-3">
            <i class="far fa-edit me-1"></i>
            Organization
        </h2>

        <div class="mb-3">
            <insite:Button runat="server" ID="TranslateButton" Text="Translate Content" ButtonStyle="Primary" Icon="fas fa-globe" CausesValidation="false" />
        </div>

        <div class="row">
            <div class="col-md-6">
                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <h3>This Content</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Organization Identifier
                                <insite:RequiredValidator runat="server" ControlToValidate="OrganizationIdentifier" ValidationGroup="Content" />
                            </label>
                            <div>
                                <insite:FindOrganization runat="server" ID="OrganizationIdentifier" />
                            </div>
                        </div>
                            
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Container Type
                            </label>
                            <insite:TextBox runat="server" ID="ContainerType" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Container Identifier
                            </label>
                            <div>
                                <asp:Label runat="server" ID="ContainerIdentifier" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Content Label
                                <insite:RequiredValidator runat="server" ControlToValidate="ContentLabel" ValidationGroup="Content" />
                            </label>
                            <insite:TextBox runat="server" ID="ContentLabel" MaxLength="100" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Content Language
                                <insite:RequiredValidator runat="server" ControlToValidate="ContentLanguage" ValidationGroup="Content" />
                            </label>
                            <div>
                                <insite:LanguageComboBox runat="server" ID="ContentLanguage" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Content Text
                            </label>
                            <insite:TextBox runat="server" ID="ContentText" TextMode="MultiLine" Rows="5" AllowHtml="true" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Content HTML
                            </label>
                            <insite:TextBox runat="server" ID="ContentHtml" TextMode="MultiLine" Rows="5" AllowHtml="true" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Content Identifier
                            </label>
                            <div>
                                <asp:Label runat="server" ID="ContentIdentifier" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <h3>Related Content</h3>
                    
                        <asp:Repeater runat="server" ID="TranslationRepeater">
                            <ItemTemplate>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                <%# Eval("ContentLabel") %>
                            </label>
                            <div>
                                <strong><a href='/ui/admin/assets/contents/edit?id=<%# Eval("ContentIdentifier") %>'><%# Eval("ContentLanguage") %></a></strong>
                                <%# Eval("ContentSnip") %>
                            </div>
                        </div>

                            </ItemTemplate>
                        </asp:Repeater>

                    </div>
                </div>
            </div>
        </div>
    </section>
       
    <div class="mb-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Content" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
