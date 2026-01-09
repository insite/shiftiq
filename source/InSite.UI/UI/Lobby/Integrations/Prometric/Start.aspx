<%@ Page Language="C#" CodeBehind="Start.aspx.cs" Inherits="InSite.UI.Lobby.Integrations.Prometric.Start" MasterPageFile="~/UI/Layout/Lobby/LobbyLogin.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div class="row">
        <div class="col-lg-5">
            
            <h2>Welcome to Prometric!</h2>
            <p>Your assessment is ready to start.</p>

            <div class="form-group mb-3">
                <h5>
                    Exam Candidate Code
                    <insite:RequiredValidator runat="server" ControlToValidate="PersonCode" FieldName="Exam Candidate Code" ValidationGroup="Authenticate" />
                </h5>
                <insite:TextBox runat="server" ID="PersonCode" EmptyMessage="Learner ID" MaxLength="100" autocomplete="off" />
            </div>

            <div class="form-group mb-3">
                <h5>
                    Exam Event Password
                    <insite:RequiredValidator runat="server" ControlToValidate="ExamEventPassword" FieldName="Exam Event Password" ValidationGroup="Authenticate" />
                </h5>
                <insite:TextBox runat="server" ID="ExamEventPassword" EmptyMessage="e.g. abcd-2468-e3f5" autocomplete="off" />
            </div>

            <div class="form-group mb-3">
                <insite:Button runat="server" ID="SubmitButton" ButtonStyle="Primary" Icon="fas fa-arrow-alt-right" IconPosition="AfterText" Text="Start" CausesValidation="true" ValidationGroup="Authenticate" />
            </div>

            <div class="form-group mb-3">
                <asp:Literal runat="server" ID="ErrorMessage" ViewStateMode="Disabled" />
            </div>

        </div>
    </div>

</asp:Content>
