<%@ Page Language="C#" CodeBehind="Support.aspx.cs" Inherits="InSite.UI.Individual.Support" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="Alert" />

    <div class="alert alert-primary" runat="server" ID="PortalSupportContent" visible="false">
        <asp:Literal runat="server" ID="PortalSupportHtml" />
    </div>

    <div class="row">
        
        <div class="col-lg-12 d-none">
            <div class="form-group mb-3">
                <label class="form-label"><%= Translate("From") %>:</label>
                <div>
                    <asp:Literal runat="server" ID="FromFullName" />
                </div>
                <div class="form-text">
                    <asp:Literal runat="server" ID="FromEmail" />
                    <insite:RequiredValidator runat="server" Display="None" ControlToValidate="Message" ValidationGroup="Request" />
                </div>
            </div>
        </div>

        <div class="col-lg-12 mb-3">
            <div class="row">
                <div class="col-lg-3">
                    <div class="form-group mb-3">
                        <label class="form-label"><%= Translate("Phone Number") %>:</label>
                        <div>
                            <insite:TextBox ID="Phone" runat="server" MaxLength="32" EmptyMessage="+1 ___ ___ ____"/>
                        </div>
                    </div>
                </div>
                <div class="col-lg-9">
                    <div class="form-group mb-3">
                        <label class="form-label"><%= Translate("Employer/Program") %>:</label>
                        <div>
                            <insite:TextBox ID="EmployerProgram" runat="server" MaxLength="100" EmptyMessage="If you know the employer or program you are associated with, please specify it."/>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-lg-12 mb-3">
            <div class="form-group mb-3">
                <label class="form-label">
                    <%= Translate("Summary") %>:
                    <insite:RequiredValidator runat="server" Display="None" ControlToValidate="Summary" ValidationGroup="Request" />
                </label>
                <div>
                    <insite:TextBox ID="Summary" runat="server" MaxLength="32" EmptyMessage="Brief summary of your request."/>
                </div>
            </div>
            <div class="form-group mb-3">
                <label class="form-label">
                    <%= Translate("Details") %>:
                    <insite:RequiredValidator runat="server" Display="None" ControlToValidate="Message" ValidationGroup="Request" />
                </label>
                <div class="mb-3">
                    <insite:TextBox runat="server" ID="Message" TextMode="MultiLine" Rows="7" MaxLength="1000" EmptyMessage="Please describe the issue(s) you are experiencing in detail, so we can better assist you."/>
                    <div class="mt-2 fs-xs text-muted">
                        Please provide a clear and detailed explanation of your issue or request. Be concise yet specific. (Maximum: 1,000 characters)
                    </div>
                </div>
            </div>
            <div>
                <insite:Button runat="server" ID="SendButton" Text="Send" ButtonStyle="Success" Icon="fas fa-paper-plane" ValidationGroup="Request" />
                <insite:CloseButton runat="server" ID="CloseButton" />
            </div>
        </div>

    </div>

</asp:Content>
