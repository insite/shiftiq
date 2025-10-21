<%@ Page CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Accounts.Organizations.Forms.Create" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Accounts/Organizations/Controls/DetailOrganization.ascx" TagName="DetailOrganization" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Accounts/Organizations/Controls/DetailConfigurationLocationAddress.ascx" TagName="DetailConfigurationLocationAddress" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Organization" />
    
    <insite:Nav runat="server" ID="NavPanel" CssClass="mb-3">

        <insite:NavItem runat="server" ID="OrganizationSection" Title="Organization" Icon="far fa-city" IconPosition="BeforeText">
            <section>
            
                <h2 class="h4 mt-4 mb-3">Organization</h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <uc:DetailOrganization runat="server" ID="DetailOrganization" />

                    </div>
                </div>

            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="LocationAddressSection" Title="Location" Icon="far fa-location-dot" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Address</h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="row">
                            <div class="col-md-6">

                                <uc:DetailConfigurationLocationAddress runat="server" ID="DetailLocationAddress" />
                    
                            </div>
                        </div>

                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>
        
    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Organization" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
