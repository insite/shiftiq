<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Custom.CMDS.Admin.Standards.Profiles.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/ProfileHierarchy.ascx" TagName="ProfileHierarchy" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:ValidationSummary runat="server" ValidationGroup="Profile" />

    <section class="mb-3">
        <h2 class="h4 mt-4 mb-3">
            <i class="far fa-ruler-triangle me-1"></i>
            Profile
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div class="row mb-3">
                    <div class="col-lg-6">

                        <h3>Details</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Name
                                <insite:RequiredValidator runat="server" FieldName="Name" ControlToValidate="TitleInput" ValidationGroup="Profile" />
                            </label>
                            <insite:TextBox ID="TitleInput" runat="server" MaxLength="256" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Category
                                <insite:RequiredValidator runat="server" FieldName="Category" ControlToValidate="Category" ValidationGroup="Profile" />
                            </label>
                            <insite:TextBox ID="Category" runat="server" MaxLength="32" />
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

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Description
                            </label>
                            <insite:TextBox ID="Description" runat="server" TextMode="MultiLine" />
                        </div>

                    </div>
                    <div class="col-lg-6">

                        <h3>Profile Visibility</h3>

                        <uc:ProfileHierarchy ID="ProfileHierarchy" runat="server" />

                    </div>
                </div>
            </div>
        </div>
    </section>

    <section>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Profile" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </section>

</asp:Content>
