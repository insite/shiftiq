<%@ Page Language="C#" CodeBehind="RegisterEmployee.aspx.cs" 
    Inherits="InSite.UI.Portal.Events.Classes.RegisterEmployee" 
    MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <h2 runat="server" id="PageTitle" class="mb-0"></h2>
    <div runat="server" id="PageSubtitle" class="mb-4"></div>

    <div runat="server" id="ChooseModePanel" class="card shadow mb-3">
        <div class="card-header">
            <h3 class="m-0 text-primary">
                <i class="far fa-id-card me-2"></i><insite:Literal runat="server" Text="Select Registration Process" />
            </h3>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-lg-12">
                    <insite:RadioButton runat="server" ID="RegistrationProcessOne" GroupName="RegistrationProcess" Value="One" Text="Register one person with one invoice/payment" />
                    <insite:RadioButton runat="server" ID="RegistrationProcessMultiple" GroupName="RegistrationProcess" Value="Multiple" Text="Register two or more people with one invoice/payment" />
                </div>
            </div>
            <div class="row" style="padding-top:15px;">
                <div class="col-lg-12">
                    <insite:NextButton runat="server" ID="NextButton" />
                    <insite:CloseButton runat="server" ID="CloseButton1" />
                </div>
            </div>
        </div>
    </div>

    <div runat="server" id="MainPanel" class="card shadow mb-3">
        
        <div class="card-header">
            <div class="float-end">
                <asp:HyperLink runat="server" ID="RegisterLink" CssClass="fs-sm"><i class="fas fa-plus-circle me-2"></i>Register New Employee</asp:HyperLink>
            </div>
            <h3 class="m-0 text-primary">
                <i class="far fa-user me-2"></i><insite:Literal runat="server" Text="Find Employee" />
            </h3>
        </div>

        <div class="card-body">

            <div class="d-flex">
                <div runat="server" id="PersonNameColumn" class="me-3">
                    <insite:TextBox ID="LastName" runat="server" MaxLength="40" Width="200" EmptyMessage="Last Name" />
                </div>
                <div class="me-3">
                    <insite:TextBox ID="Email" runat="server" MaxLength="100" Width="200" EmptyMessage="Email" />
                </div>
                <div runat="server" id="PersonCodeColumn" class="me-3">
                    <insite:TextBox ID="PersonCode" runat="server" MaxLength="100" Width="200" EmptyMessage="Tradeworker ID" />
                </div>
                <div class="me-3">
                    <insite:Button runat="server" ID="SearchButton" Text="Search" ButtonStyle="Default" Icon="fas fa-search" />
                </div>
                <div class="me-3">
                    <asp:Label runat="server" ID="SearchHint" Text="Your search..." CssClass="fs-sm text-muted" />
                </div>
            </div>

            <div runat="server" id="FieldsRequired" class="alert alert-danger mt-3" role="alert" visible="false">
                <i class="far fa-stop-circle fs-xl"></i>
                <insite:Literal runat="server" ID="FieldsRequiredText" />
            </div>

            <div runat="server" id="RequirementFailure2" class="alert alert-warning mt-3" role="alert" visible="false">
                <i class="far fa-stop-circle fs-xl"></i>
                <insite:Literal runat="server" Text="The person with this last name and tradeworker # is not found." />
            </div>

            <div runat="server" id="RequirementFailure1" class="alert alert-warning mt-3" role="alert" visible="false">
                <i class="far fa-stop-circle fs-xl"></i>
                <insite:Literal runat="server" Text="The person with this last name and email is not found." />
            </div>

            <div runat="server" id="RequirementFailure3" class="alert alert-warning mt-3" role="alert" visible="false">
                <i class="far fa-stop-circle fs-xl"></i>
                <insite:Literal runat="server" Text="No matching search results" />
            </div>

            <div runat="server" id="RequirementFailure4" class="alert alert-warning mt-3" role="alert" visible="false">
                <i class="far fa-stop-circle fs-xl"></i>
                <insite:Literal runat="server" Text="No matching search results" />
            </div>

            <div runat="server" id="PersonPanel" class="row" visible="false">
                <div class="col-lg-6">
                    <table class="table table-stripped">
                        <thead>
                            <tr>
                                <th><insite:Literal runat="server" Text="Name" /></th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater runat="server" ID="PersonRepeater">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <%# Eval("FullName") %>
                                            <div class="text-muted fs-sm">
                                                <%# Eval("Email") %>
                                            </div>
                                        </td>
                                        <td style="text-align:right;">
                                            <insite:Button runat="server" ButtonStyle="Success" Icon="fas fa-user-plus" Text="Register"
                                                Visible='<%# !IsFull && Eval("Status") == null %>'
                                                NavigateUrl='<%# "/ui/portal/events/classes/register?event=" + EventIdentifier + "&candidate=" + Eval("UserIdentifier") + "&registeremployee=1" %>'
                                            />
                                            <insite:Button runat="server" ButtonStyle="Success" Icon="fas fa-user-plus" Text="Add to Waiting List" 
                                                Visible='<%# IsFull && Eval("Status") == null %>'
                                                NavigateUrl='<%# "/ui/portal/events/classes/add-to-waiting-list?event=" + EventIdentifier + "&candidate=" + Eval("UserIdentifier") %>'
                                            />
                                            <%# Eval("Status") %>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
        
        </div>

    </div>

    <insite:CloseButton runat="server" ID="CloseButton2" />

</asp:Content>
