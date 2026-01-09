<%@ Page Language="C#" CodeBehind="View.aspx.cs" Inherits="InSite.UI.Admin.Integrations.DirectAccess.View" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="./Controls/ContactGroupGrid.ascx" TagName="ContactGroupGrid" TagPrefix="uc" %>
<%@ Register Src="./Controls/ExamWorkflowGrid.ascx" TagName="ExamWorkflowGrid" TagPrefix="uc" %>
<%@ Register Src="./Controls/MailoutGrid.ascx" TagName="MailoutGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<insite:Nav runat="server" ID="NavPanel">

    <insite:NavItem runat="server" ID="SectionIndividual" Title="Individual" Icon="far fa-user" IconPosition="BeforeText">
        
        <section>
            <h2 class="h4 mt-4 mb-3">
                Individual
            </h2>

            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">

                <div class="row">

                    <div class="col-md-6">

                        <div class="settings">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Name
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="Name" />
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            First Name
                                        </label>
                                        <div>
                                            <asp:Literal runat="server" ID="FirstName" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Middle Name
                                        </label>
                                        <div>
                                            <asp:Literal runat="server" ID="MiddleName" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Last Name
                                        </label>
                                        <div>
                                            <asp:Literal runat="server" ID="LastName" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Crm Identifier
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="CrmIdentifier" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Birthdate
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="Birthdate" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Gender
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="Gender" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Mobile
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="Mobile" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Email
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="Email" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Personal Education Number
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="PersonalEducationNumber" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Hash Code
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="HashCode" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Refreshed
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="Refreshed" />
                                </div>
                            </div>

                        </div>

                    </div>

                    <div class="col-md-6">

                        <div class="settings">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Address Line 1
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="AddressLine1" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Address Line 2
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="AddressLine2" />
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-4">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Address City
                                        </label>
                                        <div>
                                            <asp:Literal runat="server" ID="AddressCity" />
                                        </div>
                                    </div>

                                </div>
                                <div class="col-md-4">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Address Province
                                        </label>
                                        <div>
                                            <asp:Literal runat="server" ID="AddressProvince" />
                                        </div>
                                    </div>

                                </div>
                                <div class="col-md-4">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Address Postal Code
                                        </label>
                                        <div>
                                            <asp:Literal runat="server" ID="AddressPostalCode" />
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Aboriginal Indicator
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="AboriginalIndicator" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Aboriginal Identity
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="AboriginalIdentity" />
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-6">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Is New?
                                        </label>
                                        <div>
                                            <asp:Literal runat="server" ID="IsNew" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Is Active?
                                        </label>
                                        <div>
                                            <asp:Literal runat="server" ID="IsActive" />
                                        </div>
                                    </div>

                                </div>
                                <div class="col-md-6">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Is Deceased?
                                        </label>
                                        <div>
                                            <asp:Literal runat="server" ID="IsDeceased" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Is Merged?
                                        </label>
                                        <div>
                                            <asp:Literal runat="server" ID="IsMerged" />
                                        </div>
                                    </div>

                                </div>
                            </div>

                        </div>

                    </div>

                </div>

                </div>
            </div>
        </section>

        </insite:NavItem>
        <insite:NavItem runat="server" ID="ContactGroupSection" Title="Contact Groups" Icon="far fa-users" IconPosition="BeforeText">
        
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Contact Groups
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                    <uc:ContactGroupGrid runat="server" ID="ContactGroups" />
                    </div>
                </div>
            </section>

        </insite:NavItem>
        <insite:NavItem runat="server" ID="ExamWorkflowSection" Title="Exams" Icon="far fa-calendar-alt" IconPosition="BeforeText">

            <section>
                <h2 class="h4 mt-4 mb-3">
                    Exams
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                    <uc:ExamWorkflowGrid runat="server" ID="ExamWorkflows" />
                    </div>
                </div>
            </section>

        </insite:NavItem>
        <insite:NavItem runat="server" ID="NotificationSection" Title="Notifications" Icon="far fa-paper-plane" IconPosition="BeforeText">

        <section>
            <h2 class="h4 mt-4 mb-3">
                Notifications
            </h2>

            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">
                <uc:MailoutGrid runat="server" ID="Mailouts" />
                </div>
            </div>
        </section>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>
