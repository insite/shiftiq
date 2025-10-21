<%@ Page Language="C#" CodeBehind="EditUserConnection.aspx.cs" Inherits="InSite.Admin.Contacts.People.Forms.EditUserConnection" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Connection" />

    <div class="row mb-3 mt-3">
        <div class="col-lg-4">
            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <h3>
                        <i class="far fa-user me-1"></i>
                        Edit Connection
                    </h3>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            From User
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="FromUserName" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            To User
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="ToUserName" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Connected On
                        </label>
                        <insite:DateTimeOffsetSelector runat="server" ID="ConnectedOn" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Attributes
                        </label>
                        <div>
                            <insite:CheckBox runat="server" ID="IsLeader" Text="Leader" />
                            <insite:CheckBox runat="server" ID="IsManager" Text="Manager" />
                            <insite:CheckBox runat="server" ID="IsSupervisor" Text="Supervisor" />
                            <insite:CheckBox runat="server" ID="IsValidator" Text="Validator" />
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Connection" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>