<%@ Page Language="C#" CodeBehind="NewPerson.aspx.cs" Inherits="InSite.Admin.Events.Candidates.Forms.NewPerson" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Inputs" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="InputsTab" Title="Contacts" Icon="far fa-users" IconPosition="BeforeText">
            <div class="row my-3">
                <div class="col-lg-6">

                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    School
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="InputsSchoolName" /> 
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Existing Contacts
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="InputsNoExistingContacts" Text="None" />

                                    <asp:Repeater runat="server" ID="InputsExistingContacts">
                                        <HeaderTemplate><ul></HeaderTemplate>
                                        <FooterTemplate></ul></FooterTemplate>
                                        <ItemTemplate>
                                            <li>
                                                <%# Eval("FullName") %>
                                                <span class="form-text"><%# Eval("Email") %></span>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    New Contact Person(s)
                                    <insite:RequiredValidator runat="server" ControlToValidate="InputsPersons" FieldName="New Contact Person(s)" ValidationGroup="Inputs" />
                                </label>
                                <insite:TextBox runat="server" ID="InputsPersons" TextMode="MultiLine" Rows="7" />
                                <div class="form-text">
                                    Input one email address per line. The display name for each recipient must precede the
                                    address.<br />For example: <b><i>John Smith</i> john.smith@example.org</b>
                                </div>
                            </div>

                        </div>
                    </div>

                </div>
            </div>
        
            <div>
                <insite:SaveButton runat="server" ID="InputsValidateButton" Text="Validate" Icon="fas fa-check" ValidationGroup="Inputs" />
                <insite:CancelButton runat="server" ID="InputsCancelButton" />
            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" ID="ConfirmTab" Title="Confirm" Icon="far fa-check" IconPosition="BeforeText" Visible="false">
            <div class="row my-3">
                <div class="col-lg-6">

                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <asp:Repeater runat="server" ID="ConfirmChanges">
                                <HeaderTemplate>
                                    <div class="alert alert-info mt-3" role="alert">
                                        <p>Here are the changes that will occur when you save your changes:</p>
                                        <ul>
                                </HeaderTemplate>
                                <FooterTemplate>
                                        </ul>
                                    </div>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <li><%# Container.DataItem %></li>
                                </ItemTemplate>
                            </asp:Repeater>

                        </div>
                    </div>

                </div>
            </div>
        
            <div>
                <insite:SaveButton runat="server" ID="ConfirmButton" ValidationGroup="Confirm" />
                <insite:CancelButton runat="server" ID="ConfirmCancelButton" />
            </div>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>
