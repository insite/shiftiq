<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.UI.Portal.Contacts.People.Create" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="~/UI/Portal/Home/Management/Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">
    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:ValidationSummary runat="server" ValidationGroup="Person" />

    <div class="row mb-3">
        <div class="col-md-6">

            <div class="card h-100">

                <div class="card-body">
                    <h3 class="mt-3">Personal Information</h3>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            First Name
                            <insite:RequiredValidator runat="server" ControlToValidate="PersonFirstName" FieldName="First Name" ValidationGroup="Person" />
                        </label>
                        <insite:TextBox ID="PersonFirstName" runat="server" MaxLength="40" />
                    </div>
                    <div class="form-group mb-3">
                        <label class="form-label">
                            Middle Name
                        </label>
                        <insite:TextBox ID="PersonMiddleName" runat="server" MaxLength="32" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Last Name
                            <insite:RequiredValidator runat="server" ControlToValidate="PersonLastName" FieldName="Last Name" ValidationGroup="Person" />
                        </label>
                        <insite:TextBox ID="PersonLastName" runat="server" MaxLength="40" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Primary Email
                            <insite:RequiredValidator runat="server" ControlToValidate="PersonEmail" FieldName="Primary Email" ValidationGroup="Person" />
                        </label>
                        <insite:TextBox ID="PersonEmail" runat="server" MaxLength="254" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Phone
                        </label>
                        <insite:TextBox ID="PersonPhone" runat="server" MaxLength="32" />
                    </div>

                </div>

            </div>

        </div>

        <div class="col-md-6">

            <div class="card h-100">

                <div class="card-body">
                    <h3 class="mt-3">Home Address</h3>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Address 1
                        </label>
                        <insite:TextBox ID="Street1" runat="server" MaxLength="128" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Address 2
                        </label>
                        <insite:TextBox ID="Street2" runat="server" MaxLength="128" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            City
                        </label>
                        <insite:TextBox ID="City" runat="server" MaxLength="128" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            State/Province
                        </label>
                        <insite:TextBox ID="Province" runat="server" MaxLength="128" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Postal Code
                        </label>
                        <insite:TextBox ID="PostalCode" runat="server" MaxLength="16" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Country
                        </label>
                        <insite:TextBox ID="Country" runat="server" MaxLength="128" Text="Canada" />
                    </div>


                </div>
            </div>
        </div>


    </div>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Person" DisableAfterClick="true" ButtonStyle="Primary" />
        <insite:CancelButton runat="server" ID="CancelButton" NavigateUrl="/ui/portal/contacts/people/search" />
    </div>

</asp:Content>