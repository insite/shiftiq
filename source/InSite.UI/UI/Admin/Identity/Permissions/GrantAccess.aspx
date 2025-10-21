<%@ Page Language="C#" CodeBehind="GrantAccess.aspx.cs" Inherits="InSite.UI.Portal.Accounts.Users.GrantAccess" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <div runat="server" id="FormRow" class="row" visible="false">
        <div class="col-6">
            <div class="card card-hover shadow">
                <div class="card-body">

                    <div class="form-group mb-3">
                        <label class="form-label ps-0"><insite:Literal runat="server" Text="Full Name" /></label>
                        <div runat="server" id="ApproveFullName" class="form-control-plaintext"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label ps-0"><insite:Literal runat="server" Text="Email Address" /></label>
                        <div runat="server" id="ApproveEmail" class="form-control-plaintext"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label ps-0"><insite:Literal runat="server" Text="User Account Status" /></label>
                        <div class="form-control-plaintext">
                            <insite:CheckSwitch runat="server" ID="IsAccessGranted" Text="Access Granted"/>
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label ps-0"><insite:Literal runat="server" Text="Reason" /></label>
                        <div class="form-control-plaintext">
                            <insite:TextBox runat="server" ID="ApproveReason" TextMode="MultiLine" Rows="6" MaxLength="200" />
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>

</asp:Content>