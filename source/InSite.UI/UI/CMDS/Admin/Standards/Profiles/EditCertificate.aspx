<%@ Page Language="C#" CodeBehind="EditCertificate.aspx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.Profiles.EditCertificate" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/CertificateCompetencyList.ascx" TagName="CertificateCompetencyList" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Certificate" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="ProfileTab" Title="Profile" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Profile Information
                </h2>

                <div class="row">
                    <div class="col-lg-6">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Number
                                        <insite:RequiredValidator runat="server" FieldName="Number" ControlToValidate="Number" ValidationGroup="Certificate" Display="Dynamic" />
                                        <asp:CustomValidator ID="UniqueNumber" runat="server" ControlToValidate="Number" ErrorMessage="Another profile with the same number alreadys exists" ValidationGroup="Certificate" Display="Dynamic" />
                                    </label>
                                    <insite:TextBox ID="Number" runat="server" MaxLength="40" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Name
                                        <insite:RequiredValidator runat="server" FieldName="Name" ControlToValidate="TitleInput" ValidationGroup="Certificate" />
                                    </label>
                                    <insite:TextBox ID="TitleInput" runat="server" MaxLength="256" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Status
                                    </label>
                                    <asp:CheckBox runat="server" ID="CertificationIsAvailable" Text="Certification is available" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        % of Core Hours
                                    </label>
                                    <insite:NumericBox ID="CertificationHoursPercentCore" runat="server" MinValue="0" MaxValue="100" DecimalPlaces="2" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        % of Non-Core Hours
                                    </label>
                                    <insite:NumericBox ID="CertificationHoursPercentNonCore" runat="server" MinValue="0" MaxValue="100" DecimalPlaces="2" />
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="CompetencyTab" Title="Competencies" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Competencies
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <uc:CertificateCompetencyList ID="CompetencyList" runat="server" />

                    </div>
                </div>

            </section>
        </insite:NavItem>

    </insite:Nav>
    
    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Certificate" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
