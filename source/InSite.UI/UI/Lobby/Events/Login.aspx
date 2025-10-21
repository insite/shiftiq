<%@ Page Language="C#" CodeBehind="Login.aspx.cs" Inherits="InSite.UI.Lobby.Events.Login" MasterPageFile="~/UI/Layout/Lobby/LobbyLogin.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<div class="row">
    <div class="col-lg-4 col-md-6 offset-lg-1">
        <div class="view show" id="signin-view">

            <asp:Literal runat="server" ID="Welcome" />

            <asp:Literal runat="server" ID="SafeExamBrowserVersion" />

            <div class="form-group mb-3">
                <h4>
                    Exam Candidate Code
                    <insite:RequiredValidator runat="server" ControlToValidate="PersonCode" FieldName="Exam Candidate Code" ValidationGroup="Authenticate" />
                </h4>
                <insite:TextBox runat="server" ID="PersonCode" EmptyMessage="SkilledTradesBC Tradeworker ID" MaxLength="100" autocomplete="off" />
            </div>

            <div class="form-group mb-3">
                <h4>
                    Exam Event Password
                    <insite:RequiredValidator runat="server" ControlToValidate="ExamEventPassword" FieldName="Exam Event Password" ValidationGroup="Authenticate" />
                </h4>
                <insite:TextBox runat="server" ID="ExamEventPassword" EmptyMessage="e.g. abcd-2468-e3f5" autocomplete="off" />
            </div>

            <div class="form-group mb-3">
                <asp:Literal runat="server" ID="Disclaimer" />
                <label>
                    <insite:CheckBox runat="server" ID="IsAgree" Layout="Input" autocomplete="off" />
                    I understand and agree to these terms.
                </label>
            </div>

            <div class="form-group mb-3">
                <insite:Button runat="server" ID="SubmitButton" ButtonStyle="Primary" Icon="fas fa-arrow-alt-right" IconPosition="AfterText" Text="Sign In" Visible="false" CausesValidation="true" ValidationGroup="Authenticate" />
            </div>

            <div class="form-group mb-3">
                <asp:Literal runat="server" ID="ErrorMessage" ViewStateMode="Disabled" />
            </div>

        </div>
    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        (function () {

            Sys.Application.add_load(function () {

                $('#<%= PersonCode.ClientID %>').each(function () {
                    $(this).closest('.form-group').on('keydown', onPersonCodeKeyDown);
                });

                $('#<%= ExamEventPassword.ClientID %>').each(function () {
                    $(this).closest('.form-group').on('keydown', onExamEventPasscodeKeyDown);
                });
            });

            function onPersonCodeKeyDown(e) {
                if (e.which === 13) {
                    e.preventDefault();
                    $('#<%= ExamEventPassword.ClientID %>')[0].focus();
                }
            }

            function onExamEventPasscodeKeyDown(e) {
                if (e.which === 13) {
                    e.preventDefault();
                    $('#<%= IsAgree.ClientID %>')[0].focus();
                }
            }
        })();

    </script>
</insite:PageFooterContent>

</asp:Content>
