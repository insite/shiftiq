<%@ Page Language="C#" CodeBehind="ChangeField.aspx.cs" Inherits="InSite.Admin.Records.Logbooks.ChangeField" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Field" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-pencil-ruler me-1"></i>
            Field Details
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Field Name
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="FieldType" />
                            </div>
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Required Field
                            </label>
                            <div>
                                <asp:RadioButtonList runat="server" ID="IsRequired" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="true" Text="Yes" />
                                    <asp:ListItem Value="false" Text="No" />
                                </asp:RadioButtonList>
                            </div>
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Tag Text
                            </label>
                            <div>
                                <insite:TextBox runat="server" TranslationControl="LabelText" MaxLength="200" />
                                <div class="mt-1">
                                    <insite:EditorTranslation runat="server" ID="LabelText" />
                                </div>
                            </div>
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Tag Help
                            </label>
                            <div>
                                <insite:TextBox runat="server" TranslationControl="HelpText" MaxLength="200" />
                                <div class="mt-1">
                                    <insite:EditorTranslation runat="server" ID="HelpText" />
                                </div>
                            </div>
                            <div class="form-text"></div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Field" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
