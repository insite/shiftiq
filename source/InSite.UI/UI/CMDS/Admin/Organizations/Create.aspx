<%@ Page CodeBehind="Create.aspx.cs" Inherits="InSite.Cmds.Admin.Organizations.Forms.Create" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:ValidationSummary runat="server" ValidationGroup="CompanyInfo" />

    <section runat="server" ID="OrganizationSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-city me-1"></i>
            Organization Information
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <asp:CustomValidator runat="server" ID="CompanyDomainValidator" ErrorMessage="Web Site is invalid" ValidationGroup="CompanyInfo" />

                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Full Name
                                <insite:RequiredValidator runat="server" ControlToValidate="CompanyName" FieldName="Name" ValidationGroup="CompanyInfo" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="CompanyName" MaxLength="100" />
                            </div>
                            <div class="form-text">The name of the organization.</div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Short Name
                                <insite:RequiredValidator runat="server" ControlToValidate="Acronym" FieldName="Short Name" ValidationGroup="CompanyInfo" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="Acronym" MaxLength="50" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">Description</label>
                            <div>
                                <insite:TextBox runat="server" ID="Description" TextMode="MultiLine" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Web Site:
                                <insite:RequiredValidator runat="server" ControlToValidate="WebSiteUrl" FieldName="WebSiteUrl" ValidationGroup="CompanyInfo" />
                            </label>
                            <div>
                                <insite:TextBox ID="WebSiteUrl" runat="server" MaxLength="128" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Organization Code:
                                <insite:RequiredValidator runat="server" ControlToValidate="OrganizationCode" FieldName="Organization Code" ValidationGroup="CompanyInfo" />
                            </label>
                            <div>
                                <insite:TextBox ID="OrganizationCode" runat="server" MaxLength="20" />
                            </div>
                            <div class="form-text"></div>
                        </div>
                        <div class="form-group mb-3">
                            <label class="form-label">Settings:</label>
                            <div>
                                <asp:CheckBox ID="EnableDivisions" runat="server" Text="Enable Divisions" />
                            </div>
                            <div class="form-text"></div>
                        </div>

                    </div>
                </div>

            </div>
        </div>
    </section>

    <div class="mb-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="CompanyInfo" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>
</asp:Content>
