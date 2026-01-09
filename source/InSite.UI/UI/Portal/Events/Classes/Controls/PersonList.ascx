<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonList.ascx.cs" Inherits="InSite.UI.Portal.Events.Classes.Controls.PersonList" %>

<div class="row pb-2">
    <div class="col-md-12 text-end">
        <insite:Button runat="server" ID="AddButton1" ButtonStyle="Success" Text="Add" Icon="far fa-plus-circle" CausesValidation="false" />
    </div>
</div>

<div class="row">

    <asp:CustomValidator runat="server" ID="FinalValidator" Display="none" ValidationGroup="PersonList" />

    <div runat="server" id="MaxRegistrations" class="alert alert-warning" role="alert">
        The max available number of employees was added
    </div>

    <div class="col-md-12">
        <asp:Repeater runat="server" ID="PersonRepeater">
            <HeaderTemplate>
                <table class="table table-striped">
                    <thead>
                        <th>First Name</th>
                        <th>Last Name</th>
                        <th>Email</th>
                        <th>Birthdate</th>
                        <th><insite:ContentTextLiteral runat="server" ContentLabel="Person Code"/></th>
                        <th>Phone</th>
                        <th></th>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <FooterTemplate>
                    <thead style="border:transparent;">
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th><div class="form-text"><insite:Literal runat="server" Text='<%# GetCustomHelp() %>'/></div></th>
                        <th></th>
                        <th></th>
                    </thead>
                    </tbody>
                </table>
            </FooterTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <insite:TextBox ID="FirstName" runat="server" MaxLength="32" Text='<%# Eval("FirstName") %>' />
                        <insite:RequiredValidator runat="server" ControlToValidate="FirstName" Display="None" RenderMode="Dot" ValidationGroup="PersonList" />
                    </td>
                    <td>
                        <insite:TextBox ID="LastName" runat="server" MaxLength="32" Text='<%# Eval("LastName") %>' />
                        <insite:RequiredValidator runat="server" ControlToValidate="LastName" Display="None" RenderMode="Dot" ValidationGroup="PersonList" />
                    </td>
                    <td>
                        <insite:TextBox ID="Email" runat="server" MaxLength="128" Text='<%# Eval("Email") %>' />
                    </td>
                    <td>
                        <insite:DateSelector ID="Birthdate" runat="server" Value='<%# Eval("Birthdate") %>' />
                    </td>
                    <td>
                        <insite:TextBox ID="PersonCode" runat="server" MaxLength="20" Text='<%# Eval("PersonCode") %>' />
                    </td>
                    <td>
                        <insite:TextBox ID="Phone" runat="server" MaxLength="32" Text='<%# Eval("Phone") %>' />
                    </td>
                    <td>
                        <insite:Button runat="server" ToolTip="Delete" Icon="far fa-trash-alt" CommandName="Delete" CommandArgument="<%# Container.ItemIndex %>" CausesValidation="false"
                            ConfirmText="Are you sure you want to remove this person?" Visible="<%# AllowDelete %>" ButtonStyle="Default" />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>

<div class="row pb-3">
    <div class="col-md-4">
        <insite:Button runat="server" ID="AddButton2" ButtonStyle="Success" Text="Add" Icon="far fa-plus-circle" CausesValidation="false" />
    </div>
</div>
