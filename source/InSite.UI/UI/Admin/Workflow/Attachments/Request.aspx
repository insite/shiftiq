<%@ Page Language="C#" CodeBehind="Request.aspx.cs" Inherits="InSite.UI.Admin.Issues.Attachments.Require" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagName="CaseInfo" TagPrefix="uc" Src="../Cases/Controls/CaseInfo.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Request" />

    <section>

        <h2 class="h4 mb-3">
            <i class="far fa-upload me-1"></i>
            Request Document
        </h2>

        <div class="row mb-3">

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Request</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Document Status
                            </label>
                            <div>
                                <insite:ItemNameComboBox runat="server" ID="FileStatus">
                                    <Settings
                                        CollectionName="Assets/Files/Document/Status"
                                        UseCurrentOrganization="true"
                                        UseGlobalOrganizationIfEmpty="true"
                                    />
                                </insite:ItemNameComboBox>
                            </div>
                        </div>

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="DocumentTypePanel" />

                        <insite:UpdatePanel runat="server" ID="DocumentTypePanel">
                            <ContentTemplate>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Document Type
                                        <insite:RequiredValidator runat="server" ControlToValidate="FileCategory" FieldName="Document Type" ValidationGroup="Request" />
                                    </label>
                                    <div>
                                        <insite:ItemNameComboBox runat="server" ID="FileCategory">
                                            <Settings
                                                CollectionName="Assets/Files/Document/Type"
                                                UseCurrentOrganization="true"
                                                UseGlobalOrganizationIfEmpty="true"
                                            />
                                        </insite:ItemNameComboBox>
                                    </div>
                                    <div class="form-text">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Document Subtype
                                    </label>
                                    <div>
                                        <insite:DocumentSubTypeComboBox runat="server" ID="FileSubcategory" />
                                    </div>
                                    <div class="form-text">
                                    </div>
                                </div>

                            </ContentTemplate>
                        </insite:UpdatePanel>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Requested From
                                <insite:RequiredValidator runat="server" ControlToValidate="RequestedFrom" FieldName="Requested From" ValidationGroup="Request" />
                            </label>
                            <div>
                                <insite:ComboBox runat="server" ID="RequestedFrom">
                                    <Items>
                                        <insite:ComboBoxOption />
                                        <insite:ComboBoxOption Value="Candidate" Text="Candidate" />
                                        <insite:ComboBoxOption Value="Other" Text="Other" />
                                    </Items>
                                </insite:ComboBox>
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Description
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="FileDescription" TextMode="MultiLine" Rows="4" MaxLength="2400" />
                            </div>
                            <div class="form-text">
                            </div>
                        </div>

                    </div>
                </div>

            </div>
            
            <div class="col-lg-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <uc:CaseInfo runat="server" ID="CaseInfo" />
                    </div>
                </div>
            </div>

        </div>

        <insite:SaveButton runat="server" ID="SaveButton" Text="Request" ValidationGroup="Request" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </section>

</asp:Content>